using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public static class CardCheckEvent
    {
        public static UnityEvent buttonCheck = new UnityEvent();

        public static UnityEvent<bool> hideUICards = new UnityEvent<bool>();

        public static UnityEvent<string> description = new UnityEvent<string>();

        public static UnityEvent attackButton = new UnityEvent();

        public static CardCheck activeCard;

        public static float scale;

        public static float speed;

        public static bool gyroEnable;

        public static void ResetPositionNActiveDescription()
        {
            buttonCheck.Invoke();
        }
        
        public static void Hide(bool isActive)
        {
            hideUICards.Invoke(isActive);
        }

        public static void DescriptionActive(string text)
        {
            description.Invoke(text);
        }

        public static void ActiveButton()
        {
            attackButton.Invoke();
        }
    }
}
