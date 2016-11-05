using System;
using System.Collections;
using UnityEngine;

namespace Esri.PrototypeLab.HoloLens.Unity {
    public static class Elevation {
        private const string TERRAIN = "http://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer";
        public static IEnumerator GetHeights(Tile tile, Action<ElevationData> callback) {
            string url = string.Format("{0}/tile/{1}/{2}/{3}", new object[] {
                TERRAIN,
                tile.Zoom,
                tile.Y,
                tile.X
            });
            WWW www = new WWW(url);
            yield return www;

            byte[] bytes = www.bytes;

            uint[] info = new uint[7];
            double[] data = new double[3];

            uint hr = LercDecoder.lerc_getBlobInfo(bytes, (uint)bytes.Length, info, data, info.Length, data.Length);
            if (hr > 0) {
                Debug.Log(string.Format("function lerc_getBlobInfo() failed with error code {0}.", hr));
                yield break;
            }

            int version = (int)info[0]; // version
            int type = (int)info[1];    // data type
            int cols = (int)info[2];    // nCols
            int rows = (int)info[3];    // nRows
            int bands = (int)info[4];   // nBands
            int valid = (int)info[5];   // num valid pixels
            int size = (int)info[6];    // blob size

            //float min = (float)dataArray[0]; // z min
            //float max = (float)dataArray[1]; // z max
            //float max = (float)dataArray[2]; // max Z Error Used

            byte[] processed = new byte[cols * rows];
            uint values = (uint)(cols * rows * bands);

            float[] heights = new float[values];
            uint hr2 = LercDecoder.lerc_decode(bytes, (uint)bytes.Length, processed, cols, rows, bands, type, heights);
            if (hr2 > 0) {
                Debug.Log(string.Format("function lerc_decode() failed with error code {0}.", hr2));
                yield break;
            }

            float? min = null;
            float? max = null;
            foreach (var v in heights) {
                min = (min.HasValue) ? Math.Min(min.Value, v) : v;
                max = (max.HasValue) ? Math.Max(max.Value, v) : v;
            }

            //System.Diagnostics.Debug.WriteLine("Esri: min: {0}, max: {1} ", new object[] { min, max });
            //System.Diagnostics.Debug.WriteLine("Acta: min: {0}, max: {1} ", new object[] { min2, max2 });

            callback(new ElevationData() {
                Columns = cols,
                Rows = rows,
                Min = min.Value,
                Max = max.Value,
                Heights = heights
            });
        }
    }
}
