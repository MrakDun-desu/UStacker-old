
/************************************
SeedSetter.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Gameplay.Communication;
using UStacker.Gameplay.Enums;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay
{
    public class SeedSetter : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Mediator _mediator;

        private void OnEnable()
        {
            _mediator.Register<GameStateChangedMessage>(OnGameStateChanged, 10);
        }

        private void OnDisable()
        {
            _mediator.Unregister<GameStateChangedMessage>(OnGameStateChanged);
        }

        public GameSettingsSO.SettingsContainer GameSettings { get; set; }

        private void OnGameStateChanged(GameStateChangedMessage message)
        {
            if (message.NewState != GameState.Initializing)
                return;

            if (message.IsReplay)
            {
                _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
                return;
            }

            if (GameSettings.General.UseCustomSeed)
            {
                GameSettings.General.ActiveSeed = GameSettings.General.CustomSeed;
            }
            else
            {
                var seed1 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                var seed2 = (ulong) ((long) Random.Range(int.MinValue, int.MaxValue) + int.MaxValue);
                GameSettings.General.ActiveSeed = seed1 + (seed2 << 32);
            }

            _mediator.Send(new SeedSetMessage(GameSettings.General.ActiveSeed));
        }
    }
}
/************************************
end SeedSetter.cs
*************************************/
