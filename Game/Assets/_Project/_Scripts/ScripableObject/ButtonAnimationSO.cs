using UnityEngine;

[CreateAssetMenu(fileName = "ButtonAnimationSO", menuName = "Scriptable Objects/ButtonAnimationSO")]
public class ButtonAnimationSO : ScriptableObject
{
    [SerializeField] public Color borderColor;
    [SerializeField] public Color backgroundColor;
    [SerializeField] public Color fillColor;
    [SerializeField] public Color textColor;
    [SerializeField] public Vector3 scale;
    [SerializeField, Range(0f, 1f)] public float fillAmount; // Giá trị fill từ 0 đến 1
}