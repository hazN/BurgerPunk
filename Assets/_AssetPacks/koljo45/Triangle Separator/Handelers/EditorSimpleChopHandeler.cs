using UnityEngine;
using System.Collections.Generic;
using koljo45.MeshTriangleSeparator;
using System;

public class EditorSimpleChopHandeler : EditorChopHandelerBase
{

    public Transform _sword;

    private TMeshTriangleSeparator _slicer;
    private Mesh _mesh;
    private Matrix4x4 _toSword;

    void Awake()
    {
        _slicer = new TMeshTriangleSeparator(this);
    }

    void Start()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter != null)
            _mesh = filter.sharedMesh;
    }

    //Just changing the triangle order, we keep the current mesh, throw away the generated one
    public override void optimizeMesh()
    {
        if (_sword == null)
            throw new ArgumentNullException("_sword", "Cannot optimize, assign a transform to the Sword property first");
        trySkinnedMesh();
        //transform matrix update
        _toSword = _sword.transform.worldToLocalMatrix * transform.localToWorldMatrix;
        TMeshTriangleSeparator opt_slicer = new TMeshTriangleSeparator(OptimizationTriangleSeparator.CreateInstance(this));
        List<Chunk>[] chunks;
        Mesh copy = null;
        if (opt_slicer.divideMesh(_mesh, out chunks, out copy))
        {
            List<int>[] all_tris = new List<int>[_mesh.subMeshCount];
            for (int sm = 0; sm < _mesh.subMeshCount; sm++)
                all_tris[sm] = new List<int>();

            for (int ss = 0; ss < 2; ss++)
                foreach (Chunk c in chunks[ss])
                    foreach (Triangle t in c.chunk)
                    {
                        all_tris[t.submesh].Add(t.v1);
                        all_tris[t.submesh].Add(t.v2);
                        all_tris[t.submesh].Add(t.v3);
                    }
            for (int s = 0; s < _mesh.subMeshCount; s++)
                if (all_tris[s].Count > 0)
                    _mesh.SetTriangles(all_tris[s], s);
                else _mesh.SetTriangles(new List<int> { 0, 0, 0 }, s);
            setMesh(gameObject, _mesh);
        }
    }

    public override void sliceMesh()
    {
        if (_sword == null)
            throw new ArgumentNullException("_sword", "Cannot slice, assign a transform to the Sword property first");
        trySkinnedMesh();
        //transform matrix update
        _toSword = _sword.transform.worldToLocalMatrix * transform.localToWorldMatrix;

        List<Chunk>[] chunks;
        Mesh newMesh = null;
        if (_slicer.divideMesh(_mesh, out chunks, out newMesh))
        {
            MeshChunkExtractor extractor = MeshChunkExtractor.CreateInstance(newMesh);
            for (int ss = 0; ss < 2; ss++)
            {
                foreach (Chunk chunk in chunks[ss])
                {
                    Mesh cm;
                    Dictionary<int, int> reindex = extractor.extractChunk(chunk, out cm);
                    if (_capMesh)
                        foreach (List<int> edge in chunk.edges)
                        {
                            MeshChunkExtractor.capMesh(cm, MeshCalc.translateIndices(edge, reindex), _capSubMesh);
                        }
                    GameObject chunkGO = Instantiate(gameObject);
                    setMesh(chunkGO, cm);
                }
            }
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Mesh slicing failed!");
        }
    }

    private void setMesh(GameObject parent, Mesh chunkMesh)
    {
        MeshFilter filter = parent.GetComponent<MeshFilter>();
        SkinnedMeshRenderer skin = parent.GetComponent<SkinnedMeshRenderer>();
        MeshCollider collider = parent.GetComponent<MeshCollider>();
        if (filter != null)
            filter.sharedMesh = chunkMesh;
        if (skin != null)
            skin.sharedMesh = chunkMesh;
        if (collider != null)
            collider.sharedMesh = chunkMesh;
    }

    public override Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        Vector3 p = _toSword.MultiplyPoint3x4(p1);
        Vector3 v = _toSword.MultiplyVector(p2 - p1);

        Vector3 result = new Vector3(p.x - (v.x / v.z) * p.z, p.y - (v.y / v.z) * p.z, 0);
        Vector3 inverse = _toSword.inverse.MultiplyPoint3x4(result);

        return inverse;
    }

    public override bool vertexSetCheck(Vector3 p)
    {
        return _toSword.MultiplyPoint3x4(p).z < 0;
    }

    public override bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return vertexSetCheck(p);
    }

    public override bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        return vertexSetCheck(p);
    }

    public override bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return vertexSetCheck(p);
    }

    private void trySkinnedMesh()
    {
        SkinnedMeshRenderer skin = GetComponent<SkinnedMeshRenderer>();
        if (skin != null)
            skin.BakeMesh(_mesh);
        if (_mesh == null)
        {
            Debug.LogError("Can't slice - component missing (MeshFilter or SkinnedMeshRenderer)");
            return;
        }
    }
}
