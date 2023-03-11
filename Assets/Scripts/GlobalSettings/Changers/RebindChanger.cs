using System;
using System.Collections;
using System.Linq;
using UStacker.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

namespace UStacker.GlobalSettings.Changers
{
    public class RebindChanger : MonoBehaviour
    {
        [Header("Binding elements")] [SerializeField]
        private InputActionAsset _actionAsset;

        [SerializeField] private InputActionReference _action;

        [Header("Interface elements")] [SerializeField]
        private GameObject _rebindOverlay;

        [SerializeField] private TMP_Text _bindingName;
        [SerializeField] private TMP_Text[] _bindingTexts = new TMP_Text[3];

        [Header("Colors")] [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _conflictColor = Color.red;
        private RebindingOperation _currentOperation;

        private void Start()
        {
            OnValidate();
        }

        private void OnEnable()
        {
            InputPresetChanger.RebindsChanged += RefreshNames;
            InputPresetChanger.RebindsChanged += CheckBindingOverlaps;
            RebindChanged += RefreshNames;
            RebindChanged += CheckBindingOverlaps;
        }

        private void OnDisable()
        {
            InputPresetChanger.RebindsChanged -= RefreshNames;
            InputPresetChanger.RebindsChanged -= CheckBindingOverlaps;
            RebindChanged -= RefreshNames;
            RebindChanged -= CheckBindingOverlaps;
        }

        private void OnValidate()
        {
            if (_action == null) return;

            if (_bindingName != null)
            {
                var slashIndex = _action.name.LastIndexOfAny(new[]
                {
                    '/',
                    '\\'
                }) + 1;
                var nameString = _action.name[slashIndex..];
                _bindingName.text = nameString.FormatCamelCase();
            }

            RefreshNames();
            CheckBindingOverlaps();
        }

        private static event Action RebindChanged;

        private void RefreshNames()
        {
            for (var i = 0; i < _bindingTexts.Length; i++)
            {
                if (_bindingTexts[i] == null) continue;
                var bindingName = _action.action.bindings[i]
                    .ToDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);
                if (string.IsNullOrEmpty(bindingName)) bindingName = "Not bound";
                _bindingTexts[i].text = bindingName;
            }
        }

        private void CheckBindingOverlaps()
        {
            for (var i = 0; i < _bindingTexts.Length; i++)
                _bindingTexts[i].color = IsBindingUnique(i) ? _defaultColor : _conflictColor;
        }

        private bool IsBindingUnique(int index)
        {
            var newBinding = _action.action.bindings[index];
            var otherActions = _action.action.actionMap.bindings.Where(binding => binding.action != newBinding.action);

            return otherActions.All(
                binding => newBinding.effectivePath != binding.effectivePath ||
                           string.IsNullOrEmpty(newBinding.ToDisplayString()));
        }

        private void RebindCancelled(int index)
        {
            _action.action.ApplyBindingOverride(index, string.Empty);
            EndRebind();
        }

        private void EndRebind()
        {
            _currentOperation.Dispose();
            if (_rebindOverlay != null)
                StartCoroutine(DeactivateOverlay());
            RefreshNames();
            RebindChanged?.Invoke();
            _action.action.Enable();
            AppSettings.Rebinds = _actionAsset.SaveBindingOverridesAsJson();
        }

        private IEnumerator DeactivateOverlay()
        {
            yield return new WaitForSeconds(.1f);
            _rebindOverlay.SetActive(false);
        }

        public void DoRebind(int index)
        {
            if (_rebindOverlay != null)
                _rebindOverlay.SetActive(true);

            _action.action.Disable();

            _currentOperation = _action.action.PerformInteractiveRebinding(index)
                .WithControlsExcluding("Mouse")
                .WithControlsExcluding("<Keyboard>/printScreen")
                .WithCancelingThrough("<Mouse>/leftButton")
                .OnMatchWaitForAnother(.1f)
                .OnCancel(_ => RebindCancelled(index))
                .OnComplete(_ => EndRebind())
                .Start();
        }
    }
}