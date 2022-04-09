using UnityEngine;
using UniRx;
using Cinemachine;

namespace FactWorld
{
    public class LookAtCanvas : MonoBehaviour
    {
        [SerializeField] private GameObject _followPoint;
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField, Range(0f, 1f)] private float _distance;
        private void Start()
        {
            Observable.EveryUpdate().Subscribe(_ => 
            {
                var dir = Vector3.Lerp(_followPoint.transform.position, _camera.transform.position, _distance);
                gameObject.transform.position = dir;
                gameObject.transform.LookAt(_followPoint.transform.position);

            }).AddTo(this);
        }
    }
}
