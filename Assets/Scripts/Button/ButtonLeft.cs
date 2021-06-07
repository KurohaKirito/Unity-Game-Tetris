using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Button
{
    public class ButtonLeft : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            InputUtility.moveLeft?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputUtility.cancelMoveLeft?.Invoke();
        }
    }
}