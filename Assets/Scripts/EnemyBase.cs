using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        public abstract LayerMask mask { get; }
        public abstract int Damage { get; set; }

        public abstract int HP { get; }

        public abstract float ActiveRadius { get; set; }
        
        public abstract float JumpStep { get; set; }

        public virtual void PathFinding()
        {
            
        }

        public virtual void SoundFinding()
        {
           
        }

    }
}
