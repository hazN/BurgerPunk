using UnityEngine;

public abstract class CustomerBaseState
{
    /// <summary>
    /// When customer enters the state
    /// </summary>
    /// <param name="customer"></param>
    public abstract void EnterState(CustomerBehaviour customer);

    /// <summary>
    /// Frame Update
    /// </summary>
    /// <param name="customer"></param>
    public abstract void Update(CustomerBehaviour customer);
}
