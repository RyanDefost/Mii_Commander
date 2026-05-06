using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 XY(this Vector3 a) => new(a.x, a.y);
    public static Vector3 OverwriteXY(this Vector3 a, Vector2 b) => new(b.x, b.y, a.z);
}