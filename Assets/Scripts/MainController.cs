using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UniRx;
using UnityEngine;
using FactWorld.Save;
using FactWorld.Tools;

namespace FactWorld
{
    public class MainController : MonoBehaviour
    {
        #region params
        public Vector3 ActiveHexDefaultPos { get => _activeHexDefaultPos; private set => _activeHexDefaultPos = value; }
        [SerializeField] private LayerMask _hexMask;
        [SerializeField] private float _radius, _MaxPointHex, _activeHexSpeedAnim, _speedMovement, _speedRotation, _scrollSpeed, _minYLookCam, _maxYLookCam;
        [SerializeField] private Vector3  _activeHexDefaultPos;
        [SerializeField] private CinemachineVirtualCamera _cinemachine;
        [SerializeField] private List<GameObject> _activeHexSnL = new List<GameObject>();
        [SerializeField, Tooltip("1/n of current screen height")] private int _partOfScreenForRotation;
        [SerializeField] private Transform _pointToFollow;
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
        }

        private void Start()
        {
            _cinemachine.Follow = _pointToFollow;
            _cinemachine.LookAt = _pointToFollow;
            var cinemachineComponents = _cinemachine.GetCinemachineComponent<CinemachineTransposer>();
#if UNITY_WIN
            Observable.EveryFixedUpdate().Subscribe(_ => 
            {
                var dir = new Vector3(Input.GetAxis("Horizontal") * _speedMovement, 0, Input.GetAxis("Vertical") * _speedMovement);
                _pointToFollow.Translate(dir);
                var rot = Quaternion.Euler(new Vector3(0, Input.GetAxis("Rotation") * _speedRotation, 0)) * _pointToFollow.rotation;
                _pointToFollow.rotation = rot;
                var scroll = Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
                if(cinemachineComponents.m_FollowOffset.y + scroll < _maxYLookCam && cinemachineComponents.m_FollowOffset.y + scroll > _minYLookCam)
                cinemachineComponents.m_FollowOffset = new Vector3(cinemachineComponents.m_FollowOffset.x, cinemachineComponents.m_FollowOffset.y + scroll, cinemachineComponents.m_FollowOffset.z);
            }).AddTo(_disposables);
#endif
#if UNITY_ANDROID
            var activeZoneForRotation = Screen.height / _partOfScreenForRotation;
            var touch1 = new Vector2();
            var touch2 = new Vector2();
            var pastDifference = 0f;
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Input.touchCount >= 2 && Input.anyKeyDown )
                {
                    touch1 = Input.touches[0].position;
                    touch2 = Input.touches[1].position;
                    pastDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                }
                if (Input.touchCount < 2 && Input.anyKey )
                {
                    var touch = Input.touches[0];
                    if (touch.position.y <= activeZoneForRotation)
                    {
                        var rot = Quaternion.Euler(_pointToFollow.rotation.x, (touch.deltaPosition.x * _speedRotation / 10), _pointToFollow.rotation.z) * _pointToFollow.rotation;
                        _pointToFollow.rotation = rot;
                    }
                    if (touch.position.y > (float)activeZoneForRotation)
                    {
                        var dir = touch.deltaPosition * _speedMovement / 10;
                        _pointToFollow.Translate(new Vector3(-dir.x, 0, -dir.y));
                    }
                
                }
                else if(Input.touchCount >= 2 && Input.anyKey)
                {
                    touch1 = Input.touches[0].position;
                    touch2 = Input.touches[1].position;
                    var currentDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                    var normalizedDifference = pastDifference - currentDifference * _scrollSpeed;
                    if(pastDifference > currentDifference && normalizedDifference + cinemachineComponents.m_FollowOffset.y <= _maxYLookCam)
                    {
                        cinemachineComponents.m_FollowOffset = new Vector3(cinemachineComponents.m_FollowOffset.x, cinemachineComponents.m_FollowOffset.y - normalizedDifference, cinemachineComponents.m_FollowOffset.z);
                    }
                    else if(pastDifference < currentDifference && normalizedDifference + cinemachineComponents.m_FollowOffset.y >= _minYLookCam)
                    {
                        cinemachineComponents.m_FollowOffset = new Vector3(cinemachineComponents.m_FollowOffset.x, cinemachineComponents.m_FollowOffset.y + normalizedDifference, cinemachineComponents.m_FollowOffset.z);
                    }
                    pastDifference = Vector2.Distance(touch1, touch2) / Screen.height;
                }
            
            }).AddTo(this); //Movement
#endif
        }



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
