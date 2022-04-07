using UniRx;
using UnityEngine;


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
        Vector3 _offset;
        MeshRenderer _meshRenderer;
        MainController _mainController;
        CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        private void Awake()
        {
            _defaultPosition = gameObject.transform.position;
            _meshRenderer = GetComponent<MeshRenderer>();
            _mainController = GameEventManager.MainController;
        }
        private void Start()
        {

            if (_ActivePosition)
            {
                _mainController.SetStartHexParams(gameObject);
                _mainController.SetMainActiveHex(gameObject, _maxPoint);
                _canBeInteract = false;
            }
        }
        private void OnMouseUpAsButton()
        {
            if (_canBeInteract && !GameEventManager.WhichTurn)
            {
                if (_set)
                {
                    _ActivePosition = true;
                    _mainController.SetMainActiveHex(gameObject, _maxPoint);
                    ResetPosition();
                }
                else if (!_set)
                {
                    _set = true;
                    _mainController.UpHex(gameObject,  _defaultPosition);
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

        [ContextMenu("Set Enemy Position")]
        void SetEnemy()
        {
            var x = Physics.OverlapSphere(transform.position, 1f);
            print(x.Length);
            foreach (var b in x)
            {
                var offset = b.gameObject.GetComponent<IEnemy>().EnemyOffsetOnHex;
                var f =  b.gameObject.GetComponent<IEnemy>();
                if (f != null && offset != null)
                {
                    b.gameObject.transform.position = transform.position - offset;
                    b.gameObject.GetComponent<Collider>().enabled = false;
                }
            }
           
        }
        #region incaps
        public bool IsActivePosition()
        {
            return _ActivePosition;
        }
        public void Activate(bool interactActive, bool childActive)
        {
            _child?.SetActive(childActive);
            _canBeInteract = interactActive;
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
        public void SetOffset(float offset)
        {
            _offset = new Vector3(0, offset, 0);
        }
        #endregion
    }

}