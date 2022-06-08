using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using UniRx;
using UnityEngine;
using FactWorld.Save;
using FactWorld.Tools;

namespace FactWorld
{
    public class MainController : MonoBehaviour
    {
        #region const
        const float delForHeight = 1.5f;
        #endregion
        #region params
        public Vector3 ActiveHexDefaultPos { get => _activeHexDefaultPos; private set => _activeHexDefaultPos = value; }
        [SerializeField] private LayerMask _hexMask, _cardMask;
        [SerializeField] private float _radius, _MaxPointHex, _activeHexSpeedAnim, _speedMovement, _speedRotation, _scrollSpeed, _minYLookCam, _maxYLookCam;
        [SerializeField] private Vector3  _activeHexDefaultPos;
        [SerializeField] private CinemachineVirtualCamera _cinemachine;
        [SerializeField] private List<GameObject> _activeHexSnL = new List<GameObject>();
        [SerializeField, Tooltip("1/n of current screen height")] private int _partOfScreenForRotation, _sensorDeadzone;
        [SerializeField] private Transform _pointToFollow;
        [SerializeField] private List<CardSO> cards = new List<CardSO>();
        [SerializeField] private int activeCardType;
        private Camera mainCamera;
        private SaveFile _listPlaces = new SaveFile();
        private List<int> _activePlaces = new List<int>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        #endregion
        private void Awake()
        {
            mainCamera = Camera.main;
            if (SystemInfo.supportsGyroscope)
            {

            }
            GameManager.activeHexPosition = ActiveHexDefaultPos;
            GameManager.activeHexAnimationSpeed = _activeHexSpeedAnim;
            GameManager.activeCardType = activeCardType;
            GameManager.cards = cards;
            GameManager.cardTypes = Enum.GetNames(typeof(CardSO._cardsType)).Length;
        }

        private void Start()
        {
            _cinemachine.Follow = _pointToFollow;
            _cinemachine.LookAt = _pointToFollow;
           
#if UNITY_WIN
            WindowsMovement();
#endif
#if UNITY_ANDROID
            AndroidMovement();
#endif
            CardChoose();
        }

