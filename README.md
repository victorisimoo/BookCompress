# BookCompress
Proyecto para el curso de **Estructura de Datos II**, en el cual se implementa el algoritmo de compresión de **Huffman**.
Como parte del compromiso con el proyecto y el curso, se elaboraron dos soluciones, se adjunta en el link, la solución alternativa.  ([CompressionApp](https://github.com/Ale180820/CompressionApp))

## Codificación de Huffman ([Wikipedia](https://es.wikipedia.org/wiki/Codificaci%C3%B3n_Huffman))
En ciencia de la computación y teoría de la información, la codificación Huffman es un algoritmo usado para compresión de datos.

## Rutas y comportamiento de los métodos

#### /api/compress/{name}
- Recibe un archivo de texto que se deberá comprimir
- Retorna un archivo <name>.huff con el contenido del archivo comprimido

#### /api/decompress
- Recibe un archivo .huff que se deberá descomprimir
- Retorna el archivo de texto con el nombre original
- Devuelve OK si no hubo error
- Devuelve InternalServerError si hubo

#### /api/compresssions
- Devuelvve un JSON con el listado de todas las compresiones con los siguientes valores:
  - Nombre del archivo original
  - Nombre y ruta del archivo comprimido
  - Razón de compresión
  - Factor de compresión
  - Porcentaje de reducción

## Implementación
Para clonar el proyecto utilice el siguiente enlace: [https://github.com/victorisimoo/BookCompress.git]()

`$ git clone https://github.com/victorisimoo/BookCompress.git `


## Autores

Alejandra Recinos : @Ale180820

Victor Hernández  : @victorisimoo
