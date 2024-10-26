using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMChase : EnemyState
{
    private EnemyMelee enemy;
    private float lastestUpdatedTime;

    public EMChase(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerInAttackRange())
        {
            enemyStateMachine.TransitionTo(enemy.attackState);
        }

        enemy.FaceTarget(GetNextPathPoint());

        if (CanUpdateDestination())
        {
            enemy.agent.SetDestination(enemy.player.transform.position);
        }
    }

    private bool CanUpdateDestination()
    {
        if (Time.time >= lastestUpdatedTime + 0.25f)
        {
            lastestUpdatedTime = Time.time;
            return true;
        }

        return false;
    }
}
