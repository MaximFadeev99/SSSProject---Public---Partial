using Cysharp.Threading.Tasks;
using SSSProject.Ammo;
using SSSProject.DataSO.Weapons;
using SSSProject.Utilities;
using UnityEngine;
using Zenject;

namespace SSSProject.Weapons.Ranged
{
    public abstract class RangedWeapon : Weapon
    {
        [field: SerializeField] public RangedWeaponData RangedWeaponData { get; private set; }
        [field: SerializeField] public Bullet LoadedBullet { get; private set; }        
        [field: SerializeField] protected Transform FirePoint { get; private set; }

        private Transform _bulletContainer;
        private bool _isOnCoolDown = false;

        public override WeaponData WeaponData => RangedWeaponData;
        public Vector2 FirePointPosition => FirePoint.position;
        public Vector2 FirepointForward => FirePoint.forward;
        protected MonoBehaviourPool<Bullet> BulletPool { get; private set; }

        [Inject]
        private void Construct(Transform bulletContainer) 
        {
            _bulletContainer = bulletContainer;
        }

        public override void Awake()
        {
            if (GameObject != null)
                return;

            base.Awake();
            BulletPool = new(LoadedBullet, _bulletContainer, 10);
        }

        public void Aim(Vector3 worldPosition) 
        {
            Vector3 aimDirection = worldPosition - SpriteTransform.position + new Vector3 
                (FirePoint.localPosition.z, SpriteTransform.localPosition.x - RangedWeaponData.AimYCorrection, 0f);
            float angle  = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            float absAngle = Mathf.Abs(angle);

            if (absAngle <= RangedWeaponData.AimLimit)
            {
                SpriteTransform.localRotation = Quaternion.Euler(-angle, 90f, 90f);
                return;
            }

            if (absAngle <= 180f && absAngle >= 180 - RangedWeaponData.AimLimit)
            {
                SpriteTransform.localRotation = Quaternion.Euler(-angle, -90f, -90f);
                return;
            }
        }

        public override bool CheckIfMovementAllowed()
        {
            return true;
        }

        public async override void TryUse()
        {
            if (_isOnCoolDown)
                return;

            Bullet idleBullet = BulletPool.GetIdleElement();
            idleBullet.GameObject.SetActive(true);

            float angle = Vector3.SignedAngle(FirePoint.forward, Vector3.right, Vector3.forward);
            Quaternion bulletRotation = Quaternion.AngleAxis(-angle, Vector3.forward);

            idleBullet.StartFlying(FirePoint.position, bulletRotation, FirePoint.forward);
            _isOnCoolDown = true;
            await UniTask.WaitForSeconds(RangedWeaponData.FireRate);
            _isOnCoolDown = false;
        }

        public void Reset()
        {
            SpriteTransform.localRotation = Quaternion.Euler(0f, 90f, 90f);
        }
    }
}
