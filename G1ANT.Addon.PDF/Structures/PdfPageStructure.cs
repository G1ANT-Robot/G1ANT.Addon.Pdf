using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;
using G1ANT.Addon.PDF.Models;

namespace G1ANT.Addon.Pdf
{
    [Structure(Name = "pdfpage", AutoCreate = false, Tooltip = "PDF structure for maintaining pdf files")]
    public class PdfPageStructure : StructureTyped<PdfPage>
    {
        private static class IndexNames
        {
            public const string Width = "width";
            public const string Height = "height";
            public const string Orientation = "orientation";
        }

        public PdfPageStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        public PdfPageStructure(PdfPage value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        private void Init()
        {
            Indexes.Add(IndexNames.Width);
            Indexes.Add(IndexNames.Height);
            Indexes.Add(IndexNames.Orientation);
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                return new PdfPageStructure(Value, Format);
            }

            switch (index.ToLower())
            {
                case IndexNames.Width:
                    return new IntegerStructure(Value.Width, null, Scripter);
                case IndexNames.Height:
                    return new IntegerStructure(Value.Height, null, Scripter);
                case IndexNames.Orientation:
                    return new TextStructure(Value.Orientation.ToString(), null, Scripter);
            }
            throw new ArgumentException($"Unknown index '{index}'", nameof(index));
        }

        protected override PdfPage Parse(object value, string format = null)
        {
            if (value is PdfPage page)
                return page;
            throw new ArgumentException("Cannot create ");
        }

        public override string ToString()
        {
            return Value is null ? string.Empty : $"{Value.Width}x{Value.Height} - {Value.Orientation}";
        }
    }
}