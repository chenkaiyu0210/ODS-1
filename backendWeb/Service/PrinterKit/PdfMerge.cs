using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;

namespace PrinterKit
{
    public class PdfMerge
    {
        /// <summary> 合併PDF檔(集合) </summary>
        /// <param name="fileList">欲合併PDF檔之集合(一筆以上)</param>
        public byte[] mergePDFFiles(List<byte[]> fileList)
        {
            //要合併的多個 PDF 文件
            byte[] pdf_result = null;
            using (var doc = new Document())
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = PdfWriter.GetInstance(doc, stream))
                    {
                        doc.Open();

                        var contentByte = writer.DirectContent;

                        foreach (var file in fileList)
                        {
                            using (var reader = new PdfReader(file))
                            {
                                for (var i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    //設定頁面大小為當前範本的頁面的大小
                                    doc.SetPageSize(reader.GetPageSize(i));
                                    //產生新頁面
                                    doc.NewPage();
                                    //將 Reader 轉為 PdfImportedPage
                                    var newPage = writer.GetImportedPage(reader, i);
                                    //插入新頁面
                                    contentByte.AddTemplate(newPage, 0, 0);
                                }
                                //釋放 reader
                                writer.FreeReader(reader);
                            }
                        }

                        doc.Close();
                    }

                    pdf_result = stream.ToArray();
                }
            }
            return pdf_result;
        }

        /// <summary> 合併PDF檔(集合) </summary>
        /// <param name="fileList">欲合併PDF檔之集合(一筆以上)</param>
        public byte[] mergePDFFiles(List<Stream> fileList)
        {
            //要合併的多個 PDF 文件
            byte[] pdf_result = null;
            using (var doc = new Document())
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = PdfWriter.GetInstance(doc, stream))
                    {
                        doc.Open();

                        var contentByte = writer.DirectContent;

                        foreach (var file in fileList)
                        {
                            using (var reader = new PdfReader(file))
                            {
                                for (var i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    //設定頁面大小為當前範本的頁面的大小
                                    doc.SetPageSize(reader.GetPageSize(i));
                                    //產生新頁面
                                    doc.NewPage();
                                    //將 Reader 轉為 PdfImportedPage
                                    var newPage = writer.GetImportedPage(reader, i);
                                    //插入新頁面
                                    contentByte.AddTemplate(newPage, 0, 0);
                                }
                                //釋放 reader
                                writer.FreeReader(reader);
                            }
                        }

                        doc.Close();
                    }

                    pdf_result = stream.ToArray();
                }
            }
            return pdf_result;
        }
    }
}