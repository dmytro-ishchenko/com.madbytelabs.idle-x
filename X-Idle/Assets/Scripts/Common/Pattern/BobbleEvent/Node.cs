using System.Collections.Generic;

namespace Common.Pattern.BobbleEvent
{
    public class Node
    {
        private readonly List<Node> m_children = new();
        private IRootEventHandler m_rootHandler;
        public IReadOnlyList<Node> Children => m_children;


        public void SetRootHandler(IRootEventHandler rootHandler)
        {
            m_rootHandler = rootHandler;

            foreach (var child in m_children)
                child.SetRootHandler(rootHandler);
        }

        public void AddChild(Node child)
        {
            m_children.Add(child);

            if (m_rootHandler != null)
                child.SetRootHandler(m_rootHandler);
        }

        public void TriggerEvent(string eventType, IEventArgs eventArgs)
        {
            m_rootHandler?.Handle(new NodeEvent(eventType, eventArgs));
        }
    }
}