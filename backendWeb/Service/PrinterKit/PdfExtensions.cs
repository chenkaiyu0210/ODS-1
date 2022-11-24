using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrinterKit
{
    public static class PdfExtensions
    {
        //private static string str_Path = Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).ToString().Replace(@"file:\", "");
        //private static string tempFilePath = str_Path + @"" + ConfigurationManager.AppSettings["ConvertTemp"].ToString();
        //private static string templateFilePath = str_Path + @"" + ConfigurationManager.AppSettings["Template"].ToString();

        public static byte[] TemplateToPdf(this PdfTemplate template)
        {
            //string CopyFile = tempFilePath + "Temp" + Guid.NewGuid().ToString() + ".pdf";
            //if (File.Exists(CopyFile)) { File.Delete(CopyFile); }
            //File.Copy(templateFilePath + template.FileName, CopyFile);
            string CopyFile = template.TargetFilePath + "Temp" + Guid.NewGuid().ToString() + ".pdf";
            if (File.Exists(CopyFile)) { File.Delete(CopyFile); }
            File.Copy(template.SourceFilePath + template.FileName, CopyFile);

            using (var outputStream = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, outputStream))
                    {
                        if (!string.IsNullOrWhiteSpace(template.UserPw) && !string.IsNullOrWhiteSpace(template.OwrPw))
                            writer.SetEncryption(false, template.UserPw, template.OwrPw, PdfWriter.ALLOW_SCREENREADERS);
                        doc.Open();
                        var contentByte = writer.DirectContent;
                        var fonts = new List<Font>();
                        if (template.FontPaths != null)
                        {
                            foreach (var fontPath in template.FontPaths)
                            {
                                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                var font = new Font(baseFont, 40, Font.NORMAL, new BaseColor(51, 0, 153));
                                fonts.Add(font);
                            }
                        }
                        var pages = template.Pages;
                        foreach (var page in pages)
                        {
                            using (var ms = new MemoryStream())
                            {
                                using (var reader = new PdfReader(CopyFile))
                                {
                                    using (var stamper = new PdfStamper(reader, ms))
                                    {
                                        foreach (var font in fonts)
                                        {
                                            stamper.AcroFields.AddSubstitutionFont(font.BaseFont);
                                        }
                                        var fields = stamper.AcroFields;
                                        var items = page.Items;
                                        foreach (var item in items)
                                        {
                                            switch (item.Type)
                                            {
                                                case PdfItemType.Text:
                                                    {
                                                        fields.SetField(item.Key, item.Value);
                                                        //var baseFont = BaseFont.CreateFont(template.FontPaths.FirstOrDefault().ToString(), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                                                        //var font = new Font(baseFont, 1);
                                                        //fields.SetFieldProperty(item.Key, "textsize", font, null);
                                                    }
                                                    break;

                                                case PdfItemType.FontColor:
                                                    {
                                                        var baseColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml(item.Value));
                                                        fields.SetFieldProperty(item.Key, "textcolor", baseColor, null);
                                                    }
                                                    break;

                                                case PdfItemType.Barcode39:
                                                    {
                                                        var rect = fields.GetFieldPositions(item.Key)[0].position;
                                                        var pdfContentByte = stamper.GetOverContent(1);
                                                        var barcode = new Barcode39();
                                                        barcode.Code = item.Value;
                                                        barcode.AltText = item.Value;
                                                        barcode.StartStopText = true;
                                                        barcode.X = 0.8f;
                                                        barcode.InkSpreading = 0f;
                                                        barcode.N = 2f;
                                                        //barcode.Size = 10f;
                                                        //barcode.Baseline = 10f;
                                                        barcode.BarHeight = 20f;
                                                        barcode.GenerateChecksum = false;
                                                        barcode.ChecksumText = false;
                                                        if (item.Value.Length >= 20)
                                                        {
                                                            barcode.X = 0.7f;
                                                            barcode.N = 1.5f;
                                                        }
                                                        var image = barcode.CreateImageWithBarcode(pdfContentByte, BaseColor.BLACK, BaseColor.BLACK);
                                                        image.SetAbsolutePosition(rect.Left, rect.Bottom);
                                                        pdfContentByte.AddImage(image);
                                                    }
                                                    break;

                                                case PdfItemType.RadioButton:
                                                    {
                                                        fields.SetField(item.Key, item.Value, true);
                                                    }
                                                    break;

                                                case PdfItemType.CheckBox:
                                                    {
                                                        fields.SetField(item.Key, item.Value, true);
                                                    }
                                                    break;

                                                case PdfItemType.Image:
                                                    {
                                                        var image = iTextSharp.text.Image.GetInstance(item.Value);
                                                        var pushbuttonField = fields.GetNewPushbuttonFromField(item.Key);
                                                        pushbuttonField.Layout = PushbuttonField.LAYOUT_ICON_ONLY;
                                                        pushbuttonField.ProportionalIcon = true;
                                                        pushbuttonField.Image = image;
                                                        fields.ReplacePushbuttonField(item.Key, pushbuttonField.Field);
                                                    }
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        stamper.FormFlattening = true;
                                    }
                                }
                                using (var copyReader = new PdfReader(ms.ToArray()))
                                {
                                    for (var i = 1; i <= copyReader.NumberOfPages; i++)
                                    {
                                        //取得範本頁面大小
                                        doc.SetPageSize(copyReader.GetPageSize(i));

                                        doc.NewPage();
                                        var newPage = writer.GetImportedPage(copyReader, i);
                                        contentByte.AddTemplate(newPage, 0, 0);
                                    }

                                    writer.FreeReader(copyReader);
                                }
                            }
                        }
                        doc.Close();
                        byte[] pdf_byte = outputStream.ToArray();
                        File.Delete(CopyFile);
                        return pdf_byte;
                    }
                }
            }
        }

        /// <summary>
        /// pdf加密
        /// </summary>
        /// <param name="SrcPath">來源</param>
        /// <param name="OutPath">輸出</param>
        /// <param name="strength">強度(高:安全,但耗時)</param>
        /// <param name="UserPw">user密碼</param>
        /// <param name="OwrPw">owner密碼</param>
        /// <param name="pmss">權限(ex. PdfWriter.ALLOW_SCREENREADERS)</param>
        public static void EncryptPDF(byte[] Pdffiles, string OutPath, bool strength, string UserPw, string OwrPw, int pmss)
        {
            using (PdfReader reader = new PdfReader(Pdffiles))
            {
                using (var os = new FileStream(OutPath, FileMode.Create))
                {
                    PdfEncryptor.Encrypt(reader, os, strength, UserPw, OwrPw, pmss);
                }
            }
        }
    }

    public class PdfTemplate
    {
        public IEnumerable<string> FontPaths { get; set; }
        public List<PdfPage> Pages { get; set; }
        public string FileName { get; set; }
        public string UserPw { get; set; }
        public string OwrPw { get; set; }
        public string TargetFilePath { get; set; }
        public string SourceFilePath { get; set; }
    }

    public class PdfPage
    {
        //public string TemplateFileName { get; set; }
        public List<PdfItem> Items { get; set; }
    }

    public class PdfItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public PdfItemType Type { get; set; }
    }

    public enum PdfItemType
    {
        Text,
        FontColor,
        Barcode39,
        Image,
        RadioButton,
        CheckBox
    }
}