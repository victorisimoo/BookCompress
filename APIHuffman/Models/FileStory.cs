using System;

namespace APIHuffman.Models {

    /// <summary>
    /// Class for storing file metadata
    /// </summary>
    public class FileStory {
        #region Parameters
        public String fileName { get; set; } //Original file name
        public String fileCompressName { get; set; }
        public int CompressionRatio { get; set; }
        public int CompressionFactor { get; set; }
        public int ReductionPortentage { get; set; }
        #endregion

        #region Contructor
        public FileStory() { }
        #endregion

    }
}
