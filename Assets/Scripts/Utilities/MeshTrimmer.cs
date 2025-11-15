using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility class for cutting/trimming 3D meshes
/// Note: This is a basic implementation. For production, consider using a mesh cutting library
/// </summary>
public static class MeshTrimmer
{
    /// <summary>
    /// Cuts a mesh using a plane and returns the resulting mesh parts
    /// </summary>
    public static Mesh[] CutMesh(Mesh mesh, Plane cutPlane)
    {
        if (mesh == null)
        {
            Debug.LogError("Mesh is null!");
            return null;
        }

        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<int> triangles = new List<int>(mesh.triangles);
        List<Vector3> normals = new List<Vector3>(mesh.normals);
        List<Vector2> uvs = new List<Vector2>(mesh.uv);

        // Separate vertices into two groups based on which side of the plane they're on
        List<int> abovePlane = new List<int>();
        List<int> belowPlane = new List<int>();
        List<int> onPlane = new List<int>();

        for (int i = 0; i < vertices.Count; i++)
        {
            float distance = cutPlane.GetDistanceToPoint(vertices[i]);
            if (distance > 0.01f)
            {
                abovePlane.Add(i);
            }
            else if (distance < -0.01f)
            {
                belowPlane.Add(i);
            }
            else
            {
                onPlane.Add(i);
            }
        }

        // TODO: Implement full mesh cutting algorithm
        // This is a placeholder - actual implementation would:
        // 1. Find edges that cross the plane
        // 2. Create new vertices at intersection points
        // 3. Split triangles that cross the plane
        // 4. Create cap geometry for the cut surface
        // 5. Generate new meshes for both parts

        Debug.LogWarning("MeshTrimmer.CutMesh is not fully implemented. Consider using a mesh cutting library.");

        // Return original mesh for now (no cutting performed)
        return new Mesh[] { mesh };
    }

    /// <summary>
    /// Creates a plane from a point and normal
    /// </summary>
    public static Plane CreatePlane(Vector3 point, Vector3 normal)
    {
        return new Plane(normal.normalized, point);
    }

    /// <summary>
    /// Gets the intersection point of a line segment with a plane
    /// </summary>
    public static bool GetLinePlaneIntersection(Vector3 lineStart, Vector3 lineEnd, Plane plane, out Vector3 intersection)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float distance;
        Ray ray = new Ray(lineStart, lineDirection);

        if (plane.Raycast(ray, out distance))
        {
            intersection = ray.GetPoint(distance);
            // Check if intersection is within line segment
            float lineLength = Vector3.Distance(lineStart, lineEnd);
            if (distance <= lineLength)
            {
                return true;
            }
        }

        intersection = Vector3.zero;
        return false;
    }
}

