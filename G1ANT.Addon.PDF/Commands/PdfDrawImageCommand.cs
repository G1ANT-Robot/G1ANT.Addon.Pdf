using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.drawimage", Tooltip = "This command draws image on the document")]
    public class PdfDrawImageCommand : Command
    {
        public PdfDrawImageCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "PDF structure to operates on")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = true, Tooltip = "Page number")]
            public IntegerStructure Page { get; set; }

            [Argument(Required = true, Tooltip = "Rectangle within the page where image will be placed")]
            public RectangleStructure Rect { get; set; }

            [Argument(Tooltip = "Specifies when and how an image gets scaled (AlwaysScale, ScaleWhenImageBigger, ScaleWhenImageSmaller, NeverScale)")]
            public TextStructure ScaleMode { get; set; } = new TextStructure("AlwaysScale");

            [Argument(Required = true, Tooltip = "Path of the image")]
            public TextStructure ImagePath { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf?.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            if (!Enum.TryParse(arguments.ScaleMode.Value, out PdfImageScaleMode scaleMode))
                throw new ArgumentNullException(nameof(arguments.ScaleMode));

            pdf.DrawImage(arguments.Page.Value, arguments.Rect.Value, arguments.ImagePath?.Value, scaleMode);
        }
    }
}
