using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using APIHuffman.Models;
using APIHuffman.System;
using HuffmanCompress;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.FileProviders;

namespace APIHuffman.Controllers {
    [Route("api/")]
    [ApiController]
    public class FileCompressController : ControllerBase {

        private static string routeDirectory = Environment.CurrentDirectory;

        [HttpGet]
        public ActionResult Get() {
            return Ok();
        }

        [HttpPost ("compress")]
        public ActionResult Compress([FromForm] IFormFile file) {
            if (file.Length != 0 && file != null) { 
                FileController fileController = new FileController();
                fileController.CompressFile(file, routeDirectory);
                DataMapping(file);
                return returnFile(file);
            }else {
                return StatusCode(500, "Error with sent file");
            }

        }

        [HttpPost ("decompress")]
        public ActionResult Decompress([FromForm] IFormFile file) {
            if (file != null) {
                FileController fileController = new FileController();
                string path = fileController.DecompressFile(file, routeDirectory);
                return StatusCode(200, path);
            } else {
                return StatusCode(500);
            }
            
        }

        public ActionResult returnFile(IFormFile file) {
            return PhysicalFile(Storage.Instance.actualFile.CompressedFilePath, MediaTypeNames.Text.Plain, $"{Path.GetFileNameWithoutExtension(file.FileName)}.huff");
        }
        
        public void DataMapping(IFormFile file) {
            Storage.Instance.actualFile.FileName = Path.GetFileNameWithoutExtension(file.FileName);
            Storage.Instance.actualFile.CompressedFilePath = Path.Combine(
                routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.huff");
            Storage.Instance.actualFile.CompressionFactor = (double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length / (double) file.Length);
            Storage.Instance.actualFile.CompressionRatio = (double)(file.Length / (double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length));
            Storage.Instance.actualFile.ReductionPortentage = (double) ((double)(new FileInfo(Storage.Instance.actualFile.CompressedFilePath).Length) * 100) / (double) file.Length;
            Storage.Instance.files.Add(Storage.Instance.actualFile);
            Storage.Instance.actualFile = new FileHistory();
        }

        

    }
}
