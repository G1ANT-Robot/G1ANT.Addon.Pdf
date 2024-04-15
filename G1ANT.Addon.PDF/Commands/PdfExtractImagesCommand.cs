using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.extractimages", Tooltip = "This command extract images from the document")]
    public class PdfExtractImagesCommand : Command
    {
        public PdfExtractImagesCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Folder to save images to")]
            public TextStructure Folder { get; set; }

            [Argument(Required = true, Tooltip = "Filename prefix for images")]
            public TextStructure Filename { get; set; } = new TextStructure("pdfimage");
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            pdf.ExtractImages(arguments.Folder.Value, arguments.Filename.Value);
        }
    }
}
