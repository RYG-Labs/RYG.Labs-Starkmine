using System.Collections;
using UnityEngine;
using _Project._Scripts.Game.Managers;
using _Project._Scripts.Game.Poolings;

namespace _Project._Scripts.Game.Enemies
{
    public class Bullet : MonoBehaviour
    {
        private static string shootEffectAnimationName = "ShootEffect";
        private static string hitEffectAnimationName = "HitEffect";
        private Transform target;
        private float damage;
        private float speed = 10f;
        private Ship owner;
        [SerializeField] private Transform visual; // GameObject con chứa sprite/hiệu ứng hình ảnh
        [SerializeField] private BulletSO _bulletSO;
        [SerializeField] private Animator _animator;
        private bool hasHitTarget = false; // Cờ để ngăn coroutine chạy nhiều lần

        public void Setup(Transform targetPlanet, float bulletDamage, Ship spaceship, BulletSO bulletSO)
        {
            target = targetPlanet;
            damage = bulletDamage;
            owner = spaceship;
            _bulletSO = bulletSO;
            speed = bulletSO.speed;
            _animator.runtimeAnimatorController = bulletSO.animatorController;
            SoundManager.Instance.PlaySound(_bulletSO.shootSounds, AudioSourceConfig.SoundType.BulletSound);
            hasHitTarget = false; // Reset cờ khi tái sử dụng
        }

        private void OnEnable()
        {
            _animator.Play(shootEffectAnimationName);
        }

        private void Update()
        {
            if (hasHitTarget || target == null) return; // Ngăn tiếp tục di chuyển nếu đã chạm mục tiêu hoặc target null

            Vector2 direction = (target.position - transform.position).normalized;
            transform.up = direction;
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                hasHitTarget = true;
                Planet planet = target.GetComponent<Planet>();
                if (planet != null)
                {
                    planet.TakeDamage(damage);
                }

                StartCoroutine(ReturnToPoolCoroutine());
            }
        }

        private IEnumerator ReturnToPoolCoroutine()
        {
            _animator.Play(hitEffectAnimationName);

            // Lấy độ dài animation HitEffect
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;

            yield return new WaitForSeconds(animationLength);

            BulletPooling.Instance.ReturnToPool(this);
        }
    }
}