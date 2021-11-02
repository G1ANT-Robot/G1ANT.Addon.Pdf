using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Command(Name = "pdf.save", Tooltip = "Saves pdf file in given path")]
    public class PdfSaveCommand : Command
    {
        public PdfSaveCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public class Arguments : CommandArguments
        {
            [Argument(Name = "path", Required = true, Tooltip = "Path where file will be saved")]
            public TextStructure Path { get; set; }

            [Argument(Name = "pdf", Required = true, Tooltip = "Pdf structure which will be saved")]
            public PdfStructure Pdf { get; set; }

            [Argument(Name = "password", Required = false, Tooltip = "Password to protect pdf")]
            public TextStructure Password { get; set; }

            [Argument(Name = "ownerPassword", Required = false, Tooltip = "Password of the owner of pdf. If empty, it will be set to 'password' argument ")]
            public TextStructure OwnerPassword { get; set; }
        }

        public void Execute(Arguments arguments)
        {
            var pdf = arguments.Pdf.Value;
            string ownerPassword;

            ownerPassword = arguments.OwnerPassword != null ? arguments.OwnerPassword.Value : string.Empty;
           
            var encryptionHandler = arguments.Password == null ? null : new PdfStandardEncryptionHandler(ownerPassword, arguments.Password.Value);
            if (encryptionHandler != null)
            {
                pdf.SaveOptions.EncryptionHandler = encryptionHandler;
            }
            pdf.Save(arguments.Path.Value);
        }
    }
}
