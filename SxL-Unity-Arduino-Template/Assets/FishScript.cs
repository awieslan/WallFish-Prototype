using UnityEngine;

/// <summary>
/// Prompt: Please edit this script @c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\FishScript.cs to move randomly in a smooth manner within a plane created by the points outputed by @c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\HeadObjectScript.cs. The FishScript will be on a different gameobject.
/// 
/// The desired behvior loop is as follows:
/// - A target destination point within the plane defined by the raycast points is randomly selected.
/// - The fish object moves towards this point and moves around it idly for a random time (within a min/max).
/// - A new destination is selected and the process is repeated.
/// 
/// When determining the bounding area, only successful cast hits should be used. When the current cast is unsuccessful, use the most recent successful cast for that point instead. Please draw the 4 points currently being used as the boundary in the scene view in orange.
/// 
/// When the fish moves, it should move somewhat irregularly and not in a straight line. Try to have it move in a meandering manner, simulating 2D fish movement.
/// 
/// The fish begins facing right (positive x direction). When it moves, it should face the direction it is moving. When it is facing to the left, the object should be mirrored so that it does not go upside down.
/// </summary>
public class FishScript : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The HeadObjectScript that provides the boundary points")]
    [SerializeField] private HeadObjectScript headObjectScript;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float arrivalDistance = 0.5f;
    [SerializeField] private float meanderStrength = 0.3f;
    [SerializeField] private float meanderFrequency = 2f;

    [Header("Idle Behavior")]
    [SerializeField] private float idleRadius = 1f;
    [SerializeField] private float idleMoveSpeed = 0.8f;
    [SerializeField] private float idleArrivalDistance = 0.15f;
    [SerializeField] private float pauseDriftSpeed = 0.15f;
    [SerializeField] private float minPauseTime = 0.5f;
    [SerializeField] private float maxPauseTime = 2f;
    [SerializeField] private float minIdleTime = 2f;
    [SerializeField] private float maxIdleTime = 5f;

    [Header("Exclusion Zones")]
    [Tooltip("Optional: leave empty to find all ExclusionZones in the scene automatically.")]
    [SerializeField] private ExclusionZone[] exclusionZones;

    [Header("Debug")]
    [SerializeField] private bool drawBoundaryPoints = true;
    [SerializeField] private bool drawTargetPoint = true;

    // State
    private Vector3[] currentBoundaryPoints = new Vector3[4];
    private bool[] currentBoundaryValid = new bool[4];
    private Vector3 targetPoint;
    private Vector3 idleCenter;
    private float idleTimer;
    private float idleDuration;
    private bool isIdling = false;
    private Vector3 idleWaypoint;
    private bool idleMovingToWaypoint = true;
    private float idlePauseTimer;
    private float idlePauseDuration;
    private Vector3 idlePauseDriftDirection;
    private Vector3 planeNormal;
    private Vector3 planeCenter;
    private float meanderTime = 0f;

    // Store most recent successful hits
    private Vector3[] lastSuccessfulPoints = new Vector3[4];
    private bool[] hasSuccessfulPoint = new bool[4];

    private ExclusionZone[] _cachedExclusionZones;
    private bool _spawnPositionCorrected;

    void Start()
    {
        if (headObjectScript == null)
        {
            Debug.LogError("FishScript: HeadObjectScript reference is not set!");
            return;
        }

        // Initialize facing right (+X). Fish model nose is +X, so we use -90° so forward (Z) points right.
        transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);
        
        // Ensure initial scale is correct (facing right, not mirrored)
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
        
        // Initialize boundary points
        for (int i = 0; i < 4; i++)
        {
            lastSuccessfulPoints[i] = transform.position;
            hasSuccessfulPoint[i] = false;
        }

        // Resolve exclusion zones: use serialized list or find all in scene; populate Inspector list when auto-finding
        if (exclusionZones != null && exclusionZones.Length > 0)
        {
            _cachedExclusionZones = exclusionZones;
        }
        else
        {
            _cachedExclusionZones = FindObjectsOfType<ExclusionZone>();
            exclusionZones = _cachedExclusionZones; // so the Inspector list shows found zones at runtime
        }
        if (_cachedExclusionZones != null && _cachedExclusionZones.Length > 0)
            Debug.Log($"FishScript: using {_cachedExclusionZones.Length} exclusion zone(s).");

        // Start with a random target
        SelectNewTarget();
    }

    void Update()
    {
        if (headObjectScript == null) return;

        UpdateBoundaryPoints();
        UpdatePlane();

        // One-time: if fish started inside an exclusion zone, move it outside (spawn correction)
        if (!_spawnPositionCorrected)
        {
            _spawnPositionCorrected = true;
            if (IsPointInExclusionZone(transform.position))
                transform.position = PushPointOutOfExclusionZones(transform.position);
        }

        if (currentBoundaryValid[0] || currentBoundaryValid[1] || currentBoundaryValid[2] || currentBoundaryValid[3])
        {
            if (isIdling)
            {
                UpdateIdleBehavior();
            }
            else
            {
                UpdateMovement();
            }
        }
    }

    private void UpdateBoundaryPoints()
    {
        // Get current intersection points from HeadObjectScript
        Vector3[] intersectionPoints = headObjectScript.IntersectionPoints;
        bool[] hitSuccess = headObjectScript.HitSuccess;

        for (int i = 0; i < 4; i++)
        {
            if (hitSuccess[i])
            {
                // Use current successful hit
                currentBoundaryPoints[i] = intersectionPoints[i];
                currentBoundaryValid[i] = true;
                lastSuccessfulPoints[i] = intersectionPoints[i];
                hasSuccessfulPoint[i] = true;
            }
            else if (hasSuccessfulPoint[i])
            {
                // Use most recent successful hit
                currentBoundaryPoints[i] = lastSuccessfulPoints[i];
                currentBoundaryValid[i] = true;
            }
            else
            {
                // No successful hit yet, invalid
                currentBoundaryValid[i] = false;
            }
        }
    }

    private void UpdatePlane()
    {
        // Find valid points to define the plane
        int validCount = 0;
        Vector3 sum = Vector3.zero;
        Vector3[] validPoints = new Vector3[4];
        
        for (int i = 0; i < 4; i++)
        {
            if (currentBoundaryValid[i])
            {
                validPoints[validCount] = currentBoundaryPoints[i];
                sum += currentBoundaryPoints[i];
                validCount++;
            }
        }

        if (validCount < 3)
        {
            // Not enough points to define a plane
            return;
        }

        planeCenter = sum / validCount;

        // Calculate plane normal from valid points
        if (validCount >= 3)
        {
            Vector3 v1 = validPoints[1] - validPoints[0];
            Vector3 v2 = validPoints[2] - validPoints[0];
            planeNormal = Vector3.Cross(v1, v2).normalized;
            
            // Ensure normal points in a consistent direction (towards camera/head)
            if (headObjectScript != null)
            {
                Vector3 toHead = (headObjectScript.transform.position - planeCenter).normalized;
                if (Vector3.Dot(planeNormal, toHead) < 0)
                    planeNormal = -planeNormal;
            }
        }
    }

    private void UpdateMovement()
    {
        Vector3 currentPos = transform.position;
        Vector3 directionToTarget = (targetPoint - currentPos).normalized;

        // Add meandering behavior using Perlin noise
        meanderTime += Time.deltaTime * meanderFrequency;
        float noiseX = Mathf.PerlinNoise(meanderTime, 0) * 2f - 1f;
        float noiseY = Mathf.PerlinNoise(0, meanderTime) * 2f - 1f;
        
        // Create a perpendicular vector for meandering
        Vector3 right = Vector3.Cross(planeNormal, directionToTarget).normalized;
        Vector3 up = Vector3.Cross(directionToTarget, right).normalized;
        
        Vector3 meanderOffset = (right * noiseX + up * noiseY) * meanderStrength;
        Vector3 finalDirection = (directionToTarget + meanderOffset).normalized;

        // Project movement onto the plane
        Vector3 movement = finalDirection * moveSpeed * Time.deltaTime;
        movement = Vector3.ProjectOnPlane(movement, planeNormal);
        
        Vector3 newPosition = currentPos + movement;
        // Do not constrain to boundary every frame (avoids fish being dragged when boundary moves).
        // Block entry into exclusion zones by pushing position out if it would land inside.
        newPosition = PushPointOutOfExclusionZones(newPosition);
        transform.position = newPosition;

        // Face movement direction (fish model nose is +X, so rotate -90° from LookRotation)
        if (movement.magnitude > 0.001f)
        {
            ApplyFacingRotation(movement.normalized);
        }

        // Check if arrived at target
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
        if (distanceToTarget < arrivalDistance)
        {
            StartIdling();
        }
    }

    private void UpdateIdleBehavior()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            isIdling = false;
            SelectNewTarget();
            return;
        }

        if (idleMovingToWaypoint)
        {
            Vector3 toWaypoint = idleWaypoint - transform.position;
            Vector3 direction = toWaypoint.normalized;
            float distance = Vector3.ProjectOnPlane(toWaypoint, planeNormal).magnitude;

            if (distance < idleArrivalDistance)
            {
                // Reached waypoint; start pause and remember direction for drift
                idleMovingToWaypoint = false;
                idlePauseTimer = 0f;
                idlePauseDuration = Random.Range(minPauseTime, maxPauseTime);
                idlePauseDriftDirection = direction;
            }
            else
            {
                Vector3 movement = direction * idleMoveSpeed * Time.deltaTime;
                movement = Vector3.ProjectOnPlane(movement, planeNormal);
                Vector3 newPos = transform.position + movement;
                newPos = PushPointOutOfExclusionZones(newPos);
                newPos = ConstrainToBoundary(newPos); // keep within raycast boundary during idle
                transform.position = newPos;

                if (movement.magnitude > 0.001f)
                    ApplyFacingRotation(movement.normalized);
            }
        }
        else
        {
            // Drift slightly in the direction we were last moving
            Vector3 driftDir = Vector3.ProjectOnPlane(idlePauseDriftDirection, planeNormal);
            if (driftDir.sqrMagnitude > 0.001f)
            {
                Vector3 drift = driftDir.normalized * (pauseDriftSpeed * Time.deltaTime);
                Vector3 newPos = PushPointOutOfExclusionZones(transform.position + drift);
                newPos = ConstrainToBoundary(newPos); // keep within raycast boundary during idle
                transform.position = newPos;
            }

            idlePauseTimer += Time.deltaTime;
            if (idlePauseTimer >= idlePauseDuration)
            {
                // Pause over; pick new random point in radius and move
                idleMovingToWaypoint = true;
                idleWaypoint = PickRandomPointInIdleRadius();
            }
        }
    }

    /// <summary>Pick a random point within idle radius of idle center, on the plane.</summary>
    private Vector3 PickRandomPointInIdleRadius()
    {
        Vector3 right = Vector3.Cross(planeNormal, (transform.position - idleCenter).normalized);
        if (right.sqrMagnitude < 0.01f)
            right = Vector3.Cross(planeNormal, Vector3.up).normalized;
        else
            right = right.normalized;
        Vector3 up = Vector3.Cross(right, planeNormal).normalized;

        const int maxRetries = 10;
        for (int retry = 0; retry < maxRetries; retry++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float r = Random.Range(0.2f, 1f) * idleRadius;
            Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * r;
            Vector3 point = ConstrainToBoundary(idleCenter + offset);
            if (!IsPointInExclusionZone(point))
                return point;
        }
        return transform.position;
    }

    /// <summary>Set rotation to face movement direction; fish model nose is +X, so we apply -90° and mirror when facing left.</summary>
    private void ApplyFacingRotation(Vector3 lookDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection, planeNormal) * Quaternion.Euler(0f, -90f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector3 localRight = transform.right;
        float dotWithWorldRight = Vector3.Dot(localRight, Vector3.right);
        Vector3 scale = transform.localScale;
        if (dotWithWorldRight < 0)
            scale.y = -Mathf.Abs(scale.y);
        else
            scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    private void SelectNewTarget()
    {
        // Find valid boundary points
        int validCount = 0;
        Vector3[] validPoints = new Vector3[4];
        
        for (int i = 0; i < 4; i++)
        {
            if (currentBoundaryValid[i])
            {
                validPoints[validCount++] = currentBoundaryPoints[i];
            }
        }

        if (validCount < 3)
        {
            // Not enough valid points, use current position
            targetPoint = transform.position;
            return;
        }

        // Do not teleport the fish. If it is outside the boundary, it will move normally toward the new target (which is inside) and re-enter naturally.

        // Calculate bounding box of valid points
        Vector3 min = validPoints[0];
        Vector3 max = validPoints[0];
        
        for (int i = 1; i < validCount; i++)
        {
            min = Vector3.Min(min, validPoints[i]);
            max = Vector3.Max(max, validPoints[i]);
        }

        // Select random point within bounding box, projected onto plane; retry if inside or path crosses an exclusion zone
        Vector3 currentPos = transform.position;
        const int maxRetries = 25;
        for (int retry = 0; retry < maxRetries; retry++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                Random.Range(min.z, max.z)
            );

            Vector3 toPoint = randomPoint - planeCenter;
            float distance = Vector3.Dot(toPoint, planeNormal);
            targetPoint = randomPoint - planeNormal * distance;
            targetPoint = ConstrainToBoundary(targetPoint);

            if (IsPointInExclusionZone(targetPoint))
                continue;
            targetPoint = PushPointOutOfExclusionZones(targetPoint);
            if (IsPointInExclusionZone(targetPoint))
                continue;
            // Reject target if straight path would go through an exclusion zone
            if (PathCrossesExclusionZone(currentPos, targetPoint))
                continue;
            break;
        }
    }

    /// <summary>Returns true if the point is inside any exclusion zone.</summary>
    private bool IsPointInExclusionZone(Vector3 point)
    {
        if (_cachedExclusionZones == null) return false;
        for (int i = 0; i < _cachedExclusionZones.Length; i++)
        {
            if (_cachedExclusionZones[i] != null && _cachedExclusionZones[i].ContainsPoint(point))
                return true;
        }
        return false;
    }

    /// <summary>Returns true if the straight path from fromPos to toPos crosses through any exclusion zone.</summary>
    private bool PathCrossesExclusionZone(Vector3 fromPos, Vector3 toPos)
    {
        if (_cachedExclusionZones == null || _cachedExclusionZones.Length == 0) return false;
        const int samples = 12;
        for (int s = 1; s < samples; s++)
        {
            float t = s / (float)samples;
            Vector3 sample = Vector3.Lerp(fromPos, toPos, t);
            if (IsPointInExclusionZone(sample))
                return true;
        }
        return false;
    }

    /// <summary>If point is inside any zone, push it to the nearest point outside (on the zone boundary). Iterates to handle overlapping zones.</summary>
    private Vector3 PushPointOutOfExclusionZones(Vector3 point)
    {
        if (_cachedExclusionZones == null) return point;
        const int maxIterations = 10;
        for (int iter = 0; iter < maxIterations; iter++)
        {
            bool wasInside = false;
            for (int i = 0; i < _cachedExclusionZones.Length; i++)
            {
                if (_cachedExclusionZones[i] == null) continue;
                if (_cachedExclusionZones[i].ContainsPoint(point))
                {
                    point = _cachedExclusionZones[i].ClosestPointOutside(point);
                    wasInside = true;
                }
            }
            if (!wasInside) break;
        }
        return point;
    }

    private void StartIdling()
    {
        isIdling = true;
        idleTimer = 0f;
        idleDuration = Random.Range(minIdleTime, maxIdleTime);
        idleCenter = targetPoint;
        idleMovingToWaypoint = true;
        idleWaypoint = PickRandomPointInIdleRadius();
    }

    private Vector3 ConstrainToBoundary(Vector3 point)
    {
        // Simple AABB constraint using valid boundary points
        int validCount = 0;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        bool initialized = false;
        
        for (int i = 0; i < 4; i++)
        {
            if (currentBoundaryValid[i])
            {
                if (!initialized)
                {
                    min = currentBoundaryPoints[i];
                    max = currentBoundaryPoints[i];
                    initialized = true;
                }
                else
                {
                    min = Vector3.Min(min, currentBoundaryPoints[i]);
                    max = Vector3.Max(max, currentBoundaryPoints[i]);
                }
                validCount++;
            }
        }

        if (validCount == 0)
            return point;

        // Clamp to bounding box with small margin
        float margin = 0.1f;
        point.x = Mathf.Clamp(point.x, min.x + margin, max.x - margin);
        point.y = Mathf.Clamp(point.y, min.y + margin, max.y - margin);
        point.z = Mathf.Clamp(point.z, min.z + margin, max.z - margin);

        // Project back onto plane
        Vector3 toPoint = point - planeCenter;
        float distance = Vector3.Dot(toPoint, planeNormal);
        point = point - planeNormal * distance;

        // Keep point outside any exclusion zones
        point = PushPointOutOfExclusionZones(point);

        return point;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!drawBoundaryPoints && !drawTargetPoint) return;

        // Draw boundary points in orange
        if (drawBoundaryPoints && Application.isPlaying)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f); // Orange
            for (int i = 0; i < 4; i++)
            {
                if (currentBoundaryValid[i])
                {
                    Gizmos.DrawSphere(currentBoundaryPoints[i], 0.15f);
                }
            }
        }

        // Draw target point
        if (drawTargetPoint && Application.isPlaying && !isIdling)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPoint, 0.2f);
        }

        // Draw idle center
        if (drawTargetPoint && Application.isPlaying && isIdling)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(idleCenter, idleRadius);
        }
    }
#endif
}
