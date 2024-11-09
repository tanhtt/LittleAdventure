using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy_MeleeAttackData
{
    public string attackName;
    public float attackRange;
    public float moveSpeed;
    public int attackIndex;
    [Range(1f, 3f)]
    public float animationSpeed;
    public AttackType_Melee attackType;
}

public enum AttackType_Melee { Close, Charge }
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }

public class EnemyMelee : Enemy
{
    #region States
    public EMIdle idleState {  get; private set; }
    public EMMove moveState { get; private set; }
    public EMRecovery recoveryState { get; private set; }
    public EMChase chaseState { get; private set; }
    public EMAttack attackState { get; private set; }
    public EMDead deadState { get; private set; }
    public EMAbility abilityState { get; private set; }
    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    public Enemy_MeleeWeaponType weaponType;
    [SerializeField] private Transform shieldTransform;

    [Header("Attack Data")]
    public Enemy_MeleeAttackData attackData;
    public List<Enemy_MeleeAttackData> attackList;

    [Header("Dodge Data")]
    [SerializeField] private float dodgeCooldown;
    private float lastTimeDodge = -10;

    [Header("Axe throw ability")]
    public GameObject axePrefab;
    public float axeFlySpeed;
    public float axeAimTimer;
    public float axeThrowCooldown;
    private float lastTimeThrown;
    public Transform axeStartPoint;

    protected override void Awake()
    {
        base.Awake();

        idleState = new EMIdle(this, stateMachine, "Idle");
        moveState = new EMMove(this, stateMachine, "Move");
        recoveryState = new EMRecovery(this, stateMachine, "Recovery");
        chaseState = new EMChase(this, stateMachine, "Chase");
        attackState = new EMAttack(this, stateMachine, "Attack");
        deadState = new EMDead(this, stateMachine, "Idle"); // use ragdoll instead of idle
        abilityState = new EMAbility(this, stateMachine, "AxeThrow");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        InitializeSpeciality();
        enemyVisual.SetupLook();
        UpdateAttackData();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode) return;

        base.EnterBattleMode();
        stateMachine.TransitionTo(recoveryState);
    }

    private void InitializeSpeciality()
    {
        if(meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponType = Enemy_MeleeWeaponType.Throw;
        }

        if(meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }

        if (meleeType == EnemyMelee_Type.Dodge)
        {
            weaponType = Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void GetHit()
    {
        base.GetHit();

        if(healthPoints <= 0)
        {
            stateMachine.TransitionTo(deadState);
        }
    }

    public void ActiveDodgeRoll()
    {
        if (meleeType != EnemyMelee_Type.Dodge) return;
        if (stateMachine.currentState != chaseState) return;

        if (Vector3.Distance(player.transform.position, transform.position) < 2f) return;

        float dodgeAnimationDuration = GetAnimationClipDuration("DodgeRoll");
        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("DodgeRoll");
        }
    }

    public bool CanThrowAxe()
    {
        if(meleeType != EnemyMelee_Type.AxeThrow) return false;

        if(Time.time > axeThrowCooldown + lastTimeThrown)
        {
            lastTimeThrown = Time.time;
            return true;
        }
        return false;
    }

    public bool IsPlayerInAttackRange() => (Vector3.Distance(player.position, transform.position) < attackData.attackRange);

    public void EnableWeaponModel(bool active)
    {
        enemyVisual.currentWeaponModel.gameObject.SetActive(active);
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();
        walkSpeed = walkSpeed * .6f;
        EnableWeaponModel(false);
    }

    public void UpdateAttackData()
    {
        EnemyWeaponModel currentWeapon = enemyVisual.currentWeaponModel.GetComponent<EnemyWeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            this.attackList = new List<Enemy_MeleeAttackData>(currentWeapon.weaponData.attackList);
            turnSpeed = currentWeapon.weaponData.turnSpeed;
        }
    }

    private float GetAnimationClipDuration(string animationName)
    {
        AnimationClip[] animClips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip animClip in animClips)
        {
            if(animClip.name == animationName)
            {
                return animClip.length;
            }
        }
        return 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
    }
}
