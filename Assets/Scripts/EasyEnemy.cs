using System.Threading.Tasks;
using UnityEngine;

namespace FactWorld
{
    public class EasyEnemy : EnemyBase
    {
        #region params
        public override float SpeedAnimation { get => _speedAnimation; set => _speedAnimation = value; }
        public override int Damage { get => _damage; set => _damage = value; }
        public override int HP { get => _hp; set => _hp = value; }
        public override int ID { get => _id; set => _id = value; }
        public override int DiedStep { get => _diedStep;  set => _diedStep = value; }
        public override int AliveStep { get => _aliveStep; set => _aliveStep = value; }
        public override int StepsToAlive { get => _stepsToAlive; set => _stepsToAlive = value; }
        public override float ActiveRadius { get => _activeRadius; set => _activeRadius = value; }
        public override LayerMask MaskForHex { get => _mask; }
        public override LayerMask MaskForSound { get => _soundMask; }
        public override float JumpStep { get => jumpStep; set => jumpStep = value; }
        public override float SoundRadius { get => _soundRadius; set => _soundRadius = value; }
        public override Vector3 LoudestSoundPosition { get => _loudestSoundPosition; set => _loudestSoundPosition = value; }
        public override Vector3 EnemyOffsetOnHex { get => _enemyOffsetOnHex; set => _enemyOffsetOnHex = value; }
        public override Vector3 Position { get => _position; set => _position = value; }
        public override bool IsAlive { get => isAlive; set => isAlive = value; }
        public override int Regeneration { get => _regeneration; set => _regeneration = value; }

        [SerializeField] private LayerMask _mask;
        [SerializeField] private LayerMask _soundMask;
        [SerializeField, Tooltip("2.5 step")] private float _activeRadius = 2.5f; //step 2.5f
        [SerializeField, Tooltip("2.5 step")] private float _soundRadius = 2.5f;
        [SerializeField] private float _speedAnimation;
        [SerializeField] private Vector3 _loudestSoundPosition;
        [SerializeField] private Vector3 _enemyOffsetOnHex;
        [SerializeField] private Vector3 _position;
        [SerializeField] private int  _hp = 100;
        [SerializeField] private int _damage = 50;
        [SerializeField] private int _id;
        [SerializeField] private int _stepsToAlive;
        [SerializeField] private int _regeneration;
        private int _diedStep;
        private int _aliveStep = 0;
        [SerializeField] private bool isAlive = true; 
        [SerializeField, Tooltip("Default step set in MainConrtoller _MaxPointHex")] private float jumpStep;
        #endregion

        public override void Start()
        {
            base.Start();
        }

        [ContextMenu("PathFinding")]
        public override async Task PathFinding()
        {

             await base.PathFinding();
    
        }

        public override void SoundFinding()
        {
            base.SoundFinding();



        }

        public override void Rebirth()
        {
            base.Rebirth();
        }
        public override void NotDieYet()
        {
            base.NotDieYet();
        }
    }
}
