
/************************************
ChatMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.Multiplayer.LobbyUi
{
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField] private TMP_Text _senderNameLabel;
        [SerializeField] private TMP_Text _messageContent;
        [SerializeField] private RectTransform _selfTransform;

        public void Init(string senderName, string content)
        {
            _senderNameLabel.text = senderName;
            _messageContent.text = content;

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) _selfTransform.parent);

            _messageContent.ForceMeshUpdate();
            var sizeDelta = _messageContent.rectTransform.sizeDelta;
            sizeDelta = new Vector2(sizeDelta.x,
                _messageContent.preferredHeight);
            _messageContent.rectTransform.sizeDelta = sizeDelta;

            _selfTransform.sizeDelta = sizeDelta;
        }
    }
}
/************************************
end ChatMessage.cs
*************************************/
