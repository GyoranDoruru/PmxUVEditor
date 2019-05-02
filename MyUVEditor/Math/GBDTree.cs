using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyUVEditor.Math
{
    class GBDTree<Data_t>
    {
        public void AddData(Data_t data)
        {
            var exp = LeafCalculator.Calc(data);

            // step 2
            // step 2.1
            var node = Root;
            var node_i = 0;
            while (!node.IsLeafNode())
            {
                foreach (var cs in node.Slots.Select((value, index) => new {value, index }))
                {
                    if (!exp.IsContained(cs.value.Exp))
                        continue;
                    node = cs.value.Child;
                    node_i = cs.index;
                    break;
                }
            }
            // step 2.2
            GBDTreeNode<Data_t>[] addedLeaves = null; 
            if (node.Slots.Count <= MaxSlotsNum())
            {
                var new_slot = new GBDTreeSlot<Data_t>(node, data);
                new_slot.Exp = exp;
                node.Slots.Add(new_slot);
            }
            else
            {
                addedLeaves = SplitLeafNode(node);
            }

            if(addedLeaves!=null)
            {
                // step 3
                node.ParentNode().Slots.RemoveAt(node_i);
                var new_slot_0 = new GBDTreeSlot<Data_t>(node.ParentNode(), addedLeaves[0]);
                new_slot_0.Exp = SlotCalculator.Calc(new_slot_0);
                node.ParentNode().Slots.Add(new_slot_0);
                var new_slot_1 = new GBDTreeSlot<Data_t>(node.ParentNode(), addedLeaves[1]);
                new_slot_1.Exp = SlotCalculator.Calc(new_slot_1);
                node.ParentNode().Slots.Add(new_slot_1);
                node.ParentNode().Slots.Sort();
                if (node.ParentNode().Slots.Count > MaxSlotsNum())
                    SplitInterNodes();
            }


            while(node.ParentSlot != null)
            {
                bool isUpdated = false;

                foreach(var slot in node.Slots)
                    isUpdated |= node.ParentSlot.MBR.Update(slot.MBR);

                if (!isUpdated)
                    break;
                node = node.ParentNode();
            }

        }

        GBDTreeNode<Data_t>[] SplitLeafNode(GBDTreeNode<Data_t> node) { return null; }
        void SplitInterNodes() { }
        IRegionExpCalculator<Data_t> LeafCalculator;
        IRegionExpCalculator<GBDTreeSlot<Data_t>> SlotCalculator; 
        GBDTreeNode<Data_t> Root;
        static int MaxSlotsNum() { return 3; }
     }

    class GBDTreeNode<Data_t>
    {
        public GBDTreeSlot<Data_t> ParentSlot { get; set; }
        public List<GBDTreeSlot<Data_t>> Slots {get; private set;}
        public bool IsLeafNode() { return Slots.First().IsLeafSlot(); }
        public GBDTreeNode<Data_t> ParentNode()
        {
            if (ParentSlot == null)
                return null;
            return ParentSlot.Parent;
        }
    }

    class GBDTreeSlot<Data_t> : IComparable<GBDTreeSlot<Data_t>>
    {
        public GBDTreeNode<Data_t> Child { get; set; }
        public GBDTreeNode<Data_t> Parent { get; private set; }
        public MBR MBR { get; private set; }
        public Data_t Data { get; private set; }
        public bool IsLeafSlot() { return Child == null; }

        public int CompareTo(GBDTreeSlot<Data_t> other)
        {
            return this.Exp.CompareTo(other.Exp);
        }

        public RegionExpression Exp { get; set; }

        public GBDTreeSlot(GBDTreeNode<Data_t> parent, Data_t data)
        {
            Parent = parent;
            Data = data;
        }

        public GBDTreeSlot(GBDTreeNode<Data_t> parent, GBDTreeNode<Data_t> child)
        {
            Parent = parent;
            Child = child;
        }

    }

    class RegionExpression :IEquatable<RegionExpression>, IComparable<RegionExpression>
    {
        public bool IsContained(RegionExpression other)
        {
            if (other.digits == 0)
                return true;
            if (this.digits == 0)
                return false;
            if (this.digits < other.digits)
                return false;
            var ex = this.value ^ other.value;
            for(int i=0;i<other.digits;++i)
            {
                if ((ex & 1) == 1)
                    return false;
                ex = ex >> 1;
            }
            return true;
        }

        public int CompareTo(RegionExpression other)
        {
            if (this.Equals(other))
                return 0;
            if (this.IsContained(other))
                return -1;
            if (other.IsContained(this))
                return 1;

            var tv = this.value;
            var ex = this.value ^ other.value;
            var end = System.Math.Min(this.digits, other.digits);
            for (int i = 0; i < end; ++i)
            {
                if ((ex & 1) == 1)
                    break;
                ex = ex >> 1;
                tv = tv >> 1;
            }
            return ((tv & 1) == 1) ? 1 : -1;
        }

        public bool Equals(RegionExpression other)
        {
            return this.value == other.value
                && this.digits == other.digits;
        }

        private uint value { get; set; }
        private uint digits { get; set; }
    }

    interface IRegionExpCalculator<Data_t>
    {
        RegionExpression Calc(Data_t data);
    }
}
