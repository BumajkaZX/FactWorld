using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Threading.Tasks;

namespace FactWorld
{
    public abstract class EnemyBase : MonoBehaviour, IEnemy
    {
        public abstract float SpeedAnimation { get; set; }

        public abstract LayerMask MaskForHex { get; }

        public abstract LayerMask MaskForSound{ get; }
        public abstract int Damage { get; set; }

        public abstract int HP { get; set; }
        
        public abstract int ID { get; set; }

        public abstract int DiedStep { get; set; }

        public abstract int AliveStep { get; set; }

        public abstract int StepsToAlive { get; set; }

        public abstract int Regeneration { get; set; }

        public abstract float ActiveRadius { get; set; }

        public abstract float SoundRadius { get; set; }
        
        public abstract float JumpStep { get; set; }

        public abstract Vector3 LoudestSoundPosition { get; set; }
        
        public abstract Vector3 EnemyOffsetOnHex { get; set; }
        public abstract Vector3 Position { get; set; }

        public abstract bool IsAlive { get; set; }

       

        public virtual void Start()
        {
            if (Position != Vector3.zero) gameObject.transform.position = Position;
        }
        public virtual async Task PathFinding()
        {
            if (!IsAlive)
            {
                Rebirth();
                return;
            }
            SoundFinding();
            if (HP + Regeneration >= 100) HP = 100; 
            else HP += Regeneration;
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
            var goToPosition = closetPosition - EnemyOffsetOnHex;
            
            var delay = Mathf.Abs(1 / (Time.fixedDeltaTime * SpeedAnimation));
            var t = 0f;
            var middle = Vector3.Lerp(gameObject.transform.position, goToPosition, 0.5f);
            var from = gameObject.transform.position;
            var up = Mathf.Abs((transform.position.y - goToPosition.y) * 2.1f) + Mathf.Abs(EnemyOffsetOnHex.y * 2.1f);  
            middle += new Vector3(0, up , 0);
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                gameObject.transform.position = Vector3.Lerp(Vector3.Lerp(from, middle, t), Vector3.Lerp(middle, goToPosition, t), t);
                t += Time.fixedDeltaTime * SpeedAnimation;
                if (t >= 1) 
                { 
                    disposible.Clear();
                    Position = gameObject.transform.position;
                }
            }).AddTo(disposible);
            Attack();
            await Task.Delay((int)delay * 10);
        }

        public virtual void SoundFinding()
        {
            List<Transform> soundPosition = new List<Transform>();
            Collider[] inRad = Physics.OverlapSphere(transform.position, SoundRadius, MaskForSound); //Step 2.5
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
        public virtual void Attack()
        {

        }

        public virtual void Rebirth()
        {        
            AliveStep++;
            if (AliveStep == StepsToAlive + DiedStep)
            {
                IsAlive = true;
                HP = 20;
            }       
        }

        public virtual void NotDieYet() 
        {
            if (HP <= 0)
            {
                IsAlive = false;
                DiedStep = GameEventManager.step;
            }
        }
    }
}
