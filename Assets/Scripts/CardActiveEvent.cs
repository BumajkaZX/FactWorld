using UnityEngine.Events;

namespace FactWorld
{
    public static class CardActiveEvent 
    {
        public static IObject ActiveCard;

        public static bool IsAttack;

        public static UnityEvent AcitveGunEvent = new UnityEvent();

        public static float radiusAttack;

        public static void ResetHexPosition()
        {
            AcitveGunEvent.Invoke();
        }
    }
}
