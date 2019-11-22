using UnityEngine;

public static class LayerMaskManager
{
    public static LayerMask IgnoreOnlyPlayer = ~(1 << 15);
    public static LayerMask GroundMasks = (1 << 14);
}
