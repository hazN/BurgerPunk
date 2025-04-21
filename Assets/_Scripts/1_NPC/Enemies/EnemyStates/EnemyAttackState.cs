using UnityEngine;

public class EnemyAttackState : NPCBaseState
{
    EnemyBehaviour enemy;
    public override void EnterState<T>(T npc)
    {
        enemy = npc as EnemyBehaviour;
        enemy.Animator.SetBool(enemy.m_HashAttack, true);
    }

    public override void Update()
    {
    }
}
