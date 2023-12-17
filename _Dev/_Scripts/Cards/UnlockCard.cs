using DG.Tweening;
using Game.Managers;
using Game.Projectiles;
using UnityEngine;

namespace Game.Cards
{
    public class UnlockCard : CardBase
    {
        protected bool _isUnlocked;

        #region UNITY EVENTS

        protected override void Awake()
        {
            base.Awake();

        }
        private void Start()
        {
            Init();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                _currentHitCount += projectile.Power;
                projectile.Kill(true);
                ProcessHitEffect();

                if (_isUnlocked) return;

                SetBar();

                if (_currentHitCount >= cardData.Stats[0].HitsToUpgrade)
                {
                    _isUnlocked = true;
                    frontSprite.gameObject.SetActive(true);
                    lockedSprite.SetActive(false);
                    backgroundSprite.color = Color.white;
                    frontSprite.color = Color.white;
                    amountText.color = Color.black;

                    SetBar(_isUnlocked);
                }
            }
            else if (other.gameObject.CompareTag("StickMan"))
            {
                if (!_isUnlocked) return;

                _isCollected = true;
                ZoneManager.Instance.SendCardToUpgradeZone(this);
                _bar.gameObject.SetActive(false);
            }
        }

        #endregion

        #region PUBLIC METHODS

        public override CardInfo GetCardData()
        {
            var data = new CardInfo
            (
                cardType,
                ModifierType.Unlock,
                amount
            );

            return data;
        }

        #endregion

        #region PROTECTED METHODS

        protected override void Init()
        {
            SetAmountText();

            backgroundSprite.sprite = cardData.BackgroundSprites[cardData.BackgroundIndex - 1];

            //backgroundSprite.color = Color.gray;
            //            frontSprite.color = Color.gray;
            frontSprite.gameObject.SetActive(false);
            amountText.color = Color.black;
            lockedSprite.SetActive(true);

            switch (cardType)
            {
                case CardType.DoubleShot:
                    frontSprite.sprite = cardData.FrontSprites[0];
                    break;
                case CardType.TripleShot:
                    frontSprite.sprite = cardData.FrontSprites[1];
                    break;
                case CardType.RicochetProjectile:
                    frontSprite.sprite = cardData.FrontSprites[2];
                    break;
                case CardType.BoomerangProjectile:
                    frontSprite.sprite = cardData.FrontSprites[3];
                    break;
                case CardType.IceShot:
                    frontSprite.sprite = cardData.FrontSprites[4];
                    break;
                case CardType.FireShot:
                    frontSprite.sprite = cardData.FrontSprites[5];
                    break;
                case CardType.MoneyMultiplier:
                    frontSprite.sprite = cardData.FrontSprites[6];
                    break;
            }
        }


        protected override void SetBar(bool isUnlocked = false)
        {
            var currentHitRatio = Mathf.Min((float)_currentHitCount / cardData.Stats[_index].HitsToUpgrade, 1f);
            var amount = currentHitRatio;

            _bar.gameObject.SetActive(!isUnlocked);

            DOTween.Complete(this);
            DOTween.To(x => _bar.value = x, _bar.value, amount, 0.2f)
                .SetEase(Ease.Linear).SetId(this);
        }


        protected override void SetAmountText()
        {
            switch (cardType)
            {
                case CardType.DoubleShot:
                    amountText.text = $"Double Shot";
                    break;
                case CardType.TripleShot:
                    amountText.text = $"Triple Shot";
                    break;
                case CardType.RicochetProjectile:
                    amountText.text = $"Ricochet Shot";
                    break;
                case CardType.BoomerangProjectile:
                    amountText.text = $"Boomerang Shot";
                    break;
                case CardType.IceShot:
                    amountText.text = $"Ice Shot";
                    break;
                case CardType.FireShot:
                    amountText.text = $"Fire Shot";
                    break;
                case CardType.MoneyMultiplier:
                    amountText.text = $"Money Multiplier";
                    break;
            }
        }

        #endregion
    }
}