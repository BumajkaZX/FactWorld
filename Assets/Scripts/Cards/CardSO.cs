using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{

    public abstract class CardSO : ScriptableObject
    {
        public GameObject placedObject;
        public Sprite cardSprite;
        public string cardName;
        public string description;
        public _cardsType cardsType;
        public enum _cardsType
        {
            CityMain,
            Factories,
            Homes,
            Defend,
            Extensions
        };
    }
}
