using System;
using System.Collections.Generic;
using System.Text;

namespace HuffmanCompress.Controller {
    public class Node : IComparable {
        public byte caracter { get; set; }
        public double Frecuencia { get; set; }
        public Node nodoizq { get; set; }
        public Node nododer { get; set; }

        public int CompareTo(object obj) {
            return Frecuencia.CompareTo(((Node)obj).Frecuencia);
        }
    }
}
