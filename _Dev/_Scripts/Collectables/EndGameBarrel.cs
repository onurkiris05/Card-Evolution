using DG.Tweening;
using Game.Managers;
using Game.Projectiles;
using UnityEngine;

namespace Game.Collectables
{
    public class EndGameBarrel : BaseCollectable
    {
        [Header("Components")]
        [SerializeField] private GameObject barrelObject;

        public int health;
        #region UNITY EVENTS

        private void Start()
        {
            hitCountText.text = $"{hitCount}";
            moneyPrize.SetState(false);
        }

        public void SetText()
        {
            hitCount = health;
            hitCountText.text = $"{hitCount}";
        }
        protected override void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                hitCount--;
                hitCountText.text = $"{hitCount}";

                projectile.Kill(true);

                barrelObject.transform.DOComplete();
                barrelObject.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);

                if (hitCount <= 0)
                {
                    _isCollected = true;
                    ReleasePrize();
                }
            }
            else if (other.CompareTag("StickMan"))
            {
                GameManager.Instance.ChangeState(GameState.End);
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void ReleasePrize()
        {
            // VFXSpawner.Instance.PlayVFX("BarrelExplosion", transform.position);
            hitCountText.enabled = false;
            barrelObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack)
                .OnComplete(() => barrelObject.SetActive(false));
            moneyPrize.transform.DOJump(collectablePos.position, 3, 1, 0.5f)
                .OnComplete(() => moneyPrize.SetState(true));
        }

        #endregion
    }
}