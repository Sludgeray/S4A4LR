using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class NodeConnect : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Connector")
            {
                gameObject.GetComponentInParent<ConnectableNode>().OnChildTriggerEnter(other.GetComponent<ConnectableNode>(), transform);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag != "Connector")
            {
                gameObject.GetComponentInParent<ConnectableNode>().OnChildTriggerStay(other.GetComponent<ConnectableNode>(), transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag != "Connector")
            {
                gameObject.GetComponentInParent<ConnectableNode>().OnChildTriggerExit(other.GetComponent<ConnectableNode>(), transform);
            }
        }
    }
}