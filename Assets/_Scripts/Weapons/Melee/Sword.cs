using SSSProject.Utilities;
using UnityEngine;

namespace SSSProject.Weapons.Melee
{
    public class Sword : MeleeWeapon
    {
        protected override bool CheckIfCanUse()
        {
            return Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimatorParameters.SwordIdle);
        }
    }
}
