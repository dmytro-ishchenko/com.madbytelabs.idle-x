using System.Collections.Generic;

namespace Common.Pattern.BobbleEvent
{
    public class Node
    {
        private readonly List<Node> m_children = new();
        private IBobbleDispatcher m_dispatcher;

        public void SetDispatcher(IBobbleDispatcher dispatcher)
        {
            m_dispatcher = dispatcher;

            foreach (var child in m_children)
                child.SetDispatcher(dispatcher);
        }

        public void AddChild(Node child)
        {
            m_children.Add(child);

            if (m_dispatcher != null)
                child.SetDispatcher(m_dispatcher);
        }

        public void TriggerEvent<T>(T args) where T : IEventArgs => m_dispatcher.Dispatch(args);

        public void RemoveChild(Node child)
        {
            m_children.Remove(child);
        }
    }
}