using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        public abstract LayerMask MaskForHex { get; }

        public abstract LayerMask MaskForSound{ get; }
        public abstract int Damage { get; set; }

        public abstract int HP { get; }

        public abstract float ActiveRadius { get; set; }

        public abstract float SoundRadius { get; set; }
        
        public abstract float JumpStep { get; set; }

        public abstract Vector3 LoudestSoundPosition { get; set; }

        public virtual void PathFinding()
        {
            List<Transform> availablePosition = new List<Transform>();
            Collider[] inRad = Physics.OverlapSphere(transform.position, ActiveRadius, MaskForHex); //Step 2.5
            for (int i = 0; i < inRad.Length; i++)
            {
                var cl = inRad[i].GetComponent<InteractWithField>();
                var point = cl.GetMaxPoint();
                if (cl.GetMaxPoint() <= point + JumpStep && transform.position != cl.transform.position)
                {
                    availablePosition.Add(cl.transform);
                }
            }

        }

        public virtual void SoundFinding()
        {
            List<Transform> soundPosition = new List<Transform>();
            Collider[] inRad = Physics.OverlapSphere(transform.position, SoundRadius, MaskForSound); //Step 2.5
            for (int i = 0; i < inRad.Length; i++)
            {
                var obj = inRad[i].gameObject;

                if (obj.layer == MaskForSound)
                {
                    soundPosition.Add(obj.transform);
                }
                
            }

            var loudNoise = 0f;
            var posOfLoudNoise = Vector3.zero;
            for (int i = 0; i < soundPosition.Count; i++)
            {
               var noise = soundPosition[i].GetComponent<ISound>().Noise;
                if (noise > loudNoise)
                {
                    loudNoise = noise;
                    posOfLoudNoise = soundPosition[i].position;
                }
            }

            LoudestSoundPosition = posOfLoudNoise;
        }

    }
}
