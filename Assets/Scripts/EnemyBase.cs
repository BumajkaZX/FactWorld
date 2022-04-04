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
            Collider[] inRad = Physics.OverlapSphere(transform.position, ActiveRadius, mask); //Step 2.5
            for (int i = 0; i < inRad.Length; i++)
            {
                var cl = inRad[i].GetComponent<InteractWithField>();
                var point = cl.GetMaxPoint();
                if (cl.GetMaxPoint() <= point + JumpStep)
                {

                }
            }
        }

        public virtual void SoundFinding()
        {
           
        }

    }
}
