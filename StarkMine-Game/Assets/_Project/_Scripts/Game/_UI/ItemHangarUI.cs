using UnityEngine;
using UnityEngine.UI;

public class ItemHangarUI : MonoBehaviour
{
    [SerializeField] private Image shipImage;
    [SerializeField] private ImageAnimation imageAnimation;
    private ImageAnimationSO _shipImageAnimationSO;

    public void SetUp(ImageAnimationSO shipImageAnimationSO)
    {
        if (shipImageAnimationSO == null)
        {
            shipImage.gameObject.SetActive(false);
            return;
        }

        shipImage.gameObject.SetActive(true);
        _shipImageAnimationSO = shipImageAnimationSO;
        imageAnimation.ImageAnimationSO = _shipImageAnimationSO;
        shipImage.sprite = shipImageAnimationSO.sprites[0];
        shipImage.SetNativeSize();
    }
}