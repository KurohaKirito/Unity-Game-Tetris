using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Button
{
    public class ButtonDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            InputUtility.moveDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputUtility.cancelMoveDown?.Invoke();
        }
    }
}
