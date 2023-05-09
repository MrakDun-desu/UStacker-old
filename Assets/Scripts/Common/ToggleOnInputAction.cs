
/************************************
ToggleOnInputAction.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UStacker.Common
{
    public class ToggleOnInputAction : MonoBehaviour
    {
        public UnityEvent OnTurnOn;
        public UnityEvent OnTurnOff;

        public void Toggle()
        {
            var active = !gameObject.activeSelf;
            gameObject.SetActive(active);

            if (active)
                OnTurnOn.Invoke();
            else
                OnTurnOff.Invoke();
        }

        public void Toggle(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            Toggle();
        }
    }
}
/************************************
end ToggleOnInputAction.cs
*************************************/
