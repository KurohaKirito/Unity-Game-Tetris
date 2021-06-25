using System.Diagnostics;
using UnityEngine;

namespace Utility
{
    public static class AndroidUtility
    {
        [Conditional("UNITY_ANDROID")]
        public static void Vibrant()
        {
            Handheld.Vibrate();
        }
    }
}