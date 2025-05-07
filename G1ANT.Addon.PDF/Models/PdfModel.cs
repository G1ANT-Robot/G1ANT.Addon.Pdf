using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using BitMiracle.Docotic.Pdf;
using Org.BouncyCastle.Asn1.Pkcs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static G1ANT.Language.RobotMessage32;
using static G1ANT.Language.RobotWin32;
using Tesseract;

namespace G1ANT.Addon.PDF.Models
{
    public class PdfModel
    {
        protected PdfDocument pdfDocument;

        private bool IsDocumentCorrect => pdfDocument is null ? false : true;

        public PdfInfo Info => pdfDocument?.Info;
        public int PageCount => pdfDocument is null ? 0 : pdfDocument.PageCount;
        public PdfCollection<PdfPage> Pages => pdfDocument?.Pages;

        public PdfModel(PdfDocument pdfDocument)
        {
            this.pdfDocument = pdfDocument;
            SetUpDocoticLicense();
        }
        
        public PdfModel() 
        {
            pdfDocument = new PdfDocument();
            SetUpDocoticLicense();
        }

        private void SetUpDocoticLicense()
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("A74K7-4VGJW-T60RT-IIEXT-P5UHP");
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

        public void Compress(bool removeDuplicateObjects, bool compressImages, bool removeUnusedFonts, bool removeMetadata,
            bool removeStructureInformation, bool removeUnusedResources, bool removeApplicationData, bool flattenControls,
            double? compressImagesMaxRatio = null)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            if (removeDuplicateObjects)
                pdfDocument.ReplaceDuplicateObjects();
            if (compressImages)
                OptimizeImages(compressImagesMaxRatio);
            if (removeUnusedFonts)
                pdfDocument.RemoveUnusedFontGlyphs();
            if (removeMetadata)
            {
                XmpMetadata xmp = pdfDocument.Metadata;
                xmp.Unembed();
                pdfDocument.Info.Clear(false);
            }
            if (removeStructureInformation)
                pdfDocument.RemoveStructureInformation();
            if (removeUnusedResources)
                pdfDocument.RemoveUnusedResources();
            if (removeApplicationData)
                pdfDocument.RemovePieceInfo();
            if (flattenControls)
                pdfDocument.FlattenControls();
        }

        private void OptimizeImages(double? compressImagesMaxRatio = null)
        {
            var alreadyCompressedImageIds = new HashSet<string>();
            for (int i = 0; i < pdfDocument.PageCount; ++i)
            {
                var page = pdfDocument.Pages[i];
                foreach (var painted in page.GetPaintedImages())
                {
                    var image = painted.Image;

                    // Image ID may be null for inline images, which cannot be recompressed
                    if (image.Id == null)
                        continue;

                    if (alreadyCompressedImageIds.Contains(image.Id))
                        continue;

                    if (OptimizeImage(painted, compressImagesMaxRatio))
                        alreadyCompressedImageIds.Add(image.Id);
                }
            }
        }

        private static bool OptimizeImage(PdfPaintedImage painted, double? compressImagesMaxRatio = null)
        {
            var image = painted.Image;

            // inline images cannot be recompressed unless you move them to resources
            // using PdfCanvas.MoveInlineImagesToResources
            if (image.IsInline)
                return false;

            // mask images are not good candidates for recompression
            if (image.IsMask || image.Width < 8 || image.Height < 8)
                return false;

            // get size of the painted image
            var width = Math.Max(1, (int)painted.Bounds.Width);
            var height = Math.Max(1, (int)painted.Bounds.Height);

            // calculate resize ratio
            var ratio = Math.Min(image.Width / (double)width, image.Height / (double)height);

            if (ratio <= 1)
            {
                // the image size is smaller than the painted size
                return RecompressImage(image);
            }

            if (ratio < 1.1)
            {
                // with the ratio this small, the potential size
                // reduction rarely justifies resizing artefacts
                return false;
            }

            if (image.Compression == PdfImageCompression.Group4Fax ||
                image.Compression == PdfImageCompression.Group3Fax ||
                image.Compression == PdfImageCompression.JBig2 ||
                (image.ComponentCount == 1 && image.BitsPerComponent == 1))
            {
                return ResizeBilevelImage(image, ratio);
            }

            if (compressImagesMaxRatio.HasValue)
                ratio = Math.Min(ratio, compressImagesMaxRatio.Value);
            var resizedWidth = (int)Math.Floor(image.Width / ratio);
            var resizedHeight = (int)Math.Floor(image.Height / ratio);
            if ((image.ComponentCount >= 3 && image.BitsPerComponent == 8) || IsGrayJpeg(image))
            {
                image.ResizeTo(resizedWidth, resizedHeight, PdfImageCompression.Jpeg, 90);
                // or image.ResizeTo(resizedWidth, resizedHeight, PdfImageCompression.Jpeg2000, 10);
            }
            else
            {
                image.ResizeTo(resizedWidth, resizedHeight, PdfImageCompression.Flate, 9);
            }

            return true;
        }

