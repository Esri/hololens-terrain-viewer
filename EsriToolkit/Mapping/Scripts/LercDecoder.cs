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
