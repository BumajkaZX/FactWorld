using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public class Player : MonoBehaviour
    {

        public  List<AttackCard> attackCards = new List<AttackCard>(GameManager.MaxAttackCard);
        [SerializeField] private AttackCard barret;
        [SerializeField] private int maxAttackCard;

        private void Awake()
        {
            GameManager.MaxAttackCard = maxAttackCard;
        }
    }
}
