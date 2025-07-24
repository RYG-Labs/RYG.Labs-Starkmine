using System.Collections.Generic;
using _Project._Scripts.Game.Managers;
using _Project._Scripts.Game.Poolings;
using UnityEngine;
using DG.Tweening;

namespace _Project._Scripts.Game.Enemies
{
    public class Ship : MonoBehaviour
    {
        public int starLevel = 1; // Cấp sao (1-5)
        public float damage = 10f; // Sát thương mỗi phát bắn

        public float fireRate = 1f; // Tốc độ bắn (phát/giây)

        // public float durability = 100f; // Độ bền hiện tại
        // public float maxDurability = 100f; // Độ bền tối đa
        public GameObject bulletPrefab; // Prefab đạn

        public Transform targetPlanet; // Hành tinh mục tiêu

        // public GameObject explosionEffectPrefab; // Prefab hiệu ứng bắn
        public float minPatrolRadius = 1f; // Bán kính lượn lờ tối thiểu
        public float patrolRadius = 3f; // Bán kính lượn lờ tối đa
        public float patrolSpeed = 1f; // Tốc độ di chuyển
        private float fireCooldown;
        [SerializeField] private BulletSO bulletSO;
        [SerializeField] private SpriteRenderer visual;
        public ShipData ShipData { get; set; }
        [SerializeField] private List<float> listRandomDistanceTest = new();
        [SerializeField] private List<Vector2> listRandomDirectionTest = new();
        [SerializeField] private int countTest = 0;
        [SerializeField] private bool isTest = false;

        public Sprite Visual
        {
            get => visual.sprite;
            set => visual.sprite = value;
        }

        private void Start()
        {
            StartPatrol();
        }

        private void Update()
        {
            // if (targetPlanet == null || durability <= 0) return;
            if (targetPlanet == null) return;

            // Hướng phi thuyền về hành tinh
            Vector2 direction = (targetPlanet.position - transform.position).normalized;
            transform.up = direction;

            // Bắn đạn tự động
            if (fireCooldown <= 0)
            {
                Shoot();
                fireCooldown = 1f / fireRate;
                // durability -= 0.1f; // Giảm độ bền
            }

            fireCooldown -= Time.deltaTime;
        }

        public void SetUp(ShipData data, Planet target)
        {
            ShipData = data;
            Visual = data.shipSO.baseSprite;
            bulletSO = data.shipSO.bulletSO;
            targetPlanet = target.transform;
            fireRate = data.shipSO.fireRate;
        }

        private void Shoot()
        {
            BulletPooling.Instance.GetFromPool(transform.position, targetPlanet, damage, this, bulletSO);
            RecoilEffect();
        }

        private void RecoilEffect()
        {
            // Hiệu ứng giật lùi phi thuyền bằng DOTween
            // Vector3 recoilPos = transform.position - transform.up * 0.2f;
            // transform.DOMove(recoilPos, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
            // {
            //     transform.DOMove(transform.position + transform.up * 0.2f, 0.1f).SetEase(Ease.InQuad);
            // });
        }

        private void StartPatrol()
        {
            Patrol();
        }

        private void Patrol()
        {
            if (targetPlanet == null) return;

            // Chọn khoảng cách ngẫu nhiên trong khoảng minPatrolRadius đến patrolRadius
            // float randomDistance = Random.Range(minPatrolRadius, patrolRadius);
            // Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance;
            Vector2 randomDirection;
            if (isTest)
            {
                randomDistance = listRandomDistanceTest[countTest];
                randomDirection = listRandomDirectionTest[countTest];
                countTest++;
                if (countTest >= listRandomDistanceTest.Count)
                {
                    countTest = 0;
                }
            }
            else
            {
                randomDistance = Random.Range(minPatrolRadius, patrolRadius);
                randomDirection = Random.insideUnitCircle.normalized;
            }

            Vector3 patrolTarget = targetPlanet.position + (Vector3)(randomDirection * randomDistance);

            // Di chuyển mượt bằng DOTween
            transform.DOMove(patrolTarget, 1f / patrolSpeed).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                DOVirtual.DelayedCall(Random.Range(2f, 5f), Patrol);
            });
        }

        public void Upgrade()
        {
            if (starLevel < 5)
            {
                starLevel++;
                damage += 10f;
                fireRate += 0.5f;
                // maxDurability += 50f;
                // durability = maxDurability;
            }
        }
    }
}