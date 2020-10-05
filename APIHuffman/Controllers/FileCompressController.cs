using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using APIHuffman.System;
using HuffmanCompress;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace APIHuffman.Controllers {
    [Route("api/[controller]")]
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
                return StatusCode(200, ReturnFile());
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

        [HttpGet]
        public HttpResponseMessage ReturnFile() {
            var path = Storage.Instance.actualFile.CompressedFilePath;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = stream.Length;
            return result;
        }

        public void DataMapping(IFormFile file) {
            Storage.Instance.actualFile.FileName = Path.GetFileNameWithoutExtension(file.FileName);
            Storage.Instance.actualFile.CompressedFilePath = Path.Combine(
                routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.huff");
        }

        

    }
}
