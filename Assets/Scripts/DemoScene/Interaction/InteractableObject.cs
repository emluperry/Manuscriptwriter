using System;
using UnityEngine;

namespace Demo.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private InteractionIcon interactionIcon;
        [SerializeField] private SpriteRenderer iconRenderer;

        public Action OnInteract;

        protected virtual void Awake()
        {
            iconRenderer.sprite = interactionIcon.InteractionImage;
            iconRenderer.enabled = false;
        }
        
        public virtual void StartInteract()
        {
            Debug.Log($"Interacted with {this.gameObject.name}.");
            this.OnInteract?.Invoke();
        }

        public virtual void OnOverlap()
        {
            if(!iconRenderer)
            {
                return;
            }

            iconRenderer.enabled = true;
        }

        public virtual void StopOverlap()
        {
            if (!iconRenderer)
            {
                return;
            }

            iconRenderer.enabled = false;
        }
    }
}
