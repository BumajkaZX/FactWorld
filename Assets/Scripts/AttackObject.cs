using UnityEngine;

namespace FactWorld
{
    [CreateAssetMenu(menuName = "Card/Attack Card")]
    public class AttackObject : ScriptableObject
    {
        public GameObject Card { get => _card; set => _card = value; }
        public int Damage { get => _damage; set => _damage = value; }
        public float AttackRadius { get => _attackRadius; set => _attackRadius = value; }
        public float AttackNoise { get => _attackNoise; set => _attackNoise = value; }
        public float NoiseRadius { get => _noiseRadius; set => _noiseRadius = value; }

        [SerializeField] private GameObject _card;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackRadius;
        [SerializeField] private float _attackNoise;
        [SerializeField] private float _noiseRadius;
    }
}
