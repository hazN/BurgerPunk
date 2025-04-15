using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public enum TargetType
{
    CashRegister,
    Chair,
    WaitingQueue
}

public class NPCTarget : MonoBehaviour
{
    public TargetType TargetType;
}
