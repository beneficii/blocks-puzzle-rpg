using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace FancyToolkit
{
    public class FancyUpdateCtrl : MonoBehaviour
    {
        static FancyUpdateCtrl _current;
        public static FancyUpdateCtrl current
        {
            get
            {
                if (!_current)
                {
                    var obj = new GameObject("FancyUpdateCtrl");
                    _current = obj.AddComponent<FancyUpdateCtrl>();
                }

                return _current;
            }
        }

        int blockInput;
        
        public void AddInputBlock()
        {
            blockInput++;
        } 

        public void RemoveInputBlock()
        {
            blockInput--;
            Assert.IsTrue(blockInput >= 0);
        }

        public bool GetInputBlock() => blockInput > 0;
    }
}