        private void CardChoose()
        {
            Observable.EveryUpdate().Subscribe(_ => 
            { 
                if(Input.anyKeyDown && Input.touchCount < 2)
                {
                    RaycastHit hit;
                    var rayTouchPos = mainCamera.ScreenPointToRay(Input.touches[0].position);
                    if(Physics.Raycast(rayTouchPos,out hit,float.PositiveInfinity, _cardMask))
                    {
                        for (int i = 0; i < cards.Count; i++)
                        {
                            if (cards[i].name == hit.collider.name)
                            {
                                GameManager.activeCard = cards[i];
                                GameManager.UpCard(hit.collider.gameObject);
                            }
                        }
                        
                        print(hit.collider.name);
                    }
                    else
                    {
                        GameManager.activeCard = null;
                        GameManager.UpCard(null);
                    }

                }
            }).AddTo(this);

        }
#if UNITY_ANDROID
        private void AndroidMovement()
        {
            var cinemachineComponents = _cinemachine.GetCinemachineComponent<CinemachineTransposer>();
            var activeZoneForRotation = Screen.height / _partOfScreenForRotation;
            var touch1 = new Vector2();
            var touch2 = new Vector2();
            var pastDifference = 0f;
            var distanceZoom = 0f;
            var deadzone = Screen.height / _sensorDeadzone;
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Input.touchCount >= 2 && Input.anyKeyDown)
                {
                    touch1 = Input.touches[0].position;
                    touch2 = Input.touches[1].position;
                    pastDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                    distanceZoom = Vector2.Distance(touch1, touch2);
                }
                if (Input.touchCount < 2 && Input.anyKeyDown)
                {
                    touch1 = Input.touches[0].position;
                }
                if (Input.touchCount < 2 && Input.anyKey)
                {
                    var touch = Input.touches[0];
                    if (touch.position.y <= activeZoneForRotation && (touch.position.x >= touch1.x + deadzone || touch.position.x <= touch1.x - deadzone))
                    {
                        var rot = Quaternion.Euler(_pointToFollow.rotation.x, (touch.deltaPosition.x * _speedRotation / 10), _pointToFollow.rotation.z) * _pointToFollow.rotation;
                        _pointToFollow.rotation = rot;
                    } // Rotation
                    if (touch.position.y > activeZoneForRotation && (touch.position.x >= touch1.x + deadzone || touch.position.y >= touch1.y + (deadzone / delForHeight) ||
                    touch.position.y <= touch1.y - (deadzone / delForHeight) || touch.position.x <= touch1.x - deadzone))
                    {
                        var dir = touch.deltaPosition * _speedMovement / 10;
                        _pointToFollow.Translate(new Vector3(-dir.x, 0, -dir.y));
                    } // Movement

                }
                else if (Input.touchCount >= 2 && Input.anyKey)
                {
                    touch1 = Input.touches[0].position;
                    touch2 = Input.touches[1].position;
                    var currentDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                    var normalizedDifference = (pastDifference - currentDifference) * _scrollSpeed;
                    if ((distanceZoom + (deadzone * 2) <= Vector2.Distance(touch1, touch2)) || (distanceZoom - (deadzone * 2) >= Vector2.Distance(touch1, touch2)))
                    {
                        var cinemachineOffset = cinemachineComponents.m_FollowOffset;
                        if (pastDifference > currentDifference && normalizedDifference + cinemachineOffset.y <= _maxYLookCam)
                        {
                            cinemachineComponents.m_FollowOffset = new Vector3(cinemachineOffset.x, cinemachineOffset.y + normalizedDifference, cinemachineOffset.z);
                        }
                        else if (pastDifference < currentDifference && normalizedDifference + cinemachineOffset.y >= _minYLookCam)
                        {
                            cinemachineComponents.m_FollowOffset = new Vector3(cinemachineOffset.x, cinemachineOffset.y + normalizedDifference, cinemachineOffset.z);
                        }
                    }
                    pastDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                } // Zoom

            }).AddTo(this);
        }
#endif
#if UNITY_WIN
        private void WindowsMovement()
        {
            var cinemachineComponents = _cinemachine.GetCinemachineComponent<CinemachineTransposer>();
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                var dir = new Vector3(Input.GetAxis("Horizontal") * _speedMovement, 0, Input.GetAxis("Vertical") * _speedMovement);
                _pointToFollow.Translate(dir);
                var rot = Quaternion.Euler(new Vector3(0, Input.GetAxis("Rotation") * _speedRotation, 0)) * _pointToFollow.rotation;
                _pointToFollow.rotation = rot;
                var scroll = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
                if (cinemachineComponents.m_FollowOffset.y + scroll < _maxYLookCam && cinemachineComponents.m_FollowOffset.y + scroll > _minYLookCam)
                    cinemachineComponents.m_FollowOffset = new Vector3(cinemachineComponents.m_FollowOffset.x, cinemachineComponents.m_FollowOffset.y + scroll, cinemachineComponents.m_FollowOffset.z);
            }).AddTo(_disposables);
        }
#endif
        #region SaveNLoad
        [ContextMenu("Load")]
        public void Load()
        {
            string path = Application.persistentDataPath + "/Saves/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                File.Create(path + "/Saves.json");
                return;
            }
            if (!File.Exists(path + "Saves.json"))
            {
                File.Create(path + "/Saves.json");
                return;
            }
            _listPlaces = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path + "/Saves.json"));

        }
        [ContextMenu("Save")]
        public void Save()
        {

            string path = Application.persistentDataPath + "/Saves";
            //File.WriteAllText(path + "/Saves.json", JsonUtility.ToJson());
            
        }
        [ContextMenu("Clear Save")]
        public void ClearSave()
        {
            File.Delete(Application.persistentDataPath + "/Saves/Saves.json");
        }
        
       
#endregion
        public void AddList(List<GameObject> list)
        {
            _activeHexSnL.AddRange(list);
        }

        [ContextMenu("ClearList")]
        public void ClearList()
        {
            _activeHexSnL.Clear();
        } 

    }
}
