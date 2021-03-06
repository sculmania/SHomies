﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.IO;

namespace SHomies.Tienda.Clases
{
    public class ConvierteImagen : IValueConverter
    {
        public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
        {
            object respuesta = null;   
            string imagen = Utilitario.Funcion.ConvertTo<string>(value);
            if (!string.IsNullOrEmpty(imagen))
            {
                BitmapDecoder bitmap;
                using (Stream data = new MemoryStream(System.Convert.FromBase64String(imagen)))
                {
                    bitmap = BitmapDecoder.Create(data,
                            BitmapCreateOptions.PreservePixelFormat,
                            BitmapCacheOption.OnLoad);
                    int width = bitmap.Frames[0].PixelWidth;
                    int height = bitmap.Frames[0].PixelHeight;

                    //if (width != 64 ||
                    //    height != 64)
                    //{
                    //    throw new Utilitario.ExepcionSHomies("Imagen debe ser de 64x64 pixeles");
                    //}
                }
                respuesta = bitmap.Frames[0];
            }


            return respuesta;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture)
        {
            throw new NotSupportedException("Error Al convertir no aceptado para este grupo");
        }
    }
}
