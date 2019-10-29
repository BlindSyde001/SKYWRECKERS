/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using UnityEditor;

namespace DiasGames.ThirdPersonSystem.ClimbingSystem
{
    [CustomEditor(typeof(DropToClimbAbility))]
    [CanEditMultipleObjects]
    public class DropToClimbInspector : ThirdPersonAbilityInspector
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            m_EnterStateLabel = "Drop To Climb State";

        }

        protected override void FormatLabel()
        {
            label = "Drop to Climb";
            underLabel = "Climbing System";
        }
    }
}
