using UnityEngine;

namespace FactWorld
{
    public class Sound : MonoBehaviour, ISound
    {
        public float Noise { get => _noise;  set => _noise = value;  }
        public Transform NoiseTransform{ get => _soundObject;  set => _soundObject = value;  }

        private Transform _soundObject;

        private float _noise;
    }
}
