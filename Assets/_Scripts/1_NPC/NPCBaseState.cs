using UnityEngine;

public abstract class NPCBaseState
{
    /// <summary>
    /// When npc enters the state
    /// </summary>
    /// <param name="npc"></param>
    public abstract void EnterState<T>(T customer);

    /// <summary>
    /// Frame Update
    /// </summary>
    /// <param name="npc"></param>
    public abstract void Update<T>(T npc);
}
