using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
        
        public abstract Vector3 EnemyOffsetOnHex { get; set; }

        public virtual void Awake()
        {
            GameEventManager.Turn.AddListener(SoundFinding);
            GameEventManager.MainController.Enemies.Add(gameObject);
        }
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
            var minDistance = float.MaxValue;
            var closetPosition = Vector3.zero;
            for (int i = 0; i < availablePosition.Count; i++)
            {
                var distance = Vector3.SqrMagnitude(LoudestSoundPosition - availablePosition[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closetPosition = availablePosition[i].position;
                }
            }

            CompositeDisposable disposible = new CompositeDisposable();
            var goToPosition = closetPosition + EnemyOffsetOnHex;
            var t = 0f;
            Observable.EveryFixedUpdate().Subscribe(_ => 
            {
                print(t);
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, goToPosition, t);
                t += Time.fixedDeltaTime;
                if (t >= 1) { disposible.Clear(); }
            }).AddTo(disposible);

        }

        public virtual void SoundFinding()
        {
            List<Transform> soundPosition = new List<Transform>();
            Collider[] inRad = Physics.OverlapSphere(transform.position, SoundRadius, MaskForSound); //Step 2.5
            print("Sound Obj Length  =" + inRad.Length);
            for (int i = 0; i < inRad.Length; i++)
            {
                var obj = inRad[i].gameObject;
                soundPosition.Add(obj.transform);
    
            }

            var loudNoise = 0f;
            var posOfLoudNoise = LoudestSoundPosition;
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
