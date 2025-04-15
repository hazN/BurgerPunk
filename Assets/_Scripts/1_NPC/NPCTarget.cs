using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum TargetType : int
{
    Chair           = 0,
    WaitingQueue    = 1,
}

public class NPCTarget : MonoBehaviour
{
    public TargetType TargetType;
    public bool IsOccupied = false;
}
