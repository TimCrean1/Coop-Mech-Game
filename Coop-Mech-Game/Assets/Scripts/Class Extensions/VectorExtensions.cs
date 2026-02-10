using UnityEngine;

public static class VectorExtensions
{

    public static Vector3 GetDirectionFromRaycastHit(this RaycastHit hit, Vector3 origin)
    {
        return (hit.point - origin).normalized;
    }

    [Tooltip("v1 is the ending or far vector, v2 is the close one")]
    public static Vector3 GetDirectionFromVectors(this Vector3 v1, Vector3 v2)
    {
        return (v1 - v2).normalized;
    }

    public static Vector3 UnitiseVector3Components(this Vector3 vec)
    {
        return new Vector3(Mathf.Sign(vec.x), Mathf.Sign(vec.y), Mathf.Sign(vec.z));
    }

    public static Vector2 UnitiseVector2Components(this Vector2 vec)
    {
        return new Vector2(Mathf.Sign(vec.x), Mathf.Sign(vec.y));
    }
}
