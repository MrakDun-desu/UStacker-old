using FishNet.Broadcast;

namespace UStacker.Multiplayer.Broadcasts
{
    public readonly struct ChatBroadcast : IBroadcast
    {
        public readonly string Content;
        public readonly string SenderName;

        public ChatBroadcast(string content, string senderName)
        {
            Content = content;
            SenderName = senderName;
        }
    }
}