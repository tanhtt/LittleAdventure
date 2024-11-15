using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERange_AdvancePlayer : EnemyState
{
    private EnemyRange enemyRange;
    private Vector3 playerPos;

    public float lastTimeAdvanced { get; private set; }

    public ERange_AdvancePlayer(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        enemyRange = enemy as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
        enemyRange.agent.isStopped = false;
        enemyRange.agent.speed = enemyRange.advanceSpeed;
        enemyRange.enemyVisual.EnableIK(true, true);
    }

    public override void Exit()
    {
        base.Exit();
        this.lastTimeAdvanced = Time.time;
    }

    public override void Update()
    {
        base.Update();

        playerPos = enemyRange.player.transform.position;

        enemyRange.agent.SetDestination(playerPos);
        enemyRange.FaceTarget(GetNextPathPoint());

        enemyRange.UpdateAimPosition();

        if (CanEnterBattleMode())
        {
            enemyRange.stateMachine.TransitionTo(enemyRange.battleState);
        }
    }

    private bool CanEnterBattleMode()
    {
        return Vector3.Distance(playerPos, enemyRange.transform.position) <= enemyRange.advanceStopDistance
            && enemyRange.IsSeeingPlayer();
    }
}
