using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMIdle : EnemyState
{
    private EnemyMelee enemy;

    public EMIdle(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = enemyBase.idleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerInAggresionRange())
        {
            enemyStateMachine.TransitionTo(enemy.recoveryState);
            return;
        }

        if (stateTimer < 0)
        {
            enemyStateMachine.TransitionTo(enemy.moveState);
        }
    }
}
