using UnityEngine;
using UniRx;

namespace FactWorld
{
    public class CardCheck : MonoBehaviour, IObject
    {

        #region params
        public int Damage { get => damage; set => damage = value; }
        public float NoiseRadius { get => noiseRadius; set => noiseRadius = value; }

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
        private string description = "def";
        private int damage;
        private float noiseRadius;
        
        #endregion
        private void Start()
        {
            var name = gameObject.name.Replace("(Clone)","");
            gameObject.name = name;
            for (int i = 0; i < CardChangeEvent.attackObjects.Count; i++)
            {
                if (CardChangeEvent.attackObjects[i].Card.name == name)
                {
                    attackObject = CardChangeEvent.attackObjects[i];
                    description = CardChangeEvent.attackObjects[i].Description;
                    damage = CardChangeEvent.attackObjects[i].Damage;
                    noiseRadius = CardChangeEvent.attackObjects[i].NoiseRadius;
                }
            }
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
                CardCheckEvent.DescriptionActive(description);
                ResetPosition();
                return;
            }
            CardCheckEvent.hideUICards.RemoveListener(Hide);
            CardCheckEvent.Hide(false);
            CardCheckEvent.DescriptionActive(description);
            isActive = true;
            defaultPos = par.localPosition;
            var t = 0f;
            var newScale = defaultScale * scale;
            CardCheckEvent.activeCard = this;
            CardCheckEvent.ResetPositionNActiveDescription();
            CardCheckEvent.ActiveButton();
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
        public void Use()
        {
            print("f");
            CardCheckEvent.hideUICards.AddListener(Hide);
            CardCheckEvent.DescriptionActive(description);
            ResetPosition();
            CardCheckEvent.Hide(false);
            CardActiveEvent.radiusAttack = attackObject.AttackRadius;
            CardActiveEvent.ResetHexPosition();
            CardActiveEvent.ActiveCard = this;
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
            CardCheckEvent.ActiveButton();
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

    internal class SerializedFieldAttribute : System.Attribute
    {
    }
}
