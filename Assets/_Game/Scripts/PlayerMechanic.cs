using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts
{
    public class PlayerMechanic : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        public event Action<CellEntity> DropBomb;
        
        private void OnEnable()
        {
            InputManager.Instance.PointerDown += HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd += HandleOnPointerEnd;
        }

        private void OnDisable()
        {
            InputManager.Instance.PointerDown -= HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd -= HandleOnPointerEnd;
        }
        
        private void HandleOnPointerDown(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
            
            RaycastHit hit;
            Vector3 screenToWorldPoint = gameManager.mainCamera.
                ScreenToWorldPoint(eventData.position.x * Vector3.right + eventData.position.y * Vector3.up - 10 *Vector3.forward);
            if (Physics.Raycast(screenToWorldPoint, Vector3.forward, out hit, 100f))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.GetComponent<CellEntity>() != null)
                    {
                        DropBomb?.Invoke(hit.collider.GetComponent<CellEntity>());
                    }
                }
            }
        }

        private void HandleOnPointerDrag(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
        }

        private void HandleOnPointerEnd(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
        }
    }
}