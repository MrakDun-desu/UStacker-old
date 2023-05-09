
/************************************
ChatBroadcast.cs -- created by Marek Dančo (xdanco00)
*************************************/
using FishNet.Broadcast;

namespace UStacker.Multiplayer.Broadcasts
{
    public readonly struct ChatBroadcast : IBroadcast
    {
        public readonly string Content;
        public readonly int SenderId;

        public ChatBroadcast(string content, int senderId = -1)
        {
            Content = content;
            SenderId = senderId;
        }
    }
}
/************************************
end ChatBroadcast.cs
*************************************/
