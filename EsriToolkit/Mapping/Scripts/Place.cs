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

namespace Esri.PrototypeLab.HoloLens.Unity {
    public class Place {
        public string Name { get; set; }
        public Coordinate Location { get; set; }
        public int Level { get; set; }
        public static Place[] PresetPlaces {
            get {
                return new Place[] {
                    new Place() {
                        Name = "grand canyon",
                        Location = new Coordinate() {
                            Longitutude = -114.738206f,
                            Latitude = 36.015578f
                        },
                        Level = 14
                    },
                    new Place() {
                        Name = "mount everest",
                        Location = new Coordinate() {
                            Longitutude = 86.925278f,
                            Latitude = 27.988056f
                        },
                        Level = 10
                    },
                    new Place() {
                        Name = "mount saint helens",
                        Location = new Coordinate() {
                            Longitutude = -122.1944f,
                            Latitude = 46.1912f
                        },
                        Level = 10
                    }
                };
            }
        }
    }
}