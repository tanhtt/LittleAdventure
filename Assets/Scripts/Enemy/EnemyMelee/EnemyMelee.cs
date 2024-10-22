using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
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
public enum EnemyMelee_Type { Regular, Shield, Dodge }

public class EnemyMelee : Enemy
{
    public EMIdle idleState {  get; private set; }
    public EMMove moveState { get; private set; }
    public EMRecovery recoveryState { get; private set; }
    public EMChase chaseState { get; private set; }
    public EMAttack attackState { get; private set; }
    public EMDead deadState { get; private set; }
    public EMAbility abilityState { get; private set; }

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType;
    [SerializeField] private Transform shieldTransform;

    [Header("Attack Data")]
    public AttackData attackData;
    public List<AttackData> attackList;

    [Header("Dodge Data")]
    [SerializeField] private float dodgeCooldown;
    private float lastTimeDodge;

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

        lastTimeDodge = Time.time;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    private void InitializeSpeciality()
    {
        if(meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange);
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

        if (Time.time > dodgeCooldown + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("DodgeRoll");
        }
    }

    public bool IsPlayerInAttackRange() => (Vector3.Distance(player.position, transform.position) < attackData.attackRange);

    public void PullWeapon()
    {
        this.hiddenWeapon.gameObject.SetActive(false);
        this.pulledWeapon.gameObject.SetActive(true);
    }

    public void TriggerAbility()
    {
        moveSpeed = moveSpeed * .6f;
        Debug.Log("Throw axe");
        pulledWeapon.gameObject.SetActive(false);
    }
}
