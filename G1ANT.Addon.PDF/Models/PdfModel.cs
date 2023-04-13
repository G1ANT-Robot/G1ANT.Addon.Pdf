using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BitMiracle.Docotic.Pdf;
using static G1ANT.Language.RobotMessage32;

namespace G1ANT.Addon.PDF.Models
{
    public class PdfModel
    {
        private PdfDocument pdfDocument;

        private bool IsDocumentCorrect => pdfDocument is null ? false : true;

        public PdfInfo Info => pdfDocument?.Info;
        public int PageCount => pdfDocument is null ? 0 : pdfDocument.PageCount;
        public PdfCollection<PdfPage> Pages => pdfDocument?.Pages;

        public PdfModel(PdfDocument pdfDocument)
        {
            this.pdfDocument = pdfDocument;
        }
        
        public PdfModel() 
        {
            pdfDocument = new PdfDocument();
        }

        public static PdfModel Open(string filename, string password = null)
        {
            using (var fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var memoryStream = new MemoryStream();
                fileStream.CopyTo(memoryStream);
                var standardDecryptionHandler = !string.IsNullOrEmpty(password) ? new PdfStandardDecryptionHandler(password) : null;
                var pdfDoc = standardDecryptionHandler is null ? new PdfDocument(memoryStream) : new PdfDocument(memoryStream, standardDecryptionHandler);
                return new PdfModel(pdfDoc);
            }
        }

        public IEnumerable<string> BuiltInFonts()
        {
            PdfBuiltInFont[] qq = (PdfBuiltInFont[])Enum.GetValues(typeof(PdfBuiltInFont));
            return qq.Select(x => x.ToString()).ToList();
        }

        public void Save(string filename, string password = null, string ownerPassword = null)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var encrypt = !string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(ownerPassword);
            var encryptionHandler = encrypt ? new PdfStandardEncryptionHandler(ownerPassword, password) : null;
            var saveOptions = new PdfSaveOptions()
            { 
                EncryptionHandler = encryptionHandler
            };
            pdfDocument.Save(filename, saveOptions);
        }

        public void SavePageAsImage(string filename)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var options = PdfDrawOptions.Create();
            pdfDocument.SaveAsTiff(filename, options);
        }

        public void MovePage(int srcIndex, int dstIndex)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            pdfDocument.MovePage(srcIndex, dstIndex);
        }

        public void SwapPages(int firstIndex, int secondIndex)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            pdfDocument.SwapPages(firstIndex, secondIndex);
        }

        public void RemovePage(int index)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            pdfDocument.RemovePage(index);
        }

        public void AddPage(int? index = null)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            if (index.HasValue)
                pdfDocument.InsertPage(index.Value);
            else
                pdfDocument.AddPage();
        }

        public void DrawText(int pageIndex, Point point, string text, PdfFontModel font = null)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var page = pdfDocument.GetPage(pageIndex);
            var canvas = page.Canvas;

            if (font != null)
            {
                //canvas.Brush.Color = color
                canvas.FontSize = font.Size;
                canvas.Font = pdfDocument.AddFont(font.Name);
            }
            canvas.DrawString(point.X, point.Y, text);   
        }

        public void DrawLine(int pageIndex, Point from, Point to)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var page = pdfDocument.GetPage(pageIndex);
            var canvas = page.Canvas;
            canvas.CurrentPosition = new PdfPoint(from.X, from.Y);
            canvas.DrawLineTo(new PdfPoint(to.X, to.Y));
        }

        public void DrawImage(int pageIndex, Point point, string imageFilename)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var image = pdfDocument.AddImage(imageFilename);
            var page = pdfDocument.GetPage(pageIndex);
            var canvas = page.Canvas;
            canvas.DrawImage(image, new PdfPoint(point.X, point.Y));
        }

        public void ExtractImages(string folder, string filenamePrefix)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            Directory.CreateDirectory(folder);
            foreach (var image in pdfDocument.GetImages())
            {
                var filePath = Path.Combine(folder, $"{filenamePrefix}-{image.Id}");
                image.Save(filePath);
            }
        }

        public string ExtractText(int pageIndex = -1)
        {
            return ExtractText(pageIndex, false);
        }

        public string ExtractTextWithFormatting(int pageIndex = -1)
        {
            return ExtractText(pageIndex, true);
        }

        private string ExtractText(int pageIndex = -1, bool withFormatting = false)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var options = new PdfTextExtractionOptions()
            {
                WithFormatting = withFormatting
            };
            if (pageIndex >= 0)
            {
                var page = pdfDocument.GetPage(pageIndex);
                return page.GetText(options);
            }
            else
                return pdfDocument.GetText(options);
        }

        static public bool IsPasswordProtected(string filename)
        {
            var info = PdfDocument.GetEncryptionInfo(filename);
            if (info is PdfStandardEncryptionInfo standardInfo)
                return true;
            return false;
        }

        static public bool Compare(string firstFile, string secondFile)
        {
            return PdfDocument.DocumentsAreEqual(firstFile, secondFile);
        }
    }
}
