using UnityEngine;

public class ERange_BattleState : EnemyState
{
    private EnemyRange enemyRange;

    private float lastTimeShoot = -10;
    private int bulletsShoot = 0;

    private int bulletsPerAttack;
    private float weaponCooldown;

    private float checkCoverTimer = 0;

    public ERange_BattleState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName) : base(enemy, enemyStateMachine, animBoolName)
    {
        enemyRange = enemy as EnemyRange;
    }

    public override void Enter()
    {
        base.Enter();
        enemyRange.agent.isStopped = true;
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

        if(enemyRange.IsSeeingPlayer())
        {
            enemyRange.FaceTarget(enemyRange.aim.position);
        }

        if (!enemyRange.IsPlayerInRange() && IsReadyToLeaveCover())
        {
            enemyRange.stateMachine.TransitionTo(enemyRange.advancePlayerState);
        }

        ChangeCoverIfShould();

        if (WeaponOutOfBullets())
        {
            if (WeaponOnCooldown())
            {
                AttemptToResetWeapon();
            }
            return;
        }


        if (CanShoot() && enemyRange.IsAimOnPlayer())
        {
            Shoot();
        }
    }

    private void ChangeCoverIfShould()
    {
        if(enemyRange.coverPerk != CoverPerk.CanTakeAndChangeCover)
        {
            return;
        }

        checkCoverTimer -= Time.deltaTime;
        if (checkCoverTimer < 0)
        {
            checkCoverTimer = .5f;

            if (ReadyToChangeCover())
            {
                if (enemyRange.CanGetCover())
                {
                    enemyRange.stateMachine.TransitionTo(enemyRange.runToCoverState);
                }
            }
        }
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || IsPlayerClose();

        bool advancedIsOver = Time.time > enemyRange.advancePlayerState.lastTimeAdvanced + enemyRange.advancedTime;

        return inDanger && advancedIsOver;
    }

    #region Cover System

    public bool IsPlayerClose()
    {
        return Vector3.Distance(enemyRange.player.transform.position, enemyRange.transform.position) < enemyRange.safeDistance;
    }

    public bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemyRange.player.transform.position - enemyRange.transform.position;

        if (Physics.Raycast(enemyRange.transform.position, directionToPlayer, out RaycastHit hit))
        {
            return hit.collider.gameObject.GetComponentInParent<Player>();
        }
        return false;
    }

    public bool IsReadyToLeaveCover()
    {
        return Time.time > enemyRange.runToCoverState.lastTimeTookCover + enemyRange.minCoverTime;
    }

    #endregion

    #region Weapon Setup

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

    #endregion
}
