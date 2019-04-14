//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;

//namespace Assets.Scripts
//{
//    public abstract class CN_Backup : MonoBehaviour
//    {
//        static bool preventDisconnection;

//        bool dragOrigin;
//        bool mouseDrag;
//        bool connectingAfter;
//        bool connectingBetween;
//        GameObject preview;
//        public Transform before;
//        public List<GameObject> connectedNodes;

//        private void Awake()
//        {
//            connectedNodes = new List<GameObject>();
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if (mouseDrag)
//            {
//                if (other.tag == "UpperConnector")  //if this node connects before some other node
//                {
//                    connectingAfter = false;
//                    ShowPreview(true, other.transform);
//                }
//                else if (other.tag == "LowerConnector" && !before && dragOrigin)  //if this node connects after or in between some other nodes
//                {
//                    connectingAfter = true;
//                    ShowPreview(false, other.transform);
//                    connectingBetween = preventDisconnection = MoveConnected(other.transform.parent.GetComponent<ConnectableNode>().connectedNodes, other.transform.parent, 1);
//                }
//            }
//        }

//        private void OnTriggerStay(Collider other)
//        {
//            if (preview == null && mouseDrag)
//            {
//                if (other.tag == "LowerConnector" && dragOrigin)  //if this node connects after some other node
//                {
//                    connectingAfter = true;
//                    ShowPreview(false, other.transform);
//                }
//            }
//        }

//        private void OnTriggerExit(Collider other)
//        {
//            if (other.tag == "UpperConnector" || other.tag == "LowerConnector")
//            {
//                RemovePreview();
//            }

//            if (connectedNodes.Count != 0)
//            {
//                if (connectedNodes[0] == other.gameObject && !preventDisconnection)
//                {
//                    UpdateConnections(other.gameObject);
//                    other.GetComponent<ConnectableNode>().before = null;
//                    other.transform.Find("UpperTrigger").GetComponent<Collider>().enabled = true;
//                }
//            }
//        }


//        public virtual void PerformAction()
//        {

//        }

//        public void UpdateConnections(List<GameObject> newConnections)
//        {
//            if (before)
//            {
//                before.GetComponent<ConnectableNode>().connectedNodes.AddRange(newConnections);
//                before.GetComponent<ConnectableNode>().UpdateConnections(newConnections);
//            }
//        }

//        public void UpdateConnections(GameObject disconnected)
//        {
//            int from = connectedNodes.IndexOf(disconnected);
//            connectedNodes.RemoveRange(from, connectedNodes.Count - from);
//            if (before)
//            {
//                before.GetComponent<ConnectableNode>().UpdateConnections(disconnected);
//            }
//        }

//        public void UpdateConnections(List<GameObject> newConnections, GameObject after)
//        {
//            if (before)
//            {
//                int from = before.GetComponent<ConnectableNode>().connectedNodes.IndexOf(after);
//                before.GetComponent<ConnectableNode>().connectedNodes.InsertRange(from, newConnections);
//                before.GetComponent<ConnectableNode>().UpdateConnections(newConnections, after);
//            }
//        }

//        private void ShowPreview(bool isBefore, Transform other)
//        {
//            RemovePreview();

//            int y = (isBefore) ? 1 : -1;

//            preview = Instantiate(
//                     transform.Find("Preview").gameObject, other.transform.parent.TransformPoint(0, y, 0),
//                     other.transform.parent.rotation, other.transform.parent);
//            preview.GetComponent<MeshRenderer>().enabled = true;
//        }

//        private void RemovePreview()
//        {
//            if (preview)
//            {
//                MoveConnected(preview.transform.parent.GetComponent<ConnectableNode>().connectedNodes, preview.transform.parent);
//                Destroy(preview);
//            }
//        }

//        bool MoveConnected(List<GameObject> nodes, Transform parent, int offset = 0)
//        {
//            if (nodes.Count != 0)
//            {
//                int i = 0 - offset;
//                foreach (GameObject a in nodes)
//                {
//                    a.transform.position = parent.TransformPoint(0, --i, 0);
//                }
//                return true;
//            }

//            return false;
//        }

//        void MoveConnected(Transform parent, int offset = 1)
//        {
//            if (before)
//            {
//                before.transform.position = parent.TransformPoint(0, ++offset, 0);
//                before.GetComponent<ConnectableNode>().MoveConnected(parent, offset);
//            }
//        }

//        #region MouseDrag
//        Vector3 ScreenPoint;
//        Vector3 Offset;

//        private void OnMouseDown()
//        {
//            ScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
//            Offset = gameObject.transform.position
//                - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z));
//            mouseDrag = true;
//            dragOrigin = true;
//            if (connectedNodes.Count != 0)
//            {
//                connectedNodes[connectedNodes.Count - 1].GetComponent<ConnectableNode>().mouseDrag = true;
//            }


//            transform.Find("LowerTrigger").GetComponent<Collider>().enabled = false;
//            Transform upperTrigger = transform.Find("UpperTrigger");
//            if (upperTrigger)
//            {
//                upperTrigger.GetComponent<Collider>().enabled = false;
//            }
//        }

//        private void OnMouseDrag()
//        {
//            Vector3 CursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z);
//            Vector3 CursorPosition = Camera.main.ScreenToWorldPoint(CursorPoint) + Offset;
//            transform.position = CursorPosition;
//            MoveConnected(connectedNodes, transform);
//        }

//        public void OnMouseUp()
//        {
//            mouseDrag = false;
//            dragOrigin = false;
//            transform.Find("LowerTrigger").GetComponent<Collider>().enabled = true;
//            Transform upperTrigger = transform.Find("UpperTrigger");
//            if (upperTrigger)
//            {
//                upperTrigger.GetComponent<Collider>().enabled = true;
//            }

//            if (connectedNodes.Count != 0 && !connectingAfter)
//            {
//                connectedNodes[connectedNodes.Count - 1].GetComponent<ConnectableNode>().OnMouseUp();
//                return;
//            }

//            if (preview != null)
//            {
//                transform.position = preview.transform.position;
//                transform.rotation = preview.transform.rotation;
//                if (connectingAfter && !preview.transform.parent.GetComponent<ConnectableNode>().connectedNodes.Contains(gameObject))
//                {
//                    before = preview.transform.parent;
//                    List<GameObject> newConnections = new List<GameObject> { gameObject };
//                    newConnections.AddRange(connectedNodes);
//                    if (connectingBetween)
//                    {
//                        before.GetComponent<ConnectableNode>().connectedNodes[0].GetComponent<ConnectableNode>().before = transform;
//                        connectedNodes.AddRange(before.GetComponent<ConnectableNode>().connectedNodes);
//                        UpdateConnections(newConnections, before.GetComponent<ConnectableNode>().connectedNodes[0]);
//                    }
//                    else
//                    {
//                        UpdateConnections(newConnections);
//                    }
//                    upperTrigger.GetComponent<Collider>().enabled = false;
//                }
//                else if (!connectingAfter && !connectedNodes.Contains(preview.transform.parent.gameObject))
//                {
//                    preview.transform.parent.GetComponent<ConnectableNode>().before = transform;
//                    connectedNodes.Add(preview.transform.parent.gameObject);
//                    connectedNodes.AddRange(preview.transform.parent.GetComponent<ConnectableNode>().connectedNodes);
//                    UpdateConnections(connectedNodes);
//                    MoveConnected(preview.transform.parent);

//                    preview.transform.parent.Find("UpperTrigger").GetComponent<Collider>().enabled = false;
//                }
//                RemovePreview();
//            }
//        }
//        #endregion MouseDrag
//    }
//}
