using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public interface IObject 
    {
        public int Damage { get; set; }
        public float NoiseRadius { get; set; }

        public void Use();
    }
}
