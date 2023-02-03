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
                    $"Persistent data path has been changed to {newPath}. You can delete files in the old path now.",
                    AlertType.Success)
                : new Alert("Error changing data path", 
                    "Data path couldn't be changed",
                    AlertType.Error);

            AlertDisplayer.Instance.ShowAlert(shownAlert);
        }
    }
}