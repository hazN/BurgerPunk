namespace koljo45.MeshTriangleSeparator
{
    using UnityEngine;
    /// <summary>
    /// Holds the parameters used for mesh processing.
    /// Contains user implemented methods called by <see cref="TMeshTriangleSeparator"/>
    /// </summary>
    public interface ITriangleSeparator
    {
        /// <summary>
        /// If set to false additional calculations are used to ensure a proper result for non convex meshes, this takes more time...
        /// </summary>
        bool Convex { get; set; }
        /// <summary>
        /// Number of threads to be used in non convex mesh calculations. Note that this is only used when the convex option is set to false.
        /// If set to 1 the main thread is used for all calculations, no additional threads are commissioned.
        /// </summary>
        int NumThreads { get; set; }
        /// <summary>
        /// Set to true if the mesh contains double edges (double vertices).
        /// Influenced by <see cref="DuplicateVertexOffset"/>
        /// </summary>
        bool DeepScan { get; set; }
        /// <summary>
        /// Maximum distance between two vertices we consider duplicates.
        /// In most cases this should be zero (proper double edges).
        /// If too big (or small) it can cause unwanted results.
        /// </summary>
        float DuplicateVertexOffset { get; set; }
        SeparationMode SeparationMode { get; set; }
        /// <summary>
        /// User implemented method. If true is returned the provided vertex is considered to be a part of the base subset.
        /// </summary>
        /// <param name="p">Vertex position data</param>
        /// <returns>True if vertex is part of the base subset</returns>
        bool vertexSetCheck(Vector3 p);
        /// <summary>
        /// User implemented method. If true is returned the provided vertex is considered to be a part of the base subset.
        /// </summary>
        /// <param name="p">Vertex position data</param>
        /// <param name="b">Vertex BoneWeight data</param>
        /// <returns>True if vertex is part of the base subset</returns>
        bool vertexSetCheck(Vector3 p, BoneWeight b);
        /// <summary>
        /// User implemented method. If true is returned the provided vertex is considered to be a part of the base subset.
        /// </summary>
        /// <param name="p">Vertex position data</param>
        /// <param name="c">Vertex color data</param>
        /// <returns>True if vertex is part of the base subset</returns>
        bool vertexSetCheck(Vector3 p, Color32 c);
        /// <summary>
        /// User implemented method. If true is returned the provided vertex is considered to be a part of the base subset.
        /// </summary>
        /// <param name="p">Vertex position data</param>
        /// <param name="b">Vertex BoneWeight data</param>
        /// <param name="c">Vertex color data</param>
        /// <returns>True if vertex is part of the base subset</returns>
        bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c);
        /// <summary>
        /// User implemented method. Called by <see cref="TMeshTriangleSeparator"/> when <see cref="SeparationMode"/> is set to <see cref="SeparationMode.Smooth"/>.
        /// </summary>
        /// <param name="p1">First vertex of an edge caught between subsets</param>
        /// <param name="p2">Second vertex of an edge caught between subsets</param>
        /// <returns>Position for a new vertex which represents a boundary between subsets</returns>
        Vector3 vertexFunction(Vector3 p1, Vector3 p2);
    }
}
