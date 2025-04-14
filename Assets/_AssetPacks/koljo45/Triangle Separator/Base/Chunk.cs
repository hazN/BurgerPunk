using System.Collections.Generic;
namespace koljo45.MeshTriangleSeparator
{
    /// <summary>
    /// Chunk data holder.
    /// </summary>
    public struct Chunk
    {
        /// <summary>
        /// Triangles that make up the chunk
        /// </summary>
        public List<Triangle> chunk;
        /// <summary>
        /// Cutoff edges calculated by <see cref="TMeshTriangleSeparator"/>. Made up from pairs, one pair coresponds to a single edge
        /// </summary>
        public List<List<int>> edges;

        public Chunk(List<Triangle> c, List<List<int>> e)
        {
            chunk = c;
            edges = e;
        }
    }
}