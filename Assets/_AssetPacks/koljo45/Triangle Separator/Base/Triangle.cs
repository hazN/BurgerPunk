namespace koljo45.MeshTriangleSeparator
{
    /// <summary>
    /// Triangle data holder
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// Indices to an array which holds vertex data
        /// </summary>
        public int v1, v2, v3;
        /// <summary>
        /// Submesh index which points to a submesh which contains this triangle in a specific mesh 
        /// </summary>
        public int submesh;

        public Triangle(int v1, int v2, int v3, int submesh)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.submesh = submesh;
        }
    }
}