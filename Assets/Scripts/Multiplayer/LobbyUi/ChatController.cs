using FishNet;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UStacker.Common.Alerts;
using UStacker.Common.Extensions;
using UStacker.Multiplayer.Broadcasts;

namespace UStacker.Multiplayer.LobbyUi
{
    public class ChatController : MonoBehaviour
    {
        [SerializeField] private ChatMessage _chatMessagePrefab;
        [SerializeField] private RectTransform _chatListParent;
        [SerializeField] private ScrollRect _chatScrollRect;
        [SerializeField] private TMP_InputField _chatField;

        private void Awake()
        {
            _chatField.OnEnter(OnMessageSent);
            InstanceFinder.ClientManager.RegisterBroadcast<ChatBroadcast>(OnChatBroadcast);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatBroadcast>(OnChatBroadcastServer);
        }

        private void OnDestroy()
        {
            InstanceFinder.ClientManager.UnregisterBroadcast<ChatBroadcast>(OnChatBroadcast);
            InstanceFinder.ServerManager.UnregisterBroadcast<ChatBroadcast>(OnChatBroadcastServer);
        }

        private void OnChatBroadcast(ChatBroadcast broadcast)
        {
            var newChatMessage = Instantiate(_chatMessagePrefab, _chatListParent);
            newChatMessage.Init(broadcast.SenderName, broadcast.Content);
            _chatScrollRect.verticalScrollbar.SetValueWithoutNotify(0);
        }

        private void OnMessageSent(string message)
        {
            if (message.Length > 250)
            {
                message = message[..250];
                AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Chat message shortened",
                    "Your message exceeded the limit of 250 characters, so it was shortened.", 
                    AlertType.Info));
            }

            InstanceFinder.ClientManager.Broadcast(new ChatBroadcast(message, string.Empty));
            _chatField.SetTextWithoutNotify(string.Empty);
            _chatField.ActivateInputField();
        }

        [Server]
        private static void OnChatBroadcastServer(NetworkConnection senderConnection, ChatBroadcast chatBroadcast)
        {
            if (!Player.ConnectedPlayers.TryGetValue(senderConnection.ClientId, out var sender))
                return;

            var actualBroadcast = new ChatBroadcast(chatBroadcast.Content, sender.DisplayName);
            InstanceFinder.ServerManager.Broadcast(actualBroadcast);
        }
    }
}