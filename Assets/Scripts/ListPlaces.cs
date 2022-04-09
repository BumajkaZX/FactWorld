using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld {
    [Serializable]
    public class ListPlaces
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
        public int ActiveObject;
    }
}
