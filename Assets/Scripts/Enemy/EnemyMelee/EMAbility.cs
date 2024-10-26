using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMAbility : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 direction;
    private const float MAX_ATTACK_DISTANCE = 50;
    private float moveSpeed;
    private float lastTimeAxeThrown;

    public EMAbility(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.PullWeapon();

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
            enemy.FaceTarget(enemy.player.transform.position);
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

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        GameObject newAxe = ObjectPool.instance.GetObject(enemy.axePrefab);

        newAxe.transform.position = enemy.axeStartPoint.position;
        newAxe.GetComponent<EnemyAxe>().SetUpAxe(enemy.player, enemy.axeFlySpeed, enemy.axeAimTimer);
    }
}
