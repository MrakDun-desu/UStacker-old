
/************************************
MessageCollection.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UStacker.Gameplay.Communication
{
    public class MessageCollection : IEnumerable<Delegate>
    {
        private List<MessageWithPriority> Messages { get; } = new();

        public IEnumerator<Delegate> GetEnumerator()
        {
            return Messages.Select(message => message.Message).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Delegate message, uint priority)
        {
            var newMessage = new MessageWithPriority(priority, message);
            for (var i = 0; i < Messages.Count; i++)
            {
                if (newMessage.Priority <= Messages[i].Priority) continue;

                Messages.Insert(i, newMessage);
                return;
            }

            Messages.Add(newMessage);
        }

        public void Remove(Delegate message)
        {
            for (var i = 0; i < Messages.Count; i++)
                if (Messages[i].Message == message)
                    Messages.RemoveAt(i);
        }

        private class MessageWithPriority
        {
            public MessageWithPriority(uint priority, Delegate message)
            {
                Priority = priority;
                Message = message;
            }

            public uint Priority { get; }
            public Delegate Message { get; }
        }
    }
}
/************************************
end MessageCollection.cs
*************************************/