        private static bool IsGrayJpeg(PdfImage image)
        {
            var isJpegCompressed = image.Compression == PdfImageCompression.Jpeg ||
                image.Compression == PdfImageCompression.Jpeg2000;
            return isJpegCompressed && image.ComponentCount == 1 && image.BitsPerComponent == 8;
        }

        private static bool RecompressImage(PdfImage image)
        {
            if (image.ComponentCount == 1 &&
                image.BitsPerComponent == 1 &&
                image.Compression == PdfImageCompression.Group3Fax)
            {
                image.RecompressWithGroup4Fax();
                return true;
            }

            if (image.BitsPerComponent == 8 &&
                image.ComponentCount >= 3 &&
                image.Compression != PdfImageCompression.Jpeg &&
                image.Compression != PdfImageCompression.Jpeg2000)
            {
                if (image.Width < 64 && image.Height < 64)
                {
                    // JPEG better preserves detail on smaller images
                    image.RecompressWithJpeg();
                }
                else
                {
                    // you can try a larger compressio ratio for bigger images
                    image.RecompressWithJpeg2000(10);
                }

                return true;
            }

            return false;
        }

        private static bool ResizeBilevelImage(PdfImage image, double ratio)
        {
            // Fax documents usually look better if integer-ratio scaling is used
            // Fractional-ratio scaling introduces more artifacts
            var intRatio = (int)ratio;

            // decrease the ratio when it is too high
            if (intRatio > 3)
                intRatio = Math.Min(intRatio - 2, 3);

            if (intRatio == 1 && image.Compression == PdfImageCompression.Group4Fax)
            {
                // skipping the image, because the output size and compression are the same
                return false;
            }

            image.ResizeTo(image.Width / intRatio, image.Height / intRatio, PdfImageCompression.Group4Fax);
            return true;
        }

        protected int FromG1PageIndex(int value)
        {
              if (value < 1 || value > PageCount)
                throw new ArgumentOutOfRangeException($"Page index need to be in range 1-{PageCount}");
            return value - 1;
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

            srcIndex = FromG1PageIndex(srcIndex);
            dstIndex = FromG1PageIndex(dstIndex);
            pdfDocument.MovePage(srcIndex, dstIndex);
        }

