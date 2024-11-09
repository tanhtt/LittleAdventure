using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERange_MoveState : EnemyState
{
    private EnemyRange enemyRange;
    private Vector3 destination;

    public ERange_MoveState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        enemyRange = enemy as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
        enemyRange.agent.speed = enemyRange.walkSpeed;

        destination = enemyRange.GetPatrolDestination();
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

        if (enemyRange.agent.remainingDistance <= enemyRange.agent.stoppingDistance + .05f)
        {
            enemyStateMachine.TransitionTo(enemyRange.idleState);
        }
    }
}
