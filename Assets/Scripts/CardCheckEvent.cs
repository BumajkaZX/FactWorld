using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public static class CardCheckEvent
    {
        public static UnityEvent buttonCheck = new UnityEvent();

        public static UnityEvent<bool> hideUICards = new UnityEvent<bool>();

        public static UnityEvent<string> desctiption = new UnityEvent<string>();

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
            desctiption.Invoke(text);
        }
    }
}
