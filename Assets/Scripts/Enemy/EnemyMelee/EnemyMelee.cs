using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    public EMIdle idleState {  get; private set; }
    public EMMove moveState { get; private set; }
    public EMRecovery recoveryState { get; private set; }
    public EMChase chaseState { get; private set; }
    public EMAttack attackState { get; private set; }

    [SerializeField] private Transform hiddenWeapon;
    [SerializeField] private Transform pulledWeapon;

    protected override void Awake()
    {
        base.Awake();

        idleState = new EMIdle(this, stateMachine, "Idle");
        moveState = new EMMove(this, stateMachine, "Move");
        recoveryState = new EMRecovery(this, stateMachine, "Recovery");
        chaseState = new EMChase(this, stateMachine, "Chase");
        attackState = new EMAttack(this, stateMachine, "Attack");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public void PullWeapon()
    {
        this.hiddenWeapon.gameObject.SetActive(false);
        this.pulledWeapon.gameObject.SetActive(true);
    }
}
