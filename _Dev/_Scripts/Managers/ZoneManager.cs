using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Game.Cards;
using Game.Player;
using Game.Zones;

namespace Game.Managers
{
    public class ZoneManager : StaticInstance<ZoneManager>
    {
        [Header("Settings")]
        [SerializeField] private float conveyorSpeed;

        [Space] [Header("Components")]
        [SerializeField] private UpgradeZone[] upgradeZones;

        private List<CardBase> _cardsInConveyor = new();
        private PlayerController _player;
        private ZoneInputHandler _zoneInputHandler;
        private int _zoneIndex;


        #region UNITY EVENTS

        protected override void Awake()
        {
            base.Awake();

            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _zoneInputHandler = GetComponent<ZoneInputHandler>();
            _zoneInputHandler.Init(this, _player);
        }

        #endregion


        #region PUBLIC METHODS

        public void RemoveCard(CardBase card)
        {
            _cardsInConveyor.Remove(card);

            // End upgrade sequence if there is no card
            if (_cardsInConveyor.Count == 0)
            {
                // Move to next zone index if this one finished
                _zoneIndex++;
                if (_zoneIndex >= upgradeZones.Length)
                    _zoneIndex = upgradeZones.Length - 1;

                EndUpgradeSequence();
            }
        }


        public void SendCardToUpgradeZone(CardBase card)
        {
            var targetPos = upgradeZones[_zoneIndex].ConveyorEnd.position;
            var jumpPos = new Vector3(targetPos.x, card.transform.position.y, card.transform.position.z);

            _cardsInConveyor.Add(card);
            card.transform.DORotate(new Vector3(-90f, -180f, 0f), 0.5f);
            card.transform.DOJump(jumpPos, 1, 1, 1f).OnComplete(() =>
            {
                card.transform.DOMove(targetPos, conveyorSpeed).SetSpeedBased(true);
            });
        }


        public void StartUpgradeSequence(Vector3 playerStandPos)
        {
            GameManager.Instance.ChangeState(GameState.Upgrade);
            CameraManager.Instance.SetCamera(CameraType.UpgradeZone);
            _player.StopShooting();

            upgradeZones[_zoneIndex].PlaceCards(_cardsInConveyor);

            _player.transform.DOMove(playerStandPos, 1f).OnComplete(() =>
            {
                _player.PrepareToUpgrade();
                _zoneInputHandler.SetZoneInput(true);
            });
        }

        #endregion


        #region PRIVATE METHODS

        private void EndUpgradeSequence()
        {
            GameManager.Instance.ChangeState(GameState.Running);
            CameraManager.Instance.SetCamera(CameraType.Running);
        }

        #endregion
    }
}