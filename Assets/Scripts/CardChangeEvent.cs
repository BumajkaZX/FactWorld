using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public static class CardChangeEvent 
    {
        public static UnityEvent<List<AttackObject>> gunChange = new UnityEvent<List<AttackObject>>();

        public static List<AttackObject> attackObjects = new List<AttackObject>();
        public static void GunChange(List<AttackObject> guns)
        {
            gunChange.Invoke(guns);
        }
    }
}
