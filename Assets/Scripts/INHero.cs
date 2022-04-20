using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public interface INHero 
    {
        public LayerMask PlayerMask { get; set; }
        public int HP { get; set; }

        public List<AttackObject> Guns { get; set; }
        
    }
}
