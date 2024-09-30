using SSSProject.DataSO.Weapons;
using SSSProject.Utilities;
using UnityEngine;

namespace SSSProject.Weapons.Melee
{
    public abstract class MeleeWeapon : Weapon
    {
        [field: SerializeField] public MeleeWeaponData MeleeWeaponData { get; private set; }
        [SerializeField] private Collider2D _collider;

        public bool IsBlocking { get; private set; }
        public override WeaponData WeaponData => MeleeWeaponData;

        public override void TryUse()
        {
            if (CheckIfCanUse() == false)
                return;

            Animator.SetTrigger(AnimatorParameters.Swing);
        }

        public void Block() 
        {
            Animator.SetBool(AnimatorParameters.IsBlocking, true);
            IsBlocking = true;
        }

        public void StopBlocking() 
        {
            Animator.SetBool(AnimatorParameters.IsBlocking, false);
            IsBlocking = false;
        }

        public override bool CheckIfMovementAllowed() 
        {
            return !IsBlocking;      
        }

        protected abstract bool CheckIfCanUse();

        #region Animation Events
        public void ActivateCollider()
        {
            _collider.enabled = true;
        }

        public void DeactivateCollider()
        {
            _collider.enabled = false;
        }

        #endregion
    }
}
