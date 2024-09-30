using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SSSProject.Weapons.Grenades
{
    public class ExplosionArea : MonoBehaviour
    {
        private CircleCollider2D _circleCollider;
        private ParticleSystem _particleSystem;
        private Transform _transform;
        private float _endRadius;

        public Vector2 Epicenter => _transform.position;
        public int Damage { get; private set; }
        public bool IsExpanding { get; private set; }

        internal void Initialize(float explosionRadious, int damage) 
        {
            _endRadius = explosionRadious;
            Damage = damage;
            _transform = transform;
            _circleCollider = GetComponent<CircleCollider2D>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        internal async UniTask Expand() 
        {
            IsExpanding = true;
            _particleSystem.Play();

            float expansionStep = _endRadius / (_particleSystem.main.startLifetime.constant * 100f);

            while (_circleCollider.radius < _endRadius)
            {
                _circleCollider.radius += expansionStep;
                await UniTask.WaitForSeconds(0.01f);
            }

            Reset();
        }

        private void Reset()
        {
            _circleCollider.radius = 0.0001f;
            _particleSystem.Stop();
            IsExpanding = false;
        }
    }
}