using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Warden : MonoBehaviour
{
    [Header("Патрульные точки")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Настройки движения")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float waitTime = 2f;

    [Header("Настройки обнаружения")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask obstacleLayer; // Препятствия (стены и т.д.)

    [Header("События")]
    public UnityEvent onPlayerCaught;

    // Состояния надзирателя
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

        // Ищем игрока
        FindPlayer();

        // Получаем SpriteRenderer для разворота спрайта
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Проверяем наличие точек патрулирования
        if (pointA == null || pointB == null)
        {
            //Debug.LogError("Не назначены точки патрулирования! Назначьте pointA и pointB в инспекторе.");
            enabled = false;
            return;
        }

        // Начинаем движение от текущей позиции к ближайшей точке
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

        // Обновляем поиск игрока (на случай если игрок появился/исчез)
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer();
        }

        // Вычисляем дельту времени
        float deltaTime = CalculateDeltaTime();
        accumulatedTime += deltaTime;

        // Реальное поведение когда игрок в сцене
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

        // Если gameTime не изменился, используем обычный Time.deltaTime
        if (delta <= 0)
        {
            return Time.deltaTime;
        }

        return delta;
    }

    void HandlePatrolling(float deltaTime)
    {
        // Движение к цели
        Vector2 direction = ((Vector2)currentTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * patrolSpeed * deltaTime);

        // Обновляем направление взгляда
        UpdateFacingDirection(direction);

        // Проверка достижения цели
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
            // Смена цели
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

        // Движение к игроку
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * chaseSpeed * deltaTime);

        // Обновляем направление взгляда
        UpdateFacingDirection(direction);

        // Проверка столкновения с игроком
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < 0.5f)
        {
            CatchPlayer();
        }

        // Проверка потери игрока (если он вышел за радиус обнаружения)
        if (distanceToPlayer > detectionRadius * 1.2f)
        {
            ReturnToPatrol();
        }
    }

    void HandleReturning(float deltaTime)
    {
        // Движение к ближайшей точке патруля
        Vector3 returnTarget = GetClosestPatrolPoint();
        Vector2 direction = ((Vector2)returnTarget - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(direction * patrolSpeed * deltaTime);

        // Обновляем направление взгляда
        UpdateFacingDirection(direction);

        // Проверка достижения
        float distance = Vector2.Distance(transform.position, returnTarget);
        if (distance < 0.1f)
        {
            // Продолжаем патрулирование с этой точки
            currentTarget = movingToB ? pointB.position : pointA.position;
            currentState = WardenState.Patrolling;
        }
    }

    void CheckForPlayer()
    {
        if (player == null || !player.gameObject.activeInHierarchy) return;

        // Проверяем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Если игрок в радиусе обнаружения
        if (distanceToPlayer <= detectionRadius)
        {
            // Проверяем, есть ли прямая видимость
            if (HasLineOfSightToPlayer())
            {
                StartChasing();
            }
        }
    }

    bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;

        // Направление к игроку
        Vector2 directionToPlayer = (Vector2)player.position - (Vector2)transform.position;

        // Бросаем луч к игроку
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            directionToPlayer.normalized,
            detectionRadius,
            obstacleLayer
        );

        // Если луч ни во что не попал или попал в игрока - есть видимость
        if (hit.collider == null)
        {
            return true;
        }

        // Проверяем, попал ли луч в игрока
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
            //Debug.Log("Игрок обнаружен! Начинаю преследование.");
        }
    }

    void CatchPlayer()
    {
        //Debug.Log("Игрок пойман!");
        onPlayerCaught?.Invoke();

        // Останавливаем преследование
        currentState = WardenState.Waiting;
        waitTimer = 1f;
    }

    void ReturnToPatrol()
    {
        currentState = WardenState.Returning;
        //Debug.Log("Игрок потерян. Возвращаюсь к патрулированию.");
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

            // Поворачиваем спрайт если есть
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = facingDirection.x < 0;
            }
            else
            {
                // Или поворачиваем весь объект
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
            // Альтернативный поиск
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
        // Рисуем радиус обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Рисуем линию к текущей цели
        if (Application.isPlaying && isInitialized)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget);
        }

        // Рисуем точки патрулирования
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // Рисуем луч проверки видимости если есть игрок
        if (player != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    // Публичные методы для управления
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

    // Метод для получения текущего состояния (может быть полезен для других скриптов)
    public bool IsChasing()
    {
        return currentState == WardenState.Chasing;
    }
}