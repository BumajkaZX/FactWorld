using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld
{
    public class UIButtons : MonoBehaviour
    {
        [SerializeField] private GameObject _gun1;
        [SerializeField] private GameObject _gun2;

        [Space(50)]

        [SerializeField] private List<GameObject> _actions = new List<GameObject>();

        public void Attack()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
