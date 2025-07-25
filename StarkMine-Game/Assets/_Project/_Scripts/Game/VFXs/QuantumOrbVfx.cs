using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

public class QuantumOrbVfx : MonoBehaviour
{
    private static string sizePropertyName = "Size";
    private static string alphaPropertyName = "Alpha"; // Thuộc tính expose để điều khiển độ mờ
    private static string colorPropertyName = "Color"; // Thuộc tính expose để điều khiển màu sắc

    [SerializeField] private VisualEffect vfx;
    [SerializeField] private float targetSize = 18f; // Giá trị size mục tiêu
    [SerializeField] private float duration = 5f; // Thời gian thực hiện mỗi hiệu ứng scale
    [SerializeField] private float fadeDuration = 3f; // Thời gian thực hiện hiệu ứng mờ dần
    [SerializeField] private Ease easeType = Ease.Linear; // Kiểu easing
    [SerializeField] private float initialDelay = 10f; // Thời gian chờ trước khi bắt đầu scale lên
    [SerializeField] private float secondDelay = 10f; // Thời gian chờ trước khi scale xuống
    private Sequence colorSequence; // Sequence riêng cho đổi màu
    [SerializeField] private float colorChangeDuration = 3f; // Thời gian mỗi bước đổi màu (3 giây)

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
    }

    private void OnEnable()
    {
        // Đặt giá trị ban đầu
        vfx.SetFloat(sizePropertyName, 0.5f);
        vfx.SetFloat(alphaPropertyName, 1f); // Đảm bảo VFX hiển thị đầy đủ lúc đầu

        // Tạo sequence để điều khiển thứ tự các hiệu ứng
        Sequence sequence = DOTween.Sequence();

        // Sau initialDelay (10 giây), scale từ 0 lên targetSize
        sequence.AppendInterval(initialDelay);
        sequence.Append(DOVirtual.Float(0f, targetSize, duration, (value) => { vfx.SetFloat(sizePropertyName, value); })
            .SetEase(easeType));

        // Sau secondDelay (10 giây tiếp theo), scale từ targetSize về 0
        sequence.AppendInterval(secondDelay);
        sequence.Append(DOVirtual.Float(targetSize, 0f, duration, (value) => { vfx.SetFloat(sizePropertyName, value); })
            .SetEase(easeType));

        // Ngay sau khi scale về 0, bắt đầu mờ dần (Alpha từ 1 về 0)
        sequence.Append(DOVirtual.Float(1f, 0f, fadeDuration, (value) => { vfx.SetFloat(alphaPropertyName, value); })
            .SetEase(easeType));

        //
        colorSequence = DOTween.Sequence();
        colorSequence.Append(DOVirtual.Color(Helpers.Color(26, 193, 25, 255, 3), Helpers.Color(25, 26, 191, 255, 3),
            colorChangeDuration,
            (value) => { vfx.SetVector4(colorPropertyName, value); }).SetEase(easeType)); // Xanh lá -> Xanh dương

        colorSequence.Append(DOVirtual.Color(Helpers.Color(25, 26, 191, 255, 3), Helpers.Color(191, 101, 26, 255, 3),
            colorChangeDuration,
            (value) => { vfx.SetVector4(colorPropertyName, value); }).SetEase(easeType)); // Xanh dương -> Vàng cam

        colorSequence.Append(DOVirtual.Color(Helpers.Color(191, 101, 26, 255, 3), Helpers.Color(101, 26, 191, 255, 3),
            colorChangeDuration,
            (value) => { vfx.SetVector4(colorPropertyName, value); }).SetEase(easeType)); // Vàng cam -> Đỏ hồng

        colorSequence.Append(DOVirtual.Color(Helpers.Color(101, 26, 191, 255, 3), Helpers.Color(26, 193, 25, 255, 3),
                colorChangeDuration,
                (value) => { vfx.SetVector4(colorPropertyName, value); })
            .SetEase(easeType)); // Đỏ hồng -> Xanh lá (lặp lại)

        colorSequence.SetLoops(-1); // Lặp vô hạn
    }

    // [ContextMenu("Run Size Animation")]
    // public void RunSizeAnimation()
    // {
    //     // Đặt giá trị ban đầu của thuộc tính size thành 0
    //     vfx.SetFloat(sizePropertyName, 0f);
    //     
    //     // Sử dụng DOTween để thay đổi giá trị size từ 0 lên targetSize
    //     DOVirtual.Float(0f, targetSize, duration, (value) => { vfx.SetFloat(sizePropertyName, value); })
    //         .SetEase(easeType);
    // }
}