using UnityEngine;

namespace CardGame
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "CardGame/GameConfig", order = 1)]
    public class GameConfig : ScriptableObject
    {
        public int CardsPairs = 3;
        public float CardFlipAnim = 0.3f;
        public float DeselectCardsDelay = 1f;
    }
}