using DG.Tweening;
using Game.Collectables;
using Game.Projectiles;
using Game.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Enemy
{
    public class EnemyStickMan : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxHealth;
        [SerializeField] private int prizeAmount;

        [Space] [Header("Shooter Settings")]
        [SerializeField] private bool isShooter;
        [SerializeField] private ProjectileType projectileType;
        [SerializeField] private int projectileRange;
        [SerializeField] private int projectilePower;

        [Space] [Header("Components")]
        [SerializeField] private Money moneyPrefab;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Slider healthBar;
        [SerializeField] private SkinnedMeshRenderer skin;
        [SerializeField] private GameObject iceCondition;
        [SerializeField] private GameObject fireCondition;


        private Collider _collider;
        private Animator _animator;
        private bool _isDead;
        private float _currentHealth;

        bool changingColor;
        Color startColor;
        Renderer rr;
        #region UNITY EVENTS

        private void Awake()
        {
            rr = GetComponentInChildren<SkinnedMeshRenderer>();
            startColor = rr.material.color;
            _collider = GetComponent<Collider>();
            _animator = GetComponent<Animator>();
        }


        private void Start()
        {
            Init();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (_isDead) return;

            if (other.TryGetComponent(out ProjectileBase projectile))
            {
                Taptic.Light();
                _currentHealth -= projectile.Power;
                if (!changingColor)
                {
                    rr.material.color = Color.gray;
                    rr.material.DOColor(startColor, .2f).SetDelay(.1f).OnComplete(delegate {
                        changingColor = false;
	                });
                    changingColor = true;
                }
                SetUI();
                SetCondition(projectile.Modifier);
                projectile.Kill(true);

                if (_currentHealth <= 0)
                {
                    ReleasePrize();
                    Kill();
                }
            }
            else if (other.TryGetComponent(out StickMan stickMan))
            {
                stickMan.Kill();
            }
        }

        #endregion


        #region PRIVATE METHODS

        private void Init()
        {
            moneyPrefab.SetAmount(prizeAmount);
            moneyPrefab.SetState(false);
            moneyPrefab.gameObject.SetActive(false);
            _currentHealth = maxHealth;
            SetUI();

            if (isShooter)
                _animator.SetBool("isShooting", true);
        }


        private void Shoot()
        {
            // Get projectile from pool
            var projectile = ObjectPooler.Instance.Spawn(
                projectileType.ToString(),
                shootPoint.position,
                transform.rotation);

            // Set projectile data
            var layer = LayerMask.NameToLayer("Projectile_Enemy");
            var projectileData = new ProjectileData(
                projectileRange,
                projectilePower,
                transform,
                layer,
                ProjectileBehaviour.Standard,
                ProjectileModifier.Standard);

            projectile.GetComponent<ProjectileBase>().Init(projectileData);
        }


        private void ReleasePrize()
        {
            moneyPrefab.gameObject.SetActive(true);
            moneyPrefab.transform.DOScale(Vector3.zero, 0.5f).From();
            moneyPrefab.transform.DOLocalJump(moneyPrefab.transform.localPosition, 3, 1, 0.5f)
                .OnComplete(() => moneyPrefab.SetState(true));
        }


        private void SetUI()
        {
            healthBar.value = (float)_currentHealth / maxHealth;
        }


        private void SetCondition(ProjectileModifier modifier)
        {
            switch (modifier)
            {
                case ProjectileModifier.Fire:
                    _animator.speed = 0.5f;
                    iceCondition.SetActive(false);
                    fireCondition.SetActive(true);
                    skin.material.color = Color.red;
                    break;

                case ProjectileModifier.Ice:
                    _animator.speed = 0;
                    iceCondition.SetActive(true);
                    fireCondition.SetActive(false);
                    skin.material.color = Color.cyan;
                    break;
            }
        }


        private void ResetCondition()
        {
            _animator.speed = 1;
            iceCondition.SetActive(false);
            fireCondition.SetActive(false);
        }


        private void Kill()
        {
            _isDead = true;
            _collider.enabled = false;
            healthBar.gameObject.SetActive(false);
            skin.material.DOColor(Color.gray, 0.5f);

            ResetCondition();
            _animator.SetBool("isShooting", false);
            _animator.SetTrigger("Die");

            VFXSpawner.Instance.PlayVFX("StickManDeath", transform.position);
        }

        #endregion
    }
}