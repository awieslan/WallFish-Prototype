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

    /// <summary>Returns true if the world-space point is inside this zone.</summary>
    public bool ContainsPoint(Vector3 point)
    {
        if (_collider == null) return false;

        // Raycast from point outward; if we hit this collider, the point is inside
        RaycastHit hit;
        return _collider.Raycast(new Ray(point, Vector3.up), out hit, 1000f);
    }

    /// <summary>Returns the closest point on the zone boundary to the given point. Use to push a point outside.</summary>
    public Vector3 ClosestPointOutside(Vector3 point)
    {
        if (_collider == null) return point;
        return _collider.ClosestPoint(point);
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
