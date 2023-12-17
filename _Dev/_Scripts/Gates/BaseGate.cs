using DG.Tweening;
using TMPro;
using UnityEngine;
using Game.Player;
using Game.Projectiles;

namespace Game.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected GateType gateType;
        [SerializeField] protected int value;
        [SerializeField] protected bool isLocked;
        [SerializeField] protected int unlockCount;

        [Space] [Header("Components")]
        [SerializeField] protected GameObject blueGate;
        [SerializeField] protected GameObject redGate;
        [SerializeField] protected GameObject grayGate;
        [SerializeField] protected TextMeshPro gateText;
        [SerializeField] protected TextMeshPro valueText;
        [SerializeField] protected GameObject lockBar;
        [SerializeField] protected GameObject pointer;

        protected float _perUnlockCount;
        protected bool _isKilled;


        #region UNITY EVENTS

        protected virtual void Start()
        {
            InitGate(gateType, value);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_isKilled) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                InteractEffect();
                projectile.Kill(true);

                if (isLocked)
                {
                    UpdateLockBar();
                    return;
                }

                UpdateGate();
            }
            else if (other.CompareTag("StickMan"))
            {
                var player = other.GetComponentInParent<PlayerController>();
                if (player == null) return;

                if (!isLocked)
                {
                    player.ProcessGateReward(GetGateData());
                    KillGate();
                }
            }
        }

        #endregion

        #region PROTECTED METHODS

        protected virtual void InitGate(GateType gateTypes, float value)
        {
            if (isLocked)
            {
                lockBar.SetActive(isLocked);
                _perUnlockCount = Mathf.Abs(pointer.transform.localPosition.x) * 2.0f / unlockCount;
            }

            SetGate(value);

            switch (gateTypes)
            {
                case GateType.StickMan:
                    gateText.text = "Stick Man";
                    break;
                case GateType.Year:
                    gateText.text = "Year";
                    break;
                case GateType.Power:
                    gateText.text = "Power";
                    break;
                case GateType.Rate:
                    gateText.text = "Rate";
                    break;
                case GateType.Range:
                    gateText.text = "Range";
                    break;
            }
        }


        protected virtual void SetGate(float value)
        {
            var isPositiveGate = value > 0;
            valueText.text = isPositiveGate ? $"+{value}" : $"{value}";

            grayGate.SetActive(isLocked);
            blueGate.SetActive(!isLocked && isPositiveGate);
            redGate.SetActive(!isLocked && !isPositiveGate);
        }


        protected virtual void UpdateGate()
        {
            value++;
            SetGate(value);
        }


        protected virtual GateData GetGateData()
        {
            return new GateData(gateType, value);
        }


        protected virtual void UpdateLockBar()
        {
            unlockCount--;
            if (unlockCount <= 0)
            {
                isLocked = false;
                lockBar.SetActive(isLocked);
                SetGate(value);
            }

            var t = pointer.transform;
            t.DOComplete();
            t.DOLocalMove(new Vector3(t.localPosition.x + _perUnlockCount, t.localPosition.y, t.localPosition.z), 1f)
                .SetSpeedBased(true).SetEase(Ease.OutCubic);
        }


        protected virtual void InteractEffect()
        {
            transform.DOComplete();
            transform.DOShakeScale(0.15f, new Vector3(0.2f, 0.2f, 0.2f));
        }


        protected virtual void KillGate()
        {
            _isKilled = true;
            transform.DOComplete();
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InElastic)
                .OnComplete(() => Destroy(gameObject));
        }

        #endregion
    }
}