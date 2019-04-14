using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class ConnectionManager : MonoBehaviour
    {
        public Dictionary<int, List<GameObject>> connectedNodes;

        ConnectionManager()
        {
            connectedNodes = new Dictionary<int, List<GameObject>>();
        }
    }
}
