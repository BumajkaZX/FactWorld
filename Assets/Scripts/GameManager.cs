using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace FactWorld
{
    public static class GameManager 
    {
        public static MainController mainController;
        public static UnityEvent<int> activeHex = new UnityEvent<int>();
        public static UnityEvent<GameObject> activeCardUp = new UnityEvent<GameObject>();
        public static Vector3 activeHexPosition;
        public static float activeHexAnimationSpeed;
        public static int activeHexID, activeCardType, cardTypes;
        public static List<CardSO> cards;
        public static CardSO activeCard;
        public static void UpCard(GameObject card)
        {
            activeCardUp.Invoke(card);
        }
        public static void UpHex(int hexID)
        {
            activeHexID = hexID;
            Debug.Log(activeHexID);
            activeHex.Invoke(hexID);
        }
        
    }
}
