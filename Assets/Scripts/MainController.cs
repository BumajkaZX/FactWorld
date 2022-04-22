using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UniRx;
using UnityEngine;
using FactWorld.Save;

namespace FactWorld
{
    public class MainController : MonoBehaviour
    {
        #region params
        public int ActiveObjectID { get => _activeObjectID; private set => _activeObjectID = value; }
        public GameObject goTo { get => _characterHex; }
        [SerializeField] private LayerMask _hexMask, _enemyMask;
        [SerializeField] private float _radius, _mainCharacterOffset, _MaxPointHex, _activeHexSpeedAnim, _characterMoveSpeedAnim, _enemyFindRadius;
        [SerializeField] private Vector3 _activePosition, _activeHexDefaultPos, _characterMoveUp;
        [SerializeField] private GameObject _mainCharacter, _pointToFollow, _islandCentre;
        [SerializeField] private CinemachineVirtualCamera _cinemachine;
        [SerializeField] private List<GameObject> _activeHexSnL = new List<GameObject>();
        [SerializeField] private int _activeObjectID;
        [SerializeField] private Material _fogOfWarMat;
        private GameObject _activeHex, _characterHex;
        private SaveFile _listPlaces = new SaveFile();
        private List<int> _activePlaces = new List<int>();
        private List<InteractWithField> _activeHexList = new List<InteractWithField>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        #endregion
        private void Awake()
        {
            if (SystemInfo.supportsGyroscope)
            {

            }
            GameManager.Fight.AddListener(BeforeFight);
            Load();
            for (int i = 0; i < _activeHexSnL.Count; i++)
            {
                var cl = _activeHexSnL[i].GetComponent<InteractWithField>();
                if (cl.GetID() == ActiveObjectID)
                {
                    var offset = cl.GetOffset();
                    _mainCharacter.transform.position = _activeHexSnL[i].transform.position - new Vector3(0, _mainCharacterOffset - 0.1f, 0);
                    SetStartHexParams(_activeHexSnL[i], cl.GetMaxPoint());
                    cl.InteractActive(false);
                    cl.SetActive(true);
                }
            }
            SetCamFollow(_pointToFollow.transform);
            GameManager.mainController = this;

        }
        public void SetStartHexParams(GameObject hex, float maxPoint)
        {
            _activeHex = hex;
            _characterHex = hex;
            _activeHexDefaultPos = hex.transform.position;
            _pointToFollow.transform.position = hex.transform.position;
            _characterHex = hex;
            _characterHex.transform.position = _activeHexDefaultPos;
            DisableMeshRendererActiveChild();
            RotateCam();
            LerpMove(_characterHex.transform.position, _pointToFollow.transform.position);
            EnableActiveHex(_characterHex.transform.position, maxPoint);
        }
        public void SetMainActiveHex(GameObject hex, float maxPoint, int ID)
        {
            _disposables.Clear();
            ActiveObjectID = ID;
            MoveMainCharacter(_characterHex.transform.position, _activeHexDefaultPos - new Vector3(0, _mainCharacterOffset, 0), _mainCharacter);
            _characterHex = hex;
            _characterHex.transform.position = _activeHexDefaultPos;
            DisableMeshRendererActiveChild();
            RotateCam();
            LerpMove(_characterHex.transform.position, _pointToFollow.transform.position);
            EnableActiveHex(_characterHex.transform.position, maxPoint);
            
        }
        private void SetCamFollow(Transform followPoint)
        {
            _cinemachine.Follow = followPoint;
            _cinemachine.LookAt = followPoint;
        }
        public void UpHex(GameObject hex, Vector3 defaultPosition)
        {

            _activeHex.GetComponent<InteractWithField>().ResetPosition();
            _disposables.Clear();
            _activeHex = hex;
            _activeHexDefaultPos = defaultPosition;
            var up = 0f;
            var goTo = _activeHex.transform.position + _activePosition;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {

                _activeHex.transform.position = Vector3.Lerp(_activeHex.transform.position, goTo, up);
                up += Time.fixedDeltaTime * _activeHexSpeedAnim;
                if (up >= 1)
                    _disposables.Clear();

            }).AddTo(_disposables);
        }
        private void DisableMeshRendererActiveChild()
        {
            for (int i = 0; i < _activeHexList.Count; i++)
            {
                _activeHexList[i].MeshActive(false);
                _activeHexList[i].InteractActive(false);
            }
        }
        private void FindEnemyOnHex(Vector3 activeHex)
        {
            Collider[] inRad = Physics.OverlapSphere(activeHex, _enemyFindRadius, _enemyMask);
            if (inRad.Length >= 1) GameManager.StartFight();
        }
        private void EnableActiveHex(Vector3 activeHex, float maxPointActiveHex)
        {
            Collider[] inRad = Physics.OverlapSphere(activeHex, _radius, _hexMask); //Step 2.5
            for (int i = 0; i < inRad.Length; i++)
            {
                var cl = inRad[i].GetComponent<InteractWithField>();
                if (cl.GetMaxPoint() <= maxPointActiveHex + _MaxPointHex)
                {
                    cl.InteractActive(true);
                    cl.ChildActivate(true);
                    cl.MeshActive(true);
                    cl.SetIsAttack(false);
                    if (!_activePlaces.Contains(cl.GetID()))
                    {
                        _activePlaces.Add(cl.GetID());
                    }
                    _activeHexList.Add(cl);
                }
               
            }    
            _characterHex.GetComponent<InteractWithField>().InteractActive(false);
        }
        private void RotateCam()
        {
            var t = 0f;
            var from = _pointToFollow.transform.rotation;
            var target = Quaternion.LookRotation(_islandCentre.transform.position - _pointToFollow.transform.position);
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {

                _pointToFollow.transform.rotation = Quaternion.Slerp(from, target, t);
                t += Time.fixedDeltaTime;

            }).AddTo(_disposables);
        }
        private void LerpMove(Vector3 goTo, Vector3 from)
        {
            var t = 0f;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                _pointToFollow.transform.position = Vector3.Lerp(from, goTo, t);
                t += Time.fixedDeltaTime * 0.5f;
                if (t >= 1)
                    _disposables.Clear();

            }).AddTo(_disposables);
        }
        private void MoveMainCharacter(Vector3 from, Vector3 goTo, GameObject character)
        {
            CompositeDisposable dis = new CompositeDisposable();
            var t = 0f;
            var middle = Vector3.Lerp(from, goTo, 0.5f);
            middle += _characterMoveUp;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                character.transform.position = Vector3.Lerp(Vector3.Lerp(from, middle, t), Vector3.Lerp(middle, goTo, t), t);
                t += Time.fixedDeltaTime * _characterMoveSpeedAnim;
                if (t >= 1)
                {     
                    dis.Clear();
                    FindEnemyOnHex(_characterHex.transform.position);
                }

            }).AddTo(dis);
        }

      
        private void BeforeFight()
        {
             
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
            if (_listPlaces == null) return;
           
                _mainCharacter.transform.position = _listPlaces.MainCharacterPosition;
                _activePlaces = _listPlaces.Places;
                for (int i = 0; i < _activeHexSnL.Count; i++)
                {
                    for (int v = 0; v < _activePlaces.Count; v++)
                    {
                        var c = _activeHexSnL[i].GetComponent<InteractWithField>();
                        if (c.GetID() == _activePlaces[v])
                        {
                            c.ChildActivate(true);
                        }
                        if (c.GetID() == _listPlaces.ActiveObject)
                        {
                            ActiveObjectID = _listPlaces.ActiveObject;

                        }
                    }

                }

        }
        [ContextMenu("Save")]
        public void Save()
        {
            
            string path = Application.persistentDataPath + "/Saves";
            var listPlaces = new SaveFile();
            listPlaces.Places = _activePlaces;
            listPlaces.MainCharacterPosition = _mainCharacter.transform.position;
            listPlaces.ActiveObject = _activeObjectID;
            File.WriteAllText(path + "/Saves.json", JsonUtility.ToJson(listPlaces));
            
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
