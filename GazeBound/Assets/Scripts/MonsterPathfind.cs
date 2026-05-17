using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterPathfind : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderRadius = 20f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Anti-Repeat Settings")]
    public int historySize = 5;
    public float minDistanceBetweenSpots = 5f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 8f;
    public float idleLookChance = 0.3f;
    public float idleLookSpeed = 2f;

    [Header("Obstacle Settings")]
    public float obstacleDetectionRange = 3f;
    public float raycastHeight = 1f;
    public LayerMask obstacleLayer;

    private NavMeshAgent _agent;
    private Vector3 _origin;
    private Queue<Vector3> _positionHistory = new Queue<Vector3>();
    private bool _reroutePending = false;
    private bool _isTurning = false;
    private bool _isIdleLooking = false;
    private Quaternion _idleLookTarget;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.angularSpeed = 0f;
        _origin = transform.position;
        StartCoroutine(WanderRoutine());
        StartCoroutine(ObstacleCheckRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // --- Pick destination ---
            _reroutePending = false;
            _isTurning = false;
            _isIdleLooking = false;

            Vector3 destination = GetValidDestination();
            _positionHistory.Enqueue(destination);
            if (_positionHistory.Count > historySize)
                _positionHistory.Dequeue();

            // --- Start moving ---
            _agent.isStopped = false;
            _agent.SetDestination(destination);

            // Wait for path calculation
            yield return new WaitUntil(() => !_agent.pathPending);

            // --- Walk until arrived or obstacle hit ---
            while (true)
            {
                if (_reroutePending)
                    break;

                if (!_agent.pathPending
                    && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f
                    && _agent.velocity.sqrMagnitude < 0.05f)
                    break;

                yield return null;
            }

            // --- Stop agent cleanly ---
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _agent.ResetPath();

            if (_reroutePending)
            {
                // --- Reroute: turn away smoothly while stopped ---
                float randomAngle = Random.Range(130f, 230f);
                Vector3 newForward = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
                newForward.y = 0f;

                Quaternion startRot = transform.rotation;
                Quaternion targetRot = Quaternion.LookRotation(newForward.normalized);

                _isTurning = true;
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 2.5f;
                    transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }
                transform.rotation = targetRot;
                _isTurning = false;

                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                // --- Natural arrival: optional idle look ---
                if (Random.value < idleLookChance)
                {
                    _isIdleLooking = true;
                    float randomAngle = Random.Range(-90f, 90f);
                    _idleLookTarget = transform.rotation * Quaternion.Euler(0, randomAngle, 0);
                }

                yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
                _isIdleLooking = false;
            }
        }
    }

    IEnumerator ObstacleCheckRoutine()
    {
        while (true)
        {
            if (!_reroutePending && !_isTurning)
            {
                Vector3 rayOrigin = transform.position + Vector3.up * raycastHeight;
                Vector3 leftDir = Quaternion.Euler(0, -30, 0) * transform.forward;
                Vector3 rightDir = Quaternion.Euler(0, 30, 0) * transform.forward;

                Debug.DrawRay(rayOrigin, transform.forward * obstacleDetectionRange, Color.red, 0.1f);
                Debug.DrawRay(rayOrigin, leftDir * obstacleDetectionRange, Color.yellow, 0.1f);
                Debug.DrawRay(rayOrigin, rightDir * obstacleDetectionRange, Color.yellow, 0.1f);

                if (Physics.Raycast(rayOrigin, transform.forward, obstacleDetectionRange, obstacleLayer) ||
                    Physics.Raycast(rayOrigin, leftDir, obstacleDetectionRange, obstacleLayer) ||
                    Physics.Raycast(rayOrigin, rightDir, obstacleDetectionRange, obstacleLayer))
                {
                    _reroutePending = true;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    Vector3 GetValidDestination()
    {
        int attempts = 0;
        Vector3 candidate;
        do
        {
            candidate = GetRandomNavMeshPoint();
            attempts++;
        }
        while (IsTooCloseToHistory(candidate) && attempts < 10);
        return candidate;
    }

    Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += _origin;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
            return hit.position;
        return transform.position;
    }

    bool IsTooCloseToHistory(Vector3 candidate)
    {
        foreach (Vector3 past in _positionHistory)
        {
            if (Vector3.Distance(candidate, past) < minDistanceBetweenSpots)
                return true;
        }
        return false;
    }

    void Update()
    {
        if (_isTurning) return;

        if (_agent.velocity.sqrMagnitude > 0.01f)
        {
            // Moving: face travel direction
            Quaternion targetRotation = Quaternion.LookRotation(_agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else if (_isIdleLooking)
        {
            // Idle: slowly glance around
            transform.rotation = Quaternion.Slerp(transform.rotation, _idleLookTarget, Time.deltaTime * idleLookSpeed);
        }
    }
}