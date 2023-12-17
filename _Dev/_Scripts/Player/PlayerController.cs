using DG.Tweening;
using Game.Cards;
using Game.Gates;
using Game.Unit;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        public bool IsTrapped => _isTrapped;

        private MovementHandler _movementHandler;
        private StickManController _stickManController;
        private bool _isTrapped;


        public float powerEffectAmount;
        public float rangeEffectAmount;
        public float rateEffectAmount;

        public static PlayerController instance;

        #region UNITY EVENTS

        private void Awake()
        {
            _stickManController = GetComponent<StickManController>();
            _movementHandler = GetComponent<MovementHandler>();

            _stickManController.Init(this);
            _movementHandler.Init(this);
            instance = this;
        }

        #endregion

        #region PUBLIC METHODS

        public void PrepareToUpgrade()
        {
            _stickManController.StickMans_Set(UnitState.Idle);
        }

        public void StopShooting()
        {
            _stickManController.StickMans_StopShooting();
        }

        public bool IsAnyStickManIndicated()
        {
            return _stickManController.CheckIndicatedStickMans();
        }

        public void ProcessIndicatedStickMans(CardBase card)
        {
            _stickManController.ProcessIndicatedStickMans(card);
        }

        public void ProcessGateReward(GateData gateData)
        {
            _stickManController.ProcessGateReward(gateData);
        }

        public void PushBack(float pushBackDistance, float pushBackDuration)
        {
            _isTrapped = true;

            transform.DOMoveZ(transform.position.z - pushBackDistance, pushBackDuration)
                .OnComplete(() => { _isTrapped = false; });
        }

        #endregion
    }
}