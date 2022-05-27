using UniRx;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FactWorld
{
    public class UIButtonsNCards : MonoBehaviour
    {
        #region params
        [SerializeField] private Button _attackButton;
        [SerializeField] private GameObject _attackCardBase;
        [SerializeField] private float _cardsOffset, _speedAnimCardMove;
        [SerializeField] private Vector3 _newAttackCardPosition;
        private CompositeDisposable disposible = new CompositeDisposable();
        private CompositeDisposable movDisposible = new CompositeDisposable();
        private bool buttonBack ;
        private List<GameObject> attackCardList = new List<GameObject>();
        #endregion

        private void Start()
        {
            var defaultPosList = new List<Vector3>();
            attackCardList.Add(_attackCardBase);
            defaultPosList.Add(_attackCardBase.transform.localPosition);
            for (int i = 1; i < GameManager.MaxAttackCard; i++)
            {
                var obj = Instantiate(_attackCardBase, _attackCardBase.transform.position + new Vector3(0, _cardsOffset * i, 0), Quaternion.identity);
                obj.transform.SetParent(transform.GetComponentInParent<Transform>().parent);
                obj.transform.localScale = _attackCardBase.transform.localScale;
                attackCardList.Add(obj);
                defaultPosList.Add(obj.transform.localPosition);
            }
            _attackButton.OnClickAsObservable().Subscribe(_ => 
            {
                movDisposible.Clear();
                var t = 0f;
                buttonBack = !buttonBack;
                var currentPos = new List<Vector3>();
                for (int i = 0; i < attackCardList.Count; i++)
                {
                    currentPos.Add(attackCardList[i].transform.localPosition);
                }
                Observable.EveryUpdate().Subscribe(_ => 
                { 
                   for (int i = 0; i < attackCardList.Count; i++)
                    {
                        attackCardList[i].transform.localPosition = buttonBack ? Vector3.Lerp(currentPos[i], defaultPosList[i] + _newAttackCardPosition, t) :
                        Vector3.Lerp(currentPos[i], defaultPosList[i] , t);
                    }
                    t += Time.deltaTime * _speedAnimCardMove;
                    if (t >= 1f)
                    {
                        movDisposible.Clear();
                    }
                }).AddTo(movDisposible);
                
            }).AddTo(disposible);
        }
      
    }
}
