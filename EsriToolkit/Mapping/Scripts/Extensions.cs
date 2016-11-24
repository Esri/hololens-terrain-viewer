/*
    Copyright 2016 Esri

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.

    You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public static class Extensions {
        public static Tile ToTile(this Coordinate coordinate, int zoom) {
            var latrad = coordinate.Latitude * Mathf.Deg2Rad;
            var n = Mathf.Pow(2, zoom);
            var x = (coordinate.Longitude + 180) / 360 * n;
            var y = (1f - Mathf.Log(Mathf.Tan(latrad) + (1 / Mathf.Cos(latrad))) / Mathf.PI) / 2.0 * n;
            return new Tile() {
                X = (int)x,
                Y = (int)y,
                Zoom = zoom
            };
        }
    }
}
