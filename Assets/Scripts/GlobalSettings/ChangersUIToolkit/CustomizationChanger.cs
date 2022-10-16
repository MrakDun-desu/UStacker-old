using Blockstacker.Common;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.ChangersUIToolkit
{
    public class CustomizationChanger : VisualElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
        }
        
        public new class UxmlFactory : UxmlFactory<CustomizationChanger, UxmlTraits>
        {
        }

        private const string REFRESH_LISTS_BUTTON_TEXT = "Refresh lists";
        private const string SKIN_FOLDER_BUTTON_TEXT = "Open skin folder";
        private const string SOUND_FOLDER_BUTTON_TEXT = "Open sound folder";
        private const string BACKGROUND_FOLDER_BUTTON_TEXT = "Open background folder";
        
        public CustomizationChanger()
        {
            var blockSkinChanger = new SkinChanger(
                nameof(AppSettings.Customization) + "." + nameof(AppSettings.Customization.SkinFolder)
            );
            var soundPackChanger = new SoundPackChanger(
                nameof(AppSettings.Customization) + "." + nameof(AppSettings.Customization.SoundPackFolder)
            );
            var backgroundPackChanger = new BackgroundPackChanger(
                nameof(AppSettings.Customization) + "." + nameof(AppSettings.Customization.BackgroundFolder)
            );
            
            Add(blockSkinChanger);
            Add(new Button(() =>
            {
                DefaultAppOpener.OpenFile(CustomizationPaths.Skins);
            }){ text = SKIN_FOLDER_BUTTON_TEXT });
            
            Add(soundPackChanger);
            Add(new Button(() =>
            {
                DefaultAppOpener.OpenFile(CustomizationPaths.SoundPacks);
            }){ text = SOUND_FOLDER_BUTTON_TEXT });
            
            Add(backgroundPackChanger);
            Add(new Button(() =>
            {
                DefaultAppOpener.OpenFile(CustomizationPaths.BackgroundPacks);
            }){ text = BACKGROUND_FOLDER_BUTTON_TEXT });
            
            Add(new Button(() =>
            {
                blockSkinChanger.RefreshNames();
                soundPackChanger.RefreshNames();
                backgroundPackChanger.RefreshNames();
                
                blockSkinChanger.RefreshValue();
                soundPackChanger.RefreshValue();
                backgroundPackChanger.RefreshValue();
            }){ text = REFRESH_LISTS_BUTTON_TEXT });
        }
    }
}