using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public class Sound : MonoBehaviour, ISound
    {
        public float Noise { get { return _noise; } set { _noise = value; } }
        private float _noise;
    }
}
