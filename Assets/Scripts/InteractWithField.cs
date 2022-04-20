using UniRx;
using UnityEngine;
using Cinemachine;

namespace FactWorld
{
    public class InteractWithField : MonoBehaviour
    {
        #region params
        [SerializeField] bool _ActivePosition, _canBeInteract, _set;   // _whichTurn : true - enemy, false - player
        [SerializeField] Vector3 _defaultPosition, _defaultChildPosition;
        [SerializeField] int _objectID;
        [SerializeField] float _maxPoint, _activeHexSpeedAnim;
        [SerializeField] GameObject _child;
        [SerializeField] LayerMask _enemy;
        private Vector3 _offset;
        private MeshRenderer _meshRenderer;
        private MainController _mainController;
        private CinemachineVirtualCamera _camera;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameObject _cross;
        private bool isAttack = true;

        #endregion

        private void Awake()
        {
            CardActiveEvent.AcitveGunEvent.AddListener(ResetPosition);
            _cross = transform.GetChild(0).gameObject;
            _defaultPosition = gameObject.transform.position;
            _meshRenderer = GetComponent<MeshRenderer>();
            
        }
        private void Start()
        {
            _camera = GameEventManager.Camera;
            _mainController = GameEventManager.MainController;
        }
       
        private void OnMouseUpAsButton()
        {
            
                if (_canBeInteract && !GameEventManager.WhichTurn && CardActiveEvent.ActiveCard == null)
                {
                    if (_set)
                    {
                        _ActivePosition = true;
                        _mainController.SetMainActiveHex(gameObject, _maxPoint, _objectID);
                        ResetPosition();
                    }
                    else if (!_set)
                    {
                        _set = true;
                        _mainController.UpHex(gameObject, _defaultPosition);
                        _disposables.Clear();

                    }
                }
            
            
        }
        private void OnMouseEnter()
        {
            if (CardActiveEvent.ActiveCard != null && _canBeInteract) _cross.SetActive(true);

        }
        private void OnMouseUp()
        {
            if (CardActiveEvent.ActiveCard != null)
            {
                if (CardActiveEvent.IsAttack)
                {
                    var noise = Instantiate(Noise.noiseController.noisePrefab, transform.position, Quaternion.identity);
                    noise.GetComponent<Sound>();
                    Noise.noiseController.NoiseList.Add(noise);
                    Attack();
                }
                
                _cross.SetActive(false);
                GameEventManager.InverseTurn();
                GameEventManager.TurnStart();
                CardActiveEvent.ActiveCard = null;
                CardCheckEvent.Hide(true);
                _mainController.DisableAttackHex();
            }  
        }

        private void OnMouseExit()
        {
            if (CardActiveEvent.ActiveCard != null) _cross.SetActive(false);
        }
        public void ResetPosition()
        {
            _set = false;
            var c = 0f;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                transform.position = Vector3.Lerp(transform.position, _defaultPosition, c);
                c += Time.fixedDeltaTime;
                if (c >= 1)
                    _disposables.Clear();
            }).AddTo(_disposables);
        }

        [ContextMenu("Set Enemy Position")]
        private void SetEnemy()
        {
            var x = Physics.OverlapSphere(transform.position, 1f, _enemy);
            
            foreach (var b in x)
            {
                var offset = b.gameObject.GetComponent<IEnemy>().EnemyOffsetOnHex;
                var f =  b.gameObject.GetComponent<IEnemy>();
                if (f != null && offset != null)
                {
                    b.gameObject.transform.position = transform.position - offset;
                    f.Position = b.transform.position;
                    b.gameObject.GetComponent<Collider>().enabled = false;
                }
            }
           
        }
        private void Attack()
        {
            var x = Physics.OverlapSphere(transform.position, 1f, _enemy);
            print(x[0].GetComponent<IEnemy>().HP);
            if (x.Length > 1)
            {
                var random = Random.Range(0, x.Length);
                x[random].GetComponent<IEnemy>().HP -= CardActiveEvent.ActiveCard.Damage;
                x[random].GetComponent<IEnemy>().NotDieYet();
            }
            x[0].GetComponent<IEnemy>().HP -= CardActiveEvent.ActiveCard.Damage;
            x[0].GetComponent<IEnemy>().NotDieYet();

        }
        #region incaps
        public bool IsActivePosition()
        {
            return _ActivePosition;
        }
        public void ChildActivate(bool childActive)
        {
            _child?.SetActive(childActive);
            
        }
        public void InteractActive(bool interactActive)
        {
           _canBeInteract = interactActive;
        }
        public void SetActive(bool g)
        {
            _ActivePosition = g;
        }
        public void MeshActive(bool active)
        {
            _meshRenderer.enabled = active;
        }
        public void SetChild(GameObject child)
        {
            _child = child;
        }
        public float GetMaxPoint()
        {
            return _maxPoint;
        }
        public void SetMaxPoint(float max)
        {
            _maxPoint = max;
        }
        public void SetID(int ID)
        {
            _objectID = ID;
        }
        public int GetID()
        {
            return _objectID;
        }
        public Vector3 GetOffset()
        {
            return _defaultPosition;
        }
        public void SetOffset(float offset)
        {
            _offset = new Vector3(0, offset, 0);
        }

        public bool IsAttack()
        {
            return isAttack;
        }
        public void SetIsAttack(bool attack)
        {
            isAttack = attack; 
        }
        #endregion
    }

}