        public void SwapPages(int firstIndex, int secondIndex)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            firstIndex = FromG1PageIndex(firstIndex);
            secondIndex = FromG1PageIndex(secondIndex);
            pdfDocument.SwapPages(firstIndex, secondIndex);
        }

        public void RemovePage(int index)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            index = FromG1PageIndex(index);
            pdfDocument.RemovePage(index);
        }

        public PdfModel ExtractPages(int[] pageIndexes)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var indexes = pageIndexes.Select(x => FromG1PageIndex(x));
            return new PdfModel(pdfDocument.CopyPages(indexes.ToArray()));
        }

        public void AddPage(int? index = null, string orientation = "Portrait")
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            if (!Enum.TryParse<PdfPaperOrientation>(orientation, out var pageOrientation))
                throw new ApplicationException($"{orientation} is not correct page orientation");

            PdfPage newPage = null;
            if (index.HasValue)
            {
                var page = FromG1PageIndex(index.Value);
                newPage = pdfDocument.InsertPage(page);
            }
            else
                newPage = pdfDocument.AddPage();
            if (newPage != null)
                newPage.Orientation = pageOrientation;
        }

        public void DrawText(int pageIndex, Point point, string text, PdfFontModel font = null)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            pageIndex = FromG1PageIndex(pageIndex);
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

            pageIndex = FromG1PageIndex(pageIndex);
            var page = pdfDocument.GetPage(pageIndex);
            var canvas = page.Canvas;
            canvas.CurrentPosition = new PdfPoint(from.X, from.Y);
            canvas.DrawLineTo(new PdfPoint(to.X, to.Y));
        }

        private Size FitSizeWithin(Size inner, Size outer)
        {
            var innerAspectRatio = inner.Width / (float)inner.Height;
            var outerAspectRatio = outer.Width / (float)outer.Height;

            var resizeFactor = (innerAspectRatio >= outerAspectRatio) ?
                (outer.Width / (float)inner.Width) : (outer.Height / (float)inner.Height);

            var newWidth = inner.Width * resizeFactor;
            var newHeight = inner.Height * resizeFactor;
            return new Size((int)newWidth, (int)newHeight);
        }

        private Size CalculateImageSize(PdfImage image, Rectangle rectangle, PdfImageScaleMode scaleMode)
        {
            switch (scaleMode)
            {
                case PdfImageScaleMode.NeverScale:
                    break;
                case PdfImageScaleMode.AlwaysScale:
                    return FitSizeWithin(new Size(image.Width, image.Height), rectangle.Size);
                case PdfImageScaleMode.ScaleWhenImageBigger:
                    if (image.Width > rectangle.Width || image.Height > rectangle.Height)
                        return FitSizeWithin(new Size(image.Width, image.Height), rectangle.Size);
                    break;
                case PdfImageScaleMode.ScaleWhenImageSmaller:
                    if (image.Width < rectangle.Width || image.Height < rectangle.Height)
                        return FitSizeWithin(new Size(image.Width, image.Height), rectangle.Size);
                    break;
            }
            return new Size(image.Width, image.Height);
        }

        public void DrawImage(int pageIndex, Rectangle rect, string imageFilename, PdfImageScaleMode scaleMode)
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            pageIndex = FromG1PageIndex(pageIndex);
            var image = pdfDocument.AddImage(imageFilename);
            var page = pdfDocument.GetPage(pageIndex);
            var canvas = page.Canvas;
            var size = CalculateImageSize(image, rect, scaleMode);
            canvas.DrawImage(image, new PdfPoint(rect.X, rect.Y), new PdfSize(size.Width, size.Height), 0);
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

        public string ExtractText(int? pageIndex = null, string ocrLanguage = "eng")
        {
            return ExtractText(pageIndex, false, ocrLanguage);
        }

        public string ExtractTextWithFormatting(int? pageIndex = null, string ocrLanguage = "eng")
        {
            return ExtractText(pageIndex, true, ocrLanguage);
        }

        private string ExtractText(int? pageIndex = null, bool withFormatting = false, string ocrLanguage = "eng")
        {
            if (!IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            var options = new PdfTextExtractionOptions()
            {
                WithFormatting = withFormatting
            };
            var pagesRange = pageIndex != null && pageIndex >= 0 
                ? Enumerable.Range(FromG1PageIndex(pageIndex.Value), 1)
                : Enumerable.Range(0, pdfDocument.PageCount);

            var documentText = new StringBuilder();
            foreach (var pageNo in pagesRange)
            {
                if (documentText.Length > 0)
                    documentText.Append("\r\n");
                var page = pdfDocument.GetPage(pageNo);
                var searchableText = page.GetText(options);
                if (!string.IsNullOrEmpty(searchableText.Trim()))
                {
                    documentText.Append(searchableText);
                }
                else
                {
                    documentText.Append(ExtractTextWithOCR(pageNo, withFormatting, ocrLanguage));
                }
            }
            return documentText.ToString();
        }

        public void AppendDocument(PdfModel otherPdf, List<int> pagesToAppend = null)
        {
            if (!IsDocumentCorrect || !otherPdf.IsDocumentCorrect)
                throw new ApplicationException("Pdf documernt is not correct");

            using (var streamToAppend = new MemoryStream())
            {
                var pdfToAppend = otherPdf.pdfDocument;
                if (pagesToAppend != null && pagesToAppend.Count > 0)
                {
                    var pages = pagesToAppend.Select(x => otherPdf.FromG1PageIndex(x)).ToList();
                    pdfToAppend = otherPdf.pdfDocument.CopyPages(pages.ToArray());
                }
                var options = new PdfSaveOptions
                {
                    UseObjectStreams = false
                };
                pdfToAppend.Save(streamToAppend, options);

                pdfDocument.Append(streamToAppend);
                pdfDocument.ReplaceDuplicateObjects();
            }
        }

        private static TesseractEngine GetTesseractEngine(string language, bool with_formatting)
        {
            var tessdata_path = OcrOfflineHelper.OcrModelsFolder;
            if (with_formatting)
            {
                var initVars = new Dictionary<string, object>() { { "preserve_interword_spaces", true }, };
                return new TesseractEngine(tessdata_path, language, EngineMode.Default, new string[0], initVars, false);

            }
            else
                return new TesseractEngine(tessdata_path, language, EngineMode.Default);
        }

        private string ExtractTextWithOCR(int pageNo, bool withFormatting = false, string ocrLanguage = "eng")
        {
            if (string.IsNullOrEmpty(ocrLanguage))
                return string.Empty;

            var options = PdfDrawOptions.Create();
            options.BackgroundColor = new PdfRgbColor(255, 255, 255);
            options.HorizontalResolution = 300;
            options.VerticalResolution = 300;

            var page = pdfDocument.GetPage(pageNo);
            using (var pageImage = new MemoryStream())
            {
                page.Save(pageImage, options);

                using (var engine = GetTesseractEngine(ocrLanguage, withFormatting))
                {
                    using (Pix img = Pix.LoadFromMemory(pageImage.GetBuffer()))
                    {
                        using (var recognizedPage = engine.Process(img))
                        {
                            return recognizedPage.GetText();
                        }
                    }
                }
            }
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
