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

using System;
using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public class Tile {
        public int Zoom { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Tile[] GetChildren(int generation) {
            // Left to right, top to bottom.
            var power = (int)Math.Pow(2, generation);
            var tiles = new Tile[power * power];
            for (int y = 0; y < power; y++) {
                for (int x = 0; x < power; x++) {
                    tiles[y * power + x] = new Tile() {
                        Zoom = this.Zoom + generation,
                        X = power * this.X + x,
                        Y = power * this.Y + y
                    };
                }
            }
            return tiles;
        }
        public Coordinate UpperLeft {
            get {
                var n = Mathf.Pow(2, this.Zoom);
                var lon = this.X / n * 360f - 180f;
                var lat = Mathf.Atan((float)Math.Sinh(Mathf.PI * (1 - 2 * this.Y / n))) * Mathf.Rad2Deg;
                return new Coordinate() {
                    Longitude = lon,
                    Latitude = lat
                };
            }
        }
        public float Size {
            get {
                const double BASE = 156543.03392800014d;
                double resolution = BASE;
                for (int i = 0; i < this.Zoom; i++) {
                    resolution /= 2;
                }
                return Convert.ToSingle(resolution * 256);
            }
        }
    }
}
