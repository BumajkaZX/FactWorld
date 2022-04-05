using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public interface IEnemy
    {
        LayerMask MaskForHex { get; }
        int Damage { get; set; }
        int HP { get; }
        float ActiveRadius { get; set; }

        void SoundFinding();
        void PathFinding();
        
    }
}
