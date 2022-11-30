using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Gameplay.Initialization;
using Blockstacker.GlobalSettings.Changers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Blockstacker.Gameplay
{
    public class CustomReplayOpener : MonoBehaviour
    {
        [SerializeField] private StringReferenceSO _replayGameType;
        [SerializeField] private Button _openReplayButton;
        [SerializeField] private TMP_InputField _replayFilename;
        [SerializeField] private MusicOptionChanger _musicOptionChanger;
        [SerializeField] private StatCountingGroupChanger _statCountingGroupChanger;

        private void Awake()
        {
            _openReplayButton.onClick.AddListener(OpenReplay);
            _musicOptionChanger.GameTypeStr = _replayGameType;
            _statCountingGroupChanger.GameTypeStr = _replayGameType;
        }

        private void OpenReplay()
        {
            if (!GameReplay.TryLoad(_replayFilename.text, out var replay))
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Couldn't load replay!",
                    "Replay either couldn't be found or was in invalid format",
                    AlertType.Error));
                return;
            }
            
            GameInitializer.Replay = replay;
            GameInitializer.GameType = _replayGameType.Value;
            GameInitializer.InitAsReplay = true;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}