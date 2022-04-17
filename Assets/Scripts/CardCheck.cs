using UnityEngine;
using UniRx;

namespace FactWorld
{
    public class CardCheck : MonoBehaviour
    {
        #region params
        private AttackObject attackObject;
        private Vector3 defaultPos;
        private Vector3 defaultScale;
        private Quaternion defaultRotation;
        private Transform par;
        private bool isActive;
        private float scale;
        private float speed;
        private CompositeDisposable disposables = new CompositeDisposable();
        private Gyroscope gyro;
        #endregion
        private void Start()
        {
            //attackObject = GetComponent<AttackObject>();
            CardCheckEvent.hideUICards.AddListener(Hide);
            par = transform.parent.transform;
            defaultScale = transform.localScale;
            scale = CardCheckEvent.scale;
            speed = CardCheckEvent.speed;
            if (CardCheckEvent.gyroEnable)
            {
                gyro = Input.gyro;
            }
        }
        private void OnMouseUpAsButton()
        {
            
            if (isActive)
            {
                CardCheckEvent.hideUICards.AddListener(Hide);
                //CardCheckEvent.DescriptionActive(attackObject.Description);
                ResetPosition();
                return;
            }
            CardCheckEvent.hideUICards.RemoveListener(Hide);
            CardCheckEvent.Hide(false);
            //CardCheckEvent.DescriptionActive(attackObject.Description);
            isActive = true;
            defaultPos = par.localPosition;
            var t = 0f;
            var newScale = defaultScale * scale;
            CardCheckEvent.ResetPositionNActiveDescription();
            CardCheckEvent.buttonCheck.AddListener(ResetPosition);
            defaultRotation = transform.localRotation;
            Observable.EveryUpdate().Subscribe(_ => 
            {

                par.localPosition = Vector3.Lerp(defaultPos, Vector3.zero, t);
                transform.localScale = Vector3.Lerp(defaultScale, newScale, t);
                t += Time.deltaTime * speed;
                if (t >= 1)
                {
                    par.localPosition = Vector3.zero;
                    transform.localScale = newScale;
                    disposables.Clear();
                    if (CardCheckEvent.gyroEnable)
                    {
                        gyro.enabled = true;
                        var defaultRotation = transform.localRotation;
                        var defaultGyroRotation = gyro.attitude;
                        
                        Observable.EveryUpdate().Subscribe(_ => 
                        {
                            var gyroRot = gyro.attitude;
                            var rot = new Quaternion(defaultGyroRotation.y - gyroRot.y, gyroRot.x - defaultGyroRotation.x , 0, 1);
                            var currentOffsetGyro = rot;
                            transform.localRotation = currentOffsetGyro;
                        }).AddTo(disposables);
                    }
                }
            
            }).AddTo(disposables);
        }

        private void ResetPosition()
        {
            if (CardCheckEvent.gyroEnable)
            {
                gyro.enabled = false;
            }
            disposables.Clear();
            isActive = false;     
            var currentPos = par.localPosition;
            var currentScale = transform.localScale;
            var currentRotation = transform.localRotation;
            var t = 0f;
            CardCheckEvent.buttonCheck.RemoveListener(ResetPosition);
            Observable.EveryUpdate().Subscribe(_ =>
            {

                par.localPosition = Vector3.Lerp(currentPos, defaultPos, t);
                transform.localScale = Vector3.Lerp(currentScale, defaultScale, t);
                transform.localRotation = Quaternion.Lerp(currentRotation, defaultRotation, t);
                t += Time.deltaTime * speed;
                if (t >= 1)
                {
                    par.localPosition = defaultPos;
                    transform.localScale = defaultScale;
                    disposables.Clear();
                    CardCheckEvent.Hide(true);
                }

            }).AddTo(disposables);
        }

        private void Hide(bool isActive)
        {
            par.gameObject.SetActive(isActive);
        }
    }
}
