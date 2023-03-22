using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UStacker.Common.Alerts;

namespace UStacker.Common
{
    public class PersistentDataPathChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _field;

        private void Start()
        {
            _field.SetTextWithoutNotify(PersistentPaths.DataPath);
            _field.onEndEdit.AddListener(newPath => _ = OnDataPathChanged(newPath));
        }

        private static async Task OnDataPathChanged(string newPath)
        {
            var shownAlert = await PersistentPaths.TrySetDataPathAsync(newPath)
                ? new Alert("Data path changed",
                    $"Persistent data path has been changed to {newPath}. You can now delete files in the old path.",
                    AlertType.Success)
                : new Alert("Error changing data path",
                    "Data path couldn't be changed. Some of the files might have been copied, but the operation couldn't finish.",
                    AlertType.Error);

            AlertDisplayer.ShowAlert(shownAlert);
        }
    }
}