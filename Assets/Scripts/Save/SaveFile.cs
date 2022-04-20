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
        public int ActiveObject;
    }
}
