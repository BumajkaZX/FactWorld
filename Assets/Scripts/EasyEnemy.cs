using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public class EasyEnemy : EnemyBase
    {
        #region params
        public override int Damage { get { return _damage; } set { _damage = value; } }
        public override int HP { get { return _hp; } }
        public override float ActiveRadius { get { return _activeRadius; } set { _activeRadius = value; } }
        public override LayerMask MaskForHex { get { return _mask; } }
        public override LayerMask MaskForSound { get { return _soundMask; } }
        public override float JumpStep { get { return jumpStep; } set { jumpStep = value; } }
        public override float SoundRadius { get { return _soundRadius; } set { _soundRadius = value; } }

        public override Vector3 LoudestSoundPosition { get { return _loudestSoundPosition; } set { _loudestSoundPosition = value; } }

        [SerializeField] private LayerMask _mask;
        [SerializeField] private LayerMask _soundMask;
        [SerializeField, Tooltip("2.5 step")] private float _activeRadius = 2.5f; //step 2.5f
        [SerializeField, Tooltip("2.5 step")] private float _soundRadius = 2.5f;
        [SerializeField] private Vector3 _loudestSoundPosition;
        [SerializeField] private int  _hp = 100;
        [SerializeField] private int _damage = 50;
        [SerializeField, Tooltip("Default step set in MainConrtoller _MaxPointHex")] private float jumpStep;
        #endregion
        void Start()
        {
            GameEventManager.Turn.AddListener(SoundFinding);
        }
        public override void PathFinding()
        {
            base.PathFinding();
            
           
        }

        public override void SoundFinding()
        {
            base.SoundFinding();



        }

    }
}
