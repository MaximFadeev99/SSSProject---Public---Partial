using SSSProject.DataSO.Weapons;
using UnityEngine;

namespace SSSProject.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [field: SerializeField] protected Transform SpriteTransform { get; private set; }
        public abstract WeaponData WeaponData { get; }
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }
        public Vector3 WeaponRotation => SpriteTransform.localEulerAngles;
        protected Animator Animator { get; private set; }
        protected LayerMask HitLayerMask { get; private set; }

        public virtual void Awake() 
        {
            GameObject = gameObject;
            Transform = transform;
            Animator = GetComponent<Animator>();
        }

        public abstract void TryUse();

        public virtual void OnEquipped(Transform parent, Vector2 localPosition, LayerMask hitLayerMask) 
        {
            Transform.SetParent(parent);
            Transform.localPosition = localPosition;
            GameObject.SetActive(true);
            HitLayerMask = hitLayerMask;
        }

        public abstract bool CheckIfMovementAllowed();
    }
}