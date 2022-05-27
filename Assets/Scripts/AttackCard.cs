using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    [CreateAssetMenu(menuName =("Card/AttackCard"))]
    public class AttackCard : ScriptableObject
    {
        public GameObject CardObject { get => cardObj; set => cardObj = value; }

        [SerializeField] private GameObject cardObj;
        [SerializeField] private int damage;
        [SerializeField] private int ammoCapacity;

        
    }
}
