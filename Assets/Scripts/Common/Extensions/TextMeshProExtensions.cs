
/************************************
TextMeshProExtensions.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using TMPro;

namespace UStacker.Common.Extensions
{
    public static class TextMeshProExtensions
    {
        public static void OnEnter(this TMP_InputField field, Action<string> onSubmitAction)
        {
            field.onSubmit.AddListener(text =>
            {
                if (!field.wasCanceled)
                    onSubmitAction?.Invoke(text);
            });
        }
    }
}
/************************************
end TextMeshProExtensions.cs
*************************************/
