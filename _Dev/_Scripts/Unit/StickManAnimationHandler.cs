using UnityEngine;

namespace Game.Unit
{
    public class StickManAnimationHandler : MonoBehaviour
    {
        private readonly int IsShootWalking = Animator.StringToHash("isShootWalking");
        private readonly int IsShooting = Animator.StringToHash("isShooting");
        private readonly int IsThrowWalking = Animator.StringToHash("isThrowWalking");
        private readonly int IsThrowing = Animator.StringToHash("isThrowing");
        private readonly int IsDead = Animator.StringToHash("isDead");
        private Animator _animator;
        private StickMan _stickMan;


        #region UNITY EVENTS

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }


        private void OnEnable()
        {
            _stickMan.OnFireRateUpdate += SetFireRate;
        }


        private void OnDisable()
        {
            _stickMan.OnFireRateUpdate -= SetFireRate;
        }

        #endregion


        #region PUBLIC METHODS

        public void Init(StickMan stickMan)
        {
            _stickMan = stickMan;
        }


        public void SetAnimationsState(UnitState state)
        {
            _animator.SetBool(IsShootWalking, state == UnitState.ShootWalk);
            _animator.SetBool(IsShooting, state == UnitState.ShootWalk);
            _animator.SetBool(IsThrowWalking, state == UnitState.ThrowWalk);
            _animator.SetBool(IsThrowing, state == UnitState.ThrowWalk);

            if (state == UnitState.Died)
                _animator.SetTrigger(IsDead);
        }


        public void SetSpecificAnimationState(string animation, bool state)
        {
            _animator.SetBool(animation, state);
        }

        #endregion


        #region PRIVATE METHODS

        private void SetFireRate(float value)
        {
            _animator.SetFloat("fireRate", value);
        }

        #endregion
    }
}