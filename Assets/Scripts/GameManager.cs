using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public static class GameManager 
    {
        public static MainController mainController;
        public static UnityEvent<int> activeHex = new UnityEvent<int>();
        public static Vector3 activeHexPosition;
        public static float activeHexAnimationSpeed;
        public static int activeHexID;
   
        public static void UpHex(int hexID)
        {
            activeHexID = hexID;
            Debug.Log(activeHexID);
            activeHex.Invoke(hexID);
        }
    }
}
