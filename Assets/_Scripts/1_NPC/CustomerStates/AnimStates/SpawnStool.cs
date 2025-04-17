using UnityEngine;

public class SpawnStool : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CustomerBehaviour customer = animator.gameObject.GetComponent<CustomerBehaviour>();
        if (customer != null)
        {
            customer.IsOrderPlaced = true;
            customer.Stool.SetActive(true);
        }
    }
}
