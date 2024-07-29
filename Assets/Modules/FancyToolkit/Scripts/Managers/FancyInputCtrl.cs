using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FancyToolkit
{
    public class FancyInputCtrl : MonoBehaviour
    {
        public static bool IsMouseOverUI() => EventSystem.current.IsPointerOverGameObject();
    }
}
