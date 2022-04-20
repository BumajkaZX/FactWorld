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
        private CompositeDisposable _disposables = new CompositeDisposable();
        private GameObject _cross;
        private bool isAttack = true;

        #endregion

        private void Awake()
        {
            _defaultPosition = gameObject.transform.position;
            _meshRenderer = GetComponent<MeshRenderer>();
            
        }
        private void Start()
        {
            _mainController = GameManager.mainController;
        }
       
        private void OnMouseUpAsButton()
        {
            
                if (_canBeInteract)
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