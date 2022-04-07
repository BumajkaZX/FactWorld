using UnityEngine;
using UnityEngine.Events;

namespace FactWorld
{
    public class EasyEnemy : EnemyBase
    {
        #region params
        public override int Damage { get => _damage;  set => _damage = value;  }
        public override int HP { get => _hp; }
        public override float ActiveRadius { get => _activeRadius; set => _activeRadius = value;  }
        public override LayerMask MaskForHex { get => _mask;  }
        public override LayerMask MaskForSound { get => _soundMask;  }
        public override float JumpStep { get => jumpStep; set => jumpStep = value;  }
        public override float SoundRadius { get => _soundRadius; set => _soundRadius = value;  }

        public override Vector3 LoudestSoundPosition { get => _loudestSoundPosition; set => _loudestSoundPosition = value; }

        public override Vector3 EnemyOffsetOnHex { get => _enemyOffsetOnHex; set => _enemyOffsetOnHex = value; }

        [SerializeField] private LayerMask _mask;
        [SerializeField] private LayerMask _soundMask;
        [SerializeField, Tooltip("2.5 step")] private float _activeRadius = 2.5f; //step 2.5f
        [SerializeField, Tooltip("2.5 step")] private float _soundRadius = 2.5f;
        [SerializeField] private Vector3 _loudestSoundPosition;
        [SerializeField] private Vector3 _enemyOffsetOnHex;
        [SerializeField] private int  _hp = 100;
        [SerializeField] private int _damage = 50;
        [SerializeField, Tooltip("Default step set in MainConrtoller _MaxPointHex")] private float jumpStep;
        #endregion

        public override void Awake()
        {
            base.Awake();
        }

        [ContextMenu("PathFinding")]
        public override void PathFinding()
        {


            SoundFinding();
            base.PathFinding();
            
           
        }

        public override void SoundFinding()
        {
            base.SoundFinding();



        }

    }
}
