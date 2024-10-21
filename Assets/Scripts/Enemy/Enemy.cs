using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine stateMachine { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public Animator anim {  get; private set; }
    public Transform player {  get; private set; }

    [Header("Attack Data")]
    [SerializeField] private float aggresionRange;
    [SerializeField] private float attackRange;
    [SerializeField] public float attackMoveSpeed;

    private bool manualMovement;

    [Header("Idle Info")]
    public float idleTime;

    [Header("Move Data")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float chaseSpeed;

    [Space]
    [SerializeField] private float turnSpeed;
    [SerializeField] private Transform[] patrolDestinations;
    private int currentDestinationIndex;

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        InitialPatrolPoints();
    }

    protected virtual void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void ActiveManualMovement(bool active) => this.manualMovement = active;
    public bool GetManualMovementActive() => this.manualMovement;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public bool IsPlayerInAggresionRange() => (Vector3.Distance(player.position, transform.position) < aggresionRange);
    public bool IsPlayerInAttackRange() => (Vector3.Distance(player.position, transform.position) < attackRange);

    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolDestinations[currentDestinationIndex].transform.position;

        currentDestinationIndex++;

        if(currentDestinationIndex >= patrolDestinations.Length)
        {
            currentDestinationIndex = 0;
        }

        return destination;
    }

    private void InitialPatrolPoints()
    {
        foreach (Transform t in patrolDestinations)
        {
            t.SetParent(null);
        }
    }

    public Quaternion FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }
}
