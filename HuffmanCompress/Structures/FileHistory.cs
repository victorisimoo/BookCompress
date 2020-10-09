using System;

namespace APIHuffman.Models {
    public class FileHistory {
        #region Parameters
        public String FileName { get; set; }
        public String CompressedFilePath { get; set; }
        public double CompressionRatio { get; set; }
        public double CompressionFactor { get; set; }
        public double ReductionPortentage { get; set; }
        #endregion

        #region Constructor
        public FileHistory() { }
        #endregion

    }
}
