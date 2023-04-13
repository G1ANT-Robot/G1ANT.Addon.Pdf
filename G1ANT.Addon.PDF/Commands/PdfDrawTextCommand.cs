using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.drawtext", Tooltip = "This command draws text on the document")]
    public class PdfDrawTextCommand : Command
    {
        public PdfDrawTextCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Page number")]
            public IntegerStructure Page { get; set; }

            [Argument(Required = true, Tooltip = "Position on page")]
            public PointStructure Position { get; set; }

            [Argument(Required = true, Tooltip = "Text to add")]
            public TextStructure Text { get; set; }

            [Argument(Tooltip = "Font for the text")]
            public PdfFontStructure Font { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));
            
            pdf.DrawText(arguments.Page.Value, arguments.Position.Value, arguments.Text?.Value, arguments.Font?.Value);
        }
    }
}
