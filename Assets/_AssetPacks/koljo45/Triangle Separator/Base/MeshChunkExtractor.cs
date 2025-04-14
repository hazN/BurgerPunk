namespace koljo45.MeshTriangleSeparator
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Generates and caps meshes using data calculated by the <see cref="TMeshTriangleSeparator"/>
    /// </summary>
    public class MeshChunkExtractor
    {
        /// <summary>
        /// Indicates how "jagged" an edge can be before double edges (vertices) are introduced for more precise normals
        /// </summary>
        static readonly float edgeEnergyThreshold = 0.1f;

        /// <summary>
        /// Mesh from which chunks are to be extracted.
        /// </summary>
        public readonly Mesh _mesh;
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector4[] _tangents;
        private Vector2[] _uv;
        private BoneWeight[] _boneWeights;
        private Color32[] _vertexColors;
        private Matrix4x4[] _bindposes;
        private int subMeshCount;
        private bool calcBW = false;
        private bool calcCol = false;


        private MeshChunkExtractor(Mesh body)
        {
            _mesh = body;
            _vertices = _mesh.vertices;
            _normals = _mesh.normals;
            _tangents = _mesh.tangents;
            _uv = _mesh.uv;
            _boneWeights = _mesh.boneWeights;
            _vertexColors = _mesh.colors32;
            _bindposes = _mesh.bindposes;
            subMeshCount = _mesh.subMeshCount;

            calcBW = _boneWeights.Length != 0;
            calcCol = _vertexColors.Length != 0;
        }

        /// <summary>
        /// Generates a mesh using data calculated by <see cref="TMeshTriangleSeparator"/>
        /// </summary>
        /// <param name="chunk">List of triangles and edges to be extracted into a new mesh, contained within the <see cref="_mesh"/></param>
        /// <param name="outputMesh">Resulting mesh</param>
        /// <returns>A bridge between indices into the parent vertices and indices into the output mesh vertices</returns>
        public Dictionary<int, int> extractChunk(Chunk chunk, out Mesh outputMesh)
        {

            Mesh m = new Mesh();

            List<Triangle> tris = chunk.chunk;
            List<int> indi = MeshCalc.getIndices(tris);
            foreach (List<int> ed in chunk.edges)
                indi.AddRange(ed);
            List<int> vertUsed = MeshCalc.extractIndices(indi, false);
            Dictionary<int, int> reindex = MeshCalc.mapIndices(vertUsed);

            List<int>[] subMeshTris = new List<int>[subMeshCount];
            for (int i = 0; i < subMeshCount; i++)
                subMeshTris[i] = new List<int>();
            Vector3[] verts = new Vector3[vertUsed.Count];
            Vector3[] normals = new Vector3[vertUsed.Count];
            Vector4[] tangents = new Vector4[vertUsed.Count];
            Vector2[] uvs = new Vector2[vertUsed.Count];
            BoneWeight[] b_weights = null;
            Color32[] v_colors = null;

            if (calcBW)
                b_weights = new BoneWeight[vertUsed.Count];
            if (calcCol)
                v_colors = new Color32[vertUsed.Count];

            for (int i = 0; i < vertUsed.Count; i++)
            {
                verts[i] = _vertices[vertUsed[i]];
                normals[i] = _normals[vertUsed[i]];
                tangents[i] = _tangents[vertUsed[i]];
                uvs[i] = _uv[vertUsed[i]];
                if (calcBW)
                    b_weights[i] = _boneWeights[vertUsed[i]];
                if (calcCol)
                    v_colors[i] = _vertexColors[vertUsed[i]];
            }
            for (int i = 0; i < tris.Count; i++)
            {
                Triangle t = tris[i];
                int v1 = reindex[t.v1];
                int v2 = reindex[t.v2];
                int v3 = reindex[t.v3];
                subMeshTris[t.submesh].Add(v1);
                subMeshTris[t.submesh].Add(v2);
                subMeshTris[t.submesh].Add(v3);
            }

            m.vertices = verts;
            m.subMeshCount = subMeshCount;
            for (int s = 0; s < subMeshCount; s++)
                if (subMeshTris[s].Count > 0)
                    m.SetTriangles(subMeshTris[s], s);
                else m.SetTriangles(new List<int> { 0, 0, 0 }, s);
            m.normals = normals;
            m.tangents = tangents;
            m.uv = uvs;
            m.bindposes = _bindposes;
            if (calcBW)
                m.boneWeights = b_weights;
            if (calcCol)
                m.colors32 = v_colors;

            outputMesh = m;
            /*Vector3 oldCenter = m.bounds.center;
            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] -= oldCenter;
            }
            m.vertices = verts;
            m.RecalculateBounds();*/
            return reindex;
        }

        private static Quaternion capOrientation(Vector3[] verts, List<int> indices)
        {
            int third = Mathf.FloorToInt(indices.Count / 3);
            int twothird = Mathf.FloorToInt(indices.Count * 2 / 3);
            Vector3 v1 = verts[indices[0]];
            Vector3 v2 = verts[indices[third]];
            Vector3 v3 = verts[indices[twothird]];
            Vector3 cross = Vector3.Cross(v1 - v2, v3 - v2);
            if ((cross + v2).magnitude < Math.Max(v2.magnitude, cross.magnitude))
                cross = -cross;
            return Quaternion.LookRotation(cross);
        }

        /// <summary>
        /// Caps a mesh with given edges, edges contained within the mesh. Cap consists of generated triangles.
        /// </summary>
        /// <param name="parent">Mesh to be caped</param>
        /// <param name="edges">Edges used for caping, contained within the mesh</param>
        /// <param name="outputSubMesh">Submesh which contains cap triangles</param>
        public static void capMesh(Mesh parent, List<int> edges, int outputSubMesh)
        {
            if (parent == null)
                throw new System.ArgumentNullException("parent", "Input mesh cannot be null");
            if (edges == null)
                throw new System.ArgumentException("edges", "Input edges cannot be null");

            Vector3[] p_vertices = parent.vertices;
            int oldSize = p_vertices.Length;

            Vector3 center = new Vector3();

            List<int> vert_used = MeshCalc.extractIndices(edges, false);

            foreach (int i in vert_used)
                center += p_vertices[i];
            center /= vert_used.Count;

            Dictionary<int, int> reindex = MeshCalc.mapIndices(vert_used, oldSize);

            int[] triangles = new int[edges.Count * 3 / 2];

            Vector2[] uvs = parent.uv;
            Vector3[] normals = parent.normals;
            Vector4[] tangents = parent.tangents;
            BoneWeight[] weights = parent.boneWeights;
            Color32[] colors = parent.colors32;
            bool calcBW = weights.Length != 0;
            bool calcCol = colors.Length != 0;

            Quaternion plane = capOrientation(p_vertices, vert_used);
            Quaternion plane_inverse = Quaternion.Inverse(plane);
            Vector3 v0 = plane_inverse * (p_vertices[vert_used[0]]);
            // calculate uv map limits
            float[] UVLimits_x = { v0.x, v0.x };
            float[] UVLimits_y = { v0.y, v0.y };
            float[] limits_z = { v0.z, v0.z };

            for (int a = 1; a < vert_used.Count; a++)
            {
                Vector3 v = plane_inverse * (p_vertices[vert_used[a]]);
                if (v.x < UVLimits_x[0]) UVLimits_x[0] = v.x;
                if (v.x > UVLimits_x[1]) UVLimits_x[1] = v.x;
                if (v.y < UVLimits_y[0]) UVLimits_y[0] = v.y;
                if (v.y > UVLimits_y[1]) UVLimits_y[1] = v.y;
                if (v.z < limits_z[0]) limits_z[0] = v.z;
                if (v.z > limits_z[1]) limits_z[1] = v.z;
            }
            bool smooth = limits_z[1] - limits_z[0] < edgeEnergyThreshold;

            Array.Resize(ref p_vertices, oldSize + vert_used.Count + (smooth ? 1 : (edges.Count / 2)));
            Array.Resize(ref uvs, p_vertices.Length);
            Array.Resize(ref normals, p_vertices.Length);
            Array.Resize(ref tangents, p_vertices.Length);
            if (calcBW)
                Array.Resize(ref weights, p_vertices.Length);
            if (calcCol)
                Array.Resize(ref colors, p_vertices.Length);

            for (int i = 0; i < vert_used.Count; i++)
            {
                p_vertices[oldSize + i] = p_vertices[vert_used[i]];

                Vector3 v = plane_inverse * (p_vertices[vert_used[i]]);

                uvs[oldSize + i] = new Vector2(MeshCalc.normalizeL(UVLimits_x, v.x), MeshCalc.normalizeL(UVLimits_y, v.y));
                if (calcBW)
                    weights[oldSize + i] = weights[vert_used[i]];
                if (calcCol)
                    colors[oldSize + i] = colors[vert_used[i]];
            }
            for (int i = oldSize + vert_used.Count; i < p_vertices.Length; i++)
            {
                p_vertices[i] = center;

                Vector3 vi = plane_inverse * center;

                uvs[i] = new Vector2(MeshCalc.normalizeL(UVLimits_x, vi.x), MeshCalc.normalizeL(UVLimits_y, vi.y));
                if (calcBW)
                    weights[i] = weights[vert_used[0]];
                if (calcCol)
                    colors[i] = colors[vert_used[0]];
            }

            int centers_start_index = oldSize + vert_used.Count;
            for (int a = 0; a < edges.Count - 1; a += 2)
            {
                int tri_indice = a * 3 / 2; //triangle indice
                triangles[tri_indice + 0] = centers_start_index + (smooth ? 0 : a / 2);
                triangles[tri_indice + 1] = reindex[edges[a]];
                triangles[tri_indice + 2] = reindex[edges[a + 1]];
            }

            parent.vertices = p_vertices;
            parent.uv = uvs;
            if (calcBW)
                parent.boneWeights = weights;
            if (calcCol)
                parent.colors32 = colors;

            int[] old_tris = parent.GetTriangles(outputSubMesh);
            int[] all_tris = new int[old_tris.Length + triangles.Length];
            Array.Copy(old_tris, 0, all_tris, 0, old_tris.Length);
            Array.Copy(triangles, 0, all_tris, old_tris.Length, triangles.Length);
            parent.SetTriangles(all_tris, outputSubMesh);

            parent.RecalculateNormals();
            parent.RecalculateTangents();
            Vector3[] unityNormals = parent.normals;
            Vector4[] unityTangents = parent.tangents;
            for (int i = oldSize; i < normals.Length; i++)
            {
                normals[i] = unityNormals[i];
                tangents[i] = unityTangents[i];
            }
                    
            parent.normals = normals;
            parent.tangents = tangents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body">Mesh from which chunks are to be extracted</param>
        /// <returns><see cref="MeshChunkExtractor"/> instance containing mesh data from which chunks are to be extracted</returns>
        public static MeshChunkExtractor CreateInstance(Mesh body)
        {
            if (body == null)
                throw new System.ArgumentNullException("body", "Mesh cannot be null");
            return new MeshChunkExtractor(body);
        }
    }
}
