using System.Collections;
using System;
using UnityEngine;

namespace FancyToolkit
{
    public abstract class EventWaiterBase : CustomYieldInstruction
    {
        protected int receivedEvents = 0;
        protected readonly int expectedCount;

        public override bool keepWaiting => receivedEvents < expectedCount;

        protected EventWaiterBase(int expectedCount)
        {
            this.expectedCount = expectedCount;
        }

        protected abstract void Cleanup();

        ~EventWaiterBase()
        {
            Cleanup();
        }
    }

    public class EventWaiter : EventWaiterBase
    {
        private Action eventReference;

        public EventWaiter(ref Action ev, int expectedCount = 1) : base(expectedCount)
        {
            if (ev == null)
                throw new ArgumentNullException(nameof(ev), "Event cannot be null.");

            eventReference = ev;
            ev += HandleEvent;
        }

        private void HandleEvent()
        {
            if (++receivedEvents >= expectedCount)
            {
                Cleanup();
            }
        }

        protected override void Cleanup()
        {
            if (eventReference != null)
            {
                eventReference -= HandleEvent;
                eventReference = null;
            }
        }
    }

    public class EventWaiter<T1> : EventWaiterBase
    {
        private Action<T1> eventReference;

        public EventWaiter(ref Action<T1> ev, int expectedCount = 1) : base(expectedCount)
        {
            eventReference = ev;
            ev += HandleEvent;
        }

        private void HandleEvent(T1 t1)
        {
            if (++receivedEvents >= expectedCount)
            {
                Cleanup();
            }
        }

        protected override void Cleanup()
        {
            if (eventReference != null)
            {
                eventReference -= HandleEvent;
                eventReference = null;
            }
        }
    }

    public class EventWaiter<T1, T2> : EventWaiterBase
    {
        private Action<T1, T2> eventReference;

        public EventWaiter(ref Action<T1, T2> ev, int expectedCount = 1) : base(expectedCount)
        {
            if (ev == null)
                throw new ArgumentNullException(nameof(ev), "Event cannot be null.");

            eventReference = ev;
            ev += HandleEvent;
        }

        private void HandleEvent(T1 t1, T2 t2)
        {
            if (++receivedEvents >= expectedCount)
            {
                Cleanup();
            }
        }

        protected override void Cleanup()
        {
            if (eventReference != null)
            {
                eventReference -= HandleEvent;
                eventReference = null;
            }
        }
    }

    public class EventWaiter<T1, T2, T3> : EventWaiterBase
    {
        private Action<T1, T2, T3> eventReference;

        public EventWaiter(ref Action<T1, T2, T3> ev, int expectedCount = 1) : base(expectedCount)
        {
            if (ev == null)
                throw new ArgumentNullException(nameof(ev), "Event cannot be null.");

            eventReference = ev;
            ev += HandleEvent;
        }

        private void HandleEvent(T1 t1, T2 t2, T3 t3)
        {
            if (++receivedEvents >= expectedCount)
            {
                Cleanup();
            }
        }

        protected override void Cleanup()
        {
            if (eventReference != null)
            {
                eventReference -= HandleEvent;
                eventReference = null;
            }
        }
    }
}