﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extensions for the UnityEngine.LayerMask class.
    /// </summary>
    public static class LayerMaskExtensions
    {
        public const int LayerCount = 32;

        private static string[] layerMaskNames = null;
        public static string[] LayerMaskNames
        {
            get
            {
                if (layerMaskNames == null)
                {
                    LayerMaskExtensions.layerMaskNames = new string[LayerCount];
                    for (int layer = 0; layer < LayerCount; ++layer)
                    {
                        LayerMaskExtensions.layerMaskNames[layer] = LayerMask.LayerToName(layer);
                    }
                }

                return LayerMaskExtensions.layerMaskNames;
            }
        }

        public static string GetDisplayString(this LayerMask layerMask)
        {
            StringBuilder stringBuilder = null;
            for (int layer = 0; layer < LayerMaskExtensions.LayerCount; ++layer)
            {
                if ((layerMask & (1 << layer)) != 0)
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder();
                    }
                    else
                    {
                        stringBuilder.Append(" | ");
                    }

                    stringBuilder.Append(LayerMaskExtensions.LayerMaskNames[layer]);
                }
            }

            return stringBuilder == null ? "None" : stringBuilder.ToString();
        }
    }
}