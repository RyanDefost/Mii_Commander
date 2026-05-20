using UnityEngine;

/// <summary>
/// This static class is used for creating extensions on Unity's vector classes
/// Adding extra functionality
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    /// Converts a Vector3 into a Vector2 based on X and Y
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Vector2 XY(this Vector3 a) => new(a.x, a.y);
    
    /// <summary>
    /// Creates a Vector3 where its X and Y are overwritten by that of a given Vector2
    /// </summary>
    /// <param name="a">Vector3 to overwrite</param>
    /// <param name="b">Vector2 containing the new data</param>
    /// <returns></returns>
    public static Vector3 OverwriteXY(this Vector3 a, Vector2 b) => new(b.x, b.y, a.z);

    public static Vector2 DirectionTo(this Vector2 a, Vector2 b) => (b - a).normalized;
}