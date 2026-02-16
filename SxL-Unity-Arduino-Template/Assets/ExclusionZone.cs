using UnityEngine;

/// <summary>
/// Marks a volume as an exclusion zone: the fish will not enter it.
/// Add this to a GameObject that has a Collider (Box, Sphere, Capsule, or Mesh). The collider
/// can be set as Trigger. The fish will avoid targeting and moving into any point inside the volume.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ExclusionZone : MonoBehaviour
{
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        if (_collider == null)
            Debug.LogWarning("ExclusionZone requires a Collider on the same GameObject.", this);
    }

    /// <summary>Returns true if the world-space point is inside this zone. Uses a small OverlapSphere so inside/outside is correct for trigger and non-trigger colliders.</summary>
    public bool ContainsPoint(Vector3 point)
    {
        if (_collider == null) return false;
        // A point is inside if a tiny sphere at that position overlaps this collider.
        const float radius = 0.001f;
        Collider[] overlaps = Physics.OverlapSphere(point, radius, ~0, QueryTriggerInteraction.Collide);
        for (int i = 0; i < overlaps.Length; i++)
            if (overlaps[i] == _collider)
                return true;
        return false;
    }

    /// <summary>Returns a point outside the zone near the given point. If the point is inside, returns a point just outside the surface so the fish can escape.</summary>
    public Vector3 ClosestPointOutside(Vector3 point)
    {
        if (_collider == null) return point;
        if (!ContainsPoint(point))
            return point; // already outside
        Vector3 closest = _collider.ClosestPoint(point);
        // Nudge outward from surface (from point toward surface = closest - point, so outward from surface is same direction)
        Vector3 outward = (closest - point);
        if (outward.sqrMagnitude < 0.0001f)
            outward = (point - _collider.bounds.center).normalized;
        else
            outward = outward.normalized;
        return closest + outward * 0.05f;
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool drawInEditor = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0.3f, 0.25f);

    private void OnDrawGizmos()
    {
        if (!drawInEditor) return;

        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = gizmoColor;
        Bounds b = col.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoColor.a * 0.5f);
        Gizmos.DrawCube(b.center, b.size);
    }
#endif
}
