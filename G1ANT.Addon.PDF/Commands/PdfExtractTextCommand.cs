using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.extracttext", Tooltip = "Extract text from whole pdf")]
    public class PdfExtractTextCommand : Command
    {
        public PdfExtractTextCommand (AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Document to extract text from")]
            public PdfStructure Pdf { get; set; }

            [Argument(Tooltip = "Page to extract from, if not passed whole document will be extracted")]
            public IntegerStructure Page { get; set; }

            [Argument(Required = true, Tooltip = "Extract text with formatting")]
            public BooleanStructure WithFormatting { get; set; } = new BooleanStructure(true);

            [Argument(Required = true, Tooltip = "Language for OCR module")]
            public TextStructure OcrLanguage { get; set; } = new TextStructure("eng");

            [Argument(Required = false, Tooltip = "Result where extracted text will be stored")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            var ocrLang = arguments.OcrLanguage.Value;
            var text = arguments.WithFormatting.Value ? pdf.ExtractTextWithFormatting(arguments.Page?.Value, ocrLang) 
                : pdf.ExtractText(arguments.Page?.Value, ocrLang);
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new TextStructure(text, null, Scripter));
        }

        private string GetScriptVariable(string name)
        {
            var variable = Scripter.Variables.GetAttributedVariables().Where((v) => { return v.Name == name; }).FirstOrDefault();
            if (variable?.GetValue("")?.Object is string str)
                return str;
            return null;
        }

    }
}
