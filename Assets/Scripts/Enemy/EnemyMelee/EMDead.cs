using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMDead : EnemyState
{
    private EnemyMelee enemy;
    private EnemyRagdoll ragdoll;

    private bool interactionDisabled;

    public EMDead(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
        ragdoll = enemy.GetComponent<EnemyRagdoll>();
    }

    public override void Enter()
    {
        base.Enter();

        this.interactionDisabled = false;

        enemy.anim.enabled = false;
        enemy.agent.isStopped = true;

        ragdoll.RagdollActive(true);

        stateTimer = 1.5f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        DisableInteraction();
    }

    private void DisableInteraction()
    {
        if (stateTimer < 0 && interactionDisabled == false)
        {
            interactionDisabled = true;
            ragdoll.RagdollActive(false);
            ragdoll.CollidersActive(false);
        }
    }
}
