using BitMiracle.Docotic.Pdf;
using G1ANT.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G1ANT.Addon.PDF.Models
{
    public class PdfFontModel
    {
        public int Size { get; set; } = 14;
        public PdfBuiltInFont Name { get; set; } = PdfBuiltInFont.Helvetica;

        public void SetNameFromString(string value)
        {
            if (Enum.TryParse<PdfBuiltInFont>(value, out var name))
                Name = name;
            else
                throw new ArgumentException($"'{value}' is not correct font name");
        }

        public void SetSizeFromString(string value)
        {
            if (int.TryParse(value, out var size))
                Size = size;
            else
                throw new ArgumentException($"'{value}' is not correct font name");
        }
    }
}
