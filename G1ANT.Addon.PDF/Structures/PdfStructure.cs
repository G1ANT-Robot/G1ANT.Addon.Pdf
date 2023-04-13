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
    [Structure(Name = "pdf", AutoCreate = false, Tooltip = "PDF structure for maintaining pdf files")]
    public class PdfStructure : StructureTyped<PdfModel>
    {
        private static class IndexNames
        {
            public const string Title = "title";
            public const string Author = "author";
            public const string PageCount = "pagecount";
            public const string Pages = "pages";
            public const string BuiltInFonts = "builtinfonts";
        }

        public PdfStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        public PdfStructure(PdfModel value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        private void Init()
        {
            Indexes.Add(IndexNames.Author);
            Indexes.Add(IndexNames.Title);
            Indexes.Add(IndexNames.PageCount);
            Indexes.Add(IndexNames.Pages);
            Indexes.Add(IndexNames.BuiltInFonts);
        }

        private ListStructure GetPages()
        {
            var pages = Value.Pages.Select(x => new PdfPageStructure(x, null, Scripter)).Cast<object>().ToList();
            return new ListStructure(pages, null, Scripter);
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
                case IndexNames.Pages:
                    return GetPages();
                case IndexNames.BuiltInFonts:
                    return new ListStructure(Value.BuiltInFonts(), null, Scripter);
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

        public override string ToString()
        {
            return string.IsNullOrEmpty(Value?.Info?.Title) ? "" : Value?.Info?.Title;
        }
    }
}