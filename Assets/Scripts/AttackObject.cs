using UnityEngine;

namespace FactWorld
{
    [CreateAssetMenu(menuName = "Card/Attack Card")]
    public class AttackObject : ScriptableObject
    {
        public GameObject InnerCard { get => _innerCard; private set => _innerCard = value; }
        public GameObject InterfaceCard { get => _interfaceCard; private set => _interfaceCard = value; }
        public int Damage { get => _damage; private set => _damage = value; }
        public float AttackRadius { get => _attackRadius; private set => _attackRadius = value; }
        public float AttackNoise { get => _attackNoise; private set => _attackNoise = value; }
        public float NoiseRadius { get => _noiseRadius; private set => _noiseRadius = value; }

        [SerializeField] private GameObject _innerCard;
        [SerializeField] private GameObject _interfaceCard;
        [SerializeField] private int _damage;
        [SerializeField] private float _attackRadius;
        [SerializeField] private float _attackNoise;
        [SerializeField] private float _noiseRadius;
    }
}
