using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyToolkit
{
    public class WorldUpdateCtrl : MonoBehaviour
    {
        public static event System.Action<bool> OnStateUpdate;

        public static WorldUpdateCtrl _current;
        public static WorldUpdateCtrl current
        {
            get
            {
                if (!_current)
                {
                    GameObject obj = new GameObject("WorldUpdateCtrl");
                    _current = obj.AddComponent<WorldUpdateCtrl>();
                }

                return _current;
            }
        }

        private List<IWorldUpdateListener> subscribers = new List<IWorldUpdateListener>();
        
        [SerializeField] bool isUpdating;
        public bool IsUpdating
        {
            get => TasksInProgress > 0 || isUpdating;
            set
            {
                isUpdating = value;
                OnStateUpdate?.Invoke(value);
            }
        }

        public int TasksInProgress { get; set; }

        public void AddSubscriber(IWorldUpdateListener subscriber)
        {
            if (!subscribers.Contains(subscriber))
            {
                subscribers.Add(subscriber);
            }
        }

        public void RemoveSubscriber(IWorldUpdateListener subscriber)
        {
            if (subscribers.Contains(subscriber))
            {
                subscribers.Remove(subscriber);
            }
        }

        private void Update()
        {
            if (IsUpdating) return;

            foreach (var subscriber in subscribers)
            {
                if (subscriber.NeedsUpdate)
                {
                    IsUpdating = true;
                    StartCoroutine(UpdateSubscriber(subscriber));
                    break;
                }
            }
        }

        private IEnumerator UpdateSubscriber(IWorldUpdateListener subscriber)
        {
            yield return subscriber.WorldUpdate();
            IsUpdating = false;
        }
    }

    public interface IWorldUpdateListener
    {
        bool NeedsUpdate { get; }
        IEnumerator WorldUpdate();
    }
}