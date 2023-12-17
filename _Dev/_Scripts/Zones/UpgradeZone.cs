using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Game.Cards;
using Game.Managers;
using Game.Projectiles;

namespace Game.Zones
{
    public class UpgradeZone : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform playerStand;
        [SerializeField] private Transform conveyorEnd;
        [SerializeField] private Transform cardLayoutZoneMin;
        [SerializeField] private Transform cardLayoutZoneMax;

        public Transform ConveyorEnd => conveyorEnd;

        private bool _isTriggered;

        #region UNITY EVENTS

        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                projectile.Kill();
            }
            else if (other.gameObject.CompareTag("StickMan"))
            {
                _isTriggered = true;
                ZoneManager.Instance.StartUpgradeSequence(playerStand.position);
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void PlaceCards(List<CardBase> cards)
        {
            var maxColCount = 3;
            var rowCount = Mathf.CeilToInt((float)cards.Count / maxColCount);

            var middleX = (cardLayoutZoneMax.position.x + cardLayoutZoneMin.position.x) / 2;
            var offsetX = (cardLayoutZoneMax.position.x - cardLayoutZoneMin.position.x) / maxColCount;
            var offsetZ = 3.25f;

            for (int i = 0; i < cards.Count; i++)
            {
                var rowIndex = i / maxColCount;
                var colIndex = i % maxColCount;

                // Calculate the number of cards in the current row
                var cardsInCurrentRow = (rowIndex == rowCount - 1 && cards.Count % maxColCount != 0)
                    ? cards.Count % maxColCount
                    : maxColCount;

                // Calculate the total width of the cards in the current row
                var totalRowWidth = cardsInCurrentRow * offsetX;

                // Calculate the starting X position for the current row
                var startX = (middleX + (offsetX / 2)) - (totalRowWidth / 2);

                // Calculate the X position for the current card within the row
                var posX = startX + (colIndex * offsetX);

                // Calculate the Z position based on the row index
                var posZ = (cardLayoutZoneMax.position.z - offsetZ) - (rowIndex * offsetZ);

                var cardPosition = new Vector3(posX, 0.1f, posZ);

                cards[i].transform.rotation = Quaternion.Euler(90f, 90f, 90f);
                cards[i].transform.DOMove(cardPosition, 0.4f).SetEase(Ease.Linear);
            }
        }

        #endregion
    }
}