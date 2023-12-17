using UnityEngine;

namespace Game.Unit
{
    public class StickManZoneHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Material indicateMat;
        [SerializeField] private SkinnedMeshRenderer bodyRenderer;

        public bool IsIndicated => _isIndicated;

        private Material _originalMat;
        private bool _isIndicated;

        #region UNITY EVENTS

        private void OnEnable()
        {
            _originalMat = bodyRenderer.material;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Indicator") && !_isIndicated)
            {
                SetIndicated(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Indicator") && _isIndicated)
            {
                SetIndicated(false);
            }
        }

        #endregion

        #region PUBLIC METHODS

        public void SetIndicated(bool state)
        {
            _isIndicated = state;
            bodyRenderer.material = state ? indicateMat : _originalMat;
        }

        #endregion
    }
}