using System;

namespace APIHuffman.Models {
    public class FileHistory {

        public String FileName { get; set; }
        public String CompressedFilePath { get; set; }
        public int CompressionRatio { get; set; }
        public int CompressionFactor { get; set; }
        public int ReductionPortentage { get; set; }

        public FileHistory() { }

    }
}
