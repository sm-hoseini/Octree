using UnityEngine;

public static class Extensions
{
    public static int CompairTo(this Vector3 a, Vector3 b)
    {
        if (a.x < b.x && a.y < b.y && a.z < b.z)
        {
            return -1;
        }

        if (a.x > b.x && a.y > b.y && a.z > b.z)
        {
            return 1;
        }

        return 0;
    }
}