using System;
using Game.Cards;
using Game.Gates;
using Game.Incrementals;
using Game.Projectiles;
using NaughtyAttributes;
using UnityEngine;
using Game.Player;
namespace Game.Unit
{
    public class StickManShootHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform shootPoint;

        [Space] [Header("Debug")]
        [SerializeField] [ReadOnly] private float fireRate;
        [SerializeField] [ReadOnly] private float firePower;
        [SerializeField] [ReadOnly] private float fireRange;

        private StickMan _stickMan;
        private StickManData[] _stickManDatas;
        private ProjectileType _currentProjectile;
        private Action _currentShotType;
        private ProjectileBehaviour _currentProjectileBehaviour;
        private ProjectileModifier _currentProjectileModifier;
        public static StickManShootHandler instance;

        #region ENCAPSULATIONS

        public float FireRate
        {
            get => fireRate;
            private set
            {
                fireRate = value;
                _stickMan.OnFireRateUpdate?.Invoke(fireRate);
            }
        }

        public float FirePower => firePower;
        public float FireRange => fireRange;

        #endregion
        private void Awake()
        {
            instance = this;
            fireRange = PlayerPrefs.GetFloat("StartRange");
            if (fireRange == 0)
            {
                fireRange = 10;
                PlayerPrefs.SetFloat("StartRange", fireRange);
            }
        }

        #region PUBLIC METHODS

        public void Init(StickMan stickMan, StickManData[] stickManDatas)
        {
            _stickMan = stickMan;
            _stickManDatas = stickManDatas;
            _currentShotType = StandardShot;
            _currentProjectileBehaviour = ProjectileBehaviour.Standard;
            _currentProjectileModifier = ProjectileModifier.Standard;
        }


        public void SetAttributes(float fireRate, float firePower, float fireRange)
        {
            FireRate = fireRate;
            this.firePower = firePower;
            this.fireRange = fireRange;
        }


        public void AddAttributes(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.Rate:
                    Debug.Log($"Fire Rate= {FireRate} + {card.Amount}");
                    FireRate += (card.Amount*PlayerController.instance.rateEffectAmount) / _stickMan.FireRateDivider;
                    break;
                case CardType.Power:
                    Debug.Log($"Fire Power= {firePower} + {card.Amount}");
                    firePower += card.Amount*PlayerController.instance.powerEffectAmount;
                    break;
                case CardType.Range:
                    Debug.Log($"Fire Range= {fireRange} + {card.Amount}");
                    fireRange += card.Amount*PlayerController.instance.rangeEffectAmount;
                    break;
            }
        }


        public void AddAttributes(GateData gate)
        {
            switch (gate.Type)
            {
                case GateType.Rate:
                    FireRate += (gate.Amount*PlayerController.instance.rateEffectAmount) / _stickMan.FireRateDivider;
                    break;
                case GateType.Power:
                    firePower += gate.Amount*PlayerController.instance.powerEffectAmount;
                    break;
                case GateType.Range:
                    fireRange += gate.Amount*PlayerController.instance.rangeEffectAmount;
                    break;
            }
        }


        public void AddAttributes(ButtonType button)
        {
            switch (button)
            {
                case ButtonType.Range:
                    fireRange += AlpGameManager.instance.rangeAddAmount;
                    PlayerPrefs.SetFloat("StartRange", fireRange);
                    break;
            }
        }


        public void MultiplyAttributes(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.Rate:
                    Debug.Log($"Fire Rate= {FireRate} * {card.Amount}");
                    FireRate *= (card.Amount*PlayerController.instance.rateEffectAmount);
                    break;
                case CardType.Power:
                    Debug.Log($"Fire Power= {firePower} * {card.Amount}");
                    firePower *= card.Amount*PlayerController.instance.powerEffectAmount;
                    break;
                case CardType.Range:
                    Debug.Log($"Fire Range= {fireRange} * {card.Amount}");
                    fireRange *= card.Amount*PlayerController.instance.rangeEffectAmount;
                    break;
            }
        }


        public void ProcessUnlockUpgrades(CardInfo card)
        {
            switch (card.CardType)
            {
                case CardType.DoubleShot:
                    _currentShotType = DoubleShot;
                    break;
                case CardType.TripleShot:
                    _currentShotType = TripleShot;
                    break;
                case CardType.RicochetProjectile:
                    _currentProjectileBehaviour = ProjectileBehaviour.Ricochet;
                    break;
                case CardType.BoomerangProjectile:
                    _currentProjectileBehaviour = ProjectileBehaviour.Boomerang;
                    break;
                case CardType.IceShot:
                    _currentProjectileModifier = ProjectileModifier.Ice;
                    break;
                case CardType.FireShot:
                    _currentProjectileModifier = ProjectileModifier.Fire;
                    break;
            }
        }


        public void SetShootState(UnitState state)
        {
            switch (state)
            {
                case UnitState.ShootWalk:
                    break;
                case UnitState.Died:
                    FireRate = 0f;
                    break;
            }
        }


        public void SetWeapons(int currentYear)
        {
            ResetWeapons();

            // Return early if year maxed out
            if (currentYear >= _stickManDatas[^1].YearsToUpgrade)
            {
                _stickManDatas[^1].Weapon.SetActive(true);
                _currentProjectile = _stickManDatas[^1].ProjectileType;
                return;
            }

            for (int i = 0; i < _stickManDatas.Length; i++)
            {
                if (currentYear < _stickManDatas[i].YearsToUpgrade)
                {
                    _stickManDatas[i - 1].Weapon.SetActive(true);
                    _currentProjectile = _stickManDatas[i - 1].ProjectileType;
                    break;
                }
            }
        }

        #endregion


        #region PRIVATE METHODS

        private void ResetWeapons()
        {
            foreach (var data in _stickManDatas)
                data.Weapon.SetActive(false);
        }


        // CALLING FROM ANIMATION EVENTS
        private void Shoot()
        {
            _currentShotType?.Invoke();
        }


        private void StandardShot()
        {
            SpawnProjectile(transform.rotation);
        }


        private void DoubleShot()
        {
            float[] angleOffsets = { 10f, -10f };

            for (int i = 0; i < angleOffsets.Length; i++)
            {
                var angle = angleOffsets[i];
                var rotation = Quaternion.Euler(0, angle, 0);

                SpawnProjectile(transform.rotation * rotation);
            }
        }


        private void TripleShot()
        {
            float[] angleOffsets = { 0f, 15f, -15f };

            for (int i = 0; i < angleOffsets.Length; i++)
            {
                var angle = angleOffsets[i];
                var rotation = Quaternion.Euler(0, angle, 0);

                SpawnProjectile(transform.rotation * rotation);
            }
        }


        private void SpawnProjectile(Quaternion rotation)
        {
            // Get projectile from pool
            var projectile = ObjectPooler.Instance.Spawn(
                _currentProjectile.ToString(),
                shootPoint.position,
                rotation);

            // Set projectile data
            var layer = LayerMask.NameToLayer("Projectile");
            var projectileData = new ProjectileData(
                fireRange,
                firePower,
                transform,
                layer,
                _currentProjectileBehaviour,
                _currentProjectileModifier);

            projectile.GetComponent<ProjectileBase>().Init(projectileData);
        }

        #endregion
    }
}