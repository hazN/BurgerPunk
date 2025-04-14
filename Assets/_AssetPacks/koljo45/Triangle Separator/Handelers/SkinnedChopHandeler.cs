using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using koljo45.MeshTriangleSeparator;
using System;
using System.Diagnostics;

public class SkinnedChopHandeler : MonoBehaviour, ITriangleSeparator {

    [SerializeField]
    private int numThreads = 2;

    public float _boneThreshold = 0.3f;
    public LayerMask slicableLayers;
    public ParticleSystem bloodGush;
    public bool bleed = false;

    private static TMeshTriangleSeparator _slicer;

    public bool Convex
    {
        get
        {
            return true;
        }

        set
        {
            
        }
    }

    public bool DeepScan
    {
        get
        {
            return true;
        }

        set
        {

        }
    }

    public float DuplicateVertexOffset
    {
        get
        {
            return 0;
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
            return SeparationMode.Overcut;
        }

        set
        {

        }
    }

    public Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        throw new NotImplementedException();
    }

    public bool vertexSetCheck(Vector3 p)
    {
        return false;
    }

    public bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return false;
    }
    public bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        float sum = 0;
        if (affBoneIndice.Contains(b.boneIndex0))
            sum += b.weight0;
        if (affBoneIndice.Contains(b.boneIndex1))
            sum += b.weight1;
        if (affBoneIndice.Contains(b.boneIndex2))
            sum += b.weight2;
        if (affBoneIndice.Contains(b.boneIndex3))
            sum += b.weight3;
        return sum >= _boneThreshold;
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return vertexSetCheck(p, b);
    }
    void Awake()
    {
        if (_slicer == null)
            _slicer = new TMeshTriangleSeparator(this);
        affBoneIndice = new List<int>();
    }

    private Transform targetBone;
    private List<int> affBoneIndice;
    void Update () {
        if (Input.GetButtonUp("Bleed"))
            bleed = !bleed;
        if (Input.GetMouseButtonUp(0))
        {
            SkinnedMeshRenderer skin = GetComponentInChildren<SkinnedMeshRenderer>();
            if (skin == null)
            {
                UnityEngine.Debug.LogError("Add a SkinnedMeshRenderer to your hierarchy first");
                return;
            }
            if (_boneThreshold < float.Epsilon) _boneThreshold = float.Epsilon;

            if (GetComponentInChildren<ParticleSystem>()) return;

            RaycastHit hit;
            bool sucess = false;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, slicableLayers);
            if (hit.transform == null) return;
            Transform[] all_bones = skin.bones;
            foreach (Transform bone in all_bones)
                if (bone == hit.transform)
                    sucess = true;
            if (!sucess) return;
            targetBone = hit.transform;

            _slicer.setTriangleSeparator(this);

            affBoneIndice.Clear();
            foreach (Transform t in targetBone.GetComponentsInChildren<Transform>())
            {
                for (int i = 0; i < all_bones.Length; i++)
                    if (all_bones[i] == t)
                        affBoneIndice.Add(i);
            }

            Mesh old_mesh = skin.sharedMesh;
            Mesh new_mesh = null;
            List<Chunk>[] chunks = null;
            if(_slicer.divideMesh(old_mesh, out chunks, out new_mesh))
            {
                if (!(new_mesh.subMeshCount > 1))
                {
                    new_mesh.subMeshCount = new_mesh.subMeshCount + 1;
                }
                MeshChunkExtractor extractor = MeshChunkExtractor.CreateInstance(new_mesh);
                for (int ss = 0; ss < 2; ss++)
                    foreach (Chunk chunk in chunks[ss])
                    {
                        Mesh cm;
                        Dictionary<int, int> reindex = extractor.extractChunk(chunk, out cm);
                        foreach (List<int> edg in chunk.edges)
                        {
                            MeshChunkExtractor.capMesh(cm, MeshCalc.translateIndices(edg, reindex), 1);
                        }
                        makeChunk(cm, ss == 0 ? true : false);
                    }
                Destroy(gameObject);
            }
            else
            {
                UnityEngine.Debug.Log("FAIL!");
            }
        }
    }
    private void makeChunk(Mesh c, bool boneSS)
    {
        GameObject new_go = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
        SkinnedMeshRenderer skin = new_go.GetComponentInChildren<SkinnedMeshRenderer>();
        skin.sharedMesh = c;

        GameObject bone_clone = findGO(new_go, targetBone.name);
        if (boneSS)
        {
            flushBoneExcluding(new_go, bone_clone);
            bone_clone.transform.parent = skin.rootBone.parent;
            skin.rootBone.parent = bone_clone.transform;
            skin.rootBone = bone_clone.transform;
        }
        else
            flushBoneDownwards(bone_clone);

        if (bleed)
        {
            ParticleSystem blood = Instantiate(bloodGush);
            blood.transform.position = bone_clone.transform.position;
            if (boneSS)
                blood.transform.parent = bone_clone.transform;
            else blood.transform.parent = bone_clone.transform.parent;
            Destroy(blood.gameObject, blood.main.duration + 2f);
        }
    }

    private void flushBoneExcluding(GameObject bone, GameObject exclude)
    {
        if (bone != exclude)
            foreach (Transform t in bone.transform)
                flushBoneExcluding(t.gameObject, exclude);
        //DestroyImmediate because removed bones affect RigidBody in the excluded bone and it's children if we use Destroy
        DestroyImmediate(bone.GetComponent<Joint>());
        if (bone != exclude)
        {
            DestroyImmediate(bone.GetComponent<Rigidbody>());
            DestroyImmediate(bone.GetComponent<Collider>());
        }
    }
    private void flushBoneDownwards(GameObject bone)
    {
        foreach (Transform t in bone.transform)
            flushBoneDownwards(t.gameObject);

        Destroy(bone.GetComponent<Joint>());
        Destroy(bone.GetComponent<Rigidbody>());
        Destroy(bone.GetComponent<Collider>());    
    }
    private GameObject findGO(GameObject root, String name)
    {
        if (root.name.Contains(name)) return root;
        foreach (Transform t in root.transform)
        {
            GameObject find = findGO(t.gameObject, name);
            if (find != null) return find;
        }
        return null;
    }
}
