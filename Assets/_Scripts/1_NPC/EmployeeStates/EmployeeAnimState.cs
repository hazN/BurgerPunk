using UnityEngine;

public class EmployeeAnimState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EmployeeBehaviour employee = animator.gameObject.GetComponent<EmployeeBehaviour>();
        if (employee != null)
        {
            employee.Animator.SetBool(employee.m_HashCooking, false);
            if (employee.OrderStacked)
            {
                Restaurant.Instance.OrderWrapUp(employee);
                return;
            }
            employee.Animator.SetBool(employee.m_HashMove, true);
            if (employee.OrderItemsMade == employee.PendingOrder.MachinesList.Count - 1)
            {
                employee.NavMeshAgent.destination = employee.Orders_Rack.position;
                employee.OrderStacked = true;
            }
            else
            {
                employee.OrderItemsMade++;
                employee.NavMeshAgent.destination = employee.PendingOrder.MachinesList[employee.OrderItemsMade].position;
            }
        }
    }
}
