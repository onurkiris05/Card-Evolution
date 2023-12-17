using System;
using Game.Cards;
using Game.Gates;
using Game.Incrementals;
using Game.Player;
using Game.Projectiles;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Unit
{
    [Serializable]
    public class StickManData
    {
        public int YearsToUpgrade;
        public GameObject Weapon;
        public ProjectileType ProjectileType;
        public GameObject[] Costumes;
    }

    public enum UnitState
    {
        Idle,
        ThrowWalk,
        ShootWalk,
        Died,
        Finished
    }

    public class StickMan : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject[] shields;
        [SerializeField] private StickManData[] stickManDatas;

        [Space] [Header("Debug")]
        [SerializeField] [ReadOnly] private int currentYear;

        public Action<float> OnFireRateUpdate;
        public float FireRateDivider => _stickManController.FireRateDivider;
        public bool IsIndicated => stickManZoneHandler.IsIndicated;

        private StickManZoneHandler stickManZoneHandler;
        private StickManShootHandler _stickManShootHandler;
        private StickManAnimationHandler _stickManAnimationHandler;
        private StickManCostumeHandler _stickManCostumeHandler;
        private StickManController _stickManController;
        private Collider _collider;
        private int _shieldLevel;


        #region UNITY EVENTS

        private void Awake()
        {
            _stickManController = GetComponentInParent<StickManController>();
            stickManZoneHandler = GetComponent<StickManZoneHandler>();
            _stickManShootHandler = GetComponent<StickManShootHandler>();
            _stickManAnimationHandler = GetComponent<StickManAnimationHandler>();
            _stickManCostumeHandler = GetComponent<StickManCostumeHandler>();
            _collider = GetComponent<Collider>();

            _stickManCostumeHandler.Init(stickManDatas);
            _stickManShootHandler.Init(this, stickManDatas);
            _stickManAnimationHandler.Init(this);
        }
        public void AddYear()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                projectile.Kill(true);
                Kill();
            }
        }

        #endregion


        #region PUBLIC METHODS

        public void Init(StickManStats stats)
        {
            currentYear = stats.Year;
            //currentYear = PlayerPrefs.GetInt("StartYear");

            _stickManShootHandler.SetAttributes(stats.Rate, stats.Power, stats.Range);

            ProcessCostumes();
            ProcessWeapons();
        }


        public void Kill()
        {
            if (_stickManController.ProcessKilledStickMan(this))
            {
                _collider.enabled = false;
                VFXSpawner.Instance.PlayVFX("StickManDeath", transform.position);
            }
            else
            {
                _stickManController.ProcessPushBack();
            }
        }


        public void ResetIndicated()
        {
            stickManZoneHandler.SetIndicated(false);
        }


        public void ProcessStickManState(UnitState state)
        {
            if (state == UnitState.Died)
                transform.parent = null;


            if (state == UnitState.ThrowWalk || state == UnitState.ShootWalk)
            {
                var unitState = currentYear >= stickManDatas[^1].YearsToUpgrade
                    ? UnitState.ShootWalk
                    : UnitState.ThrowWalk;

                _stickManAnimationHandler.SetAnimationsState(unitState);
            }
            else
                _stickManAnimationHandler.SetAnimationsState(state);

            _stickManShootHandler.SetShootState(state);
        }


        public void ProcessAnimState(string anim, bool state)
        {
            _stickManAnimationHandler.SetSpecificAnimationState(anim, state);
        }


        public void ProcessCardReward(CardInfo cardInfo)
        {
            switch (cardInfo.ModifierType)
            {
                case ModifierType.Add:
                    switch (cardInfo.CardType)
                    {
                        case CardType.Year:
                            IncreaseYear(cardInfo.Amount);
                            break;

                        case CardType.Shield:
                            SetShield(cardInfo.Amount);
                            break;

                        default:
                            _stickManShootHandler.AddAttributes(cardInfo);
                            break;
                    }

                    break;

                case ModifierType.Multiply:
                    switch (cardInfo.CardType)
                    {
                        case CardType.Year:
                            // Maybe later
                            break;

                        default:
                            _stickManShootHandler.MultiplyAttributes(cardInfo);
                            break;
                    }

                    break;

                case ModifierType.Unlock:
                    switch (cardInfo.CardType)
                    {
                        default:
                            _stickManShootHandler.ProcessUnlockUpgrades(cardInfo);
                            break;
                    }

                    break;
            }

            VFXSpawner.Instance.PlayVFX("StickManUpgrade", transform.position);
        }


        public void ProcessGateReward(GateData gateData)
        {
            switch (gateData.Type)
            {
                case GateType.Year:
                    IncreaseYear(gateData.Amount);
                    VFXSpawner.Instance.PlayVFX("StickManUpgrade", transform.position);
                    break;
                default:
                    _stickManShootHandler.AddAttributes(gateData);
                    break;
            }
        }


        public void ProcessUpgradeButton(ButtonType _button)
        {
            if (_button== ButtonType.Year)
            {
                IncreaseYear((int)AlpGameManager.instance.yearAddAmount);
                PlayerPrefs.SetInt("StartYear",PlayerPrefs.GetInt("StartYear")+ (int)AlpGameManager.instance.yearAddAmount);
            }
            else
                _stickManShootHandler.AddAttributes(_button);

            VFXSpawner.Instance.PlayVFX("StickManUpgrade", transform.position);
        }


        public StickManStats GetStats()
        {
            return new StickManStats
            (
                currentYear,
                _stickManShootHandler.FirePower,
                _stickManShootHandler.FireRange,
                _stickManShootHandler.FireRate
            );
        }

        #endregion


        #region PRIVATE METHODS

        private void IncreaseYear(int year)
        {
            currentYear += year;

            ProcessCostumes();
            ProcessWeapons();
        }


        private void ProcessCostumes()
        {
            _stickManCostumeHandler.SetCostumes(currentYear);
        }


        private void ProcessWeapons()
        {
            _stickManShootHandler.SetWeapons(currentYear);
        }


        private void SetShield(int amount)
        {
            ResetShields();

            _shieldLevel += amount;

            if (_shieldLevel > shields.Length)
                _shieldLevel = shields.Length;

            shields[_shieldLevel - 1].SetActive(true);
        }


        private void ResetShields()
        {
            foreach (var shield in shields)
                shield.SetActive(false);
        }

        #endregion
    }
}