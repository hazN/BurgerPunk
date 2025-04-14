using UnityEngine;
namespace koljo45.MeshTriangleSeparator
{
    [ExecuteInEditMode]
    public abstract class EditorChopHandelerBase : MonoBehaviour, ITriangleSeparator
    {
        public bool _capMesh = true;
        public int _capSubMesh;

        [SerializeField]
        private bool _convex = true;
        [SerializeField]
        private int _numThreads = 2;
        [SerializeField]
        private bool _deepScan = true;
        [SerializeField]
        private float _duplicateVertexOffset = 0.1f;
        [SerializeField]
        private SeparationMode _separationMode = SeparationMode.Smooth;

        private TMeshTriangleSeparator _slicer;

        public abstract void optimizeMesh();

        public abstract void sliceMesh();

        public bool Convex
        {
            get
            {
                return _convex;
            }

            set
            {
                _convex = value;
            }
        }

        public int NumThreads
        {
            get
            {
                return _numThreads;
            }

            set
            {
                _numThreads = value;
            }
        }

        public bool DeepScan
        {
            get
            {
                return _deepScan;
            }

            set
            {
                _deepScan = value;
            }
        }

        public float DuplicateVertexOffset
        {
            get
            {
                return _duplicateVertexOffset;
            }

            set
            {
                _duplicateVertexOffset = value;
            }
        }

        public SeparationMode SeparationMode
        {
            get
            {
                return _separationMode;
            }

            set
            {
                _separationMode = value;
            }
        }

        public abstract bool vertexSetCheck(Vector3 p);

        public abstract bool vertexSetCheck(Vector3 p, BoneWeight b);

        public abstract bool vertexSetCheck(Vector3 p, Color32 c);

        public abstract bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c);

        public abstract Vector3 vertexFunction(Vector3 p1, Vector3 p2);
    }
}
