using System;
using System.Collections.Generic;
using System.Text;
namespace UnityScratch.core
{
    //左子女右兄弟类型的二叉树
    public class BinaryTree<T>
    {
        public BinaryTree(T data)
        {
            Data = data;
        }
        public T Data { get; protected internal set; }
        public BinaryTree<T> Next { get; protected internal set; }
        public BinaryTree<T> Prev { get; protected internal set; }
        public BinaryTree<T> Child { get; protected internal set; }
        public BinaryTree<T> Parent { get; protected internal set; }
        public void Traverse(Action<T> onTraverse)
        {
            onTraverse(this.Data);
            if (Child != null)
            {
                Child.Traverse(onTraverse);
            }
            if (Next != null)
            {
                Next.Traverse(onTraverse);
            }
        }
        public BinaryTree<T> Traverse(Func<BinaryTree<T>, int, int, object, BinaryTree<T>> onTraverse, int depth, int offset, object param)
        {
            BinaryTree<T> result = null;
            result = onTraverse(this, depth, offset, param);
            if (result != null)
                return result;
            int childHeight = 0;
            if (Child != null)
            {
                childHeight = Child.size();
                result = Child.Traverse(onTraverse, depth + 1, offset + 1, param);
                if (result != null)
                    return result;
            }
            if (Next != null)
            {
                result = Next.Traverse(onTraverse, depth, offset + 1 + childHeight, param);
                if (result != null)
                    return result;
            }
            return null;
        }
        public void AddChild(T data)
        {
            AddChild(new BinaryTree<T>(data));
        }
        //添加到子节点
        public void AddChild(BinaryTree<T> add)
        {
            BinaryTree<T> oldChild = Child;
            add.Unlink();
            if (oldChild != null)
                oldChild.Unlink();

            //连接 this 和 add
            this.Child = add;
            add.Parent = this;
            if (oldChild != null)
            {
                //连接 add 和 oldChild
                var lastAdd = add.NextestNode();
                lastAdd.Next = oldChild;
                oldChild.Prev = lastAdd;
            }
        }
        public void AddNext(T data)
        {
            AddNext(new BinaryTree<T>(data));
        }

        public void AddNext(BinaryTree<T> add)
        {
            BinaryTree<T> oldNext = Next;
            add.Unlink();
            if (oldNext != null)
                oldNext.Unlink();

            //连接this和add
            this.Next = add;
            add.Prev = this;

            if (oldNext != null)
            {
                //连接add和oldNext
                var lastAdd = add.NextestNode();
                lastAdd.Next = oldNext;
                oldNext.Prev = lastAdd;
            }
        }
        /// 把自己及所有孩子及后面的兄弟节点从父节点或上一个兄弟节点断开连接
        public void Unlink()
        {
            if (Parent != null)
            {
                Parent.Child = null;
                Parent = null;
            }
            if (Prev != null)
            {
                Prev.Next = null;
                Prev = null;
            }
        }
        public BinaryTree<T> NextestNode()
        {
            var node = this;
            while (node.Next != null)
            {
                node = node.Next;
            }
            return node;
        }

        #region private


        //返回自身及自身的子节点及后面的兄弟节点的个数
        private int size()
        {
            int count = 1;
            var node = this.Child;
            if (node != null)
            {
                count += node.size();
            }
            node = this.Next;
            if (node != null)
            {
                count += node.size();
            }
            return count;
        }
        #endregion
    }
}
