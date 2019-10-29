using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    [CreateAssetMenu]
    public class CameraData : ScriptableObject
    {
        public Vector3 Offset;
        public float FieldOfView = 60f;
    }
}