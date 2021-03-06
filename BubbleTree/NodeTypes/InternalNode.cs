﻿using System.Collections.Generic;

namespace BubbleTreeComponent.NodeTypes
{
    public class InternalNode<T> : BaseNode<T>
    {
        public BaseNode<T> Parent;
        public List<BaseNode<T>> Children { get; set; }
    }
}