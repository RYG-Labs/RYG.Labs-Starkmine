using UnityEngine;

namespace _Project._Scripts.Game.Enemies
{
    public class ReciveItem : MonoBehaviour
    {
        [SerializeField] private Sprite visual;
        [SerializeField] private TextMesh amountText;

        public void Setup(Sprite visual, TextMesh amountText)
        {
            this.visual = visual;
            this.amountText = amountText;
        }
    
    
    
    }
}