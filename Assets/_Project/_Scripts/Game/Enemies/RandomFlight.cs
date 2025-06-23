using System.Collections.Generic;
using UnityEngine;

public class RandomFlight : MonoBehaviour
{
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 5f); // Kích thước khu vực 2D (X, Y)
    [SerializeField] private float speed = 2f; // Tốc độ bay
    [SerializeField] private float changeTargetInterval = 2f; // Thời gian đổi mục tiêu (giây)
    [SerializeField] private Vector2 flapAmplitudeRange = new Vector2(0.3f, 0.7f); // Phạm vi độ cao nhấp nhô
    [SerializeField] private Vector2 flapFrequencyRange = new Vector2(1.5f, 3f); // Phạm vi tần suất nhấp nhô
    // [SerializeField] private List<SpriteRenderer> spriteRenderers; // Danh sách sprite
    [SerializeField] private bool isFacingRightByDefault = false; // Hướng mặc định của sprite (true = phải, false = trái)

    // private SpriteRenderer _currentSpriteRenderer;
    private Vector2 areaCenter; // Tâm khu vực bay, cố định khi chơi game
    private Vector2 targetPosition; // Vị trí mục tiêu ngẫu nhiên
    private float timeSinceLastChange; // Thời gian kể từ lần đổi mục tiêu cuối
    private float flapAmplitude; // Độ cao nhấp nhô hiện tại
    private float flapFrequency; // Tần suất nhấp nhô hiện tại
    private float flapTime; // Thời gian riêng cho nhấp nhô

    void Start()
    {
        // SetupSprite();
        // Gán vị trí ban đầu của bướm làm tâm khu vực
        areaCenter = transform.position;
        // Chọn mục tiêu ngẫu nhiên ban đầu
        targetPosition = GetRandomPositionInArea();
        // Khởi tạo ngẫu nhiên nhấp nhô
        SetRandomFlapValues();
    }

    private void SetupSprite()
    {
        // foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        // {
        //     spriteRenderer.gameObject.SetActive(false);
        // }
        //
        // _currentSpriteRenderer = spriteRenderers[Random.Range(0, spriteRenderers.Count)];
        // _currentSpriteRenderer.gameObject.SetActive(true);
        // _currentSpriteRenderer.flipX = isFacingRightByDefault; // Đặt lại flipX về mặc định ban đầu
    }

    void Update()
    {
        // Cập nhật thời gian
        timeSinceLastChange += Time.deltaTime;
        flapTime += Time.deltaTime;

        // Đổi mục tiêu sau khoảng thời gian
        if (timeSinceLastChange >= changeTargetInterval)
        {
            targetPosition = GetRandomPositionInArea();
            timeSinceLastChange = 0f;
            SetRandomFlapValues(); // Chọn lại độ cao và tần suất ngẫu nhiên
        }

        // Tính hướng di chuyển
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        // Lật sprite theo hướng bay, dựa trên hướng mặc định
        // if (direction.x > 0) // Bay sang phải
        //     _currentSpriteRenderer.flipX = !isFacingRightByDefault; // Lật nếu mặc định là trái
        // else if (direction.x < 0) // Bay sang trái
        //     _currentSpriteRenderer.flipX = isFacingRightByDefault; // Lật nếu mặc định là phải

        // Di chuyển tới mục tiêu với nhấp nhô
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
        float flapOffset = Mathf.Sin(flapTime * flapFrequency) * flapAmplitude; // Tạo nhấp nhô
        transform.position = new Vector3(newPosition.x, newPosition.y + flapOffset, transform.position.z);
    }

    void SetRandomFlapValues()
    {
        flapAmplitude = Random.Range(flapAmplitudeRange.x, flapAmplitudeRange.y); // Chọn độ cao ngẫu nhiên
        flapFrequency = Random.Range(flapFrequencyRange.x, flapFrequencyRange.y); // Chọn tần suất ngẫu nhiên
        flapTime = 0f; // Reset thời gian nhấp nhô
    }

    Vector2 GetRandomPositionInArea()
    {
        // Tính vị trí ngẫu nhiên trong khu vực 2D quanh tâm
        float x = Random.Range(areaCenter.x - areaSize.x / 2f, areaCenter.x + areaSize.x / 2f);
        float y = Random.Range(areaCenter.y - areaSize.y / 2f, areaCenter.y + areaSize.y / 2f);
        return new Vector2(x, y);
    }

    // Hiển thị khu vực trong Editor để dễ debug
    void OnDrawGizmos()
    {
        Vector2 center = Application.isPlaying ? areaCenter : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, areaSize.y, 0f));
    }
}