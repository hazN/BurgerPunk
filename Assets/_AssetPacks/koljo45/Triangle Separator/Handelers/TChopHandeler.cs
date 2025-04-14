using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using koljo45.MeshTriangleSeparator;

public class TChopHandeler : MonoBehaviour, ITriangleSeparator {

    public GameObject _sword;
    /// <see cref="ITriangleSeparator"/>
    [SerializeField]
    private bool convex = true;
    [SerializeField]
    private bool deepScan = true;
    public bool capMesh = true;
    [SerializeField]
    private SeparationMode separationMode = SeparationMode.Smooth;
    [SerializeField]
    private int numThreads = 4;
    public LayerMask slicableLayers;
    //Transform vertex position from this transform to the sword transform
    private Matrix4x4 _toSword;

    private static TMeshTriangleSeparator _slicer;
    private Mesh _myMesh;

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

    public int NumThreads
    {
        get
        {
            return numThreads;
        }

        set
        {
            numThreads = value;
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

    public Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        Vector3 p = _toSword.MultiplyPoint3x4(p1);
        Vector3 v = _toSword.MultiplyVector(p2 - p1);

        Vector3 result = new Vector3(p.x - (v.x / v.z) * p.z, p.y - (v.y / v.z) * p.z, 0);
        Vector3 inverse = _toSword.inverse.MultiplyPoint3x4(result);

        return inverse;
    }

    public bool vertexSetCheck(Vector3 p)
    {
        return _toSword.MultiplyPoint3x4(p).z < 0;
    }

    public bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        return vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return vertexSetCheck(p);
    }

    void Awake()
    {
        //sharing a single slicer instance
        if (_slicer == null)
            _slicer = new TMeshTriangleSeparator(this);
    }

    void Start () {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
            _myMesh = filter.sharedMesh;
	}

    private volatile bool done = false;
    //Chunks formed out of neighbouring triangles. No two chunks are interconnected
    List<Chunk>[] _chunks;
    //Contains vertex (color, normal...) data that coresponds to the chunks and edges indices
    Mesh newMesh;
    void Update () {
        if (Input.GetMouseButtonUp(0))
        {
            SkinnedMeshRenderer skin = GetComponent<SkinnedMeshRenderer>();
            if (skin != null)
                skin.BakeMesh(_myMesh);
            if (_myMesh == null)
            {
                Debug.LogError("Can't slice - component missing (MeshFilter or SkinnedMeshRenderer)");
                return;
            }
            //transform matrix update
            _toSword = _sword.transform.worldToLocalMatrix * transform.localToWorldMatrix;
            //only slice objects in front of the sword
            RaycastHit hit;
            Physics.Raycast(_sword.transform.position, _sword.transform.right, out hit, 100, slicableLayers);
            if (hit.collider != GetComponent<Collider>()) return;
            //we have to do this because we are sharing a single slicer instance between all gameobjects, they "take turns"...
            _slicer.setTriangleSeparator(this);
            _slicer.divideMesh(_myMesh, eventHandeler);
        }
        if (done)
        {
            try
            {
                //make sure we have another submesh for the cap
                if (!(newMesh.subMeshCount > 1))
                {
                    newMesh.subMeshCount = newMesh.subMeshCount + 1;
                    newMesh.SetTriangles(new int[] { 0, 0, 0 }, newMesh.subMeshCount - 1);
                }
                MeshChunkExtractor extractor = MeshChunkExtractor.CreateInstance(newMesh);
                for (int ss = 0; ss < 2; ss++)
                {
                    foreach (Chunk chunk in _chunks[ss])
                    {
                        Mesh cm;
                        Dictionary<int, int> reindex = extractor.extractChunk(chunk, out cm);
                        if(capMesh)
                        foreach (List<int> edge in chunk.edges)
                        {
                            MeshChunkExtractor.capMesh(cm, MeshCalc.translateIndices(edge, reindex), 1);
                        }
                        makeChunk(cm);
                    }
                }
                Destroy(gameObject);
            }catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            finally
            {
                done = false;
            }
        }
	}

    public void eventHandeler(List<Chunk>[] chunks, Mesh m)
    {
        _chunks = chunks;
        newMesh = m;
        done = true;
    }

    private void makeChunk(Mesh chunkMesh)
    {
        GameObject chunk = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
        MeshFilter filter = chunk.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skin = chunk.GetComponent<SkinnedMeshRenderer>();
        if (filter != null)
            filter.sharedMesh=chunkMesh;
        if (skin != null)
            skin.sharedMesh=chunkMesh;

        Collider col = chunk.GetComponent<Collider>();
        MeshCollider mCol;
        if (col as MeshCollider == null)
        {
            if (col != null)
                Destroy(col);
            mCol = chunk.AddComponent<MeshCollider>();
        }
        else
            mCol = col as MeshCollider;
        mCol.convex = true;
        mCol.sharedMesh = chunkMesh;

        if (chunk.GetComponent<Rigidbody>() == null) chunk.AddComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        Rigidbody parentRB = GetComponent<Rigidbody>();
        if (parentRB != null)
            chunk.GetComponent<Rigidbody>().linearVelocity = parentRB.linearVelocity;
    }
}
