using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts
{
    public class InputManager : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        public RectTransform draggingPlane;

        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> PointerDrag;
        public event Action<PointerEventData> PointerEnd;
        
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            PointerDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PointerEnd?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(eventData);
        }
    }
}