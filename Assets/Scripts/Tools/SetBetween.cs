using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactWorld.Tools
{
    public class SetBetween : MonoBehaviour
    {
        [SerializeField] Transform first, second;

        [ContextMenu("Set")]
        public void Set()
        {
            transform.position = Vector3.Lerp(new Vector3(first.position.x, 0, first.position.z), new Vector3(second.position.x, 0, second.position.z), 0.5f);
        }
    }
}
