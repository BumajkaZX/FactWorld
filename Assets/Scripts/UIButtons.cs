using UniRx;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FactWorld
{
    public class UIButtons : MonoBehaviour
    {
        [SerializeField] private Button _attackButton;
        [SerializeField] private TextMeshProUGUI _textAttack;
        [SerializeField] private string _hideTextAttack;
        [SerializeField] private string _activeTextAttack;
        [SerializeField] private List<GameObject> _guns = new List<GameObject>();
        [SerializeField] private Vector3 _activePosAttack;
        [SerializeField] private float _speedAnimAttack;
        private List<Vector3> defaultPosAttack = new List<Vector3>();
        private bool isActiveAttack;
        [Space(50)]

        [SerializeField] private List<GameObject> _actions = new List<GameObject>();
        [SerializeField] private Button _actionButton;
        [SerializeField] private TextMeshProUGUI _textAction;
        [SerializeField] private string _hideTextAction;
        [SerializeField] private string _activeTextAction;
        [SerializeField] private Vector3 _activePosAction;
        [SerializeField] private float _speedAnimAction;
        private List<Vector3> defaultPosAction = new List<Vector3>();
        private bool isActiveAction;

        private void Start()
        {
            for (int i = 0; i < _guns.Count; i++)
            {
                defaultPosAttack.Add(_guns[i].transform.localPosition);
            }

            for (int i = 0; i < _actions.Count; i++)
            {
                defaultPosAction.Add(_actions[i].transform.localPosition);
            }
        }

        public void Attack()
        {
            if (isActiveAction) return;
            Move(_guns, _textAttack, _hideTextAttack, _activeTextAttack, _activePosAttack, defaultPosAttack, _speedAnimAttack, isActiveAttack, _attackButton);
            isActiveAttack = !isActiveAttack;
            
        }

        public void Action()
        {
            if (isActiveAttack) return;
            Move(_actions, _textAction, _hideTextAction, _activeTextAction, _activePosAction, defaultPosAction, _speedAnimAction, isActiveAction, _actionButton);
            isActiveAction = !isActiveAction;
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
                    
                    print(t);
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
