using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EMAttack : EnemyState
{
    private EnemyMelee enemy;
    private Vector3 attackDirection;
    private float MAX_ATTACK_DISTANCE = 50f;
    private float attackMoveSpeed;

    public EMAttack(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        this.enemy = enemy as EnemyMelee;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.EnableWeaponModel(true);

        attackMoveSpeed = enemy.attackData.moveSpeed;
        enemy.anim.SetFloat("AttackAnimSpeed", enemy.attackData.animationSpeed);
        enemy.anim.SetFloat("AttackIndex", enemy.attackData.attackIndex);
        enemy.anim.SetFloat("SlashAttackIndex", Random.Range(0,5));


        enemy.agent.isStopped = true;
        enemy.agent.velocity = Vector3.zero;

        attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
    }

    public override void Exit()
    {
        base.Exit();
        SetupNextAttack();
    }

    private void SetupNextAttack()
    {
        float recoveryIndex = IsPlayerClose() ? 1 : 0;
        enemy.anim.SetFloat("RecoveryIndex", recoveryIndex);
        enemy.attackData = GetRandomAttackType();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.GetManualRotationActive())
        {
            enemy.FaceTarget(enemy.player.transform.position);
            attackDirection = enemy.transform.position + (enemy.transform.forward * MAX_ATTACK_DISTANCE);
        }

        if (enemy.GetManualMovementActive())
        {
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, attackDirection, attackMoveSpeed * Time.deltaTime);
        }

        if(triggerCalled)
        {
            if (enemy.IsPlayerInAttackRange())
            {
                enemyStateMachine.TransitionTo(enemy.recoveryState);
            }
            else
            {
                enemyStateMachine.TransitionTo(enemy.chaseState);
            }
        }
    }

    private bool IsPlayerClose() => Vector3.Distance(enemy.transform.position, enemy.player.transform.position) <= 1f;

    private AttackData GetRandomAttackType()
    {
        List<AttackData> attackList = new List<AttackData>(enemy.attackList);

        if (IsPlayerClose())
        {
            attackList.RemoveAll(parameterAttack => parameterAttack.attackType == AttackType_Melee.Charge);
        }
        int randomAttackData = Random.Range(0, attackList.Count);

        return attackList[randomAttackData];
    }
}
