using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine enemyStateMachine;
    protected string animBoolName;
    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy enemy, EnemyStateMachine enemyStateMachine, string animBoolName)
    {
        this.enemyBase = enemy;
        this.enemyStateMachine = enemyStateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        enemyBase.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);
    }

    public void AnimationTrigger() => triggerCalled = true;

    public virtual void AbilityTrigger()
    {

    }

    public Vector3 GetNextPathPoint()
    {
        NavMeshAgent agent = enemyBase.agent;
        NavMeshPath path = agent.path;

        if (path.corners.Length < 2)
        {
            return agent.destination;
        }

        for (int i = 0; i < path.corners.Length; i++)
        {
            if (Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
            {
                return path.corners[i + 1];
            }
        }

        return agent.destination;
    }
}
