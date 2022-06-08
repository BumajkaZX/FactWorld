using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;

namespace FactWorld
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button _leftChangeTypeCardButton, _rightChangeTypeCardButton;
        [SerializeField] private Button _cardsUp;
        [SerializeField] private Transform _circleRotator;
        [SerializeField] private RectTransform _cardPosition;
        [SerializeField] private GameObject _icon, _iconsParent, _cardReference;
        [SerializeField] private List<Sprite> _iconTypes = new List<Sprite>();
        [SerializeField] private int _maxCards;
        [SerializeField] private Vector2 _cardsOffset;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject _description;
        private int _activeCardType; //Save
        private float _iconWidth;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private CompositeDisposable _disposables1 = new CompositeDisposable();
        private Vector3 _defaultPos;
        private RectTransform _rect, _rectParent;
        private Vector2 _rectParDefPos;
        private bool _isActiveCardUp;
        private List<CardSO> _cards;
        private List<GameObject> _cardsReference = new List<GameObject>();
        private List<CardSO[]> _sortedCards;
        private GameObject marker;
        private TextMeshProUGUI _descriptionText;
        private void Start()
        {
            _activeCardType = GameManager.activeCardType;
            GameManager.activeCardUp.AddListener(ActiveCardDescription);
            marker = _description.transform.GetChild(1).gameObject;
            _descriptionText = _description.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            _cards = GameManager.cards;
            _text.text = Enum.GetName(typeof(CardSO._cardsType), _activeCardType);
            SpawnIcons();
            CardsSpawn();
            ChangeTypeofCard();
            CardsUpDown();
        }
        private void SpawnIcons()
        {
            _iconWidth = _cardsUp.GetComponent<RectTransform>().rect.width;
            _rect = _icon.GetComponent<RectTransform>();
            _defaultPos = _rect.anchoredPosition;
            for (int i = 0; i < _iconTypes.Count; i++)
            {
                var obj = Instantiate(_icon.gameObject, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(_iconsParent.transform);
                obj.transform.localScale = _icon.transform.localScale;
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(_defaultPos.x + _iconWidth * 2 * i, _defaultPos.y);
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
                obj.GetComponent<SpriteRenderer>().sprite = _iconTypes[i];
            }
            _rectParent = _iconsParent.GetComponent<RectTransform>();
            _rectParDefPos = _rectParent.anchoredPosition;
            _rectParent.anchoredPosition = new Vector2(-_iconWidth * 2 * _activeCardType, _rectParent.anchoredPosition.y);
            _rectParDefPos = _rectParent.anchoredPosition;
        }
        private void ChangeTypeofCard()
        {
            _leftChangeTypeCardButton.OnClickAsObservable().Subscribe(_ => 
            {
                if (_activeCardType == 0) return;
                _activeCardType--;
                if (_isActiveCardUp) ActiveCardsChange(_activeCardType + 1, true);
                _text.text = Enum.GetName(typeof(CardSO._cardsType), _activeCardType);
                _disposables.Clear();
                var f = 0f;
                var rectparentpos = new Vector2(-(_iconWidth * 2 * (_activeCardType + 1)), _rectParDefPos.y);
                Observable.EveryFixedUpdate().Subscribe(_ => 
                {
                    _rectParent.anchoredPosition = Vector2.Lerp(_rectParent.anchoredPosition, new Vector2(rectparentpos.x + _iconWidth * 2, rectparentpos.y), f);
                    f += Time.fixedDeltaTime;
                    if (f >= 1f)
                    {
                        _disposables.Clear();
                    }
                }).AddTo(_disposables);
            }).AddTo(this);
            _rightChangeTypeCardButton.OnClickAsObservable().Subscribe(_ => 
            {
                if (_activeCardType == _iconTypes.Count - 1) return;
                _activeCardType++;
                if (_isActiveCardUp) ActiveCardsChange(_activeCardType - 1, false);
                _text.text = Enum.GetName(typeof(CardSO._cardsType), _activeCardType);
                _disposables.Clear();
                var f = 0f;
                var rectparentpos = new Vector2(-(_iconWidth * 2 * (_activeCardType - 1)), _rectParDefPos.y);
                Observable.EveryFixedUpdate().Subscribe(_ =>
                {
                    _rectParent.anchoredPosition = Vector2.Lerp(_rectParent.anchoredPosition, new Vector2(rectparentpos.x - _iconWidth * 2, rectparentpos.y), f);
                    f += Time.fixedDeltaTime;
                    if (f >= 1f)
                    {
                        _disposables.Clear();
                    }
                }).AddTo(_disposables);
            }).AddTo(this);
         
        }
        private void CardsUpDown()
        {
            List<CardSO[]> cardsTypes = new List<CardSO[]>();
            for (int i = 0; i < GameManager.cardTypes; i++)
            {
                var x = 0;
                for (int v = 0; v < _cards.Count; v++)
                {
                    if ((int)_cards[v].cardsType == i)
                    {
                        
                        x++;
                    }
                }
                var cardSO = new CardSO[x];
                x = 0;
                for (int v = 0; v < _cards.Count; v++)
                {
                    if ((int)_cards[v].cardsType == i)
                    {
                        cardSO[x] = _cards[v];
                        x++;
                    }
                }
                cardsTypes.Add(cardSO);
            }
            _sortedCards = cardsTypes;
            _cardsUp.OnClickAsObservable().Select(_ => cardsTypes).Subscribe(x =>
            {
                var maxActiveCard = x[_activeCardType].Length;
                _isActiveCardUp = !_isActiveCardUp;
                for (int i = 0; i < maxActiveCard; i++)
                {
                    _cardsReference[i].SetActive(true);
                    var card = x[_activeCardType];
                    _cardsReference[i].transform.GetChild(0).GetComponent<TextMeshPro>().text = card[i].cardName;
                    _cardsReference[i].gameObject.name = card[i].name;
                    _cardsReference[i].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = card[i].cardSprite;
                }
                
                _disposables1.Clear();
                var f = 0f;
                Observable.EveryFixedUpdate().Subscribe(_ => 
                {
                    for(int i = 0; i < maxActiveCard; i++)
                    {
                        var posCard = _cardsReference[i].GetComponent<RectTransform>().anchoredPosition;
                        _cardsReference[i].GetComponent<RectTransform>().anchoredPosition = !_isActiveCardUp ? Vector2.Lerp(posCard, new Vector2(posCard.x, _cardPosition.anchoredPosition.y - _cardsOffset.y), f) : 
                            Vector2.Lerp(posCard, new Vector2(posCard.x, _cardPosition.anchoredPosition.y), f);
                        
                        
                    }
                    f += Time.fixedDeltaTime;
                    if (f >= 1f)
                    {
                        _disposables1.Clear();
                    }
                }).AddTo(_disposables1);

            }).AddTo(this);
        }
        private void ActiveCardDescription(GameObject cardPos)
        {
            if (cardPos == null)
            {
                _description.SetActive(false);
                return;
            }
            _description.SetActive(true);
            _descriptionText.text = GameManager.activeCard.description;
            var marker = _description.transform.GetChild(1);
            marker.transform.position = new Vector3(cardPos.transform.position.x, marker.transform.position.y, marker.transform.position.z);
        }
        private void ActiveCardsChange(int last, bool LR)
        {
            var maxActiveCard = _sortedCards[last].Length;
            _disposables1.Clear();
            var f = 0f;
            var isUp = false;
            Observable.EveryFixedUpdate().Subscribe(_ =>
            {
                for (int i = 0; i < _cardsReference.Count; i++)
                {   
                    var posCard = _cardsReference[i].GetComponent<RectTransform>().anchoredPosition;
                    if (!isUp)
                    {
                        _cardsReference[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(posCard, new Vector2(posCard.x, _cardPosition.anchoredPosition.y - _cardsOffset.y), f);
                                
                    }
                    else if (i < maxActiveCard)
                    {
                        _cardsReference[i].GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(posCard, new Vector2(posCard.x, _cardPosition.anchoredPosition.y), f);
                    }
                }
                f += Time.fixedDeltaTime * 2;
                if (f >= 1f)
                {
                    if (isUp) _disposables1.Clear();
                    maxActiveCard = LR ? _sortedCards[last - 1].Length : _sortedCards[last + 1].Length;
                    for (int v = 0; v < maxActiveCard; v++)
                    {
                        _cardsReference[v].SetActive(true);
                        var card = _sortedCards[_activeCardType];
                        _cardsReference[v].transform.GetChild(0).GetComponent<TextMeshPro>().text = card[v].cardName;
                        _cardsReference[v].gameObject.name = card[v].name;
                        _cardsReference[v].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = card[v].cardSprite; 
                    }
                    print(f);
                    isUp = true;
                    f = 0f;

                }
            }).AddTo(_disposables1);   
            
        }
        private void CardsSpawn()
        {
            for (int i = 0; i < _maxCards; i++)
            {
                var obj = Instantiate(_cardReference, Vector3.zero, Quaternion.identity, _cardPosition);
                obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(_cardPosition.anchoredPosition.x + i * _cardsOffset.x, _cardPosition.anchoredPosition.y - _cardsOffset.y, 0);
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _cardsReference.Add(obj);
                obj.SetActive(false);
            }
        }
       
    }
}
