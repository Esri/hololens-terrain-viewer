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

using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public static class GeocodeServer {
        private const string URL = "https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";
        public static IEnumerator ReverseGeocode(Coordinate coordinate, Action<Address> callback) {
            string url = string.Format("{0}/reverseGeocode?location={1},{2}&f=json", new object[] {
                URL,
                coordinate.Longitude,
                coordinate.Latitude
            });
            WWW www = new WWW(url);
            yield return www;

            // Extract token from parsed response
            var text = www.text.Replace(":null", ":\"\"");
            var json = JSON.Parse(text);
            if (json.ContainsKey("error")) {
                callback(null);
                yield break;
            }
            var address = json["address"];
            callback(new Address() {
                Street = address["Address"].Value,
                City = address["City"].Value,
                Region = address["Region"].Value,
                Postal = address["Postal"].Value,
                Country = address["CountryCode"].Value,
                SingleLine = address["Match_addr"].Value
            });
        }
    }
}
