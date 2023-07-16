using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public static class ChromaKeyHelper
    {
        public static bool IsChromaKeyColor(Color pixelColor, Color chromaKeyColor, int threshold)
        {
            int rDiff = Math.Abs(pixelColor.R - chromaKeyColor.R);
            int gDiff = Math.Abs(pixelColor.G - chromaKeyColor.G);
            int bDiff = Math.Abs(pixelColor.B - chromaKeyColor.B);

            // Проверяем, соответствует ли цвет хромакею
            return rDiff <= threshold && gDiff <= threshold && bDiff <= threshold;
        }
    }
}
