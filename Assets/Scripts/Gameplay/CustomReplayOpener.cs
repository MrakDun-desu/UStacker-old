using System.Threading.Tasks;
using UStacker.Common;
using UStacker.Common.Alerts;
using UStacker.Gameplay.Initialization;
using UStacker.GlobalSettings.Changers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UStacker.Gameplay
{
    public class CustomReplayOpener : MonoBehaviour
    {
        [SerializeField] private StringReferenceSO _replayGameType;
        [SerializeField] private Button _openReplayButton;
        [SerializeField] private TMP_InputField _replayFilename;
        [SerializeField] private MusicOptionChanger _musicOptionChanger;
        [FormerlySerializedAs("_statCountingGroupChanger")] [SerializeField] private StatCounterGroupOverrideChanger _statCounterGroupOverrideChanger;

        private void Awake()
        {
            _openReplayButton.onClick.AddListener(OpenReplay);
            _musicOptionChanger.GameTypeStr = _replayGameType;
            _statCounterGroupOverrideChanger.GameTypeStr = _replayGameType;
        }

        private void OpenReplay()
        {
            _ = OpenReplayAsync();
        }

        private async Task OpenReplayAsync()
        {
            var (replayValid, replay) = await GameReplay.TryLoad(_replayFilename.text);
            if (!replayValid)
            {
                AlertDisplayer.ShowAlert(new Alert(
                    "Couldn't load replay!",
                    "Replay either couldn't be found or was in invalid format",
                    AlertType.Error));
                return;
            }

            GameInitializer.Replay = replay;
            GameInitializer.GameType = _replayGameType.Value;
            SceneManager.LoadScene("Scene_Game_Singleplayer");
        }
    }
}