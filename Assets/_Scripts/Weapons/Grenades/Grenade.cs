using Cysharp.Threading.Tasks;
using SSSProject.DataSO.Weapons;
using SSSProject.Utilities;
using UnityEngine;

namespace SSSProject.Weapons.Grenades
{
    public class Grenade : Weapon
    {
        [field: SerializeField] public GrenadeData GrenadeData { get; private set; }
        [SerializeField] private ExplosionArea _explosionArea;
        [SerializeField] private Rigidbody2D _spriteRigidbody;
        [SerializeField] private Transform _spriteTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _pointerTransform;

        private float _distanceToTarget;
        private bool _isOnCoolDown;

        public override WeaponData WeaponData => GrenadeData;
        public Vector2 SpriteForward => _spriteTransform.right;

        public override void Awake()
        {
            base.Awake();
            _spriteRigidbody.simulated = false;
            _explosionArea.Initialize(GrenadeData.ExplosionRadius, GrenadeData.Damage);
        }

        public override bool CheckIfMovementAllowed()
        {
            return true;
        }

        public void Aim(Vector3 worldPosition)
        {
            if (_isOnCoolDown)
                return;

            Vector3 aimDirection = worldPosition - Transform.position;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            float absAngle = Mathf.Abs(angle);

            if (absAngle <= GrenadeData.AimLimit)
            {
                Transform.localRotation = Quaternion.Euler(0f, 0f, angle);
                ShapePointer(worldPosition, true);
                return;
            }

            if (absAngle <= 180f && absAngle >= 180 - GrenadeData.AimLimit)
            {
                Transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
                ShapePointer(worldPosition, false);
                return;
            }
        }

        public async override void TryUse()
        {
            if (_isOnCoolDown)
                return;

            _isOnCoolDown = true;
            _spriteTransform.SetParent(null);
            _spriteRigidbody.simulated = true;

            float throwForce = _distanceToTarget.Remap(0f, GrenadeData.MaxThrowDistance, 0f, GrenadeData.MaxThrowForce);
            _spriteRigidbody.AddForce(_spriteTransform.right * throwForce, ForceMode2D.Impulse);
            _pointerTransform.localScale = new(_pointerTransform.localScale.x, _pointerTransform.localScale.y, 0f);
            Transform.localRotation = Quaternion.Euler(Vector3.zero);

            await UniTask.WaitForSeconds(GrenadeData.TimeToExplosion);
            await Explode();

            Reset();
        }

        private void Reset()
        {
            _spriteRenderer.enabled = true;
            _spriteRigidbody.simulated = false;
            _spriteRigidbody.freezeRotation = false;
            _spriteTransform.SetParent(Transform);
            _spriteTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _isOnCoolDown = false;
        }

        private void ShapePointer(Vector3 worldPosition, bool isFacingRight) 
        {
            _distanceToTarget = Vector2.Distance(worldPosition, _spriteTransform.position);
            float zScale = _distanceToTarget.Remap(0f, GrenadeData.MaxThrowDistance, 0.01f, GrenadeData.MaxPointerLength);

            zScale = isFacingRight ? zScale : -zScale;
            _pointerTransform.localScale = new(_pointerTransform.localScale.x, _pointerTransform.localScale.y,
                zScale);        
        }

        private async UniTask Explode() 
        {
            _spriteRenderer.enabled = false;
            _spriteRigidbody.freezeRotation = true;
            await _explosionArea.Expand();
            await UniTask.WaitForSeconds(0.5f);
        }        
    }
}
