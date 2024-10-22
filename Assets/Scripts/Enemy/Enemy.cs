using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int healthPoints = 5;

    public EnemyStateMachine stateMachine { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public Animator anim {  get; private set; }
    public Transform player {  get; private set; }

    private bool manualMovement;
    private bool manualRotation;

    [Header("Idle Info")]
    public float idleTime;
    [SerializeField] private float aggresionRange;

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

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }

    public void ActiveManualMovement(bool active) => this.manualMovement = active;
    public bool GetManualMovementActive() => this.manualMovement;

    public void ActiveManualRotation(bool active) => this.manualRotation = active;
    public bool GetManualRotationActive() => this.manualRotation;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void GetHit()
    {
        healthPoints--;
    }

    public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }

    public bool IsPlayerInAggresionRange() => (Vector3.Distance(player.position, transform.position) < aggresionRange);

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

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
}
