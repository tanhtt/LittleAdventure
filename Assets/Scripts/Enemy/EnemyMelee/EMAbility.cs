using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMAbility : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 direction;
    private const float MAX_ATTACK_DISTANCE = 50;
    private float moveSpeed;

    public EMAbility(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        moveSpeed = enemy.moveSpeed;
        direction = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.moveSpeed = moveSpeed;
        enemy.anim.SetFloat("RecoveryIndex", 0);
    }

    public override void Update()
    {
        base.Update();

        if (enemy.GetManualRotationActive())
        {
            enemy.transform.rotation = enemy.FaceTarget(enemy.player.transform.position);
            direction = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }


        if (enemy.GetManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, direction, enemy.moveSpeed * Time.deltaTime);
        }

        if (triggerCalled)
        {
            enemyStateMachine.TransitionTo(enemy.recoveryState);
        }
    }
}
