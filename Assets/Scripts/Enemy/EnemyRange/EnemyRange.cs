using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyRange : Enemy
{
    [Header("Cover System")]
    public bool canUseCovers = true;
    public CoverPoint lastCover {  get; private set; }
    public CoverPoint currentCover { get; private set; }

    [Header("Enemy Settings")]
    public Enemy_RangeWeaponType weaponType;
    public EnemyRangeWeaponData weaponData;

    [Space]

    [SerializeField] private List<EnemyRangeWeaponData> availableWeaponData;

    [Header("For Ref")]
    public Transform WeaponHolder;

    [Header("Fire Info")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform gunPoint;

    #region States
    public ERange_IdleState idleState { get; private set; }
    public ERange_MoveState moveState { get; private set; }
    public ERange_BattleState battleState { get; private set; }
    public ERange_RunToCoverState runToCoverState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        idleState = new ERange_IdleState(this, stateMachine, "Idle");
        moveState = new ERange_MoveState(this, stateMachine, "Move");
        battleState = new ERange_BattleState(this, stateMachine, "Battle");
        runToCoverState = new ERange_RunToCoverState(this, stateMachine, "Run");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        enemyVisual.SetupLook();
        SetupWeaponData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public void ShootSingleBullet()
    {
        anim.SetTrigger("Shoot");

        GameObject newBullet = ObjectPool.instance.GetObject(this.enemyBulletPrefab);

        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<EnemyBullet>().BulletSetup();

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
        Vector3 bulletDirection = (player.transform.position - gunPoint.position + new Vector3(0,1,0)).normalized;

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = weaponData.ApplyWeaponSpread(bulletDirection) * weaponData.bulletSpeed;

    }

    public override void EnterBattleMode()
    {
        if (inBattleMode) return;

        base.EnterBattleMode();

        if(CanGetCover())
        {
            stateMachine.TransitionTo(runToCoverState);
        }
        else
        {
            stateMachine.TransitionTo(battleState);
        }
    }

    private void SetupWeaponData()
    {
        List<EnemyRangeWeaponData> filteredWeaponData = new List<EnemyRangeWeaponData>();

        foreach(var weaponData in availableWeaponData)
        {
            if(weaponData.weaponType == this.weaponType)
            {
                filteredWeaponData.Add(weaponData);
            }
        }

        if (filteredWeaponData.Count > 0)
        {
            int random = Random.Range(0, filteredWeaponData.Count);
            weaponData = filteredWeaponData[random];
        }
        else
        {
            Debug.Log("No available weapont data found!");
        }

        gunPoint = enemyVisual.currentWeaponModel.GetComponent<EnemyRangeWeaponModel>().gunPoint;
    }

    private List<Cover> CollectNearbyCovers()
    {
        float radiusCheck = 30;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach(Collider collider in colliders)
        {
            Cover cover = collider.GetComponent<Cover>();

            if (cover != null && collectedCovers.Contains(cover) == false)
            {
                collectedCovers.Add(cover);
            }
        }

        return collectedCovers;
    }

    public bool CanGetCover()
    {
        if (canUseCovers == false) return false;

        currentCover = AttempToFindCoverPoint()?.GetComponent<CoverPoint>();

        if(lastCover != currentCover && currentCover != null)
        {
            return true;
        }

        Debug.LogWarning("No cover found!");
        return false;
    }

    private Transform AttempToFindCoverPoint()
    {
        List<CoverPoint> coverPoints = new List<CoverPoint>();

        foreach(Cover cover in CollectNearbyCovers())
        {
            coverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closetCoverPoint = null;
        float closetDistance = float.MaxValue;

        foreach(CoverPoint coverPoint in coverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if(currentDistance < closetDistance)
            {
                closetCoverPoint = coverPoint;
                closetDistance = currentDistance;
            }
        }

        if(closetCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closetCoverPoint;
            currentCover.SetOccupied(true);

            return currentCover.transform;
        }

        return null;
    }
}
