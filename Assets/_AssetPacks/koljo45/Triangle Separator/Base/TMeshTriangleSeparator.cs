namespace koljo45.MeshTriangleSeparator
{
    using UnityEngine;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a method signature for the callback method.
    /// </summary>
    /// <param name="chunks">Lists to be filled with chunks. [0] list contains chunks that are in the base subset, [1] list chunks that are not</param>
    /// <param name="m">Mesh containing data indexed by the chunks and edges</param>
    public delegate void SeparatorFinishedHandeler(List<Chunk>[] chunks, Mesh m);

    /// <remarks>Every triangle has exactly three vertices. Vertex subset is determined by the <see cref="ITriangleSeparator"/> implementation</remarks>
    public enum SeparationMode
    {
        Smooth,
        /// <summary>Include only those triangles in which more than one vertex is in set.</summary>
        Undercut,
        /// <summary>Include triangles in which one or more vertices are in set.</summary>
        Overcut
    }

    /// <summary>
    /// Provides the ability to section out meshes based on user defined subset functions.
    /// Can calculate chunks for both convex and concave meshes if proper mesh data is used.
    /// By using a callback method all calculations can be offloaded onto a worker thread, further multithreading is used in concave mesh calculations.
    /// Double edges (vertices) can also be detected for more appropriate results.
    /// </summary>
    public class TMeshTriangleSeparator
    {

        ///<see cref="ITriangleSeparator"/>
        private bool _convex;
        private bool _deepScan;
        private int _numThreads;

        private ITriangleSeparator _latestTriangleSeparator;
        private ITriangleSeparator _triangleSeparator;

        private Mesh _mesh;

        //Arrays into which we load our mesh data

        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector4[] _tangents;
        private Vector2[] _uv;
        private BoneWeight[] _boneWeights;
        private bool _calcBW;
        private Color32[] _vertexColors;
        private bool _calcCol;

        //Lists which contain output data, to be filled by our calculations

        private List<List<Triangle>>[] _chunks;
        private List<List<int>>[][] _edges;
        private List<Triangle>[] _trisList;
        private List<Triangle>[] _affectedTriangles;

        //Array which contains indices to vertices that we assume are duplicates of vertices at the given index.
        //It's a 1-1 map. This array is the same size as the _vertices array,
        //because of this every index in this array corresponds to an index in the _vertices array.
        //Stored int values act as indices to the _vertices array as well.
        private int[] _duplicateVertices;

        //Used to optimize calculations
        private int[][] _optVirtualVertUse;

        private List<List<int>>[] _unsortedEdges;

        private SeparationMode separationMode;
        private AutoResetEvent QueueHolder;
        private SeparatorFinishedHandeler handelerData;

        public TMeshTriangleSeparator(ITriangleSeparator tc)
        {
            setTriangleSeparator(tc);
            QueueHolder = new AutoResetEvent(true);

            _chunks = new List<List<Triangle>>[2];
            _chunks[0] = new List<List<Triangle>>();
            _chunks[1] = new List<List<Triangle>>();
            _edges = new List<List<int>>[2][];
            _unsortedEdges = new List<List<int>>[2];
            _unsortedEdges[0] = new List<List<int>>();
            _unsortedEdges[1] = new List<List<int>>();
            _trisList = new List<Triangle>[2];
            _trisList[0] = new List<Triangle>();
            _trisList[1] = new List<Triangle>();

            _optVirtualVertUse = new int[2][];

            _affectedTriangles = new List<Triangle>[2];
            _affectedTriangles[0] = new List<Triangle>();
            _affectedTriangles[1] = new List<Triangle>();

            t_flag = new AutoResetEvent[_numThreads];
            for (int i = 0; i < _numThreads; i++)
                t_flag[i] = new AutoResetEvent(false);
        }
        private void cleanUp()
        {
            _vertices = null;
            _normals = null;
            _tangents = null;
            _uv = null;
            _boneWeights = null;
            _vertexColors = null;
            _mesh = null;
            _chunks[0] = new List<List<Triangle>>();
            _chunks[1] = new List<List<Triangle>>();
            _edges[0] = null;
            _edges[1] = null;
            _unsortedEdges[0].Clear();
            _unsortedEdges[1].Clear();
            _trisList[0].Clear();
            //_trisList[0].TrimExcess();
            _trisList[1].Clear();
            //_trisList[1].TrimExcess();

            _affectedTriangles[0].Clear();
            //_affectedTriangles[0].TrimExcess();
            _affectedTriangles[1].Clear();
            //_affectedTriangles[1].TrimExcess();
        }
        private void loadUp(out List<Chunk>[] chunks)
        {
            chunks = new List<Chunk>[2];
            for(int ss = 0; ss < 2; ss++)
            {
                chunks[ss] = new List<Chunk>(_chunks[ss].Count);
                for(int c = 0; c < _chunks[ss].Count; c++)
                {
                    chunks[ss].Add(new Chunk(_chunks[ss][c], _edges[ss][c]));
                }
            }
        }

        public void setTriangleSeparator(ITriangleSeparator tc)
        {
            if (tc == null)
                throw new System.ArgumentNullException("tc", "Input triangle separator cannot be null");
            _latestTriangleSeparator = tc;
        }

        /// <summary>
        /// Divides the mesh into chunks based on the <see cref="ITriangleSeparator"/> implementation.
        /// Both chunks and edges are divided into the 0th and 1th subset.
        /// </summary>
        /// <param name="m">Input mesh</param>
        /// <param name="chunks">Lists to be filled with chunks. [0] list contains chunks that are in the base subset, [1] list chunks that are not</param>
        /// <param name="mesh">Mesh containing data indexed by the chunks and edges</param>
        /// <returns>True if division was successful, false if it wasn't</returns>
        public bool divideMesh(Mesh m, out List<Chunk>[] chunks, out Mesh mesh)
        {
            if (m == null)
                throw new ArgumentNullException("m", "Input mesh cannot be null");
            QueueHolder.WaitOne();
            _triangleSeparator = _latestTriangleSeparator;

            _mesh = m;
            mesh = _mesh;
            try
            {
                chunks = null;
                if (rootSeparation())
                {
                    mesh = _mesh;
                    if (!_convex) startMeshDivision(_vertices.Length);
                    loadUp(out chunks);
                    return true;
                }
                mesh = null;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                chunks = null;
                mesh = null;
            }
            finally
            {
                cleanUp();
                QueueHolder.Set();
            }
            return false;
        }

        /// <summary>
        /// Divides the mesh into chunks based on the <see cref="ITriangleSeparator"/> implementation.
        /// </summary>
        /// <param name="m">Input mesh</param>
        /// <param name="ev">Callback to be used upon completion</param>
        public void divideMesh(Mesh m, SeparatorFinishedHandeler ev)
        {
            if (m == null)
                throw new ArgumentNullException("m", "Input mesh cannot be null");
            if (ev == null)
                throw new ArgumentNullException("ev", "SeparatorFinishedHandeler cannot be null");

            QueueHolder.WaitOne();
            _triangleSeparator = _latestTriangleSeparator;

            handelerData = ev;
            _mesh = m;

            try
            {
                if (rootSeparation())
                    if (!_convex)
                    {
                        ThreadPool.QueueUserWorkItem(divisionEntry, _vertices.Length);
                        return;
                    }
                    else
                    {
                        List<Chunk>[] chunks = null;
                        loadUp(out chunks);
                        handelerData(chunks, _mesh);
                    }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
                cleanUp();
                QueueHolder.Set();
            }
            cleanUp();
            QueueHolder.Set();
        }

        private void divisionEntry(System.Object vertexCount)
        {
            try
            {
                startMeshDivision(Convert.ToInt32(vertexCount));

                List<Chunk>[] chunks = null;
                loadUp(out chunks);
                handelerData(chunks, _mesh);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                cleanUp();
                QueueHolder.Set();
            }
        }

        private bool rootSeparation()
        {
            _convex = _triangleSeparator.Convex;
            _numThreads = _triangleSeparator.NumThreads;
            _deepScan = _triangleSeparator.DeepScan;
            separationMode = _triangleSeparator.SeparationMode;

            _vertices = _mesh.vertices;
            _normals = _mesh.normals;
            _tangents = _mesh.tangents;
            _uv = _mesh.uv;
            _boneWeights = _mesh.boneWeights;
            _calcBW = _boneWeights.Length != 0;
            _vertexColors = _mesh.colors32;
            _calcCol = _vertexColors.Length != 0;

            _duplicateVertices = new int[_vertices.Length];

            bool[] vertexSetMap = setSeparation(_trisList, _affectedTriangles);

            if (separationMode == SeparationMode.Smooth)
            {
                if (!_affectedTriangles[0].Any() && !_affectedTriangles[1].Any())
                    return false;
            }
            else
                    if (!_trisList[0].Any() || !_trisList[1].Any())
                return false;

            if (_deepScan)
            {
                List<int>[] samples = new List<int>[2];
                if (!_convex)
                {
                    samples[0] = new List<int>(_vertices.Length / 2);
                    samples[1] = new List<int>(_vertices.Length / 2);
                    for (int m = 0; m < vertexSetMap.Length; m++)
                        samples[vertexSetMap[m] ? 0 : 1].Add(m);
                }
                else
                {
                    List<int> tris3 = MeshCalc.getIndices(_affectedTriangles[0]);
                    tris3.AddRange(MeshCalc.getIndices(_affectedTriangles[1]));
                    samples[0] = MeshCalc.extractIndices(tris3, false);
                }
                List<int> duplicates = new List<int>();
                for (int s = 0; s < (_convex ? 1 : 2); s++)
                {
                    List<int> sample = samples[s];
                    while (sample.Count > 0)
                    {

                        duplicates.Add(sample[0]);
                        sample.RemoveAt(0);
                        for (int i = sample.Count - 1; i >= 0; i--)
                        {
                            if ((_vertices[duplicates[0]] - _vertices[sample[i]]).magnitude <= _triangleSeparator.DuplicateVertexOffset)
                            {
                                duplicates.Add(sample[i]);
                                sample.RemoveAt(i);
                            }
                        }
                        int low = duplicates.Min();
                        foreach (int i in duplicates)
                            _duplicateVertices[i] = low;
                        duplicates.Clear();
                    }
                }
            }
            else
            {
                for (int i = 0; i < _duplicateVertices.Length; i++)
                    _duplicateVertices[i] = i;
            }

            findLoops();

            if (_convex)
            {
                _chunks[0].Add(new List<Triangle>(_trisList[0]));
                _chunks[1].Add(new List<Triangle>(_trisList[1]));
                _edges[0] = new List<List<int>>[1];
                _edges[0][0] = new List<List<int>>(_unsortedEdges[0]);
                _edges[1] = new List<List<int>>[1];
                _edges[1][0] = new List<List<int>>(_unsortedEdges[1]);
            }
            return true;
        }

        private void startMeshDivision(int vertexCount)
        {
            threadFailed = false;

            _optVirtualVertUse[0] = new int[vertexCount];
            _optVirtualVertUse[1] = new int[vertexCount];
            for (int i = 0; i < 2; i++)
            {
                for (int l = 0; l < _trisList[i].Count; l++)
                {
                    Triangle t = _trisList[i][l];
                    _optVirtualVertUse[i][_duplicateVertices[t.v1]]++;
                    _optVirtualVertUse[i][_duplicateVertices[t.v2]]++;
                    _optVirtualVertUse[i][_duplicateVertices[t.v3]]++;
                }
            }
            _numThreads = Math.Abs(_numThreads);
            if (_numThreads < 2)
            {
                findChunks();
                return;
            }
            //THREADING
            #region THREAD DATA init
            int uCnt = _trisList[0].Count;
            int oCnt = _trisList[1].Count;
            int uThreads = 1;
            int oThreads = 1;
            int undistributedThreads = _numThreads - 2;
            int uTSupremacy = (int)Math.Round((Convert.ToDouble(uCnt) / (uCnt + oCnt)) * (undistributedThreads));
            uThreads += uTSupremacy;
            oThreads += undistributedThreads - uTSupremacy;

            for (int i = uThreads; i >= 1; i--)
            {
                if (uCnt / uThreads < t_minTris)
                    uThreads = i;
                else break;
            }
            for (int i = oThreads; i >= 1; i--)
            {
                if (oCnt / oThreads < t_minTris)
                    oThreads = i;
                else break;
            }

            int actualNumThreads = uThreads + oThreads;
            t_trisList = new LinkedList<Triangle>[actualNumThreads];
            t_fragments = new List<List<Triangle>>[actualNumThreads];
            t_fragmentOutline = new List<List<Triangle>>[actualNumThreads];
            t_frangmentsMask = new int[actualNumThreads][];
            t_optVertUseIndice = new int[actualNumThreads];

            int uBaseSize = uCnt / uThreads;
            int uCei = uThreads - 1;
            for (int i = 0; i < uCei; i++)
            {
                t_trisList[i] = new LinkedList<Triangle>(_trisList[0].GetRange(i * uBaseSize, uBaseSize));
                t_frangmentsMask[i] = new int[_optVirtualVertUse[0].Length];
                Array.Copy(_optVirtualVertUse[0], t_frangmentsMask[i], t_frangmentsMask[i].Length);
                t_optVertUseIndice[i] = 0;
            }
            int uLastChunk = uCei * uBaseSize;
            t_trisList[uCei] = new LinkedList<Triangle>(_trisList[0].GetRange(uLastChunk, uCnt - uLastChunk));
            t_frangmentsMask[uCei] = new int[_optVirtualVertUse[0].Length];
            Array.Copy(_optVirtualVertUse[0], t_frangmentsMask[uCei], t_frangmentsMask[uCei].Length);
            t_optVertUseIndice[uCei] = 0;

            int oBaseSize = oCnt / oThreads;
            int lI = actualNumThreads - 1;
            for (int i = 0; i < (oThreads - 1); i++)
            {
                int absI = i + uThreads;
                t_trisList[absI] = new LinkedList<Triangle>(_trisList[1].GetRange(i * oBaseSize, oBaseSize));
                t_frangmentsMask[absI] = new int[_optVirtualVertUse[1].Length];
                Array.Copy(_optVirtualVertUse[1], t_frangmentsMask[absI], t_frangmentsMask[absI].Length);
                t_optVertUseIndice[absI] = 1;
            }
            int oLCFloor = (oThreads - 1) * oBaseSize;
            t_trisList[lI] = new LinkedList<Triangle>(_trisList[1].GetRange(oLCFloor, oCnt - oLCFloor));
            t_frangmentsMask[lI] = new int[_optVirtualVertUse[1].Length];
            Array.Copy(_optVirtualVertUse[1], t_frangmentsMask[lI], t_frangmentsMask[lI].Length);
            t_optVertUseIndice[lI] = 1;

            if (t_flag.Length > actualNumThreads)
            {
                Array.Resize(ref t_flag, actualNumThreads);
            }
            else if (t_flag.Length < actualNumThreads)
            {
                int oldSize = t_flag.Length;
                Array.Resize(ref t_flag, actualNumThreads);
                for (int i = 0; i < (actualNumThreads - oldSize); i++)
                    t_flag[oldSize + i] = new AutoResetEvent(false);
            }

            for (int i = 0; i < (actualNumThreads); i++)
            {
                ThreadPool.QueueUserWorkItem(findChunks, i);
            }
            #endregion

            WaitHandle.WaitAll(t_flag);

            if (threadFailed) throw new Exception("One or more threads failed while processing triangles");

            //POST-THREAD
            #region THREAT DATA reformat
            int uFragmentCnt = 0;
            for (int i = 0; i < uThreads; i++)
                uFragmentCnt += t_fragments[i].Count;
            int oFragmentCnt = 0;
            for (int i = uThreads; i < actualNumThreads; i++)
                oFragmentCnt += t_fragments[i].Count;

            List<List<Triangle>> uFragments = new List<List<Triangle>>(uFragmentCnt);
            List<List<Triangle>> oFragments = new List<List<Triangle>>(oFragmentCnt);
            List<List<Triangle>> uOutlines = new List<List<Triangle>>(uFragmentCnt);
            List<List<Triangle>> oOutlines = new List<List<Triangle>>(oFragmentCnt);

            for (int i = 0; i < uThreads; i++)
            {
                uFragments.AddRange(t_fragments[i]);
                uOutlines.AddRange(t_fragmentOutline[i]);
            }
            for (int i = uThreads; i < actualNumThreads; i++)
            {
                oFragments.AddRange(t_fragments[i]);
                oOutlines.AddRange(t_fragmentOutline[i]);
            }
            #endregion
            //TUNA
            #region THREAD DATA combine
            List<int> box0 = new List<int>();
            List<int> box = new List<int>();
            for (int l = 0; l < 2; l++)
            {
                List<List<Triangle>> fragments = l == 0 ? uFragments : oFragments;
                List<List<Triangle>> outlines = l == 0 ? uOutlines : oOutlines;
                box0.Clear();
                box0.Capacity = outlines.Count;
                for (int i = 0; i < outlines.Count; i++)
                    box0.Add(i);

                int curr_elem = 0;
                int to_go = 0;

                while (box0.Count > 0)
                {
                    box.Clear();
                    box.Add(box0[0]);
                    box0.RemoveAt(0);
                    curr_elem = 0;
                    to_go = 1;

                    while (to_go > 0)
                    {
                        for (int i = box0.Count - 1; i >= 0; i--)
                        {
                            if (chunksOverlap(outlines[box[curr_elem]], outlines[box0[i]]))
                            {
                                box.Add(box0[i]);
                                box0.RemoveAt(i);
                                to_go++;
                            }
                        }
                        to_go--;
                        curr_elem++;
                    }

                    int size = 0;
                    for (int k = 0; k < box.Count; k++)
                        size += fragments[box[k]].Count;
                    _chunks[l].Add(new List<Triangle>(size));
                    for (int k = 0; k < box.Count; k++)
                        _chunks[l][_chunks[l].Count - 1].AddRange(fragments[box[k]]);
                }
            }
            #endregion
            for (int s = 0; s < 2; s++)
            {
                _edges[s] = new List<List<int>>[_chunks[s].Count];
                if (_chunks[s].Count < 2)
                {
                    _edges[s][0] = new List<List<int>>();
                    foreach (List<int> edge in _unsortedEdges[s])
                        _edges[s][0].Add(edge);
                    continue;
                }
                for (int c = 0; c < _chunks[s].Count; c++)
                {
                    _edges[s][c] = new List<List<int>>();
                    List<Triangle> chunk = _chunks[s][c];
                    for (int e = 0; e < _unsortedEdges[s].Count; e++)
                    {
                        List<int> edge = _unsortedEdges[s][e];
                        foreach (Triangle t in chunk)
                            if (edge.Contains(t.v1) || edge.Contains(t.v2) || edge.Contains(t.v3))
                            {
                                _edges[s][c].Add(edge);
                                break;
                            }
                    }
                }
            }
            /*foreach (List<Triangle> frag in uFragments)
            {
                Color c = UnityEngine.Random.ColorHSV();
                for (int i = 0; i < frag.Count; i++)
                {
                    Triangle t = frag[i];
                    UnityEngine.Debug.DrawLine(_vertices[t.v1] * 5, _vertices[t.v2] * 5, c, 1000);
                    UnityEngine.Debug.DrawLine(_vertices[t.v2] * 5, _vertices[t.v3] * 5, c, 1000);
                    UnityEngine.Debug.DrawLine(_vertices[t.v3] * 5, _vertices[t.v1] * 5, c, 1000);
                }
            }*/
        }

        private bool chunksOverlap(List<Triangle> c1, List<Triangle> c2)
        {
            if (c1.Count < 1 || c2.Count < 1) return false;
            List<int> f1 = MeshCalc.getIndices(c1);
            List<int> f2 = MeshCalc.getIndices(c2);
            for (int i = 0; i < f1.Count; i++)
                f1[i] = _duplicateVertices[f1[i]];
            for (int i = 0; i < f2.Count; i++)
                f2[i] = _duplicateVertices[f2[i]];
            List<int> vert1 = MeshCalc.extractIndices(f1, true);
            List<int> vert2 = MeshCalc.extractIndices(f2, true);
            if (vert2[0] > vert1.Last() || vert1[0] > vert2.Last()) return false;

            for (int i = 0; i < vert1.Count; i++)
                if (vert1[i] >= vert2[0] && vert1[i] <= vert2.Last())
                    if (vert2.Contains(vert1[i]))
                        return true;
            return false;
        }

        private bool[] setSeparation(List<Triangle>[] tl, List<Triangle>[] affTris)
        {
            bool[] vertexSetMap = new bool[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
                if (_calcBW && _calcCol)
                    vertexSetMap[i] = _triangleSeparator.vertexSetCheck(_vertices[i], _boneWeights[i], _vertexColors[i]);
                else if (_calcBW)
                    vertexSetMap[i] = _triangleSeparator.vertexSetCheck(_vertices[i], _boneWeights[i]);
                else if (_calcCol)
                    vertexSetMap[i] = _triangleSeparator.vertexSetCheck(_vertices[i], _vertexColors[i]);
                else
                    vertexSetMap[i] = _triangleSeparator.vertexSetCheck(_vertices[i]);

            for (int subMesh = 0; subMesh < _mesh.subMeshCount; subMesh++)
            {
                int[] tris = _mesh.GetTriangles(subMesh);
                for (int t = 0; t < tris.Length; t += 3)
                {
                    int v1 = tris[t];
                    int v2 = tris[t + 1];
                    int v3 = tris[t + 2];
                    bool bVert1 = vertexSetMap[v1];
                    bool bVert2 = vertexSetMap[v2];
                    bool bVert3 = vertexSetMap[v3];

                    int cnt = (bVert1 ? 1 : 0) + (bVert2 ? 1 : 0) + (bVert3 ? 1 : 0);

                    if (cnt > 0)
                    {
                        if (cnt == 3)
                        {
                            tl[0].Add(new Triangle(v1, v2, v3, subMesh));
                            continue;
                        }
                        if (separationMode != SeparationMode.Smooth)
                        {
                            if (cnt == 2 || separationMode == SeparationMode.Overcut)
                            {
                                tl[0].Add(new Triangle(v1, v2, v3, subMesh));
                                //continue;
                            }
                            else
                            {
                                tl[1].Add(new Triangle(v1, v2, v3, subMesh));
                                //continue;
                            }
                        }

                        int undercut;
                        bool v1_v2 = bVert1 == bVert2;
                        bool v1_v3 = bVert1 == bVert3;
                        bool v2_v3 = bVert2 == bVert3;
                        if (!v1_v2 && !v1_v3)
                        {
                            undercut = bVert1 ? 1 : 0;

                            affTris[undercut].Add(new Triangle(v1, v2, v3, subMesh));
                        }
                        else if (!v1_v2 && !v2_v3)
                        {
                            undercut = bVert2 ? 1 : 0;

                            affTris[undercut].Add(new Triangle(v2, v3, v1, subMesh));
                        }
                        else if (!v1_v3 && !v2_v3)
                        {
                            undercut = bVert3 ? 1 : 0;

                            affTris[undercut].Add(new Triangle(v3, v1, v2, subMesh));
                        }
                    }
                    else
                    {
                        tl[1].Add(new Triangle(v1, v2, v3, subMesh));
                    }
                }
            }
            return vertexSetMap;
        }

        private void findLoops()
        {
            List<int> pairs = new List<int>();
            int mode = separationMode == SeparationMode.Undercut ? 0 : 1;
            List<Triangle>[] affTris = _affectedTriangles;
            if (separationMode == SeparationMode.Smooth)
            {
                Dictionary<int, Vector3> edgeVertices = new Dictionary<int, Vector3>(affTris[0].Count * 2 / 3);
                List<int>[] newVerticesSubMeshMap = new List<int>[_mesh.subMeshCount];
                Dictionary<int, List<int>> duplicateEdgVer = new Dictionary<int, List<int>>(affTris[0].Count * 2 / 3);
                List<Triangle>[] new_tris = { new List<Triangle>(affTris[0].Count * 2 + affTris[1].Count),
                                          new List<Triangle>(affTris[1].Count * 2 + affTris[0].Count) };

                for (int i = 0; i < affTris.Length; i++)
                {
                    List<Triangle> tris = affTris[i];
                    for (int l = 0; l < tris.Count; l++)
                    {
                        Triangle t = tris[l];
                        int v1 = t.v1;
                        int v2 = t.v2;
                        int v3 = t.v3;
                        /*int hash_a = v1 >= v2 ? v1 * v1 + v1 + v2 : v1 + v2 * v2;
                        int hash_b = v1 >= v3 ? v1 * v1 + v1 + v3 : v1 + v3 * v3;*/
                        int hash_a = v1 > v2 ? (v1 * v1 + v1) / 2 + v2 : (v2 * v2 + v2) / 2 + v1;
                        int hash_b = v1 > v3 ? (v1 * v1 + v1) / 2 + v3 : (v3 * v3 + v3) / 2 + v1;
                        bool hash_a_calculated = edgeVertices.ContainsKey(hash_a);
                        bool hash_b_calculated = edgeVertices.ContainsKey(hash_b);

                        List<int> map = newVerticesSubMeshMap[t.submesh];
                        if (!hash_a_calculated || !hash_b_calculated)
                            if (map == null)
                                map = new List<int>();

                        if (!hash_a_calculated)
                        {
                            map.Add(hash_a);
                            edgeVertices[hash_a] = _triangleSeparator.vertexFunction(_vertices[v1], _vertices[v2]);

                            if (_deepScan)
                            {
                                int d_v1 = _duplicateVertices[v1];
                                int d_v2 = _duplicateVertices[v2];
                                int d_hash_a = d_v1 > d_v2 ? (d_v1 * d_v1 + d_v1) / 2 + d_v2 : (d_v2 * d_v2 + d_v2) / 2 + d_v1;
                                if (!duplicateEdgVer.ContainsKey(d_hash_a)) duplicateEdgVer[d_hash_a] = new List<int>(2);
                                duplicateEdgVer[d_hash_a].Add(hash_a);
                            }
                        }
                        if (!hash_b_calculated)
                        {
                            map.Add(hash_b);
                            edgeVertices[hash_b] = _triangleSeparator.vertexFunction(_vertices[v1], _vertices[v3]);

                            if (_deepScan)
                            {
                                int d_v1 = _duplicateVertices[v1];
                                int d_v3 = _duplicateVertices[v3];
                                int d_hash_b = d_v1 > d_v3 ? (d_v1 * d_v1 + d_v1) / 2 + d_v3 : (d_v3 * d_v3 + d_v3) / 2 + d_v1;
                                if (!duplicateEdgVer.ContainsKey(d_hash_b)) duplicateEdgVer[d_hash_b] = new List<int>(2);
                                duplicateEdgVer[d_hash_b].Add(hash_b);
                            }
                        }
                    }
                }
                Vector3[] new_vertices = _vertices;
                Vector3[] normals = _normals;
                Vector4[] tangents = _tangents;
                Vector2[] uvs = _uv;
                BoneWeight[] bw = _boneWeights;
                Color32[] colors = _vertexColors;
                Array.Resize(ref new_vertices, _vertices.Length + edgeVertices.Count);
                Array.Resize(ref normals, new_vertices.Length);
                Array.Resize(ref tangents, new_vertices.Length);
                Array.Resize(ref uvs, new_vertices.Length);
                Array.Resize(ref bw, new_vertices.Length);
                Array.Resize(ref colors, new_vertices.Length);
                Array.Resize(ref _duplicateVertices, new_vertices.Length);

                Dictionary<int, int> reindex = new Dictionary<int, int>(edgeVertices.Count);
                int index = _vertices.Length;
                if (_deepScan)
                    foreach (KeyValuePair<int, List<int>> l in duplicateEdgVer)
                    {
                        int low = index;
                        foreach (int hash in l.Value)
                        {
                            new_vertices[index] = edgeVertices[hash];
                            _duplicateVertices[index] = low;
                            reindex[hash] = index;
                            index++;
                        }
                    }
                else
                    foreach (KeyValuePair<int, Vector3> v in edgeVertices)
                    {
                        new_vertices[index] = v.Value;
                        _duplicateVertices[index] = index;
                        reindex[v.Key] = index;
                        index++;
                    }
                for (int i = 0; i < affTris.Length; i++)
                {
                    HashSet<int> passedVertices = new HashSet<int>();
                    List<Triangle> tris = affTris[i];
                    for (int l = 0; l < tris.Count; l++)
                    {
                        Triangle t = tris[l];
                        int v1 = t.v1;
                        int v2 = t.v2;
                        int v3 = t.v3;
                        int hash_a = v1 > v2 ? (v1 * v1 + v1) / 2 + v2 : (v2 * v2 + v2) / 2 + v1;
                        int vertex_a = reindex[hash_a];
                        if (!passedVertices.Contains(hash_a))
                        {
                            passedVertices.Add(hash_a);
                            float normalize_a = (new_vertices[vertex_a] - new_vertices[v1]).magnitude / (new_vertices[v2] - new_vertices[v1]).magnitude;
                            int close_vertex = normalize_a < 0.5f ? v1 : v2;

                            uvs[vertex_a] = (uvs[v2] - uvs[v1]) * normalize_a + uvs[v1];
                            normals[vertex_a] = Vector3.Lerp(normals[v1], normals[v2], normalize_a);
                            tangents[vertex_a] = Vector4.Lerp(tangents[v1], tangents[v2], normalize_a);
                            if (_calcBW)
                                bw[vertex_a] = bw[close_vertex];
                            if (_calcCol)
                                colors[vertex_a] = colors[close_vertex];
                        }

                        int hash_b = v1 > v3 ? (v1 * v1 + v1) / 2 + v3 : (v3 * v3 + v3) / 2 + v1;
                        int vertex_b = reindex[hash_b];
                        if (!passedVertices.Contains(hash_b))
                        {
                            passedVertices.Add(hash_b);
                            float normalize_b = (new_vertices[vertex_b] - new_vertices[v1]).magnitude / (new_vertices[v3] - new_vertices[v1]).magnitude;
                            int close_vertex = normalize_b < 0.5f ? v1 : v3;

                            uvs[vertex_b] = (uvs[v3] - uvs[v1]) * normalize_b + uvs[v1];
                            normals[vertex_b] = Vector3.Lerp(normals[v1], normals[v3], normalize_b);
                            tangents[vertex_b] = Vector4.Lerp(tangents[v1], tangents[v3], normalize_b);
                            if (_calcBW)
                                bw[vertex_b] = bw[close_vertex];
                            if (_calcCol)
                                colors[vertex_b] = colors[close_vertex];
                        }

                        new_tris[i].Add(new Triangle(vertex_b, vertex_a, v3, t.submesh));

                        new_tris[i].Add(new Triangle(vertex_a, v2, v3, t.submesh));

                        new_tris[1 - i].Add(new Triangle(vertex_b, v1, vertex_a, t.submesh));

                        if (i == 0)
                        {
                            pairs.Add(vertex_a);
                            pairs.Add(vertex_b);
                        }
                        else
                        {
                            pairs.Add(vertex_b);
                            pairs.Add(vertex_a);
                        }
                    }
                }

                Mesh _smoothMesh = Mesh.Instantiate(_mesh);

                _smoothMesh.vertices = new_vertices;
                _smoothMesh.normals = normals;
                _smoothMesh.tangents = tangents;
                _smoothMesh.uv = uvs;
                if (_calcBW)
                    _smoothMesh.boneWeights = bw;
                if (_calcCol)
                    _smoothMesh.colors32 = colors;

                _vertices = new_vertices;
                _mesh = _smoothMesh;
                _trisList[0].InsertRange(0, new_tris[0]);
                _trisList[1].InsertRange(0, new_tris[1]);
                /* for (int s = 0; s < newVerticesSubMeshMap.Length; s++)
                 {
                     List<int> map = newVerticesSubMeshMap[s];
                     if (map == null)
                         continue;
                     for (int i = 0; i < map.Count; i++)
                         map[i] = reindex[map[i]];
                     int[] new_ind = map.ToArray();
                     int[] old_ind = _mesh.GetIndices(s);
                     int[] all_ind = new int[old_ind.Length + new_ind.Length];
                     Array.Copy(old_ind, 0, all_ind, 0, old_ind.Length);
                     Array.Copy(new_ind, 0, all_ind, old_ind.Length, new_ind.Length);
                     _smoothMesh.SetIndices(all_ind, MeshTopology.Points, s);
                 }*/
            }
            else
            {
                for (int l = 0; l < affTris[mode].Count; l++)
                {
                    if (mode == 1)
                    {
                        pairs.Add(affTris[mode][l].v3);
                        pairs.Add(affTris[mode][l].v2);
                    }
                    else
                    {
                        pairs.Add(affTris[mode][l].v2);
                        pairs.Add(affTris[mode][l].v1);
                        pairs.Add(affTris[mode][l].v1);
                        pairs.Add(affTris[mode][l].v3);
                    }
                }
            }
            List<List<int>> ed = new List<List<int>>();
            _unsortedEdges[0] = ed;
            int curr_loop = 0;
            while (pairs.Count > 0)
            {
                ed.Add(new List<int>());
                ed[curr_loop].Add(pairs[0]);
                ed[curr_loop].Add(pairs[1]);
                pairs.RemoveRange(0, 2);

                bool done = false;
                while (!done)
                {
                    done = true;
                    for (int i = pairs.Count - 2; i >= 0; i -= 2)
                    {
                        if (edgeOverlap(ed[curr_loop], pairs[i], pairs[i + 1]))
                        {
                            ed[curr_loop].Add(pairs[i]);
                            ed[curr_loop].Add(pairs[i + 1]);
                            pairs.RemoveRange(i, 2);
                            done = false;
                        }
                    }
                }
                curr_loop++;
            }
            List<List<int>> ed1 = new List<List<int>>(_unsortedEdges[0].Count);
            foreach (List<int> edge in ed)
            {
                List<int> e = new List<int>(edge);
                for (int i = 0; i < e.Count; i += 2)
                {
                    MeshCalc.Swap(e, i, i + 1);
                }
                ed1.Add(e);
            }
            _unsortedEdges[1] = ed1;
        }

        private AutoResetEvent[] t_flag;
        private LinkedList<Triangle>[] t_trisList;
        private List<List<Triangle>>[] t_fragments;
        private List<List<Triangle>>[] t_fragmentOutline;
        private int[] t_optVertUseIndice;
        private int[][] t_frangmentsMask;
        private readonly int t_minTris = 100;
        private volatile bool threadFailed = false;

        private void findChunks(System.Object num)
        {
            int index = (int)num;
            try
            {
                LinkedList<Triangle> tris_list = t_trisList[index];
                int[] frangments_mask = t_frangmentsMask[index];
                int[] vert_use = _optVirtualVertUse[t_optVertUseIndice[index]];

                List<List<Triangle>> fragments = new List<List<Triangle>>();
                LinkedList<Triangle> live_tris = new LinkedList<Triangle>();

                int curr_fragment = 0;
                while (tris_list.Count > 0)
                {
                    fragments.Add(new List<Triangle>());
                    live_tris.Clear();
                    curr_fragment = fragments.Count - 1;

                    Triangle t_first = tris_list.First.Value;
                    fragments[curr_fragment].Add(t_first);
                    live_tris.AddFirst(t_first);
                    tris_list.RemoveFirst();
                    Interlocked.Decrement(ref vert_use[_duplicateVertices[t_first.v1]]);
                    Interlocked.Decrement(ref vert_use[_duplicateVertices[t_first.v2]]);
                    Interlocked.Decrement(ref vert_use[_duplicateVertices[t_first.v3]]);
                    frangments_mask[_duplicateVertices[t_first.v1]]--;
                    frangments_mask[_duplicateVertices[t_first.v2]]--;
                    frangments_mask[_duplicateVertices[t_first.v3]]--;

                    bool done = false;
                    while (!done)
                    {
                        done = true;
                        LinkedListNode<Triangle> inx = tris_list.First;
                        while (inx != null)
                        {
                            Triangle t = inx.Value;
                            LinkedListNode<Triangle> inx_old = inx;
                            inx = inx.Next;

                            if (trisOverlap(live_tris, t, vert_use))
                            {
                                frangments_mask[_duplicateVertices[t.v1]]--;
                                frangments_mask[_duplicateVertices[t.v2]]--;
                                frangments_mask[_duplicateVertices[t.v3]]--;
                                fragments[curr_fragment].Add(t);
                                live_tris.AddFirst(t);
                                tris_list.Remove(inx_old);
                                done = false;
                            }
                        }
                    }
                }
                //Mask out the outline
                List<List<Triangle>> outlines = new List<List<Triangle>>(fragments.Count);
                for (int i = 0; i < fragments.Count; i++)
                {
                    List<Triangle> fragment = fragments[i];
                    List<Triangle> outline = new List<Triangle>(fragment.Count / 2);
                    for (int n = 0; n < fragment.Count; n++)
                    {
                        Triangle t = fragment[n];
                        if (frangments_mask[_duplicateVertices[t.v1]] +
                            frangments_mask[_duplicateVertices[t.v2]] +
                            frangments_mask[_duplicateVertices[t.v3]] > 0)
                            outline.Add(t);
                    }
                    outline.TrimExcess();
                    outlines.Add(outline);
                }

                fragments.TrimExcess();
                t_fragments[index] = fragments;
                t_fragmentOutline[index] = outlines;
                t_flag[index].Set();
            }
            catch (Exception e)
            {
                threadFailed = true;
                UnityEngine.Debug.LogException(e);
                t_flag[index].Set();
            }
        }

        //SINGLETHREAD
        private void findChunks()
        {
            for (int s = 0; s < 2; s++)
            {
                LinkedList<Triangle> tris_list = new LinkedList<Triangle>(_trisList[s]);
                int[] vert_use = _optVirtualVertUse[s];

                List<List<Triangle>> fragments = _chunks[s];
                LinkedList<Triangle> live_tris = new LinkedList<Triangle>();

                int curr_fragment = 0;
                while (tris_list.Count > 0)
                {
                    fragments.Add(new List<Triangle>());
                    live_tris.Clear();
                    curr_fragment = fragments.Count - 1;

                    Triangle t_first = tris_list.First.Value;
                    fragments[curr_fragment].Add(t_first);
                    live_tris.AddFirst(t_first);
                    tris_list.RemoveFirst();
                    vert_use[_duplicateVertices[t_first.v1]]--;
                    vert_use[_duplicateVertices[t_first.v2]]--;
                    vert_use[_duplicateVertices[t_first.v3]]--;

                    bool done = false;
                    while (!done)
                    {
                        done = true;
                        LinkedListNode<Triangle> inx = tris_list.First;
                        while (inx != null)
                        {
                            Triangle t = inx.Value;
                            LinkedListNode<Triangle> inx_old = inx;
                            inx = inx.Next;

                            if (trisOverlap(live_tris, t, vert_use))
                            {
                                fragments[curr_fragment].Add(t);
                                live_tris.AddFirst(t);
                                tris_list.Remove(inx_old);
                                done = false;
                            }
                        }
                    }
                    fragments[curr_fragment].TrimExcess();
                }
            }
        }

        private bool trisOverlap(LinkedList<Triangle> tris, Triangle tri, int[] vert_use)
        {
            int t_v1 = _duplicateVertices[tri.v1];
            int t_v2 = _duplicateVertices[tri.v2];
            int t_v3 = _duplicateVertices[tri.v3];

            LinkedListNode<Triangle> i = tris.First;
            while (i != null)
            {
                Triangle t = i.Value;
                LinkedListNode<Triangle> i_old = i;
                i = i.Next;
                int v1 = _duplicateVertices[t.v1];
                int v2 = _duplicateVertices[t.v2];
                int v3 = _duplicateVertices[t.v3];
                if (t_v1 == v1 || t_v1 == v2 || t_v1 == v3 ||
                    t_v2 == v1 || t_v2 == v2 || t_v2 == v3 ||
                    t_v3 == v1 || t_v3 == v2 || t_v3 == v3)
                {
                    Interlocked.Decrement(ref vert_use[t_v1]);
                    Interlocked.Decrement(ref vert_use[t_v2]);
                    Interlocked.Decrement(ref vert_use[t_v3]);
                    return true;
                }
                if (vert_use[v1] + vert_use[v2] + vert_use[v3] < 1)
                {
                    tris.Remove(i_old);
                }
            }
            return false;
        }

        private bool edgeOverlap(List<int> list, int p1, int p2)
        {
            int v1 = _duplicateVertices[p1];
            int v2 = _duplicateVertices[p2];

            foreach (int i in list)
            {
                int l = _duplicateVertices[i];
                if (l == v1 || l == v2)
                    return true;
            }
            return false;
        }
    }
}