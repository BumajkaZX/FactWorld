using UnityEngine;

namespace FactWorld
{
    public interface ISound
    {
        public Transform NoiseTransform { get; set; }
        public float Noise { get; set; }
    }
}
