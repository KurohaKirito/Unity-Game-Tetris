using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Button
{
    public class ButtonUp : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            InputUtility.rotate?.Invoke();
        }
    }
}