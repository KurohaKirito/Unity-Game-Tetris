using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Button
{
    public class ButtonRight : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            InputUtility.moveRight?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputUtility.cancelMoveRight?.Invoke();
        }
    }
}