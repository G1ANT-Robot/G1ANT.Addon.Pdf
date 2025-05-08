using System;
using G1ANT.Language;

namespace G1ANT.Addon.Pdf
{
    [Command(Name = "pdf.optimize", Tooltip = "Compress and optimize PDF document")]
    public class PdfOptimizeCommand : Command
    {
        public PdfOptimizeCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Pdf structure which will be saved")]
            public PdfStructure Pdf { get; set; }

            [Argument(Required = false, Tooltip = "Remove duplicate objects in PDF documents")]
            public BooleanStructure RemoveDuplicateObjects { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Compress images in PDF")]
            public BooleanStructure CompressImages { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Compress images maximum ratio (>1.0, grater value lower image quelity)")]
            public FloatStructure CompressImagesRatio { get; set; }

            [Argument(Required = false, Tooltip = "Remove glyphs unused in a PDF document")]
            public BooleanStructure RemoveUnusedFonts { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Remove metadata from PDF document")]
            public BooleanStructure RemoveMetadata { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Remove information about PDF logical structure")]
            public BooleanStructure RemoveStructureInformation { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Remove unused resources from PDF")]
            public BooleanStructure RemoveUnusedResources { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Remove private application data from PDF, like Adobe software")]
            public BooleanStructure RemoveApplicationData { get; set; } = new BooleanStructure(true);

            [Argument(Required = false, Tooltip = "Flatten PDF form fields and annotations")]
            public BooleanStructure FlattenControls { get; set; } = new BooleanStructure(true);
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            if (pdf is null)
                throw new ArgumentNullException(nameof(arguments.Pdf));

            pdf.Compress(
                removeDuplicateObjects:arguments.RemoveDuplicateObjects.Value, 
                compressImages: arguments.CompressImages.Value, 
                compressImagesMaxRatio: arguments.CompressImagesRatio?.Value,
                removeUnusedFonts:arguments.RemoveUnusedFonts.Value, 
                removeMetadata:arguments.RemoveMetadata.Value,
                removeStructureInformation:arguments.RemoveStructureInformation.Value, 
                removeUnusedResources:arguments.RemoveUnusedResources.Value, 
                removeApplicationData:arguments.RemoveApplicationData.Value, 
                flattenControls:arguments.FlattenControls.Value
             );
        }
    }
}
