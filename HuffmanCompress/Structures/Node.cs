using System;

namespace HuffmanCompress.Controller { 

    public class Node : IComparable {

        public byte character { get; set; }
        public double frecuency { get; set; }
        public Node nodeLeft { get; set; }
        public Node nodeRight { get; set; }

        public int CompareTo(object obj) {
            return frecuency.CompareTo(((Node)obj).frecuency);
        }
    }
}
