﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extensions for the UnityEnditor.EditorGUILayout class.
    /// </summary>
    public static class EditorGUILayoutExtensions
    {
        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return Button(text, GUI.skin.button, options);
        }

        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIExtensions.Indent);
            bool pressed = GUILayout.Button(text, style, options);
            EditorGUILayout.EndHorizontal();
            return pressed;
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            Label(text, EditorStyles.label, options);
        }

        public static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIExtensions.Indent);
            GUILayout.Label(text, style, options);
            EditorGUILayout.EndHorizontal();
        }
    }
}