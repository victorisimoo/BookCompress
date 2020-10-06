using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCompress.Interfaces {

   public interface ICompressor {

        void Compress(IFormFile file, string routeDirectory);
        string Decompress(IFormFile file, string routeDirectory);

    }
}
