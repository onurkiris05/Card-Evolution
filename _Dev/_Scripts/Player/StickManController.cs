using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Gates;
using Game.Incrementals;
using Game.Managers;
using Game.Unit;
using UnityEngine;

namespace Game.Player
{
    public class StickManController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int startStickManCount;
        [SerializeField] float fireRate;
        [SerializeField] float fireRateDivider;
        [SerializeField] int firePower;
        [SerializeField] float fireRange;
        [SerializeField] [Range(0, 1)] private float distanceFactor;
        [SerializeField] [Range(0, 1)] private float radiusFactor;

        [Space] [Header("Components")]
        [SerializeField] private StickMan stickManPrefab;

        public float FireRateDivider => fireRateDivider;

        private PlayerController _player;
        private List<StickMan> _stickMans = new();
        private Dictionary<StickManStats, int> _stickMansToCreate = new();
        private StickManStats _stickManStats;


        #region UNITY EVENTS

        private void Start()
        {
            fireRange = PlayerPrefs.GetFloat("StartRange");
            if (fireRange == 0)
            {
                PlayerPrefs.SetInt("StartRange", 10);
                fireRange = 10;
            }
            int startYear = PlayerPrefs.GetInt("StartYear");
            _stickManStats = new StickManStats(startYear, firePower, fireRange, fireRate);

            StickMans_Create(startStickManCount, _stickManStats);
            StickMans_Set(UnitState.Idle);
            StickMans_Format();
        }


        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += OnGameStateChange;
        }


        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= OnGameStateChange;
        }

        #endregion


        #region PUBLIC METHODS

        public void Init(PlayerController player)
        {
            _player = player;
        }


        public void ProcessPushBack()
        {
            _player.PushBack(5, 1);
        }


        public bool CheckIndicatedStickMans()
        {
            bool isIndicated = false;

            foreach (var stickMan in _stickMans)
            {
                if (stickMan.IsIndicated)
                {
                    isIndicated = true;
                    break;
                }
            }

            return isIndicated;
        }


        public bool ProcessKilledStickMan(StickMan stickMan)
        {
            var isKilled = StickMans_Kill(stickMan);
            StickMans_Format();

            return isKilled;
        }


        public void ProcessIndicatedStickMans(CardBase card)
        {
            foreach (var stickMan in _stickMans)
            {
                if (stickMan.IsIndicated)
                {
                    // Turn off indicated color on stick man
                    stickMan.ResetIndicated();
                    var rewardData = card.GetCardData();

                    // Process reward data at here if its StickMan or MoneyMultiplier
                    // Otherwise, send reward data to stick man
                    switch (rewardData.CardType)
                    {
                        case CardType.StickMan:
                            ProcessStickManCardReward(stickMan.GetStats(), rewardData);
                            break;

                        case CardType.MoneyMultiplier:
                            ProcessMoneyCardReward();
                            break;

                        default:
                            stickMan.ProcessCardReward(rewardData);
                            break;
                    }
                }
            }

            // Create stick mans after iteration is done
            if (_stickMansToCreate.Count > 0)
            {
                foreach (var kvp in _stickMansToCreate)
                {
                    StickMans_Create(kvp.Value, kvp.Key);
                }

                StickMans_Format();
                _stickMansToCreate.Clear();

                var vfxPos = transform.position + Vector3.up;
                VFXSpawner.Instance.PlayVFX("StickManCreate", vfxPos);
            }
        }

        public void ProcessIncrementalButton(ButtonType _buttonType)
        {
            if (_buttonType == ButtonType.Range)
            {
                fireRange += AlpGameManager.instance.rangeAddAmount;
            }
            foreach (var stickMan in _stickMans)
            {
                stickMan.ProcessUpgradeButton(_buttonType);
            }
        }
            

        public void ProcessGateReward(GateData gateData)
        {
            // If type is Stick Man, process and return early
            if (gateData.Type == GateType.StickMan)
            {
                if (gateData.Amount > 0)
                {
                    StickMans_Create(gateData.Amount, _stickManStats);
                    StickMans_Set(UnitState.ThrowWalk);
                }
                else
                    StickMans_Kill(gateData.Amount);

                StickMans_Format();
                return;
            }

            // Otherwise, process reward on stick mans
            foreach (var stickMan in _stickMans)
            {
                stickMan.ProcessGateReward(gateData);
            }
        }


        public void StickMans_Set(UnitState unitState)
        {
            foreach (var stickMan in _stickMans)
            {
                stickMan.ProcessStickManState(unitState);
            }
        }

        #endregion


        #region PRIVATE METHODS

        private void StickMans_Create(int count, StickManStats stats)
        {
            for (int i = 0; i < count; i++)
            {
                StickMan stickMan = Instantiate(stickManPrefab, transform.position, Quaternion.identity, transform);
                stickMan.Init(stats);
                _stickMans.Add(stickMan);
            }

            Debug.Log($"Stick man created: {count}");
        }

        private void StickMans_Kill(int count)
        {
            List<StickMan> stickMansToRemove = new();

            for (int i = 0; i < Mathf.Abs(count); i++)
            {
                // Always left one stick man alive
                if (stickMansToRemove.Count >= _stickMans.Count - 1)
                    break;

                // Set removed stick man
                _stickMans[i].ProcessStickManState(UnitState.Died);
                stickMansToRemove.Add(_stickMans[i]);
            }

            foreach (var stickMan in stickMansToRemove)
                _stickMans.Remove(stickMan);

            Debug.Log($"Stick man killed: {stickMansToRemove.Count}");
        }


        private bool StickMans_Kill(StickMan killedStickMan)
        {
            List<StickMan> stickMansToRemove = new();

            foreach (var stickMan in _stickMans)
            {
                // Always left one stick man alive
                if (stickMansToRemove.Count >= _stickMans.Count - 1)
                    return false;

                if (killedStickMan.Equals(stickMan))
                {
                    // Set removed stick man
                    stickMan.ProcessStickManState(UnitState.Died);
                    stickMansToRemove.Add(stickMan);
                    break;
                }
            }

            foreach (var stickMan in stickMansToRemove)
                _stickMans.Remove(stickMan);

            Debug.Log($"Stick man killed: {stickMansToRemove.Count}");
            return true;
        }


        private void StickMans_Format()
        {
            for (int i = 0; i < _stickMans.Count; i++)
            {
                var x = distanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i) * radiusFactor;
                var z = distanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i) * radiusFactor;

                var pos = new Vector3(x, 0f, z);
                _stickMans[i].transform.DOLocalMove(pos, 0.3f).SetEase(Ease.OutBack);
            }
        }


        public void StickMans_StopShooting()
        {
            foreach (var stickMan in _stickMans)
            {
                stickMan.ProcessAnimState("isShooting", false);
                stickMan.ProcessAnimState("isThrowing", false);
            }
        }


        private void OnGameStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.Start:
                    StickMans_Set(UnitState.Idle);
                    break;
                case GameState.Running:
                    StickMans_Set(UnitState.ThrowWalk);
                    break;
                case GameState.End:
                    //StickMans_Set(UnitState.Died);
                    StickMans_Set(UnitState.Finished);
                    break;
            }
        }


        private void ProcessStickManCardReward(StickManStats stats, CardInfo cardInfo)
        {
            // Store count of stick mans to be created
            switch (cardInfo.ModifierType)
            {
                case ModifierType.Add:
                    if (_stickMansToCreate.Count <= 0)
                    {
                        _stickMansToCreate[stats] = cardInfo.Amount;
                    }

                    break;

                case ModifierType.Multiply:
                    _stickMansToCreate[stats] = cardInfo.Amount - 1;
                    break;
            }
        }


        private void ProcessMoneyCardReward()
        {
            //            EconomyManager.Instance.SetMultiplierMode(true);
            AlpGameManager.instance.multiplierOn = true;
        }

        #endregion
    }
}