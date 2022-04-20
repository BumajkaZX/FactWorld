using UniRx;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FactWorld
{
    public class UIButtons : MonoBehaviour
    {
        #region params
       
        private CompositeDisposable disposible = new CompositeDisposable();
        #endregion
        private void Awake()
        {
            
        }
        private void Start()
        {
           
            
        }


        private void Move(List<GameObject> obj, TextMeshProUGUI text, string hideText, string activeText, Vector3 activePos, List<Vector3> defaultPos, float speedAnim, bool isActive, Button button)
        {
            Vector3[] activePosMas = new Vector3[obj.Count];
            for (int c = 0; c< obj.Count; c++)
            {
                activePosMas[c] = defaultPos[c] + activePos;
            }
            
            button.interactable = false;
            var t = 0f;
            var i = 0;
            CompositeDisposable disposables = new CompositeDisposable();
            Observable.EveryUpdate().Subscribe(_ => 
            {
                    var gun = obj[i];
                    gun.transform.localPosition = isActive ? Vector3.Lerp(activePosMas[i], defaultPos[i],  t) :  Vector3.Lerp(defaultPos[i], activePosMas[i], t);
                    i++;
                    if (i >= obj.Count)
                    {
                        i = 0;
                        t += Time.deltaTime * speedAnim;
                    }
                    if (t >= 1)
                    {
                        button.interactable = true;
                        text.text = isActive ? activeText : hideText;
                        disposables.Clear();
                    }

            }).AddTo(disposables);
        }


    }
}
