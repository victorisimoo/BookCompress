using System;

namespace HuffmanCompress.Structures {
    public class Converter {

        /// <summary>
        /// Method for returning a binary element
        /// </summary>
        /// <param name="element"> sPart of the object to be converted </param>
        /// <returns> Converted element return </returns>
        public string GetBinary(string element) {
            var number = Convert.ToInt32(element);
            var aux = "";
            var binary = "";

            while (number >= 2) {
                aux = aux + (number % 2).ToString();
                number = number / 2;
            }

            aux = aux + number.ToString();

            for (int i = aux.Length; i >= 1; i += -1) {
                binary = binary + aux.Substring(i - 1, 1);
            }
            return binary;
        }
    }
}
