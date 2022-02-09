using UnityEngine;

public static class ColliderExtensions
{

    public static bool Overlaps(this Collider c1, Collider c2)
    {
        var t1 = c1.transform;
        var t2 = c2.transform;
        return Physics.ComputePenetration(
            c1, t1.position, t1.rotation,
            c2, t2.position, t2.rotation,
            out var _, out var _
        );
    }

}
