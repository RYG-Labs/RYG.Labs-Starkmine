using System.Collections.Generic;
using System.Globalization;
using _Project._Scripts.Game.Enemies;
using UnityEngine;

namespace _Project._Scripts.Game.Poolings
{
    public class BulletPooling : StaticInstance<BulletPooling>
    {
        public Bullet bulletPrefab;

        [SerializeField] private int poolSize = 5;
        private readonly List<Bullet> _bulletList = new();

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        public void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                Bullet bulletObj = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity, transform);
                bulletObj.gameObject.SetActive(false);
                _bulletList.Add(bulletObj);
            }
        }

        public Bullet GetFromPool(Vector3 position, Transform targetPlanet, float bulletDamage, Ship spaceship,
            BulletSO bulletSO)
        {
            Bullet bulletObj = _bulletList.Find(coin => !coin.gameObject.activeInHierarchy);
            if (bulletObj == null)
            {
                bulletObj = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity, transform);
                _bulletList.Add(bulletObj);
            }

            bulletObj.transform.position = position;
            bulletObj.Setup(targetPlanet, bulletDamage, spaceship, bulletSO);
            bulletObj.gameObject.SetActive(true);
            return bulletObj;
        }

        public void ReturnToPool(Bullet bulletObj)
        {
            bulletObj.gameObject.SetActive(false);
        }

        public void ClearPool()
        {
            _bulletList.Clear();
            transform.DestroyChildren();
        }
    }
}