﻿using HuffmanCompress.Controller;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCompress
{
    public class FileController {

        public void CompressFile(IFormFile file, string routeDirectory) {

            var dataTable = new Dictionary<byte, double>();
            var treeList = new List<Node>();
            double totalCharacters = 0;

            if (!Directory.Exists(Path.Combine(routeDirectory, "compress"))) {
                Directory.CreateDirectory(Path.Combine(routeDirectory, "compress"));
            }

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                const int bufferLength = 10000;
                var byteBuffer = new byte[bufferLength];
                while (reader.BaseStream.Position != reader.BaseStream.Length) {
                    byteBuffer = reader.ReadBytes(bufferLength);
                    foreach (var words in byteBuffer) {
                        if (dataTable.ContainsKey(words)) {
                            dataTable[words]++;
                        } else {
                            dataTable.Add(words, 1);
                        }
                    }
                }

                foreach (var words in dataTable) {
                    totalCharacters += words.Value;
                }

                foreach (var words in dataTable) {
                    treeList.Add(new Node { character = words.Key, frecuency = words.Value / totalCharacters });
                }
                treeList.Sort();
            }

            InsertElement(treeList, file, routeDirectory);
        }

        public static void InsertElement(List<Node> nodeList, IFormFile file, string routeDirectory) {

            var prefixDictionary = new Dictionary<byte, string>();
            var road = "";

            while (nodeList.Count != 1) {
                var nodeAux = new Node();
                nodeAux.frecuency = nodeList[0].frecuency + nodeList[1].frecuency;
                nodeAux.nodeLeft = nodeList[1];
                nodeAux.nodeRight = nodeList[0];

                nodeList.RemoveRange(0, 2);
                nodeList.Add(nodeAux);
                nodeList.Sort();
            }

            TravelFile(ref prefixDictionary, nodeList[0], road);
            CompressInFile(prefixDictionary, file, routeDirectory);
        }

        public static void TravelFile(ref Dictionary<byte, string> prefixDictionary, Node root, string road) {

            if (root != null) {
                var caminoDer = $"{ road }1";
                var caminoIzq = $"{road}0";
                
                TravelFile(ref prefixDictionary, root.nodeRight, caminoDer);

                if (root.character != 0) {
                    prefixDictionary.Add(root.character, road);
                } 
                TravelFile(ref prefixDictionary, root.nodeLeft, caminoIzq);
            }
        }
        
        public static void CompressInFile(Dictionary<byte, string> keyDictionary, IFormFile file, string routeDirectory) {

            const int bufferLength = 10000;

            var byteBuffer = new byte[bufferLength];
            var cadenaAux = "";

            using (var reader = new BinaryReader(file.OpenReadStream())) {
                using (var streamWriter = new FileStream(Path.Combine(routeDirectory, "compress", $"{Path.GetFileNameWithoutExtension(file.FileName)}.huff"), FileMode.OpenOrCreate)) {
                    using (var writer = new BinaryWriter(streamWriter)) {
                        writer.Write(Encoding.UTF8.GetBytes(Convert.ToString(keyDictionary.Count).PadLeft(8, '0').ToCharArray()));
                        foreach(var item in keyDictionary) {
                            writer.Write(item.Key); 
                            var aux = $"{item.Value}|";
                            writer.Write(aux.ToCharArray());
                        }

                        while(reader.BaseStream.Position != reader.BaseStream.Length){
                            byteBuffer = reader.ReadBytes(bufferLength);
                            foreach (var letraRecibida in byteBuffer) {
                                foreach (var clave in keyDictionary) {
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

        public  string DecompressFile (IFormFile file, string routeDirectory) {

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
                                Linea += GetBinary(Convert.ToString(item)).PadLeft(8, '0');
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
                                Linea += GetBinary(Convert.ToString(item)).PadLeft(8, '0');
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


        private static string GetBinary (string snumero) {
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
