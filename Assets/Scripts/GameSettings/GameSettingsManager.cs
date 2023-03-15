using UStacker.GlobalSettings.Changers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UStacker.GameSettings
{
    public class GameSettingsManager : MonoBehaviour
    {
        [SerializeField] private Button _openGameButton;
        [SerializeField] private GameSettingsOverrideChanger _overrideChanger;
        [SerializeField] private MusicOptionChanger _musicOptionChanger;
        [FormerlySerializedAs("_statCountingGroupChanger")] [SerializeField] private StatCounterGroupOverrideChanger _statCounterGroupOverrideChanger;
        [SerializeField] private GameObject _startingLevelChanger;
        [SerializeField] private TMP_Text _gameTitle;
        [SerializeField] private TMP_Text _gameDescription;
        [SerializeField] private UnityEvent<GameSettingsSO> _gameStarted;

        public static SingleplayerGameAsset GameAsset { get; set; }

        private void Awake()
        {
            _overrideChanger.ChangedOverrideName = GameAsset.GameSettings.GameType;
            _musicOptionChanger.GameTypeStr = GameAsset.GameSettings.GameType;
            _statCounterGroupOverrideChanger.GameTypeStr = GameAsset.GameSettings.GameType;
            _gameTitle.text = GameAsset.GameSettings.GameType.Value;
            _gameDescription.text = GameAsset.GameDescription;
            _openGameButton.onClick.AddListener(() => _gameStarted.Invoke(GameAsset.GameSettings));
            _startingLevelChanger.gameObject.SetActive(GameAsset.ShowStartingLevelInSettings);
        }
    }
}