using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public float duration = 0.1f; // Thời gian mỗi frame
    public float pauseDuration = 3f; // Thời gian tạm dừng (3 giây)

    [SerializeField] public ImageAnimationSO ImageAnimationSO;

    private Image image;
    private int index = 0;
    private float timer = 0;
    private bool isPaused = false;

    void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (ImageAnimationSO == null) return;
        if (isPaused)
        {
            // Nếu đang tạm dừng, giảm timer và kiểm tra xem đã hết thời gian tạm dừng chưa
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isPaused = false; // Kết thúc tạm dừng
                index = 0; // Reset về sprite đầu tiên
            }

            return;
        }

        // Cập nhật timer và chuyển sprite
        // if ((timer += Time.deltaTime) >= duration/ ImageAnimationSO.sprites.Length)
        if ((timer += Time.deltaTime) >= duration)
        {
            timer = 0;
            image.sprite = ImageAnimationSO.sprites[index];
            index++;

            // Nếu đã đến sprite cuối cùng, kích hoạt chế độ tạm dừng
            if (index >= ImageAnimationSO.sprites.Length)
            {
                isPaused = true;
                timer = pauseDuration; // Đặt timer cho thời gian tạm dừng
            }
        }
    }
}