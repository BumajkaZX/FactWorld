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
        public override LayerMask mask { get { return mask; } }

        public override float JumpStep { get { return jumpStep; } set { jumpStep = value; } }

        [SerializeField] private LayerMask _mask;
        [SerializeField, Tooltip("2.5 step")] private float _activeRadius = 2.5f; //step 2.5f
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
       
            Collider[] inRad = Physics.OverlapSphere(transform.position, _activeRadius, _mask); //Step 2.5
            for (int i = 0; i < inRad.Length; i++)
            {
                var cl = inRad[i].GetComponent<InteractWithField>();
                var point = cl.GetMaxPoint();
                if (cl.GetMaxPoint() <= point + jumpStep)
                {
                    
                }
            }
           
        }

        public override void SoundFinding()
        {
          
        }

    }
}
