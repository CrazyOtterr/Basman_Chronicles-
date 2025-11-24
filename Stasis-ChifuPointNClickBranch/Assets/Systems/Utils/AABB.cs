using UnityEngine;

public struct AABB {
    public AABB(Vector2 min, Vector2 max) {
        this.min = min; this.max = max;
    }
    public Vector2 min, max;
    public Vector2 GetCenter() {
        return (max + min) / 2.0f;
    }
    public Vector2 GetExtents() {
        return (max - min) / 2.0f;
    }
    public static AABB operator +(AABB aabb, Vector2 position) {
        return new AABB(aabb.min + position, aabb.max + position);
    }
    public void DrawGizmo() {
        Gizmos.DrawLine(min, new Vector3(min.x, max.y));
        Gizmos.DrawLine(new Vector3(min.x, max.y), max);
        Gizmos.DrawLine(max, new Vector3(max.x, min.y));
        Gizmos.DrawLine(new Vector3(max.x, min.y), min);
    }
}
