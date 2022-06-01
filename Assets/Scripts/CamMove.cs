using UnityEngine;
using UniRx;


namespace FactWorld 
{ 
    public class CamMove : MonoBehaviour
    {
        [SerializeField] float speedMovement, speedRotation;
        [SerializeField] GameObject mainCameraOrigin;
        void Start()
        {
            Observable.EveryFixedUpdate().Subscribe(_ => 
            {
                var dir = new Vector3(Input.GetAxis("Horizontal") * speedMovement, 0, Input.GetAxis("Vertical") * speedMovement);
                mainCameraOrigin.transform.Translate(dir);
                var rot = Quaternion.Euler(new Vector3(0, -Input.GetAxis("Rotation") * speedRotation , 0)) * mainCameraOrigin.transform.rotation;
                mainCameraOrigin.transform.rotation = rot;
            }).AddTo(this);
        }
    }
}