using DG.Tweening;
using Game.Managers;
using Game.Projectiles;
using Game.Unit;
using UnityEngine;

namespace Game.Collectables
{
    public class InGameBarrel : BaseCollectable
    {
        [Header("Components")]
        [SerializeField] private GameObject barrelObject;

        Color startColor;
        bool changingColor;
        public Renderer rr;
        #region UNITY EVENTS

        private void Awake()
        {
            startColor = rr.material.color;
        }
        private void Start()
        {
            hitCountText.text = $"{hitCount}";
            moneyPrize.SetState(false);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                hitCount--;
                hitCountText.text = $"{hitCount}";

                if (!changingColor)
                {
                    rr.material.color = Color.grey;
                    rr.material.DOColor(startColor, .2f).SetDelay(.1f).OnComplete(delegate {
                        changingColor = false;
	                });
                    changingColor = true;
                }
                projectile.Kill(true);

                barrelObject.transform.DOComplete();
                barrelObject.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);

                if (hitCount <= 0)
                {
                    _isCollected = true;
                    VFXSpawner.Instance.PlayVFX("InGameBarrelExplosion", transform.position);
                    ReleasePrize();
                }
            }
            else if (other.TryGetComponent(out StickMan stickMan))
            {
                stickMan.Kill();
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