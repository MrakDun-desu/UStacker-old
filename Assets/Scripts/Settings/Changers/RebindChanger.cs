using System;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

namespace Blockstacker.Settings.Changers
{
    public class RebindChanger : MonoBehaviour
    {
        [Header("Binding elements")]
        [SerializeField] private InputActionAsset _actionAsset;
        [SerializeField] private InputActionReference _action;
        [Header("Interface elements")]
        [SerializeField] private GameObject _rebindOverlay;
        [SerializeField] private TMP_Text _bindingName;
        [SerializeField] private TMP_Text[] _bindingTexts = new TMP_Text[3];

        private static event Action RebindChanged;
        private RebindingOperation _currentOperation;

        private void OnValidate()
        {
            if (_action == null) return;

            if (_bindingName != null) {
                var slashIndex = _action.name.LastIndexOf("/") + 1;
                var nameString = _action.name[slashIndex..];
                _bindingName.text = nameString.FormatCamelCase();
            }

            RefreshNames();
            CheckBindingOverlaps();
        }

        private void OnEnable()
        {
            RebindChanged += RefreshNames;
        }

        private void OnDisable()
        {
            RebindChanged -= RefreshNames;
        }

        private void RefreshNames()
        {
            for (var i = 0; i < _bindingTexts.Length; i++) {
                if (_bindingTexts[i] == null) continue;
                var bindingName = _action.action.bindings[i]
                    .ToDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);
                if (string.IsNullOrEmpty(bindingName)) bindingName = "Not bound";
                _bindingTexts[i].text = bindingName;
            }
        }

        private void CheckBindingOverlaps()
        {
            for (var i = 0; i < _bindingTexts.Length; i++) {
                _bindingTexts[i].color = IsBindingUnique(i) ? Color.red : Color.black;
            }
        }

        private bool IsBindingUnique(int index)
        {
            var newBinding = _action.action.bindings[index];
            foreach (var binding in _action.action.actionMap.bindings) {
                if (binding.action == newBinding.action)
                    continue;

                if (newBinding.effectivePath == binding.effectivePath &&
                    !string.IsNullOrEmpty(newBinding.ToDisplayString()))
                    return false;
            }
            return true;
        }

        private void RebindCancelled(int index)
        {
            _action.action.RemoveBindingOverride(index);
            EndRebind();
        }

        private void EndRebind()
        {
            _currentOperation.Dispose();
            if (_rebindOverlay != null)
                _rebindOverlay.SetActive(false);
            RefreshNames();
            RebindChanged?.Invoke();
            AppSettings.Rebinds = _actionAsset.SaveBindingOverridesAsJson();
        }

        public void DoRebind(int index)
        {
            if (_rebindOverlay != null)
                _rebindOverlay.SetActive(true);

            _currentOperation = _action.action.PerformInteractiveRebinding(index)
                .WithControlsExcluding("Mouse")
                .WithControlsExcluding("<Keyboard>/anyKey")
                .WithCancelingThrough("<Mouse>/leftButton")
                .OnMatchWaitForAnother(.1f)
                .OnCancel(_ => RebindCancelled(index))
                .OnComplete(_ => EndRebind())
                .Start();
        }
    }
}