using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERange_IdleState : EnemyState
{
    private EnemyRange enemyRange;

    public ERange_IdleState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemyRange = enemy as EnemyRange;
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
            enemyStateMachine.TransitionTo(enemyRange.moveState);
        }

    }
}
