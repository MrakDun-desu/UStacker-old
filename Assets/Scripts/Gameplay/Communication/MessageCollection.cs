using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UStacker.Gameplay.Communication
{
    public class MessageCollection : IEnumerable<Delegate>
    {
        public List<MessageWithPriority> Messages { get; } = new();

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

        public void Add(MessageWithPriority message) => Add(message.Message, message.Priority);

        public void Remove(Delegate message)
        {
            for (var i = 0; i < Messages.Count; i++)
            {
                if (Messages[i].Message == message)
                    Messages.RemoveAt(i);
            }
        }

        public void Clear()
        {
            Messages.Clear();
        }

        public int Count => Messages.Count;

        public Delegate this[int key] => Messages[key].Message;

        public class MessageWithPriority
        {
            public uint Priority { get; }
            public Delegate Message { get; }

            public MessageWithPriority(uint priority, Delegate message)
            {
                Priority = priority;
                Message = message;
            }
        }

        public IEnumerator<Delegate> GetEnumerator() => Messages.Select(message => message.Message).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}