using Cinemachine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UniRx;
using UnityEngine;

namespace FactWorld
{
    public class MainController : MonoBehaviour
    {
        #region params
        public int ActiveObjectID { get => _activeObjectID; private set => _activeObjectID = value; }
        public GameObject goTo { get => _characterHex; }
        public List<EnemyBase> Enemies { get => _enemies; set => _enemies = value; }
        [SerializeField] private LayerMask _hexMask;
        [SerializeField] private float _radius, _mainCharacterOffset, _MaxPointHex, _activeHexSpeedAnim, _characterMoveSpeedAnim;
        [SerializeField] private Vector3 _activePosition, _activeHexDefaultPos, _characterMoveUp;
        [SerializeField] private GameObject _mainCharacter, _pointToFollow, _islandCentre;
        [SerializeField] private CinemachineVirtualCamera _cinemachine;
        [SerializeField] private List<GameObject> _activeHexSnL = new List<GameObject>();
        [SerializeField] private List<EnemyBase> _enemiesJumpStepNSave = new List<EnemyBase>();
        [SerializeField] private List<float> jumpStep = new List<float>();
        [SerializeField] private int _activeObjectID;
        [SerializeField] private List<EnemyBase> _enemies = new List<EnemyBase>();
        private GameObject _activeHex, _characterHex;
        private ListPlaces _listPlaces = new ListPlaces();
        private List<int> _activePlaces = new List<int>();
        private List<InteractWithField> _activeHexList = new List<InteractWithField>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        #endregion
        private void Awake()
        {
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
            for (int i = 0; i < _enemiesJumpStepNSave.Count; i++)
            {
                _enemiesJumpStepNSave[i].JumpStep = jumpStep[i] + _MaxPointHex;
            }
            _cinemachine.Follow = _pointToFollow.transform;
            _cinemachine.LookAt = _pointToFollow.transform;
            GameEventManager.MainController = this;
            GameEventManager.Camera = _cinemachine;
            GameEventManager.Turn.AddListener(EnemyTurn);


        }
        public async void EnemyTurn()
        {
           
            for (int i = 0; i < _enemies.Count; i++)
            {
               await _enemies[i].PathFinding();
            }
            await Task.Yield();
            await Save();
            GameEventManager.InverseTurn();
            
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
            print(Enemies.Count);
            _disposables.Clear();
            ActiveObjectID = ID;
            MoveMainCharacter(_characterHex.transform.position, _activeHexDefaultPos - new Vector3(0, _mainCharacterOffset, 0), _mainCharacter);
            _characterHex = hex;
            _characterHex.transform.position = _activeHexDefaultPos;
            DisableMeshRendererActiveChild();
            RotateCam();
            LerpMove(_characterHex.transform.position, _pointToFollow.transform.position);
            EnableActiveHex(_characterHex.transform.position, maxPoint);
            GameEventManager.InverseTurn();
            

        }
        public void Turn()
        {
            GameEventManager.TurnStart();
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
                    GameEventManager.TurnStart();
                }

            }).AddTo(dis);
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
            _listPlaces = JsonUtility.FromJson<ListPlaces>(File.ReadAllText(path + "/Saves.json"));
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
                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (Enemies[i].ID != _listPlaces.EnemiesID[i]) return;
                    Enemies[i].Damage = _listPlaces.EnemiesDamage[i];
                    Enemies[i].HP = _listPlaces.EnemiesHP[i];
                    Enemies[i].ActiveRadius = _listPlaces.EnemiesActiveRadius[i];
                    Enemies[i].SoundRadius = _listPlaces.EnemiesSoundRadius[i];
                    Enemies[i].JumpStep =_listPlaces.EnemiesJumpStep[i];
                    Enemies[i].Position = _listPlaces.EnemiesPosition[i];
                    Enemies[i].LoudestSoundPosition = _listPlaces.EnemiesLoudPosition[i];
                    Enemies[i].EnemyOffsetOnHex = _listPlaces.EnemiesOffsetOnHex[i];

                }



        }

        public async Task Save()
        {
            await Task.Delay(2000);
            string path = Application.persistentDataPath + "/Saves";
            var listPlaces = new ListPlaces();
            var id = new List<int>();
            var damage = new List<int>();
            var hp = new List<int>();
            var activeRadius = new List<float>();
            var soundRadius = new List<float>();
            var jumpStep = new List<float>();
            var position = new List<Vector3>();
            var loudestPosition = new List<Vector3>();
            var offset = new List<Vector3>();
            for (int i = 0; i < Enemies.Count; i++)
            {
                id.Add(Enemies[i].ID);
                damage.Add(Enemies[i].Damage);
                hp.Add(Enemies[i].HP);
                activeRadius.Add(Enemies[i].ActiveRadius);
                soundRadius.Add(Enemies[i].SoundRadius);
                jumpStep.Add(Enemies[i].JumpStep);
                position.Add(Enemies[i].Position);
                loudestPosition.Add(Enemies[i].LoudestSoundPosition);
                offset.Add(Enemies[i].EnemyOffsetOnHex);
                
            }
            listPlaces.EnemiesID = id;
            listPlaces.EnemiesDamage = damage;
            listPlaces.EnemiesHP = hp;
            listPlaces.EnemiesActiveRadius = activeRadius;
            listPlaces.EnemiesSoundRadius = soundRadius;
            listPlaces.EnemiesJumpStep = jumpStep;
            listPlaces.EnemiesPosition = position;
            listPlaces.EnemiesLoudPosition = loudestPosition;
            listPlaces.EnemiesOffsetOnHex = offset;
            listPlaces.ActiveObject = ActiveObjectID;
            listPlaces.Places = _activePlaces;
            listPlaces.MainCharacterPosition = _mainCharacter.transform.position;
            File.WriteAllText(path + "/Saves.json", JsonUtility.ToJson(listPlaces));

            await Task.Yield();
            
        }
        [ContextMenu("Clear Save")]
        public async void ClearSave()
        {
            _activePlaces.Clear();
            
            await Save();
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

        [ContextMenu("Clear Enemies List")]
        public void ClearEnemyList()
        {
            Enemies.Clear();
        }
    }
}
