using System.Collections;
using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Wait,
        Patrol,
        Chase,
        Attack,
        TakeHit,
        Die
    }

    [Header("Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private GameObject Weapone;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float gravity = -20f;

    [Header("Detection Ranges")]
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float attackRange = 2f;

    [Header("Timer Values")]
    [SerializeField] private float patrolWaitTime = 3f;
    [SerializeField] private float attackCooldownTime = 3f;
    [SerializeField] private float hitStateDurationTime = 2f;
    [SerializeField] private float attackAnimationCompleteTime = 1f;

    // State
    public EnemyState CurrentState { get; private set; } = EnemyState.Idle;

    // Components
    private CharacterController controller;

    // Patrol
    private Transform currentPatrolPoint;

    // Movement
    private Vector3 targetDirection;
    private float verticalVelocity;

    // Coroutines
    private Coroutine hitCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine patrolCoroutine;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentPatrolPoint = pointA;
    }

    private void Start()
    {
        ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        ApplyGravity();

        if (CurrentState == EnemyState.Wait ||
            CurrentState == EnemyState.Attack ||
            CurrentState == EnemyState.TakeHit ||
            CurrentState == EnemyState.Die)
        {
            return;
        }

        CalculatePlayerDistance();

        switch (CurrentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;

            case EnemyState.Patrol:
                PatrolState();
                break;

            case EnemyState.Chase:
                ChaseState();
                break;
        }
    }

    // ============================================================
    // STATE MANAGEMENT
    // ============================================================

    private void ChangeState(EnemyState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
    }

    // ============================================================
    // IDLE
    // ============================================================

    private void IdleState()
    {
        ResetAnimation();
    }

    // ============================================================
    // PLAYER DETECTION
    // ============================================================

    private void CalculatePlayerDistance()
    {
        if (player == null)
            return;

        float playerDistance =
            Vector3.Distance(transform.position, player.position);

        if (playerDistance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            AttackState();
        }
        else if (playerDistance <= chaseRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    // ============================================================
    // PATROL
    // ============================================================

    private void PatrolState()
    {
        if (currentPatrolPoint == null)
            return;

        float targetDistance =
            Vector3.Distance(
                transform.position,
                currentPatrolPoint.position
            );

        if (targetDistance <= 0.1f)
        {
            ResetAnimation();

            ChangeState(EnemyState.Wait);

            if (patrolCoroutine != null)
                StopCoroutine(patrolCoroutine);

            patrolCoroutine = StartCoroutine(IE_PatrolWaitTime());

            return;
        }

        animator.SetBool("isWalk", true);

        targetDirection =
            (currentPatrolPoint.position - transform.position).normalized;

        controller.Move(
            targetDirection * patrolSpeed * Time.deltaTime
        );

        RotateTowards();
    }

    private void SwitchPatrolPoint()
    {
        currentPatrolPoint =
            currentPatrolPoint == pointA ? pointB : pointA;
    }

    // ============================================================
    // CHASE
    // ============================================================

    private void ChaseState()
    {
        if (player == null)
            return;

        float targetDistance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (targetDistance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            AttackState();
            return;
        }

        animator.SetBool("isWalk", false);

        targetDirection =
            (player.position - transform.position).normalized;

        controller.Move(
            targetDirection * chaseSpeed * Time.deltaTime
        );

        RotateTowards();
    }

    // ============================================================
    // ATTACK
    // ============================================================

    private void AttackState()
    {
        ResetAnimation();

        if (player != null)
        {
            Vector3 direction =
                player.position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction);
            }
        }

        animator.SetTrigger("Attack");

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine =
            StartCoroutine(IE_AttackCooldown());
    }

    // ============================================================
    // TAKE HIT
    // ============================================================

    public void TakeHit()
    {
        if (CurrentState == EnemyState.Die)
            return;

        ChangeState(EnemyState.TakeHit);

        ResetAnimation();

        animator.SetTrigger("Hit");

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine =
            StartCoroutine(IE_HitAnimationComplete());
    }

    // ============================================================
    // DIE
    // ============================================================

    public void Die()
    {
        if (CurrentState == EnemyState.Die)
            return;

        ChangeState(EnemyState.Die);

        ResetAnimation();

        animator.SetTrigger("Die");
        Weapone.SetActive(false);
    }

    // ============================================================
    // ANIMATION
    // ============================================================

    private void ResetAnimation()
    {
        animator.SetBool("isWalk", false);
    }

    // ============================================================
    // MOVEMENT
    // ============================================================

    private void RotateTowards()
    {
        Vector3 horizontalDirection = targetDirection;

        horizontalDirection.y = 0f;

        if (horizontalDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(horizontalDirection);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    // ============================================================
    // COROUTINES
    // ============================================================

    private IEnumerator IE_PatrolWaitTime()
    {
        yield return new WaitForSeconds(patrolWaitTime);

        if (CurrentState != EnemyState.TakeHit &&
            CurrentState != EnemyState.Die)
        {
            SwitchPatrolPoint();

            ChangeState(EnemyState.Patrol);
        }

        patrolCoroutine = null;
    }

    private IEnumerator IE_AttackCooldown()
    {
        yield return new WaitForSeconds(
            attackAnimationCompleteTime
        );

        if (CurrentState != EnemyState.Attack)
            yield break;

        ChangeState(EnemyState.Wait);

        yield return new WaitForSeconds(
            attackCooldownTime
        );

        if (CurrentState == EnemyState.Wait)
        {
            ChangeState(EnemyState.Chase);
        }

        attackCoroutine = null;
    }

    private IEnumerator IE_HitAnimationComplete()
    {
        yield return new WaitForSeconds(
            hitStateDurationTime
        );

        hitCoroutine = null;

        if (CurrentState != EnemyState.Die)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    // ============================================================
    // ANIMATION EVENTS
    // ============================================================

    public void AttackAnimationStart()
    {
        Weapone.SetActive(true);
    }

    public void AttackAnimationComplete()
    {
        Weapone.SetActive(false);
    }

    // ============================================================
    // DEBUG
    // ============================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            chaseRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        if (pointA != null && pointB != null)
        {
            Gizmos.DrawLine(
                pointA.position,
                pointB.position
            );
        }
    }
}
