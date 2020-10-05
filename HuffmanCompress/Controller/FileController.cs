using HuffmanCompress.Controller;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCompress {
    public class FileController {
        public void Comprimir(IFormFile file, string routeDirectory) {

            if (!Directory.Exists(Path.Combine(routeDirectory, "compress"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            var TablaLetras = new Dictionary<byte, double>();
            var ListaNodosArbol = new List<Node>();

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                const int bufferLength = 10000;
                var byteBuffer = new byte[bufferLength];
                while (reader.BaseStream.Position != reader.BaseStream.Length) {
                    byteBuffer = reader.ReadBytes(bufferLength);
                    foreach (var letra in byteBuffer) {
                        if (TablaLetras.ContainsKey(letra)) {
                            TablaLetras[letra]++;
                        } else {
                            TablaLetras.Add(letra, 1);
                        }
                    }
                }

                double totalLetras = 0;

                foreach (var letra in TablaLetras) {
                    totalLetras += letra.Value;
                }

                foreach (var letra in TablaLetras) {
                    ListaNodosArbol.Add(new Node { caracter = letra.Key, Frecuencia = letra.Value / totalLetras });
                }
                ListaNodosArbol.Sort();
            }

            Insert(ListaNodosArbol, file, routeDirectory);
        }

        public static void Insert(List<Node> ListaNodo, IFormFile file, string routeDirectory) {
            while (ListaNodo.Count != 1) {
                var nodoAux = new Node();
                nodoAux.Frecuencia = ListaNodo[0].Frecuencia + ListaNodo[1].Frecuencia;
                nodoAux.nodoizq = ListaNodo[1];
                nodoAux.nododer = ListaNodo[0];

                ListaNodo.RemoveRange(0, 2);
                ListaNodo.Add(nodoAux);
                ListaNodo.Sort();
            }

            var DiccionarioPrefijos = new Dictionary<byte, string>();
            var camino = "";
            Recorrido(ref DiccionarioPrefijos, ListaNodo[0], camino);
            ComprimirArchivo(DiccionarioPrefijos, file, routeDirectory);
        }

        public static void Recorrido(ref Dictionary<byte, string> DiccionarioPre, Node raiz, string camino) {
            if (raiz != null) {
                var caminoDer = $"{ camino }1";
                Recorrido(ref DiccionarioPre, raiz.nododer, caminoDer);
                if (raiz.caracter != 0) {
                    DiccionarioPre.Add(raiz.caracter, camino);
                }
                var caminoIzq = $"{camino}0";
                Recorrido(ref DiccionarioPre, raiz.nodoizq, caminoIzq);
            }
        }
        
        public static void ComprimirArchivo(Dictionary<byte, string> DiccionarioClave, IFormFile file, string routeDirectory) {
            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.huff"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        writer.Write(Encoding.UTF8.GetBytes(Convert.ToString(DiccionarioClave.Count).PadLeft(8, '0').ToCharArray()));
                        foreach(var item in DiccionarioClave) {
                            writer.Write(item.Key); 
                            var aux = $"{item.Value}|";
                            writer.Write(aux.ToCharArray());
                        }

                        const int bufferLength = 10000;

                        var byteBuffer = new byte[bufferLength];
                        var cadenaAux = "";

                        while(reader.BaseStream.Position != reader.BaseStream.Length){
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var letraRecibida in byteBuffer) {
                                foreach (var clave in DiccionarioClave) {
                                    if (letraRecibida == clave.Key) {
                                        cadenaAux += clave.Value;
                                        if ((cadenaAux.Length / 8) != 0) {
                                            for (int i = 0; i < cadenaAux.Length / 8; i++) {
                                                var nuevaCadena = cadenaAux.Substring(0, 8);
                                                writer.Write((byte)Convert.ToInt32(nuevaCadena, 2));
                                                cadenaAux = cadenaAux.Substring(8);
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        if (cadenaAux.Length <= 8) {
                            writer.Write((byte)Convert.ToInt32(cadenaAux.PadRight(8, '0'), 2));
                        }
                    }
                }
            }
        }

        public  string Descomprimir (IFormFile file, string routeDirectory) {

            var TablaPrefijos = new Dictionary<string, byte>();
            var Extension = string.Empty;

            if (!Directory.Exists(Path.Combine(routeDirectory, "decompress"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "decompress"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "decompress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.txt"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        int bufferLength = 10000;
                        var byteBuffer = new byte[bufferLength];
                        byteBuffer = reader.ReadBytes(8);
                        var cantDiccionario = Convert.ToInt32(Encoding.UTF8.GetString(byteBuffer));

                        bufferLength = 1;
                        byteBuffer = reader.ReadBytes(bufferLength);

                        for (int i = 0; i < cantDiccionario; i++) {
                            var camino = new List<byte>();
                            var letra = byteBuffer[0];
                            byteBuffer = reader.ReadBytes(bufferLength);
                            var DentroCamino = true;
                            while (DentroCamino) {
                                if (byteBuffer[0] != 124) {
                                    camino.Add(byteBuffer[0]);
                                }else {
                                    DentroCamino = false;
                                }
                                byteBuffer = reader.ReadBytes(bufferLength);
                            }
                            TablaPrefijos.Add(Encoding.UTF8.GetString(camino.ToArray()), letra);
                        }

                        bufferLength = 1000;
                        var auxCadena = string.Empty;
                        var Linea = string.Empty;
                        while (reader.BaseStream.Position != reader.BaseStream.Length) {
                            foreach (var item in byteBuffer) {
                                Linea += ObtenerBinario(Convert.ToString(item)).PadLeft(8, '0');
                                while (Linea.Length > 0) {
                                    if (TablaPrefijos.ContainsKey(auxCadena)) {
                                        writer.Write(TablaPrefijos[auxCadena]);
                                        auxCadena = string.Empty;
                                    }else {
                                        auxCadena += Linea.Substring(0, 1);
                                        Linea = Linea.Substring(1);
                                    }
                                }
                            }
                            byteBuffer = reader.ReadBytes(1000);
                        }

                        if (auxCadena.Length != 0) {
                            foreach (var item in byteBuffer) {
                                Linea += ObtenerBinario(Convert.ToString(item)).PadLeft(8, '0');
                                while (Linea.Length > 0) {
                                    if (TablaPrefijos.ContainsKey(auxCadena)) {
                                        writer.Write(TablaPrefijos[auxCadena]);
                                        auxCadena = string.Empty;
                                    }else {
                                        auxCadena += Linea.Substring(0, 1);
                                        Linea = Linea.Substring(1);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return Path.Combine(routeDirectory, "decompress", file.FileName);
        }


        private static string ObtenerBinario (string snumero) {
            var numero = Convert.ToInt32(snumero);
            var aux = "";
            var binario = "";

            while(numero >= 2) {
                aux = aux + (numero % 2).ToString();
                numero = numero / 2;
            }

            aux = aux + numero.ToString();

            for (int i = aux.Length; i >= 1; i += -1) {
                binario = binario + aux.Substring(i - 1, 1);
            }

            return binario;
        }

    }
}
