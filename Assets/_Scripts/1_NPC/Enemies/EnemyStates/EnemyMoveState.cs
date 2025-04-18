using UnityEngine;

public class EnemyMoveState : NPCBaseState
{
    EnemyBehaviour enemy;
    public override void EnterState<T>(T npc)
    {
        enemy = npc as EnemyBehaviour;
        enemy.Animator.SetBool(enemy.m_HashMove, true);
    }

    public override void Update()
    {
        if(Helper.HaveReached(enemy.NavMeshAgent))
        {
            enemy.Animator.SetBool(enemy.m_HashMove, false);
            enemy.TransitionToState(enemy.mEnemyAttackState);
        }
    }
}
