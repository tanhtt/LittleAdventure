using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMIdle : EnemyState
{
    private EnemyMelee enemyMelee;

    public EMIdle(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemyMelee = enemy as EnemyMelee;
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

        if (stateTimer < 0)
        {
            enemyStateMachine.TransitionTo(enemyMelee.moveState);
        }
    }
}
