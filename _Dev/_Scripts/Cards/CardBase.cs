using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Game.Projectiles;

namespace Game.Cards
{
    [Serializable]
    public class CardStat
    {
        public int HitsToUpgrade;
        public int UpgradeAmount;
    }

    [Serializable]
    public class CardData
    {
        [Range(1, 4)] public int BackgroundIndex;
        [ShowAssetPreview] public Sprite[] BackgroundSprites;
        [ShowAssetPreview] public Sprite[] FrontSprites;
        public CardStat[] Stats;
    }

    public abstract class CardBase : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected SpriteRenderer backgroundSprite;
        [SerializeField] protected SpriteRenderer frontSprite;
        [SerializeField] protected TextMeshProUGUI maxText;
        [SerializeField] protected TextMeshProUGUI amountText;

        [Space] [Header("Settings")]
        [SerializeField] protected CardType cardType;
        [SerializeField] protected int amount;
        [SerializeField] [Range(0.5f, 5f)] protected float indicatorScale;
        [SerializeField] protected CardData cardData;

        public float IndicatorScale => indicatorScale;

        protected Slider _bar;
        protected BoxCollider _collider;
        protected float _currentHitCount;
        protected int _index;
        protected bool _isMaxed;
        protected bool _isCollected;
        public GameObject lockedSprite;
        #region UNITY EVENTS

        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _bar = GetComponentInChildren<Slider>();
            lockedSprite.SetActive(false);
            if (lockedSprite == null)
            {
                lockedSprite = transform.GetChild(0).GetChild(3).gameObject;
            }
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_isCollected) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                _currentHitCount += (projectile.Power);
                projectile.Kill(true,true);
                ProcessHitEffect();

                if (_isMaxed) return;

                SetBar();

                if (_currentHitCount >= cardData.Stats[_index].HitsToUpgrade)
                {
                    amount += cardData.Stats[_index].UpgradeAmount;
                    SetAmountText();

                    _currentHitCount -= cardData.Stats[_index].HitsToUpgrade;

                    if (_index >= cardData.Stats.Length - 1)
                        _isMaxed = true;
                    else
                        _index++;

                    SetBar(_isMaxed);
                }
            }
            else if (other.gameObject.CompareTag("StickMan"))
            {
                _isCollected = true;
                ZoneManager.Instance.SendCardToUpgradeZone(this);
                _bar.gameObject.SetActive(false);
                maxText.gameObject.SetActive(false);
            }
        }

        #endregion


        #region PUBLIC METHODS

        public virtual void SetState(bool state)
        {
            _collider.enabled = state;
            transform.SetActiveChildren(state);
        }


        public abstract CardInfo GetCardData();

        #endregion


        #region PROTECTED METHODS

        protected abstract void Init();


        protected abstract void SetAmountText();


        protected virtual void SetBar(bool isMaxed = false)
        {
            maxText.gameObject.SetActive(isMaxed);
            var currentHitRatio = Mathf.Min((float)_currentHitCount / cardData.Stats[_index].HitsToUpgrade, 1f);
            var amount = isMaxed ? 1f : currentHitRatio;

            DOTween.Complete(this);
            DOTween.To(x => _bar.value = x, _bar.value, amount, 0.2f)
                .SetEase(Ease.Linear).SetId(this);
        }


        protected virtual void ProcessHitEffect()
        {
            transform.DOComplete();
            transform.DOShakeScale(0.2f, 0.2f);
        }

        #endregion
    }
}