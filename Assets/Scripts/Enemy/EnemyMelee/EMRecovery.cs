using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMRecovery : EnemyState
{
    private EnemyMelee enemy;

    public EMRecovery(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemy.transform.rotation = enemy.FaceTarget(enemy.player.transform.position);

        if(triggerCalled)
        {
            enemyStateMachine.TransitionTo(enemy.chaseState);
        }
    }
}
