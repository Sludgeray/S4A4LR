using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Delay : ConnectableNode
    {
        public int delay;

        public override void PerformAction()
        {
            StartCoroutine(_Delay());
        }

        private IEnumerator _Delay()
        {
            yield return new WaitForSeconds(delay * 0.001f);
            base.PerformAction();
        }
    }
}
