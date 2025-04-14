using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using koljo45.MeshTriangleSeparator;

[RequireComponent(typeof(MeshFilter))]
public class SimpleChopHandeler : MonoBehaviour, ITriangleSeparator
{
    public GameObject _sword;

    /// <see cref="ITriangleSeparator"/>
    [SerializeField]
    private bool convex = false;
    [SerializeField]
    private bool deepScan = true;
    public bool capMesh = true;
    [SerializeField]
    private SeparationMode separationMode = SeparationMode.Smooth;
    [SerializeField]
    private int threads = 4;
    //Number of triangles a new chunk must contain
    public uint minChunkSize;
    public LayerMask slicableLayers;

    private static TMeshTriangleSeparator _slicer;
    Mesh _myMesh;
    //Transform vertex position from this transform to the sword transform
    Matrix4x4 _toSword;

    void Awake()
    {
        //sharing a single slicer instance
        if (_slicer == null)
            _slicer = new TMeshTriangleSeparator(this);
    }

    void Start()
    {
        _myMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    /// <see cref="ITriangleSeparator"/>
    public bool Convex
    {
        get
        {
            return convex;
        }

        set
        {
            convex = value;
        }
    }

    public int NumThreads
    {
        get
        {
            return threads;
        }

        set
        {
            threads = value;
        }
    }

    public bool DeepScan
    {
        get
        {
            return deepScan;
        }

        set
        {
            deepScan = value;
        }
    }

    public float DuplicateVertexOffset
    {
        get
        {
            return 0.1f;
        }
        set
        {

        }
    }

    public SeparationMode SeparationMode
    {
        get
        {
            return separationMode;
        }

        set
        {
            separationMode = value;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //transform matrix update
            _toSword = _sword.transform.worldToLocalMatrix * transform.localToWorldMatrix;
            //only slice objects in front of the sword
            RaycastHit hit;
            Physics.Raycast(_sword.transform.position, _sword.transform.right, out hit, 100, slicableLayers);
            if (hit.collider != GetComponent<Collider>()) return;
            //we have to do this because we are sharing a single slicer instance between all gameobjects, they "take turns"...
            _slicer.setTriangleSeparator(this);
            Stopwatch s1, s2, s3;

            s1 = Stopwatch.StartNew();
            s2 = new Stopwatch();
            s3 = new Stopwatch();
            //Chunks formed out of neighbouring triangles. No two chunks are interconnected
            List<Chunk>[] chunks;
            Mesh copy = null;
            if (_slicer.divideMesh(_myMesh, out chunks, out copy))
            {
                s1.Stop();
                //make sure we have another submesh for the cap
                if (!(copy.subMeshCount > 1))
                {
                    copy.subMeshCount = copy.subMeshCount + 1;
                    copy.SetTriangles(new int[] { 0, 0, 0 }, copy.subMeshCount - 1);
                }
                MeshChunkExtractor extractor = MeshChunkExtractor.CreateInstance(copy);
                for (int w = 0; w < 2; w++)
                    foreach (Chunk chunk in chunks[w])
                    {
                        if (chunk.chunk.Count <= minChunkSize) continue;
                        Mesh cm;

                        s2.Start();
                        Dictionary<int, int> reindex = extractor.extractChunk(chunk, out cm);
                        if (capMesh)
                            foreach (List<int> ed in chunk.edges)
                            {
                                try
                                {
                                    List<int> edReindexed = MeshCalc.translateIndices(ed, reindex);
                                    MeshChunkExtractor.capMesh(cm, edReindexed, 1);
                                }
                                catch (KeyNotFoundException)
                                {
                                    UnityEngine.Debug.LogError("Mesh could not be caped with given edges");
                                }
                                catch (System.Exception e)
                                {
                                    UnityEngine.Debug.LogError(e);
                                }
                            }
                        s2.Stop();

                        s3.Start();

                        GameObject go = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
                        //go.GetComponent<ChopHandeler>().enabled = false;
                        go.GetComponentInChildren<MeshFilter>().sharedMesh = cm;

                        /*BoxCollider col = go.GetComponent<BoxCollider>();
                        if (col == null) col = go.AddComponent<BoxCollider>();
                        col.size = chunk.bounds.size;
                        col.center = chunk.bounds.center;*/
                        Collider col = go.GetComponent<Collider>();
                        MeshCollider mCol;
                        if (col as MeshCollider == null)
                        {
                            if (col != null)
                                Destroy(col);
                            mCol = go.AddComponent<MeshCollider>();
                        }
                        else
                            mCol = col as MeshCollider;

                        mCol.convex = true;
                        mCol.sharedMesh = cm;
                        if (go.GetComponent<Rigidbody>() == null) go.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                        Rigidbody parentRB = GetComponent<Rigidbody>();
                        if (parentRB != null)
                            go.GetComponent<Rigidbody>().linearVelocity = parentRB.linearVelocity;
                        s3.Stop();
                    }

                UnityEngine.Debug.Log("SUCESS!");

                UnityEngine.Debug.Log("Division: " + s1.ElapsedMilliseconds);
                UnityEngine.Debug.Log("Extraction: " + s2.ElapsedMilliseconds);
                UnityEngine.Debug.Log("Instantiation: " + s3.ElapsedMilliseconds);

                Destroy(gameObject);
            }
            else UnityEngine.Debug.Log("FAIL!");
        }
    }

    public bool vertexSetCheck(Vector3 p)
    {
        return _toSword.MultiplyPoint3x4(p).z < 0;
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        Vector3 p = _toSword.MultiplyPoint3x4(p1);
        Vector3 v = _toSword.MultiplyVector(p2 - p1);

        Vector3 result = new Vector3(p.x - (v.x / v.z) * p.z, p.y - (v.y / v.z) * p.z, 0);
        Vector3 inverse = _toSword.inverse.MultiplyPoint3x4(result);

        return inverse;
    }
}
