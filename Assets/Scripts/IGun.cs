using UnityEngine;

namespace FactWorld
{
    public interface IGun 
    {
        public GameObject Card { get; set; }
        public int Damage { get; set; }
        public float AttackRadius { get; set; }
        public float AttackNoise { get; set; }
        public float NoiseRadius { get; set; }
    }
}
