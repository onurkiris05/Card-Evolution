using System;
using UnityEngine;
using DG.Tweening;

namespace Game.Projectiles
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected float projectileSpeed;
        [SerializeField] protected ProjectileType projectileType;
        [SerializeField] protected LayerMask ricochetLayer;
        [SerializeField] protected float boomerangAngleSharpness;
        [SerializeField] protected float boomerangTravelTime;

        [Space] [Header("Components")]
        [SerializeField] private GameObject iceVFX;
        [SerializeField] private GameObject fireVFX;

        public float Power => _power;
        public ProjectileModifier Modifier => _modifier;

        protected Transform _parent;
        protected ProjectileModifier _modifier;
        protected TrailRenderer _trailRenderer;
        protected Rigidbody _rigidbody;
        protected Vector3 _startPos;
        protected Vector3 _targetPos;
        protected Action _projectileBehaviour;
        protected bool _isBoomeranging;
        protected float _startTime;
        protected float _range;
        protected float _power;
        private bool canKillonBoomerang;
        private float boomerangCounter;

        #region UNITY EVENTS

        protected void Awake() => _trailRenderer = GetComponent<TrailRenderer>();

        protected virtual void OnEnable()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            if (GetComponent<TrailRenderer>())
            {
                GetComponent<TrailRenderer>().enabled = true;
            }
            
        }

        protected void OnDisable() => _trailRenderer.Clear();

        protected virtual void Update()
        {
            _projectileBehaviour?.Invoke();
        }

        #endregion


        #region PUBLIC METHODS

        public virtual void Init(ProjectileData data)
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            if (GetComponent<TrailRenderer>())
            {
                GetComponent<TrailRenderer>().enabled = true;
            }
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            _startTime = Time.time;
            GetComponent<Collider>().enabled = true;
            _range = data.Range;
            _power = data.Power;
            _parent = data.Parent;
            gameObject.layer = data.Layer;
            _startPos = transform.position;
            _targetPos = _startPos + transform.forward * _range;

            _modifier = data.Modifier;
            SetTrail(_modifier);

            switch (data.Behaviour)
            {
                case ProjectileBehaviour.Standard:
                    _projectileBehaviour = Behaviour_Standard;
                    break;
                case ProjectileBehaviour.Ricochet:
                    _projectileBehaviour = Behaviour_Ricochet;
                    break;
                case ProjectileBehaviour.Boomerang:
                    _projectileBehaviour = Behaviour_Boomerang;
                    break;
            }
        }

        public virtual void Kill(bool hitEffect = false,bool onCard=false)
        {
            if (hitEffect)
            {
                if (onCard)
                {
                    VFXSpawner.Instance.PlayVFX("ProjectileHit", transform.position-new Vector3(0,0,1.5f));
                }
                else
                {
                    VFXSpawner.Instance.PlayVFX("ProjectileHit", transform.position);
                }
            }
            else
            {

            }

            _isBoomeranging = false;
            GetComponent<Collider>().enabled = false;
            StopTrail(_modifier);
            ObjectPooler.Instance.ReleasePooledObject(projectileType.ToString(), gameObject);
        }

        #endregion


        #region PROTECTED METHODS
        public void StopTrail(ProjectileModifier modifier)
        {
            _trailRenderer.enabled = false;
            _trailRenderer.emitting = false;
            iceVFX.SetActive(false);
            fireVFX.SetActive(false);
        }
        protected virtual void Behaviour_Standard()
        {
            if (Vector3.Distance(_startPos, transform.position) > _range)
                Kill();
        }

        protected virtual void Behaviour_Ricochet()
        {
            RaycastHit hit;
            // Check if projectile is hitting something with small ray
            if (Physics.Raycast(transform.position, _rigidbody.velocity.normalized,
                    out hit, 0.5f, ricochetLayer))
            {
                _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, hit.normal);
            }

            if (Vector3.Distance(_startPos, transform.position) > _range)
                Kill();
        }

        protected virtual void Behaviour_Boomerang()
        {
            // Flip target pos if its started to return
            if (_isBoomeranging)
            {
                //   _targetPos = new Vector3(_parent.position.x, transform.position.y, _parent.position.z);
                Vector3 targetToGoPosition = new Vector3(_parent.position.x, transform.position.y, _parent.position.z);
                _targetPos = Vector3.Lerp(_targetPos, targetToGoPosition, Time.deltaTime * 2f);
                boomerangCounter += Time.deltaTime;
                if (boomerangCounter > .2f)
                {
                    canKillonBoomerang = true;
                }
            }

            // Calculate center between start and target
            var center = (_startPos + _targetPos) / 2;
            
            // Subtract angle sharpness to make it curved
            var dir = _isBoomeranging ? Vector3.left : Vector3.right;
            center -= dir * boomerangAngleSharpness;

            // Calculate data for Slerp (see Vector3.Slerp documentation)
            var startRelCenter = _startPos - center;
            var finishRelCenter = _targetPos - center;
            var fracComplete = (Time.time - _startTime) / boomerangTravelTime;

            // Finally set curved position with Slerp
            transform.position = Vector3.Slerp(startRelCenter, finishRelCenter, fracComplete);
            transform.position += center;
            //transform.LookAt(_targetPos);
            transform.DOLookAt(_targetPos,.2f);

            // If reached to range, change direction or kill if it reached to end
            if (Vector3.Distance(_startPos, transform.position) > _range - 0.1f && !_isBoomeranging)
            {
                _isBoomeranging = true;
                _startPos = transform.position;
                _startTime = Time.time;
            }
            else if (_isBoomeranging && Vector3.Distance(_targetPos, transform.position) < 0.2f)
            {
                if (canKillonBoomerang)
                {
                    Kill();
                    canKillonBoomerang = false;
                    boomerangCounter = 0;
                }
            }
        }


        protected virtual void SetTrail(ProjectileModifier modifier)
        {
            _trailRenderer.enabled = true;
            _trailRenderer.emitting = modifier == ProjectileModifier.Standard;
            iceVFX.SetActive(modifier == ProjectileModifier.Ice);
            fireVFX.SetActive(modifier == ProjectileModifier.Fire);
        }

        #endregion
    }
}