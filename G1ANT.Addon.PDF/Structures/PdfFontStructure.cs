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
    [Structure(Name = "pdffont", AutoCreate = false, Tooltip = "PDF structure for maintaining pdf fonts")]
    public class PdfFontStructure : StructureTyped<PdfFontModel>
    {
        private static class IndexNames
        {
            public const string Size = "size";
            public const string Name = "name";
        }

        public PdfFontStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        public PdfFontStructure(PdfFontModel value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        private void Init()
        {
            Indexes.Add(IndexNames.Size);
            Indexes.Add(IndexNames.Name);
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                return new PdfFontStructure(Value, Format);
            }

            switch (index.ToLower())
            {
                case IndexNames.Size:
                    return new IntegerStructure(Value.Size, null, Scripter);
                case IndexNames.Name:
                    return new TextStructure(Value.Name.ToString(), null, Scripter);
            }
            throw new ArgumentException($"Unknown index '{index}'", nameof(index));
        }

        public override void Set(Structure structure, string index = "")
        {
            if (structure?.Object == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }

            switch (index.ToLower())
            {
                case IndexNames.Size:
                    Value.SetSizeFromString(structure.ToString());
                    break;
                case IndexNames.Name:
                    Value.SetNameFromString(structure.ToString());
                    break;
            }
            throw new ArgumentException($"Unknown index '{index}'", nameof(index));
        }

        public override string ToString()
        {
            return $"{Value.Name}:{Value.Size}";
        }

        protected override PdfFontModel Parse(string value, string format = null)
        {
            var elems = value.Split(':');
            var font = new PdfFontModel();
            if (elems.Count() > 0)
                font.SetNameFromString(elems[0]);
            if (elems.Count() > 1)
                font.SetSizeFromString(elems[1]);
            return font;
        }
    }
}