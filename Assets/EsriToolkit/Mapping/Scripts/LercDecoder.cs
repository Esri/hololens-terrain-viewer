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

using System.Runtime.InteropServices;

namespace Esri.PrototypeLab.HoloLens.Unity {
    internal class LercDecoder {
        private const string DLL = "Lerc32";
        public enum DataType { dt_char, dt_uchar, dt_short, dt_ushort, dt_int, dt_uint, dt_float, dt_double }
        [DllImport(DLL)]
        public static extern uint lerc_getBlobInfo(byte[] pLercBlob, uint blobSize, uint[] infoArray, double[] dataRangeArray, int infoArraySize, int dataRangeArraySize);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, sbyte[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, byte[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, short[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, ushort[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, int[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, uint[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, float[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decode(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, int dataType, double[] pData);
        [DllImport(DLL)]
        public static extern uint lerc_decodeToDouble(byte[] pLercBlob, uint blobSize, byte[] pValidBytes, int nCols, int nRows, int nBands, double[] pData);
    }
}
