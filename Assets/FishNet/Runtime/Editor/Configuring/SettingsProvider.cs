#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnitySettingsProviderAttribute = UnityEditor.SettingsProviderAttribute;
using UnitySettingsProvider = UnityEditor.SettingsProvider;

namespace FishNet.Configuring.Editing
{
    internal static class SettingsProvider
    {
        private static Vector2 _scrollView;

        [UnitySettingsProvider]
        private static UnitySettingsProvider Create()
        {
            return new UnitySettingsProvider("Project/Fish-Networking/Configuration", SettingsScope.Project)
            {
                label = "Configuration",

                guiHandler = OnGUI,

                keywords = new[]
                {
                    "Fish",
                    "Networking",
                    "Configuration"
                }
            };
        }

        private static void OnGUI(string searchContext)
        {
            var configuration = Configuration.LoadConfigurationData();

            if (configuration == null)
            {
                EditorGUILayout.HelpBox("Unable to load configuration data.", MessageType.Error);

                return;
            }

            EditorGUI.BeginChangeCheck();

            var scrollViewStyle = new GUIStyle
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            _scrollView = GUILayout.BeginScrollView(_scrollView, scrollViewStyle);

            EditorGUILayout.BeginHorizontal();

            var toggleStyle = new GUIStyle(EditorStyles.toggle)
            {
                richText = true
            };

            configuration.CodeStripping.StripReleaseBuilds = GUILayout.Toggle(
                configuration.CodeStripping.StripReleaseBuilds,
                $"{ObjectNames.NicifyVariableName(nameof(configuration.CodeStripping.StripReleaseBuilds))} <color=yellow>(Pro Only)</color>",
                toggleStyle);

            EditorGUILayout.EndHorizontal();

            if (configuration.CodeStripping.StripReleaseBuilds)
            {
                EditorGUI.indentLevel++;
                //Stripping Method.
                var enumStrings = new List<string>();
                foreach (var item in Enum.GetNames(typeof(StrippingTypes)))
                    enumStrings.Add(item);
                configuration.CodeStripping.StrippingType = EditorGUILayout.Popup(
                    $"{ObjectNames.NicifyVariableName(nameof(configuration.CodeStripping.StrippingType))}",
                    configuration.CodeStripping.StrippingType, enumStrings.ToArray());

                EditorGUILayout.HelpBox(
                    "Development builds will not have code stripped. Additionally, if you plan to run as host disable code stripping.",
                    MessageType.Warning);
                EditorGUI.indentLevel--;
            }

            GUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck()) Configuration.Configurations.Write(true);
        }
    }
}

#endif