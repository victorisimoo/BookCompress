using System;

namespace HuffmanCompress.Controller
{

    public class Node : IComparable {

        #region Parameters
        public byte character { get; set; }
        public double frecuency { get; set; }
        public Node left { get; set; }
        public Node right { get; set; }
        #endregion

        /// <summary>
        /// Method for performing object comparison
        /// </summary>
        /// <param name="obj">Comparable object</param>
        /// <returns>Comparation result</returns>
        public int CompareTo(object obj) {
            return frecuency.CompareTo(((Node)obj).frecuency);
        }
    }
}
