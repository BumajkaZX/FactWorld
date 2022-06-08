using UniRx;
using UnityEngine;
using Cinemachine;

namespace FactWorld.Tools
{
    public class InteractWithField : MonoBehaviour
    {
        #region params
        [SerializeField] Vector3 _defaultPosition, _defaultChildPosition;
        [SerializeField] int _objectID;
        [SerializeField] float _maxPoint, _activeHexSpeedAnim;
        [SerializeField] GameObject _child;
        private float _speedAnimation;
        private Vector3 _offset;
        private MeshRenderer _meshRenderer;
        private CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        private void Awake()
        {
            _defaultPosition = gameObject.transform.position;
            _meshRenderer = GetComponent<MeshRenderer>();
            GameManager.activeHex.AddListener(ResetPosition);
        }
        private void Start()
        {
            _speedAnimation = GameManager.activeHexAnimationSpeed;
        }
       
        private void OnMouseUpAsButton()
        {
            UpHex();
            GameManager.UpHex(_objectID);
        }
        private void UpHex()
        {
            _disposables.Clear();
            var c = 0f;
            var currentPos = transform.position;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                transform.position = Vector3.Lerp(currentPos,_defaultPosition + GameManager.activeHexPosition, c);
                c += Time.fixedDeltaTime * _speedAnimation;
                if (c >= 1)
                    _disposables.Clear();
            }).AddTo(_disposables);
        }
        private void ResetPosition(int id)
        {
            if (id == _objectID) return;
            _disposables.Clear();
            var c = 0f;
            var currentPos = transform.position;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                transform.position = Vector3.Lerp(currentPos, _defaultPosition, c);
                c += Time.fixedDeltaTime * _speedAnimation;
                if (c >= 1)
                    _disposables.Clear();
            }).AddTo(_disposables);
        }

        #region incaps
        public void SetMaxPoint(float max)
        {
            _maxPoint = max;
        }
        public void SetID(int ID)
        {
            _objectID = ID;
        }
        public void SetOffset(float offset)
        {
            _offset = new Vector3(0, offset, 0);
        }
        #endregion
    }

}