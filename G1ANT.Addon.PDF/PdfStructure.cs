using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using G1ANT.Language;
using BitMiracle.Docotic.Pdf;

namespace G1ANT.Addon.PDF
{
    [Structure(Name = "pdf", AutoCreate =false, Tooltip = "PDF structure for maintaining pdf files")]
    public class PdfStructure : StructureTyped<PdfDocument>
    {
        private const string title = "title";
        public PdfStructure() : base(new PdfDocument()) 
        {
        
        }
        public PdfStructure(object value, string format = null, AbstractScripter scripter = null)
            : base(value, format, scripter)
        {
            Init();
        }

        public PdfStructure(PdfDocument value, string format = null, AbstractScripter scripter = null)
            : this((object)value, format, scripter)
        { }

        private void Init()
        {

        }
    }
}
