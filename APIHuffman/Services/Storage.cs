

using APIHuffman.Models;
using System.Collections.Generic;

namespace APIHuffman.System {
    public class Storage {

        private static Storage _instance = null;

        public static Storage Instance { 
            get {
                if (_instance == null) _instance = new Storage();
                return _instance;
            }
        }

        public List<FileHistory> files = new List<FileHistory>();
        public FileHistory actualFile = new FileHistory();
    }
}
