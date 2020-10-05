using System;

namespace APIHuffman.Models {
    public class FileStory {

        public String fileName { get; set; }
        public String pathFile { get; set; }
        public String fileCompressName { get; set; }
        public int CompressionRatio { get; set; }
        public int CompressionFactor { get; set; }
        public int ReductionPortentage { get; set; }

        public FileStory() { }

    }
}
