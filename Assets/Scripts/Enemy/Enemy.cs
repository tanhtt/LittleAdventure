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
    public EnemyVisual enemyVisual { get; private set; }

    protected bool inBattleMode = false;

    [Header("Idle Info")]
    public float idleTime;
    [SerializeField] private float aggresionRange;

    [Header("Move Data")]
    [SerializeField] public float walkSpeed = 1.5f;
    [SerializeField] public float runSpeed = 3f;
    private bool manualMovement;
    private bool manualRotation;

    [Space]
    [SerializeField] protected float turnSpeed;

    [Header("Patrol Points Data")]
    [SerializeField] private Transform[] patrolDestinations;
    private Vector3[] patrolPointsPosition;
    private int currentDestinationIndex;

    protected virtual void Awake()
    {
        stateMachine = new EnemyStateMachine();
        
        enemyVisual = GetComponent<EnemyVisual>();

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
        if (ShouldEnterBattleMode())
        {
            EnterBattleMode();
        }
    }

    protected bool ShouldEnterBattleMode()
    {
        bool isInAgressionRange = (Vector3.Distance(player.position, transform.position) < aggresionRange);
        if(isInAgressionRange && !inBattleMode)
        {
            EnterBattleMode();
            return true;
        }
        return false;
    }

    public virtual void EnterBattleMode()
    {
        inBattleMode = true;
    }

    public void FaceTarget(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        Vector3 currentEulerAngles = transform.rotation.eulerAngles;

        float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
    }

    #region Animation Event
    public void ActiveManualMovement(bool active) => this.manualMovement = active;
    public bool GetManualMovementActive() => this.manualMovement;

    public void ActiveManualRotation(bool active) => this.manualRotation = active;
    public bool GetManualRotationActive() => this.manualRotation;

    public void AnimationTrigger() => stateMachine.currentState.AnimationTrigger();

    public virtual void AbilityTrigger()
    {
        stateMachine.currentState.AbilityTrigger();
    }
    #endregion

    #region Hit Logic
    public virtual void GetHit()
    {
        EnterBattleMode();
        healthPoints--;
    }

    public virtual void DeathImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        StartCoroutine(DeathImpactCoroutine(force, hitPoint, rb));
    }

    private IEnumerator DeathImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);

        rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
    #endregion

    #region Patrol points
    public Vector3 GetPatrolDestination()
    {
        Vector3 destination = patrolPointsPosition[currentDestinationIndex];

        currentDestinationIndex++;

        if(currentDestinationIndex >= patrolPointsPosition.Length)
        {
            currentDestinationIndex = 0;
        }

        return destination;
    }

    private void InitialPatrolPoints()
    {
        patrolPointsPosition = new Vector3[patrolDestinations.Length];

        for(int i = 0; i < patrolDestinations.Length; i++)
        {
            patrolPointsPosition[i] = patrolDestinations[i].position;
            patrolDestinations[i].gameObject.SetActive(false);
        }
    }

    #endregion

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, aggresionRange);
    }
}
