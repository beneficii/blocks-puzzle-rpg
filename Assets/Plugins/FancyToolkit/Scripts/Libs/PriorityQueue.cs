using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class PriorityQueue<TValue> where TValue : System.IComparable<TValue>
{
    LinkedList<TValue> list;

    public int Count => list.Count;

    public PriorityQueue()
    {
        list = new LinkedList<TValue>();
    }

    public void Add(TValue value)
    {
        var node = list.First;
        while(node != null)
        {
            if(value.CompareTo(node.Value) <= 0)
            {
                list.AddBefore(node, value);
                return;
            }

            node = node.Next;
        }

        list.AddLast(value);
    }

    public void Add(PriorityQueue<TValue> other)
    {
        var node = list.First;
        var otherNode = other.list.First;

        while(node != null)
        {
            while(otherNode != null && otherNode.Value.CompareTo(node.Value) <= 0)
            {
                list.AddBefore(node, otherNode.Value);
                otherNode = otherNode.Next;
                if(otherNode == null) break;
            }

            node = node.Next;
        }

        while(otherNode != null)
        {
            list.AddLast(otherNode.Value);
            otherNode = otherNode.Next;
        }
    }

    public void AddAll(PriorityQueue<TValue> other) => Add(other);

    public TValue DeleteMin()
    {
        var value = list.First.Value;
        list.RemoveFirst();

        return value;
    }
}