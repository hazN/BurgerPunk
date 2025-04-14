namespace koljo45.MeshTriangleSeparator
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public static class MeshCalc
    {

        public static float normalizeL(float[] limits, float value)
        {
            return (value - limits[0]) / (limits[1] - limits[0]);
        }

        /// <summary>
        /// Obtain a qualitative image of an index group
        /// </summary>
        /// <param name="index_group">Index group</param>
        /// <param name="sort">Output set sorted</param>
        /// <returns>Qualitative image stored in a HashSet</returns>
        public static List<int> extractIndices(List<int> index_group, bool sort)
        {
            if (index_group == null)
                throw new System.ArgumentNullException("index_group", "Input index group cannot be null");

            HashSet<int> extract = new HashSet<int>(Enumerable.Range(0, index_group.Count));
            extract.Clear();

            foreach (int i in index_group)
                extract.Add(i);

            List<int> list = extract.ToList();
            if (sort)
                list.Sort();

            return list;
        }

        public static List<int> getIndices(List<Triangle> triangles)
        {
            if (triangles == null)
                throw new System.ArgumentNullException("triangles", "Input triangles cannot be null");

            List<int> indices = new List<int>(triangles.Count * 3);
            foreach (Triangle t in triangles)
            {
                indices.Add(t.v1);
                indices.Add(t.v2);
                indices.Add(t.v3);
            }
            return indices;
        }

        public static void addRange<T>(List<T> source, List<T> destination, int index, int count)
        {
            if (source == null)
                throw new System.ArgumentNullException("source", "Source list cannot be null");
            if (destination == null)
                throw new System.ArgumentNullException("destination", "Destination list cannot be null");
            if (index < 0)
                throw new System.ArgumentOutOfRangeException("index", "Index cannot be negative");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Count cannot be negative");
            if (source.Count - index < count)
                throw new System.ArgumentException("index, count", "Requested range out of range");

            for (int i = index; i < (index + count); i++)
                destination.Add(source[i]);
        }

        public static void addRange<T>(List<T> destination, LinkedListNode<T> start, int count)
        {
            if (destination == null)
                throw new System.ArgumentNullException("destination", "Destination list cannot be null");
            if (start == null)
                throw new System.ArgumentNullException("start", "Starting linked list node cannot be null");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Count cannot be negative");

            LinkedListNode<T> node = start;
            for (int i = 0; i < count; i++)
            {
                if (node == null) break;
                destination.Add(node.Value);
                node = node.Next;
            }
        }

        public static void addRange<T>(LinkedList<T> destination, LinkedListNode<T> start, int count)
        {
            if (destination == null)
                throw new System.ArgumentNullException("destination", "Destination linked list cannot be null");
            if (start == null)
                throw new System.ArgumentNullException("start", "Starting linked list node cannot be null");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Count cannot be negative");

            LinkedListNode<T> node = start;
            for (int i = 0; i < count; i++)
            {
                if (node == null) break;
                destination.AddLast(node.Value);
                node = node.Next;
            }
        }

        public static void RemoveRange<T>(LinkedListNode<T> start, int count)
        {
            if (start == null)
                throw new System.ArgumentNullException("start", "Starting linked list node cannot be null");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Count cannot be negative");

            LinkedListNode<T> node = start;
            for (int i = 0; i < count; i++)
            {
                LinkedListNode<T> next = node.Next;
                node.List.Remove(node);
                if (next == null) break;
                node = next;
            }
        }

        public static Dictionary<int, int> mapIndices(List<int> indices)
        {
            if (indices == null)
                throw new System.ArgumentNullException("indices", "Index list cannot be null");

            Dictionary<int, int> d = new Dictionary<int, int>(indices.Count);
            for (int i = 0; i < indices.Count; i++)
                d[indices[i]] = i;
            return d;
        }

        public static Dictionary<int, int> mapIndices(List<int> indices, int offset)
        {
            if (indices == null)
                throw new System.ArgumentNullException("indices", "Index list cannot be null");

            Dictionary<int, int> d = new Dictionary<int, int>(indices.Count);
            for (int i = 0; i < indices.Count; i++)
                d[indices[i]] = i + offset;
            return d;
        }

        public static List<int> translateIndices(List<int> source, Dictionary<int, int> map)
        {
            if (source == null)
                throw new System.ArgumentNullException("source", "Source list cannot be null");
            if (map == null)
                throw new System.ArgumentNullException("map", "Map cannot be null");

            List<int> t = new List<int>(source.Count);
            try
            {
                for (int i = 0; i < source.Count; i++)
                    t.Add(map[source[i]]);
                return t;
            }
            catch (KeyNotFoundException)
            {
                UnityEngine.Debug.LogError("Provided index list could not be translated. Please make sure you are using a map that corresponds to your index list");
                throw;
            }
        }

        public static void Swap(IList<int> list, int indexA, int indexB)
        {
            int tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}
