using UnityEngine;
using Game.Cards;
using Game.Player;
using Game.Managers;

namespace Game.Zones
{
    public class ZoneInputHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform indicator;

        private bool _isZoneInput;
        private bool _isDragging;
        private ZoneManager _zoneManager;
        private PlayerController _player;
        private CardBase _currentCard;
        private Vector3 _cardOriginalPos;
        private RaycastHit _hit;
        private Ray _ray;


        #region UNITY EVENTS

        private void Update()
        {
            if (!_isZoneInput) return;

            if (Input.GetMouseButtonDown(0) && !_isDragging)
            {
                OnFingerDown();
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                OnFingerMove();
            }
            else if (Input.GetMouseButtonUp(0) && _isDragging)
            {
                OnFingerUp();
            }
        }

        #endregion


        #region PUBLIC METHODS

        public void Init(ZoneManager zoneManager, PlayerController player)
        {
            _zoneManager = zoneManager;
            _player = player;
        }

        
        public void SetZoneInput(bool state)
        {
            _isZoneInput = state;
            Debug.Log($"{gameObject.name} - Zone Input State is: {state}");
        }

        #endregion


        #region PRIVATE METHODS

        private void OnFingerDown()
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hit, 100f, targetLayer))
            {
                _isDragging = true;

                if (_hit.transform.TryGetComponent(out CardBase card))
                {
                    _currentCard = card;
                    _cardOriginalPos = _currentCard.transform.position;

                    indicator.localScale = Vector3.one * _currentCard.IndicatorScale;
                    indicator.gameObject.SetActive(true);

                    MoveCard();
                }
            }
        }

        
        private void OnFingerMove()
        {
            MoveCard();
        }

        
        private void OnFingerUp()
        {
            _isDragging = false;

            if (_player.IsAnyStickManIndicated())
            {
                _currentCard.SetState(false);
                _player.ProcessIndicatedStickMans(_currentCard);
                _zoneManager.RemoveCard(_currentCard);
                _currentCard = null;
                indicator.gameObject.SetActive(false);
            }
            else
            {
                _currentCard.transform.position = _cardOriginalPos;
                _currentCard = null;
                indicator.gameObject.SetActive(false);
            }
        }

        
        private void MoveCard()
        {
            var inputPos = Camera.main.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 15f));
            var cardPos = new Vector3(inputPos.x, 2f, inputPos.z);
            var indicatorPos = new Vector3(cardPos.x, 0f, cardPos.z + 5f);

            _currentCard.transform.position = cardPos;
            indicator.transform.position = indicatorPos;
        }

        #endregion
    }
}