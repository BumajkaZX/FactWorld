using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public static class GameManager 
    {
        public static MainController mainController;
        public static UnityEvent Fight = new UnityEvent();
        public static int MaxAttackCard;
   
        public static void StartFight()
        {
            Debug.Log("Fight Start");
            Fight.Invoke();
        }
    }
}
