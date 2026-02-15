using UnityEngine;

/// <summary>
/// Performs 4 raycasts in a rectangular prism (view-frustum style) in the positive local Z direction.
/// Exposes the 4 intersection points on hit surfaces for use as a 4-sided polygon (e.g. quad).
/// Corner order: [0]=TopLeft, [1]=TopRight, [2]=BottomRight, [3]=BottomLeft (clockwise when viewed from origin).
/// 
/// Prompt: Please edit @c:\Users\awies\Desktop\Lab 2\Unity Projects\WallFish-Prototype\SxL-Unity-Arduino-Template\Assets\HeadObjectScript.cs to create a script that does 4 raycasts in a configurable rectangular projection area. The desired output is the 4 intersection points when each cast hits a planar surface. These points will later be accessed by another script to create a 4 sided polygon.
/// Please make it so that all 4 raycasts originate from the gameobject that contains this script. They should create a prisim in the positive z direction similar to how a camera projects its boundaries. Please make it so that the width and height of this pisim is adjustable via the angles between the raycasts.
/// 
/// </summary>
public class HeadObjectScript : MonoBehaviour
{
    [Header("Projection angles (degrees from center)")]
    [Tooltip("Half-angle left/right of center. Total horizontal FOV = 2 × this value.")]
    [SerializeField] private float horizontalHalfAngle = 30f;

    [Tooltip("Half-angle up/down from center. Total vertical FOV = 2 × this value.")]
    [SerializeField] private float verticalHalfAngle = 20f;

    [Header("Raycast settings")]
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private LayerMask layerMask = ~0;
    [Tooltip("Use Collide to hit trigger colliders (e.g. trigger Box Colliders). Use Ignore for standard behavior.")]
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    /// <summary>World-space intersection points. Order: TopLeft, TopRight, BottomRight, BottomLeft.</summary>
    public Vector3[] IntersectionPoints { get; private set; } = new Vector3[4];

    /// <summary>True if the corresponding ray hit something this frame.</summary>
    public bool[] HitSuccess { get; private set; } = new bool[4];

    private Vector3[] _rayDirections = new Vector3[4];

    void Start()
    {
        CacheRayDirections();
    }

    void Update()
    {
        CacheRayDirections();
        PerformRaycasts();
    }

    /// <summary>Recompute the 4 ray directions from current angles and transform.</summary>
    private void CacheRayDirections()
    {
        Vector3 fwd = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        // Local directions (assuming +Z forward, +X right, +Y up) then transform to world
        // TopLeft:  -horizontal, +vertical
        _rayDirections[0] = (Quaternion.AngleAxis(-horizontalHalfAngle, up) * Quaternion.AngleAxis(verticalHalfAngle, right) * fwd).normalized;
        // TopRight: +horizontal, +vertical
        _rayDirections[1] = (Quaternion.AngleAxis(horizontalHalfAngle, up) * Quaternion.AngleAxis(verticalHalfAngle, right) * fwd).normalized;
        // BottomRight: +horizontal, -vertical
        _rayDirections[2] = (Quaternion.AngleAxis(horizontalHalfAngle, up) * Quaternion.AngleAxis(-verticalHalfAngle, right) * fwd).normalized;
        // BottomLeft:  -horizontal, -vertical
        _rayDirections[3] = (Quaternion.AngleAxis(-horizontalHalfAngle, up) * Quaternion.AngleAxis(-verticalHalfAngle, right) * fwd).normalized;
    }

    private void PerformRaycasts()
    {
        Vector3 origin = transform.position;

        for (int i = 0; i < 4; i++)
        {
            RaycastHit[] hits = Physics.RaycastAll(origin, _rayDirections[i], maxRayDistance, layerMask, triggerInteraction);
            bool foundWall = false;
            float closestDistance = float.MaxValue;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Wall") && hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    IntersectionPoints[i] = hit.point;
                    foundWall = true;
                }
            }

            if (foundWall)
                HitSuccess[i] = true;
            else
            {
                IntersectionPoints[i] = origin + _rayDirections[i] * maxRayDistance;
                HitSuccess[i] = false;
            }
        }
    }

    /// <summary>Call from another script to get the 4 corner points for a polygon. Only returns points that hit (or all 4 if you prefer to use fallback positions).</summary>
    public Vector3[] GetPolygonCorners(bool onlySuccessfulHits = false)
    {
        if (!onlySuccessfulHits)
            return (Vector3[])IntersectionPoints.Clone();

        int count = 0;
        for (int i = 0; i < 4; i++)
            if (HitSuccess[i]) count++;

        Vector3[] corners = new Vector3[count];
        int idx = 0;
        for (int i = 0; i < 4; i++)
            if (HitSuccess[i])
                corners[idx++] = IntersectionPoints[i];
        return corners;
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool drawRaysInEditor = true;
    [SerializeField] private bool drawHitPoints = true;

    private void OnDrawGizmos()
    {
        if (!drawRaysInEditor && !drawHitPoints) return;

        CacheRayDirections();
        Vector3 origin = transform.position;

        if (drawRaysInEditor)
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
            for (int i = 0; i < 4; i++)
                Gizmos.DrawRay(origin, _rayDirections[i] * maxRayDistance);
        }

        if (drawHitPoints && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < 4; i++)
                if (HitSuccess[i])
                    Gizmos.DrawSphere(IntersectionPoints[i], 0.1f);
        }
    }
#endif
}
