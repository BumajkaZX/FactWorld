using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld.Save 
{
    [Serializable]
    public class SaveFile
    {
        public List<int> Places;
        public Vector3 MainCharacterPosition;
        public List<int> EnemiesID;
        public List<int> EnemiesHP;
        public List<int> EnemiesDamage;
        public List<Vector3> EnemiesPosition;
        public List<Vector3> EnemiesLoudPosition;
        public List<Vector3> EnemiesOffsetOnHex;
        public List<float> EnemiesJumpStep;
        public List<float> EnemiesActiveRadius;
        public List<float> EnemiesSoundRadius;
        public List<bool> IsAlive;
        public int ActiveObject;
        public int Step;
    }
}
