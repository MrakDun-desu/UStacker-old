using UnityEngine;

namespace UStacker.Common
{
    public class DocumentationOpener : MonoBehaviour
    {
        public void OpenDocumentation()
        {
            Application.OpenURL(StaticSettings.WikiUrl);
        }
    }
}