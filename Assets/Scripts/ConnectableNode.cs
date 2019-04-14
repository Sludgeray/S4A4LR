﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class ConnectableNode : MonoBehaviour
    {
        static GameObject preview;
        static ConnectableNode previewOwner;
        static bool isAfter;
        static bool isBetween;
       

        public List<ConnectableNode> connectedNodes;
        public ConnectableNode before;
        public bool dragTarget;
        public bool dragInvolved;
       
        private void Awake()
        {
            connectedNodes = new List<ConnectableNode>();
        }

        public void OnChildTriggerEnter(ConnectableNode other, Transform child)
        {
            if (child.name == "LowerTrigger" && other.tag != "Event")
            {
                if(connectedNodes.FirstOrDefault() != other)
                {
                    if(!other.before)
                    {
                        isAfter = true;
                        ShowPreview(other.transform);
                        previewOwner = this;
                        isBetween = MoveConnected(connectedNodes, transform, 1);
                    }
                }
            }
            else if (child.name == "UpperTrigger")
            {
                if (!other.connectedNodes.Any())
                {
                    isAfter = isBetween = false;
                    ShowPreview(other.transform);
                    previewOwner = this;
                }
            }
        }

        public void OnChildTriggerStay(ConnectableNode other, Transform child)
        {
            if(!preview && other.GetComponent<ConnectableNode>().dragTarget && other.tag != "Event")
            {
                isAfter = child.name == "LowerTrigger";
                ShowPreview(other.transform);
                previewOwner = this;
            }
        }

        public void OnChildTriggerExit(ConnectableNode other, Transform child)
        {
            
            if(other.dragInvolved)
            {
                RemovePreview();
                if (connectedNodes.FirstOrDefault() == other)
                {
                    other.before = null;
                    UpdateConnections(other);
                }
            }
        }
        
        public virtual void PerformAction()
        {
            
        }

        public void UpdateConnections(List<ConnectableNode> newConnections)
        {
            connectedNodes.AddRange(newConnections);
            if (before)
            {
                before.UpdateConnections(newConnections);
            }
        }

        public void UpdateConnections(ConnectableNode disconnected)
        {
            int from = connectedNodes.IndexOf(disconnected);
            connectedNodes.RemoveRange(from, connectedNodes.Count - from);
            if (before)
            {
                before.UpdateConnections(disconnected);
            }
        }

        public void UpdateConnections(List<ConnectableNode> newConnections, ConnectableNode after)
        {
            int from = connectedNodes.IndexOf(after);
            connectedNodes.InsertRange(from, newConnections);
            if (before)
            {
                before.UpdateConnections(newConnections, after);
            }
        }

        private void ShowPreview(Transform other)
        {
            RemovePreview();

            int y = (isAfter) ? -1 : 1;

            preview = Instantiate(other.Find("Preview").gameObject, transform.TransformPoint(0, y, 0), transform.rotation, transform);
            preview.GetComponent<MeshRenderer>().enabled = true;
        }

        private void RemovePreview()
        {
            if (preview)
            {
                MoveConnected(connectedNodes, transform);
                Destroy(preview);
                preview = null;
                previewOwner = null;
            }
        }

        bool MoveConnected(List<ConnectableNode> nodes, Transform parent, int offset = 0)
        {
            if (nodes.Count != 0)
            {
                int i = 0 - offset;
                foreach (ConnectableNode a in nodes)
                {
                    a.transform.position = parent.TransformPoint(0, --i, 0);
                }
                return true;
            }
    
            return false;
        }

        void MoveConnected(Transform parent, int offset = 1)
        {
            if(before)
            {
                before.transform.position = parent.TransformPoint(0, ++offset, 0);
                before.MoveConnected(parent, offset);
            }
        }

        #region MouseDrag
        Vector3 ScreenPoint;
        Vector3 Offset;

        private void OnMouseDown()
        {
            ScreenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            Offset = gameObject.transform.position
                - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z));

            dragTarget = dragInvolved = true;
            transform.Find("LowerTrigger").GetComponent<Collider>().enabled = false;
            Transform upperTrigger = transform.Find("UpperTrigger");
            if (upperTrigger)
            {
                upperTrigger.GetComponent<Collider>().enabled = false;
            }
            foreach (ConnectableNode a in connectedNodes)
            {
                a.transform.Find("LowerTrigger").GetComponent<Collider>().enabled = false;
                a.dragInvolved = true;
            }
        }

        private void OnMouseDrag()
        {
            Vector3 CursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, ScreenPoint.z);
            Vector3 CursorPosition = Camera.main.ScreenToWorldPoint(CursorPoint) + Offset;
            transform.position = CursorPosition;

            MoveConnected(connectedNodes, transform);
        }

        public void OnMouseUp()
        {
            dragTarget = dragInvolved = false;

            if(preview)
            {
                if(!isAfter)
                {
                    ConnectableNode first = connectedNodes.Any() ? connectedNodes.LastOrDefault() : this;
                    List<ConnectableNode> newConnections = new List<ConnectableNode> { previewOwner };
                    newConnections.AddRange(previewOwner.connectedNodes);
                    first.UpdateConnections(newConnections);
                    previewOwner.before = this;
                    previewOwner.transform.Find("UpperTrigger").GetComponent<Collider>().enabled = false; 
                }
                else
                {
                    transform.position = preview.transform.position;
                    transform.rotation = preview.transform.rotation;
                    before = previewOwner;
                    if (before.connectedNodes.FirstOrDefault() != this)
                    {   
                        List<ConnectableNode> newConnections = new List<ConnectableNode> { this };
                        newConnections.AddRange(connectedNodes);
                        if (isBetween)
                        {
                            before.connectedNodes[0].before = this;
                            connectedNodes.AddRange(before.connectedNodes);
                            before.UpdateConnections(newConnections, before.connectedNodes[0]);
                        }
                        else
                        {
                            before.UpdateConnections(newConnections);
                        }
                    }
                }
                RemovePreview();
            }

            transform.Find("LowerTrigger").GetComponent<Collider>().enabled = true;
            Transform upperTrigger = transform.Find("UpperTrigger");
            if (upperTrigger && !before)
            {
                upperTrigger.GetComponent<Collider>().enabled = true;
            }
            foreach (ConnectableNode a in connectedNodes)
            {
                a.transform.Find("LowerTrigger").GetComponent<Collider>().enabled = true;
                a.dragInvolved = false;
            }
        }
        #endregion MouseDrag
    }
}
