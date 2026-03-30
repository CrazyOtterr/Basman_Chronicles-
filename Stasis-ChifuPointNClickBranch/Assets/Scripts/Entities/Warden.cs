using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Warden : MonoBehaviour
{
    [Header("���������� �����")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("��������� ��������")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float waitTime = 2f;

    [Header("��������� �����������")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask obstacleLayer; // ����������� (����� � �.�.)

    [Header("�������")]
    public UnityEvent onPlayerCaught;

    // ��������� �����������
    private enum WardenState
    {
        Patrolling,
        Waiting,
        Chasing,
        Returning
    }

    private WardenState currentState = WardenState.Patrolling;
    private Transform player;
    private Vector3 currentTarget;
    private bool movingToB = true;
    private float waitTimer = 0f;
    private float lastGameTime = 0f;
    private float accumulatedTime = 0f;
    private Vector3 lastSimulatedPosition;
    private bool isInitialized = false;
    private SpriteRenderer spriteRenderer;
    private Vector2 facingDirection = Vector2.right;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (isInitialized) return;

        // ���� ������
        FindPlayer();

        // �������� SpriteRenderer ��� ��������� �������
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ��������� ������� ����� ��������������
        if (pointA == null || pointB == null)
        {
            // Debug.LogError("?? ????????? ????? ??????????????! ????????? pointA ? pointB ? ??????????.");
            enabled = false;
            return;
        }

        // �������� �������� �� ������� ������� � ��������� �����
        float distToA = Vector2.Distance(transform.position, pointA.position);
        float distToB = Vector2.Distance(transform.position, pointB.position);

        if (distToA < distToB)
        {
            currentTarget = pointA.position;
            movingToB = true;
        }
        else
        {
            currentTarget = pointB.position;
            movingToB = false;
        }

        lastGameTime = Stats.Instance?.gameTime ?? 0f;
        lastSimulatedPosition = transform.position;
        accumulatedTime = 0f;

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // ��������� ����� ������ (�� ������ ���� ����� ��������/�����)
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer();
        }

        // ��������� ������ �������
        float deltaTime = CalculateDeltaTime();
        accumulatedTime += deltaTime;

        // �������� ��������� ����� ����� � �����
        switch (currentState)
        {
            case WardenState.Patrolling:
                HandlePatrolling(deltaTime);
                CheckForPlayer();
                break;

            case WardenState.Waiting:
                HandleWaiting(deltaTime);
                CheckForPlayer();
                break;

            case WardenState.Chasing:
                HandleChasing(deltaTime);
                break;

            case WardenState.Returning:
                HandleReturning(deltaTime);
                CheckForPlayer();
                break;
        }

        lastGameTime = Stats.Instance?.gameTime ?? lastGameTime;
    }

    float CalculateDeltaTime()
    {
        if (Stats.Instance == null)
        {
            return Time.deltaTime;
        }

        float currentGameTime = Stats.Instance.gameTime;
        float delta = currentGameTime - lastGameTime;

        // ���� gameTime �� ���������, ���������� ������� Time.deltaTime
        if (delta <= 0)
        {
            return Time.deltaTime;
        }

        return delta;
    }

    void HandlePatrolling(float deltaTime)
    {
        // �������� � ����
        Vector2 direction = ((Vector2)currentTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * patrolSpeed * deltaTime);

        // ��������� ����������� �������
        UpdateFacingDirection(direction);

        // �������� ���������� ����
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget);
        if (distanceToTarget < 0.1f)
        {
            currentState = WardenState.Waiting;
            waitTimer = waitTime;
        }
    }

    void HandleWaiting(float deltaTime)
    {
        waitTimer -= deltaTime;

        if (waitTimer <= 0)
        {
            // ����� ����
            if (movingToB)
            {
                currentTarget = pointA.position;
                movingToB = false;
            }
            else
            {
                currentTarget = pointB.position;
                movingToB = true;
            }

            currentState = WardenState.Patrolling;
        }
    }

    void HandleChasing(float deltaTime)
    {
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            ReturnToPatrol();
            return;
        }

        // �������� � ������
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * chaseSpeed * deltaTime);

        // ��������� ����������� �������
        UpdateFacingDirection(direction);

        // �������� ������������ � �������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < 0.5f)
        {
            CatchPlayer();
        }

        // �������� ������ ������ (���� �� ����� �� ������ �����������)
        if (distanceToPlayer > detectionRadius * 1.2f)
        {
            ReturnToPatrol();
        }
    }

    void HandleReturning(float deltaTime)
    {
        // �������� � ��������� ����� �������
        Vector3 returnTarget = GetClosestPatrolPoint();
        Vector2 direction = ((Vector2)returnTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * patrolSpeed * deltaTime);

        // ��������� ����������� �������
        UpdateFacingDirection(direction);

        // �������� ����������
        float distance = Vector2.Distance(transform.position, returnTarget);
        if (distance < 0.1f)
        {
            // ���������� �������������� � ���� �����
            currentTarget = movingToB ? pointB.position : pointA.position;
            currentState = WardenState.Patrolling;
        }
    }

    void CheckForPlayer()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        // ��������� ���������� �� ������
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ���� ����� � ������� �����������
        if (distanceToPlayer <= detectionRadius)
        {
            // ���������, ���� �� ������ ���������
            if (HasLineOfSightToPlayer())
            {
                StartChasing();
            }
        }
    }

    bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;

        // ����������� � ������
        Vector2 directionToPlayer = (Vector2)player.position - (Vector2)transform.position;

        // ������� ��� � ������
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            detectionRadius,
            obstacleLayer
        );

        // ���� ��� �� �� ��� �� ����� ��� ����� � ������ - ���� ���������
        if (hit.collider == null)
        {
            return true;
        }

        // ���������, ����� �� ��� � ������
        if (hit.collider.transform == player ||
            hit.collider.transform.IsChildOf(player) ||
            hit.collider.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }

    void StartChasing()
    {
        if (currentState != WardenState.Chasing)
        {
            currentState = WardenState.Chasing;
            //Debug.Log("����� ���������! ������� �������������.");
        }
    }

    void CatchPlayer()
    {
        //Debug.Log("����� ������!");
        onPlayerCaught?.Invoke();

        // ������������� �������������
        currentState = WardenState.Waiting;
        waitTimer = 1f;
    }

    void ReturnToPatrol()
    {
        currentState = WardenState.Returning;
        //Debug.Log("����� �������. ����������� � ��������������.");
    }

    Vector3 GetClosestPatrolPoint()
    {
        float distanceToA = Vector2.Distance(transform.position, pointA.position);
        float distanceToB = Vector2.Distance(transform.position, pointB.position);

        return distanceToA < distanceToB ? pointA.position : pointB.position;
    }

    void UpdateFacingDirection(Vector2 direction)
    {
        if (direction.x != 0)
        {
            facingDirection = direction.x > 0 ? Vector2.right : Vector2.left;

            // ������������ ������ ���� ����
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection.x < 0;
            }
            else
            {
                // ��� ������������ ���� ������
                transform.localScale = new Vector3(
                    Mathf.Sign(facingDirection.x),
                    1,
                    1
                );
            }
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            // �������������� �����
            PnC_Player playerComponent = FindFirstObjectByType<PnC_Player>();
            if (playerComponent != null)
            {
                player = playerComponent.transform;
            }
        }
        else
        {
            player = playerObj.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        // ������ ������ �����������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ������ ����� � ������� ����
        if (Application.isPlaying && isInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget);
        }

        // ������ ����� ��������������
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // ������ ��� �������� ��������� ���� ���� �����
        if (player != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    // ��������� ������ ��� ����������
    public void ForceDetectPlayer()
    {
        if (player != null)
        {
            StartChasing();
        }
    }

    public void ResetToPatrol()
    {
        currentState = WardenState.Patrolling;
        currentTarget = pointA.position;
        transform.position = pointA.position;
        movingToB = true;
    }

    // ����� ��� ��������� �������� ��������� (����� ���� ������� ��� ������ ��������)
    public bool IsChasing()
    {
        return currentState == WardenState.Chasing;
    }
}