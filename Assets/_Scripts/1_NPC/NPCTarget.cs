using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum TargetType : int
{
    DineIn      = 0,
    TakeOut     = 1,
}

public class NPCTarget : MonoBehaviour
{
    public TargetType TargetType;
    public bool IsOccupied = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
