using System;
using System.Collections;
using System.Collections.Generic;

namespace FancyToolkit
{
    public class CoroutineEvent
    {
        private List<Func<IEnumerator>> subscribers = new List<Func<IEnumerator>>();

        public void Add(Func<IEnumerator> subscriber)
        {
            if (subscribers.Contains(subscriber)) return;

            subscribers.Add(subscriber);
        }

        public void Remove(Func<IEnumerator> subscriber)
        {
            if (!subscribers.Contains(subscriber)) return;

            subscribers.Remove(subscriber);
        }

        public IEnumerator Invoke()
        {
            foreach (var subscriber in subscribers)
            {
                yield return subscriber();
            }
        }
    }
}
