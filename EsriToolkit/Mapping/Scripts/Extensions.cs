using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public static class Extensions {
        public static Tile ToTile(this Coordinate coordinate, int zoom) {
            var latrad = coordinate.Latitude * Mathf.Deg2Rad;
            var n = Mathf.Pow(2, zoom);
            var x = (coordinate.Longitutude + 180) / 360 * n;
            var y = (1f - Mathf.Log(Mathf.Tan(latrad) + (1 / Mathf.Cos(latrad))) / Mathf.PI) / 2.0 * n;
            return new Tile() {
                X = (int)x,
                Y = (int)y,
                Zoom = zoom
            };
        }
    }
}
