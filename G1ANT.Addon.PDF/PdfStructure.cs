using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Structure(Name = "pdf", AutoCreate = false, Tooltip = "PDF structure for maintaining pdf files")]
    public class PdfStructure : StructureTyped<PdfDocument>
    {

        private static class IndexNames
        {
            public const string Title = "title";
            public const string Author = "author";
            public const string PageCount = "pagecount";
            public const string PasswordProtected = "passwordprotected";
        }

        public PdfStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        public PdfStructure(PdfDocument value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        private void Init()
        {
            Indexes.Add(IndexNames.Author);
            Indexes.Add(IndexNames.Title);
            Indexes.Add(IndexNames.PageCount);
            Indexes.Add(IndexNames.PasswordProtected);
        }

        public override Structure Get(string index = "")
        {
            if (string.IsNullOrWhiteSpace(index))
            {
                return new PdfStructure(Value, Format);
            }

            switch (index.ToLower())
            {
                case IndexNames.Author:
                    return new TextStructure(Value.Info.Author, null, Scripter);
                case IndexNames.Title:
                    return new TextStructure(Value.Info.Title, null, Scripter);
                case IndexNames.PageCount:
                    return new IntegerStructure(Value.PageCount, null, Scripter);
            }
            throw new ArgumentException($"Unknown index '{index}'", nameof(index));
        }

        public override void Set(Structure structure, string index = null)
        {
            if (structure?.Object == null)
            {
                throw new ArgumentNullException(nameof(structure));
            }

            switch (index.ToLower())
            {
                case IndexNames.Author:
                    Value.Info.Author = structure.ToString();
                    break;
                case IndexNames.Title:
                    Value.Info.Title = structure.ToString();
                    break;
                default:
                    throw new ArgumentException($"Unknown index '{index}'", nameof(index));
            }
        }

    }
}
