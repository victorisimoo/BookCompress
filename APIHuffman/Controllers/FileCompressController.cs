using System;
using System.IO;
using HuffmanCompress;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                fileController.Comprimir(file, routeDirectory);
                return StatusCode(200, "The file is good");
            }else {
                return StatusCode(500, "Error with sent file");
            }

        }
        
        [HttpPost ("decompress")]
        public ActionResult Decompress([FromForm] IFormFile file) {
            if (file != null) {
                FileController fileController = new FileController();
                string path = fileController.Descomprimir(file, routeDirectory);
                return StatusCode(200, path);
            } else {
                return StatusCode(500);
            }
            
        }

    }
}
