using FishNet;
using FishNet.Connection;
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
        }

        private void OnEnable()
        {
            InstanceFinder.ClientManager.RegisterBroadcast<ChatBroadcast>(OnChatBroadcastClient);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatBroadcast>(OnChatBroadcastServer);
        }

        private void OnDisable()
        {
            if (InstanceFinder.ClientManager != null)
                InstanceFinder.ClientManager.UnregisterBroadcast<ChatBroadcast>(OnChatBroadcastClient);
            
            if (InstanceFinder.ServerManager != null)
                InstanceFinder.ServerManager.UnregisterBroadcast<ChatBroadcast>(OnChatBroadcastServer);
        }

        private void OnChatBroadcastClient(ChatBroadcast broadcast)
        {
            if (!Player.ConnectedPlayers.TryGetValue(broadcast.SenderId, out var sender))
                return;
            
            var newChatMessage = Instantiate(_chatMessagePrefab, _chatListParent);
            newChatMessage.Init(sender.DisplayName, broadcast.Content);
            _chatScrollRect.verticalScrollbar.SetValueWithoutNotify(0);
        }

        private void OnMessageSent(string message)
        {
            if (message.Length > 250)
            {
                message = message[..250];
                AlertDisplayer.ShowAlert(new Alert(
                    "Chat message shortened",
                    "Your message exceeded the limit of 250 characters.", 
                    AlertType.Info));
            }

            InstanceFinder.ClientManager.Broadcast(new ChatBroadcast(message));
            _chatField.SetTextWithoutNotify(string.Empty);
            _chatField.ActivateInputField();
        }

        private static void OnChatBroadcastServer(NetworkConnection senderConnection, ChatBroadcast chatBroadcast)
        {
            if (!Player.ConnectedPlayers.ContainsKey(senderConnection.ClientId))
                return;

            var actualBroadcast = new ChatBroadcast(chatBroadcast.Content, senderConnection.ClientId);
            InstanceFinder.ServerManager.Broadcast(actualBroadcast);
        }
    }
}