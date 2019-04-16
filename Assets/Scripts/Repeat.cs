using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Repeat : ConnectableNode
    {
        public float blockHeight;

        private void Start()
        {
            blockHeight = 2.3f;
        }

        protected override void ShowPreview(Transform other)
        {
            RemovePreview();

            float y = (isAfter) ? -blockHeight : 1;

            preview = Instantiate(other.Find("Preview").gameObject, transform.TransformPoint(0, y, 0), transform.rotation, transform);
            preview.GetComponent<MeshRenderer>().enabled = true;
        }

        protected override bool MoveConnected(List<ConnectableNode> nodes, int offset = 0)
        {
            if (nodes.Count != 0)
            {
                float nextOffset = -blockHeight - offset;
                foreach (ConnectableNode a in nodes)
                {
                    a.transform.position = transform.TransformPoint(0, nextOffset, 0);
                    nextOffset -= (a.GetType() == typeof(Repeat)) ? (a as Repeat).blockHeight : 1;
                }
                return true;
            }

            return false;
        }
    }
}
