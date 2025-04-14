using UnityEngine;
using koljo45.MeshTriangleSeparator;
using System.Collections;
using System;

public class OptimizationTriangleSeparator : ITriangleSeparator {

    ITriangleSeparator _separator;

    private OptimizationTriangleSeparator(ITriangleSeparator s)
    {
        _separator = s;
    }

    public bool Convex
    {
        get
        {
            return false;
        }
        set {}
    }

    public bool DeepScan
    {
        get
        {
            return _separator.DeepScan;
        }

        set{}
    }

    public float DuplicateVertexOffset
    {
        get
        {
            return _separator.DuplicateVertexOffset;
        }

        set{}
    }

    public int NumThreads
    {
        get
        {
            return 1;
        }

        set{}
    }

    public SeparationMode SeparationMode
    {
        get
        {
            return SeparationMode.Undercut;
        }

        set{}
    }

    public Vector3 vertexFunction(Vector3 p1, Vector3 p2)
    {
        return _separator.vertexFunction(p1, p2);
    }

    public bool vertexSetCheck(Vector3 p)
    {
        return _separator.vertexSetCheck(p);
    }

    public bool vertexSetCheck(Vector3 p, Color32 c)
    {
        return _separator.vertexSetCheck(p, c);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b)
    {
        return _separator.vertexSetCheck(p, b);
    }

    public bool vertexSetCheck(Vector3 p, BoneWeight b, Color32 c)
    {
        return _separator.vertexSetCheck(p, b, c);
    }

    public static OptimizationTriangleSeparator CreateInstance(ITriangleSeparator s)
    {
        if (s == null)
            throw new System.ArgumentNullException("s", "Separator cannot be null");
        return new OptimizationTriangleSeparator(s);
    }
}
