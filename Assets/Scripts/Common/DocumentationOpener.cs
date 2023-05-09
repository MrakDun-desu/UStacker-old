
/************************************
DocumentationOpener.cs -- created by Marek Dančo (xdanco00)
*************************************/
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
/************************************
end DocumentationOpener.cs
*************************************/
