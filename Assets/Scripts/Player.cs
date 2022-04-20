using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public class Player : MonoBehaviour, INHero
    {
        public LayerMask PlayerMask { get => _playerMask; set => _playerMask = value; }
        public int HP { get => _hp; set => _hp = value; }
        public List<AttackObject> Guns { get => _guns; set => _guns = value; }

        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private int _hp;
        [SerializeField] private List<AttackObject> _guns;

        private void Awake()
        {
            
        }

        [ContextMenu("Set gun")]
        private void NewGun()
        {
            CardChangeEvent.GunChange(Guns);
        }
        
    }
}
