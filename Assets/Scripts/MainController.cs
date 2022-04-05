using Cinemachine;
using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

namespace FactWorld
{
    public class MainController : MonoBehaviour
    {
        #region params
        [SerializeField] private LayerMask _hexMask;
        [SerializeField] private float _radius, _mainCharacterOffset, _MaxPointHex, _activeHexSpeedAnim, _characterMoveSpeedAnim;
        [SerializeField] private Vector3 _activePosition, _activeHexDefaultPos, _characterMoveUp;
        [SerializeField] private GameObject  _mainCharacter, _pointToFollow, _islandCentre;
        [SerializeField] private CinemachineVirtualCamera _cinemachine;
        [SerializeField] private List<GameObject> _activeHexSnL = new List<GameObject>();
        [SerializeField] private List<EnemyBase> enemies = new List<EnemyBase>();
        [SerializeField] private List<float> jumpStep = new List<float>();
        private GameObject _activeHex, _characterHex;
        private ListPlaces _listPlaces = new ListPlaces();
        private List<int> _activePlaces = new List<int>();
        private List<InteractWithField> _activeHexList = new List<InteractWithField>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        #endregion
        private void Awake()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].JumpStep = jumpStep[i] + _MaxPointHex;
            }
            _cinemachine.Follow = _pointToFollow.transform;
            _cinemachine.LookAt = _pointToFollow.transform;
            GameEventManager._mainController = this;
            Load();
        }
        public void SetStartHexParams(GameObject hex)
        {
            _activeHex = hex;
            _characterHex = hex;
            _activeHexDefaultPos = hex.transform.position;
            _pointToFollow.transform.position = hex.transform.position;

        }
        public void SetMainActiveHex(GameObject hex, float maxPoint)
        {
            _disposables.Clear();
            MoveMainCharacter(_characterHex.transform.position, _activeHexDefaultPos - new Vector3(0, _mainCharacterOffset, 0), _mainCharacter);
            _characterHex = hex;
            _characterHex.transform.position = _activeHexDefaultPos;
            DisableMeshRendererActiveChild();
            RotateCam();
            LerpMove(_characterHex.transform.position, _pointToFollow.transform.position);
            EnableActiveHex(_characterHex.transform.position, maxPoint);
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
                _activeHexList[i].Activate(false, true);
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
                    cl.Activate(true, true);
                    cl.MeshActive(true);
                    if (!_activePlaces.Contains(cl.GetID()))
                    {
                        _activePlaces.Add(cl.GetID());
                    }
                    _activeHexList.Add(cl);
                }
            }
            _characterHex.GetComponent<InteractWithField>().Activate(false, true);
            Save(_activePlaces);
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
                t += Time.fixedDeltaTime;
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
                    dis.Clear();

            }).AddTo(dis);
        }

        #region SaveNLoad
        [ContextMenu("Load")]
        public void Load()
        {
            string path = Application.persistentDataPath + "/Saves";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                File.Create(path + "/Saves.json");
            }
            _listPlaces = JsonUtility.FromJson<ListPlaces>(File.ReadAllText(path + "/Saves.json"));
            if (_listPlaces != null)
            {
                _activePlaces = _listPlaces.Places;
                for (int i = 0; i < _activeHexSnL.Count; i++)
                {
                    for (int v = 0; v < _activePlaces.Count; v++)
                    {
                        var c = _activeHexSnL[i].GetComponent<InteractWithField>();
                        if (c.GetID() == _activePlaces[v])
                        {
                            c.Activate(false, true);
                        }

                    }
                }
            }
        }

        public void Save(List<int> places)
        {
            string path = Application.persistentDataPath + "/Saves";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                File.Create(path + "/Saves.json");
            }
            _listPlaces.Places = places;
            File.WriteAllText(path + "/Saves.json", JsonUtility.ToJson(_listPlaces));
        }
        [ContextMenu("Clear Save")]
        public void ClearSave()
        {
            Save(new List<int>());
        }
        [Serializable]
        public class ListPlaces
        {
            public List<int> Places;
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
