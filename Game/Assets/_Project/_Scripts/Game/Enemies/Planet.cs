using _Project._Scripts.Game.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project._Scripts.Game.Enemies
{
    public class Planet : MonoBehaviour
    {
        public string planetName;
        public int resourcePerHit = 10;
        public int cost = 100;
        public bool isUnlocked;
        public SpriteRenderer spriteRenderer;

        [Header("Effect Settings")] [SerializeField]
        private GameObject explosionSpritePrefab; // Prefab sprite cho hiệu ứng cháy nổ

        [Header("Item Drop Settings")] [SerializeField]
        private GameObject itemPrefab; // Prefab của item (viên ngọc)

        [SerializeField] private float itemDropChance = 0.3f; // Xác suất rơi item (30%)
        [SerializeField] private GameObject resourceTextPrefab; // Prefab TextMeshProUGUI cho số lượng

        public void TakeDamage(float damage)
        {
            if (!isUnlocked) return;

            // Tăng tài nguyên
            // Shake();
        }

        // public void BuyPlanet()
        // {
        //     if (!isUnlocked && GameManager.Instance.resources >= cost)
        //     {
        //         GameManager.Instance.resources -= cost;
        //         isUnlocked = true;
        //     }
        // }

        // private void Shake()
        // {
        //     spriteRenderer.transform.localPosition = Vector3.zero;
        //     spriteRenderer.transform.DOShakePosition(0.2f, 0.1f, 10, 90f).SetEase(Ease.InOutSine)
        //         .OnComplete(() => spriteRenderer.transform.localPosition = Vector3.zero);
        // }
    }
}