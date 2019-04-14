using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ToggleDigital : ConnectableNode
    {
        public int portN;
        public bool isOn;

        public override void PerformAction()
        {
            if (isOn)
            {
                Debug.Log($"{portN} set on");
            }
            else
            {
                Debug.Log($"{portN} set off");
            }

            base.PerformAction();
        }
    }
}
