using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimatorManager))]
    public class AnimatorManagerInspector : BaseInspector
    {
        protected override void FormatLabel()
        {
            label = "Animator Manager";
            underLabel = "Third Person System";
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(HealthManager))]
    public class HealthManagerInspector : BaseInspector
    {
        protected override void FormatLabel()
        {
            label = "Health Manager";
            underLabel = "Third Person System";
        }
    }
}