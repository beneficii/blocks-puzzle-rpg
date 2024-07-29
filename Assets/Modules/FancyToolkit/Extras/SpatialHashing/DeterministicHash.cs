using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterministicHash<TValue>
{
    private LinkedList<TValue> linkedList = new LinkedList<TValue>();
    private Dictionary<TValue, LinkedListNode<TValue>> dictionary = new Dictionary<TValue, LinkedListNode<TValue>>();

    public void Add(TValue item)
    {
        var node = linkedList.AddLast(item);
        dictionary[item] = node;
    }

    public bool Remove(TValue item)
    {
        if (dictionary.TryGetValue(item, out LinkedListNode<TValue> node))
        {
            linkedList.Remove(node);
            dictionary.Remove(item);
            return true;
        }
        return false;
    }

    public bool Contains(TValue item)
    {
        return dictionary.ContainsKey(item);
    }

    public int Count
    {
        get { return linkedList.Count; }
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        return linkedList.GetEnumerator();
    }
}