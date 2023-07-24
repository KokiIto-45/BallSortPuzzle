namespace MyApp.MyBSP
{
    using System.Collections.Generic;
    using UnityEngine.Events;
    public class EventManager
    {
        public List<UnityEvent> events;
        public EventManager()
        {

        }
        public EventManager(UnityEvent e) : this(new List<UnityEvent>() { e })
        {

        }
        public EventManager(List<UnityEvent> events)
        {
            this.events = events;
        }

        public EventManager AddEvent(UnityEvent e)
        {
            if (e != null)
            {
                if (events == null)
                {
                    events = new List<UnityEvent>();
                }
                events.Add(e);
            }
            return this;
        }
        public void Invoke()
        {
            if (events == null) return;
            foreach (UnityEvent e in events)
            {
                if (e == null) continue;
                e.Invoke();
            }
        }
    }
}