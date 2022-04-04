using UnityEngine;


namespace FactWorld.Tools
{
    public class CamMove : MonoBehaviour
    {
        [SerializeField]
        float speed;
        void Update()
        {
            var _dir = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);
            gameObject.transform.position += _dir;
        }
    }
}