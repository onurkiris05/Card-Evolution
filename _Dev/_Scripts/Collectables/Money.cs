using DG.Tweening;
using Game.Managers;
using UnityEngine;

namespace Game.Collectables
{
    public class Money : BaseCollectable
    {
        [Header("Settings")]
        [SerializeField] private int amount;

        private BoxCollider _collider;
        

        #region UNITY EVENTS

        private void Awake() => _collider = GetComponent<BoxCollider>();

        protected override void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.CompareTag("StickMan"))
            {
                _isCollected = true;

                transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
                {
                    ImageSpawner.Instance.SpawnAndMove("Money", transform.position,
                        AlpGameManager.instance.coinText.rectTransform, 3);
                    if (AlpGameManager.instance.multiplierOn)
                    {
                        amount *= 2;
                    }
                    PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + amount);
                    AlpGameManager.instance.RefreshCoinText();
                    Kill();
                });
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void SetState(bool state) => _collider.enabled = state;

        public void SetAmount(int amount) => this.amount = amount;

        public void Kill() => Destroy(gameObject);

        #endregion
    }
}