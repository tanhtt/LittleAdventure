using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ERange_BattleState : EnemyState
{
    private EnemyRange enemyRange;

    private float lastTimeShoot = -10;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    public ERange_BattleState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        enemyRange = enemy as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
        enemyRange.enemyVisual.EnableIK(true, true);

        bulletsPerAttack = enemyRange.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemyRange.weaponData.GetWeaponCooldown();
    }

    public override void Exit()
    {
        base.Exit();
        enemyRange.enemyVisual.EnableIK(false, false);
    }

    public override void Update()
    {
        base.Update();

        enemyRange.FaceTarget(enemyRange.player.transform.position);

        if (WeaponOutOfBullets())
        {
            if(WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }
            return;
        }


        if (CanShoot())
        {
            Shoot();
        }
    }

    private void AttemptToResetWeapon()
    {
        this.bulletsShoot = 0;
        bulletsPerAttack = enemyRange.weaponData.GetBulletsPerAttack();
        weaponCooldown = enemyRange.weaponData.GetWeaponCooldown();
    }

    private bool WeaponOnCooldown() => Time.time > lastTimeShoot + weaponCooldown;

    private bool WeaponOutOfBullets() => this.bulletsShoot >= bulletsPerAttack;

    private bool CanShoot()
    {
        return Time.time > lastTimeShoot + (1f / enemyRange.weaponData.fireRate);
    }

    private void Shoot()
    {
        enemyRange.ShootSingleBullet();
        lastTimeShoot = Time.time;
        bulletsShoot++;
    }
}
