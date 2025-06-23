using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace _Project._Scripts.Game.Managers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] public bool disable;

        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 5f; // Tốc độ di chuyển camera

        [SerializeField] private float parallaxFactor = 0.5f; // Tỷ lệ di chuyển background
        [SerializeField] private Transform background; // Transform của background

        [Header("Zoom Settings")] [SerializeField]
        private float zoomSpeed = 2f; // Tốc độ zoom

        [SerializeField] private float minZoom = 2f; // Orthographic size tối thiểu
        [SerializeField] private float maxZoom = 10f; // Orthographic size tối đa
        [SerializeField] private float zoomDelta = 0.1f; // Hệ số điều chỉnh zoom

        [Header("Boundary Settings")] [SerializeField]
        private Vector2 xBounds = new Vector2(-4f, 4f); // Giới hạn x (-4, 4)

        [SerializeField] private Vector2 yBounds = new Vector2(-3f, 3f); // Giới hạn y (-3, 3)

        private Camera mainCamera;
        private Vector3 lastMousePosition; // Vị trí chuột trước đó
        private float targetZoom; // Orthographic size mục tiêu khi zoom
        private bool isDragging; // Trạng thái kéo camera

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            targetZoom = mainCamera.orthographicSize;
            if (background == null)
            {
                Debug.LogWarning("Background not assigned in CameraController.");
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.OnDragCamera += HandleDragCamera;
            InputManager.Instance.OnZoomCamera += HandleZoomCamera;
        }

        private void OnDisable()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnDragCamera -= HandleDragCamera;
                InputManager.Instance.OnZoomCamera -= HandleZoomCamera;
            }
        }

        private void Update()
        {
            if (Disable()) return;

            if (!isDragging) return;

            Vector3 mousePosition = InputManager.Instance.MousePosition;
            Vector3 mouseDelta = mousePosition - lastMousePosition;
            // Chuyển đổi delta chuột sang tọa độ thế giới
            Vector3 moveDelta = new Vector3(
                -mouseDelta.x * moveSpeed * Time.deltaTime / Screen.width * 2000,
                -mouseDelta.y * moveSpeed * Time.deltaTime / Screen.height * 2000,
                0);
            // Tính vị trí mới của camera
            Vector3 newPosition = transform.position + moveDelta;
            // Giới hạn vị trí camera trong phạm vi x (-4, 4) và y (-3, 3)
            newPosition.x = Mathf.Clamp(newPosition.x, xBounds.x, xBounds.y);
            newPosition.y = Mathf.Clamp(newPosition.y, yBounds.x, yBounds.y);
            // Di chuyển camera mượt bằng DOTween
            transform.DOMove(newPosition, 0.1f).SetEase(Ease.OutSine);
            // Di chuyển background (parallax)
            if (background != null)
            {
                Vector3 bgTarget = background.position + moveDelta * parallaxFactor;
                bgTarget.x = Mathf.Clamp(bgTarget.x, xBounds.x / 4, xBounds.y / 4);
                bgTarget.y = Mathf.Clamp(bgTarget.y, yBounds.x / 4, yBounds.y / 4);
                background.DOMove(bgTarget, 0.2f).SetEase(Ease.OutSine);
            }

            lastMousePosition = mousePosition;
        }

        private void HandleDragCamera(object sender, InputManager.OnDragCameraEventArgs e)
        {
            switch (e.Type)
            {
                case InputManager.OnDragCameraEventArgs.EventType.Start:
                    isDragging = true;
                    lastMousePosition = e.MousePosition;
                    break;
                case InputManager.OnDragCameraEventArgs.EventType.Cancel:
                    isDragging = false;
                    lastMousePosition = Vector3.zero;
                    break;
            }
        }

        private void HandleZoomCamera(object sender, InputManager.OnZoomCameraEventArgs e)
        {
            if (Disable()) return;
            // Điều chỉnh zoom dựa trên lăn chuột
            targetZoom -= e.Value * zoomSpeed * zoomDelta;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            // Zoom mượt bằng DOTween
            DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, targetZoom, 0.2f)
                .SetEase(Ease.OutSine);
        }

        public bool Disable()
        {
            return UIManager.Instance.isHoverUI || disable;
        }
    }
}