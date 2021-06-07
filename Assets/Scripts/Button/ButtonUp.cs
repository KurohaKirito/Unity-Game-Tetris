using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Button
{
    public class ButtonUp : MonoBehaviour, IPointerDownHandler//, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            InputUtility.rotate?.Invoke();
        }

        // public void OnPointerUp(PointerEventData eventData)
        // {
        //     
        // }
    }
}