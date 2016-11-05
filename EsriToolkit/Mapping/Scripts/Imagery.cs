using System;
using System.Collections;
using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public static class Imagery {
        private const string MAP = "http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer";
        public static IEnumerator GetTexture(Tile tile, Action<Texture2D> callback) {
            string url = string.Format("{0}/tile/{1}/{2}/{3}", new object[] {
                MAP,
                tile.Zoom,
                tile.Y,
                tile.X
            });
            WWW www = new WWW(url);
            yield return www;
            Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
            www.LoadImageIntoTexture(texture);

            callback(texture);
        }
    }
}
