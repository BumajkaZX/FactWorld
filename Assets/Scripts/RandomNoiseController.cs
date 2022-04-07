using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public  class RandomNoiseController : MonoBehaviour
    {
        #region params

        public GameObject noisePrefab;
        public LayerMask _hexMask;
        public float ActiveRadius { get => _activeRadius; private set => _activeRadius = value; }
        public float ChanceForNoise { get => _chanceForNoise; set => _chanceForNoise = value; }
        public int MinNumberOfNoises { get => _minNumberOfNoises; set => _minNumberOfNoises = value; }
        public int MaxNumberOfNoises { get => _maxNumberOfNoises; set => _maxNumberOfNoises = value; }
        [SerializeField, Range(0, 100)] private float _chanceForNoise;
        [SerializeField] private float _minNoise = 0;
        [SerializeField] private float _maxNoise = 1;
        [SerializeField] private float _activeRadius;
        [SerializeField] private int _minNumberOfNoises;
        [SerializeField] private int _maxNumberOfNoises;
        [SerializeField] private List<GameObject> noiseListObject = new List<GameObject>();

        #endregion
        private void Awake()
        {
            GameEventManager.NoiseChanse = _chanceForNoise;
            GameEventManager.Turn.AddListener(MakeNoise);
        }
        [ContextMenu("Make Noise")]
        public void MakeNoise()
        {
            _chanceForNoise = GameEventManager.NoiseChanse;
            var chance = Random.Range(0, 100f);
            if (_chanceForNoise >= chance)
            {
                var numberOfNoises = Random.Range(_minNumberOfNoises, _maxNumberOfNoises);
                Collider[] inRad = Physics.OverlapSphere(transform.position, _activeRadius, _hexMask); //Step 2.5
                for (int i = 0; i < numberOfNoises; i++)
                {
                    var c = Random.Range(0, inRad.Length);
                    var position = inRad[c].gameObject.transform.position;
                    var pref = Instantiate(noisePrefab, position, Quaternion.identity);
                    noiseListObject.Add(pref);
                    pref.transform.parent = gameObject.transform;
                    var soundClass = pref.GetComponent<Sound>();
                    soundClass.NoiseTransform = pref.transform;
                    soundClass.Noise = Random.Range(_minNoise, _maxNoise);
                }
            }
        }
        [ContextMenu("Clear List")]
        public void ClearNoise()
        {
            for (int i = 0; i < noiseListObject.Count; i++)
            {
                DestroyImmediate(noiseListObject[i]);
            }
            noiseListObject.Clear();
        }
    }
}
