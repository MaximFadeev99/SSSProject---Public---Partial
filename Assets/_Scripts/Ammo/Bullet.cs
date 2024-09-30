using SSSProject.DataSO.Ammo;
using SSSProject.Utilities;
using UnityEngine;

namespace SSSProject.Ammo
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Bullet : MonoBehaviour, IMonoBehaviourPoolElement
    {
        [field: SerializeField] public BulletData BulletData { get; private set; }

        private bool _isFlying = false;
        private float _flyingTimer = 0f;
        private Vector2 _flyingDirection;

        public Transform Transform { get; private set; }
        public GameObject GameObject { get; private set; }
        public Vector3 LaunchPoint { get; private set; }
        protected Rigidbody2D Rigidbody { get; private set; }

        public void Awake()
        {
            GameObject = gameObject;
            Transform = transform;
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (_isFlying == false)
                return;

            _flyingTimer += Time.fixedDeltaTime;

            if (_flyingTimer > BulletData.FlyingTime)
                EndFlying();

            Rigidbody.velocity = _flyingDirection * BulletData.FlyingForce;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            EndFlying();
        }

        public void StartFlying(Vector3 startFlyingPosition, Quaternion startFlyingRotation, Vector3 flyingDirection) 
        {
            Transform.SetPositionAndRotation(startFlyingPosition, startFlyingRotation);
            _flyingDirection = flyingDirection;
            LaunchPoint = Transform.position;
            _isFlying = true;
        }

        private void EndFlying() 
        {
            GameObject.SetActive(false);
            _isFlying = false;
            _flyingTimer = 0f;
        }
    }
}
