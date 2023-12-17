using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Managers
{
    public class UIManager : StaticInstance<UIManager>
    {
        [Header("Canvases")]
        [SerializeField] private GameObject canvasStartGame;
        [SerializeField] private GameObject canvasGameUI;
        [SerializeField] private GameObject canvasGameEnd;

        [Header("Components")]
        [SerializeField] private GameObject moneyMultiplierUI;
        public GameObject confetti;

        #region UNITY EVENTS

        
        private void OnEnable()
        {
            GameManager.OnAfterStateChanged += AdjustCanvases;
        }
        

        private void OnDisable()
        {
            GameManager.OnAfterStateChanged -= AdjustCanvases;
        }
        

        private void Start()
        {
            AdjustCanvases(GameManager.Instance.State);
            Camera.main.GetComponent<AudioListener>().enabled = false;
            
        }

        #endregion

        #region PUBLIC METHODS
        public void SetMoneyMultiplierUI(bool state)
        {
            moneyMultiplierUI.SetActive(state);
        }
        #endregion

        #region PRIVATE METHODS
        private void AdjustCanvases(GameState state)
        {
            canvasStartGame.SetActive(state == GameState.Start);
            // canvasGameUI.SetActive(state == GameState.Start || state == GameState.Running);
            canvasGameUI.SetActive(true);
            if(state == GameState.End)
            {
                canvasGameEnd.SetActive(state == GameState.End);
                Vector3 scale = canvasGameEnd.transform.localScale;
                canvasGameEnd.transform.localScale = Vector3.zero;
                canvasGameEnd.transform.DOScale(scale, .2f).SetDelay(.2f);
                foreach(ParticleSystem ps in CameraManager.Instance.endingCams.GetComponentsInChildren<ParticleSystem>())
                {
                    ps.Play();
                }
            }
        }

        #endregion
    }
}