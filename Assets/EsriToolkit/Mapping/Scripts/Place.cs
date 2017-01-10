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
                        Name = "hoover dam",
                        Location = new Coordinate() {
                            Longitude = -114.737778f,
                            Latitude = 36.015556f
                        },
                        Level = 12
                    },
                    new Place() {
                        Name = "mount everest",
                        Location = new Coordinate() {
                            Longitude = 86.925278f,
                            Latitude = 27.988056f
                        },
                        Level = 10
                    },
                    new Place() {
                        Name = "mount saint helens",
                        Location = new Coordinate() {
                            Longitude = -122.1944f,
                            Latitude = 46.1912f
                        },
                        Level = 10
                    },
                    new Place() {
                        Name = "paris",
                        Location = new Coordinate() {
                            Longitude = 2.3508f,
                            Latitude = 48.8567f
                        },
                        Level = 9
                    },
                    new Place() {
                        Name = "los angeles",
                        Location = new Coordinate() {
                            Longitude = -118.25f,
                            Latitude = 34.05f
                        },
                        Level = 9
                    },
                    new Place() {
                        Name = "machu picchu",
                        Location = new Coordinate() {
                            Longitude = -72.545556f,
                            Latitude = -13.163333f
                        },
                        Level = 10
                    },
                    new Place() {
                        Name = "mount taranaki",
                        Location = new Coordinate() {
                            Longitude = 174.064722f,
                            Latitude = -39.296389f
                        },
                        Level = 11
                    },
                    new Place() {
                        Name = "mount maunganui",
                        Location = new Coordinate() {
                            Longitude = 176.185f,
                            Latitude = -37.643f
                        },
                        Level = 12
                    },
                    new Place() {
                        Name = "wellington",
                        Location = new Coordinate() {
                            Longitude = 174.777222f,
                            Latitude = -41.288889f
                        },
                        Level = 12
                    }
                };
            }
        }
    }
}