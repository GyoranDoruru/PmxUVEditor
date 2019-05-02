using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using PEPlugin.Pmx;


namespace MyUVEditor.Math
{
    public class MBR
    {
        public MBR(Vector2 v1, Vector2 v2) { }
        public MBR(MBR mbr) { }
        public bool IsContains(Vector2 pos) { return false; }
        public bool IsIntersected(MBR mbr) { return false; }
        public float Distance2(Vector2 pos) { return 0.0f; }
        public bool Update(MBR subMBR) { return false; }
        public float Extent(Vector2 pos) { return 0.0f; }
        public float Area() { return 0.0f; }

    }

    public class UVRTreeNode
    {
        public MBR MBR { get; protected set; }
        public List<UVRTreeNode> ChildNodes { get; protected set; }
        public UVRTreeNode Parent { get; protected set; }
        public bool IsContains(Vector2 pos) { return MBR.IsContains(pos); }
        public bool IsIntersected(MBR mbr) { return MBR.IsIntersected(mbr); }
        public float Distance2(Vector2 pos) { return MBR.Distance2(pos); }

        public bool IsLeaf() { return Vertex != null; }

        public IPXVertex Vertex { get; private set; }
        public void SetVertex(IPXVertex v)
        {
            this.Vertex = Vertex;
            this.MBR = new MBR(Vertex.UV, Vertex.UV);
            for (UVRTreeNode node = this; node.Parent != null; node = node.Parent)
                if (!node.Parent.MBR.Update(node.MBR))
                    break;
        }


        public static UVRTreeNode InsertNewNode(UVRTreeNode parent)
        {
            var new_node = new UVRTreeNode();
            if (!parent.Full())
            {
                parent.ChildNodes.Add(new_node);
                new_node.Parent = parent;
            }
            else
            {

            }
            return new_node;
        }

        public bool Full()
        {
            return ChildNodes.Count >= ChildNodes.Capacity;
        }

        bool Balanced()
        {
            return ChildNodes.Count >= ChildNodes.Capacity / 2
                && ChildNodes.Count <= ChildNodes.Capacity;
        }
    }

    public class UVRTree
    {
        public UVRTreeNode Root { get; private set; }

        List<UVRTreeNode> SearchIntersected(MBR mbr)
        {
            var res = new List<UVRTreeNode>();
            var stack = new Stack<UVRTreeNode>();
            stack.Push(Root);
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top.IsLeaf())
                    res.Add(top);
                else
                    foreach (var n in top.ChildNodes)
                        if (n.IsIntersected(mbr))
                            stack.Push(n);
            }
            return res;

        }

        UVRTreeNode SearchNearestLeaf(Vector2 tg)
        {
            var stack = new Stack<UVRTreeNode>();
            stack.Push(Root);
            float min_len2 = float.MaxValue;
            UVRTreeNode res = null;
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top.IsLeaf())
                {
                    var tmp_len2 = top.Distance2(tg);
                    if (tmp_len2 <= min_len2)
                    {
                        min_len2 = tmp_len2;
                        res = top;
                    }
                } else {
                    foreach (var c in top.ChildNodes)
                    {
                        if (c.IsContains(tg))
                            stack.Push(c);
                        else {
                            var tmp_len2 = c.Distance2(tg);
                            if (tmp_len2 <= min_len2)
                            {
                                min_len2 = tmp_len2;
                                stack.Push(c);
                            }
                        }

                    }

                }
            }
            return res;
        }

        UVRTreeNode SelectLeaf(Vector2 tg)
        {
            for(var n = Root;true;)
            {
                if (n.IsLeaf())
                    return n;
                float minExtent = float.MaxValue;
                float minArea = float.MaxValue;
                UVRTreeNode next = null;
                foreach (var c in n.ChildNodes)
                {
                    var cExtent = c.MBR.Extent(tg);
                    if (cExtent > minExtent)
                        continue;

                    if (cExtent < minExtent)
                    {
                        minExtent = cExtent;
                        next = c;
                        continue;
                    }
                    var cArea = c.MBR.Area();
                    if (cArea < minArea)
                    {
                        minArea = cArea;
                        next = c;
                    }
                }
                n = next;
            }
        }

        void Insert(IPXVertex v)
        {
            // l1
            var node = SelectLeaf(v.UV).Parent;
            UVRTreeNode[] new_nodes = { null, null };
            // l2
            if (!node.Full())
            {
                new_nodes[0] = UVRTreeNode.InsertNewNode(node);
                new_nodes[0].SetVertex(v);
            }
            else
            {
                SplitNodes(v, node, new_nodes);
            }

        }

        void SplitNodes( IPXVertex v, UVRTreeNode in_node, UVRTreeNode[] out_nodes)
        {
        }

    }

}
