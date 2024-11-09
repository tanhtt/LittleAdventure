using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERange_RunToCoverState : EnemyState
{
    private EnemyRange enemyRange;
    private Vector3 destination;

    public ERange_RunToCoverState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        enemyRange = enemy as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
        destination = enemyRange.currentCover.transform.position;

        enemyRange.enemyVisual.EnableIK(true, false);

        enemyRange.agent.speed = enemyRange.runSpeed;
        enemyRange.agent.isStopped = false;
        enemyRange.agent.SetDestination(destination);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        enemyRange.FaceTarget(GetNextPathPoint());

        if(Vector3.Distance(enemyRange.transform.position, destination) < .5f)
        {
            enemyRange.stateMachine.TransitionTo(enemyRange.battleState);
        }
    }
}
