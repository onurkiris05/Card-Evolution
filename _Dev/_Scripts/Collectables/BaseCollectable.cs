using TMPro;
using UnityEngine;

namespace Game.Collectables
{
    public abstract class BaseCollectable : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] protected int hitCount;

        [Space] [Header("Components")]
        [SerializeField] protected Money moneyPrize;
        [SerializeField] protected Transform collectablePos;
        [SerializeField] protected TextMeshPro hitCountText;

        protected bool _isCollected;


        protected abstract void OnTriggerEnter(Collider other);
    }
}