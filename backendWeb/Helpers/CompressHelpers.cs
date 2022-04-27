using Ionic.Zip;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace backendWeb.Helpers
{
    /// <summary>
    /// 解壓縮/壓縮功能共同介面
    /// </summary>
    public abstract class CompressHelpers
    {
        /// <summary>
        /// 壓縮
        /// </summary>
        public abstract byte[] Compression(Dictionary<string, Stream> dic, string password = null);
        /// <summary>
        /// 解壓縮
        /// </summary>
        public abstract void UnCompress(string zipFileName, string targetPath, string password = null, string reName = null);
        // 取得檔名(去除路徑)
        protected string GetBasename(string fullName)
        {
            string result;
            int lastBackSlash = fullName.LastIndexOf("\\");
            result = fullName.Substring(lastBackSlash + 1);

            return result;
        }
    }
    /// <summary>
    /// DotNetZip解壓縮/壓縮功能
    /// </summary>
    public class DotNetZipHelpers : CompressHelpers
    {
        /// <summary>
        /// 壓縮
        /// </summary>
        public override byte[] Compression(Dictionary<string, Stream> dic, string password = null)
        {
            MemoryStream ms = new MemoryStream();
            using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
            {
                if (password != null && password != string.Empty) zip.Password = password;
                foreach (var d in dic)
                {
                    zip.AddEntry(d.Key, d.Value);
                }
                zip.Save(ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 解壓縮
        /// </summary>
        public override void UnCompress(string zipFileName, string targetPath, string password = null, string reName = null)
        {
            // 解壓縮檔案，傳入參數： 來源壓縮檔, 解壓縮後的目的路徑
            ReadOptions options = new ReadOptions();
            options.Encoding = Encoding.UTF8;
            ZipFile unzip = ZipFile.Read(zipFileName, options);
            if (password != null && password != string.Empty) unzip.Password = password;
            //若目的路徑不存在，則先建立路徑
            DirectoryInfo di = new DirectoryInfo(targetPath);
            if (!di.Exists)
                di.Create();
            if (string.IsNullOrEmpty(reName))
            {
                unzip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                unzip.FlattenFoldersOnExtract = true;
                unzip.ExtractAll(targetPath);
            }
            else
            {
                for (int i = 0; i < unzip.Entries.Count; i++)
                {
                    ZipEntry zips = unzip[i];
                    zips.FileName = $"{reName}_{(i).ToString("000")}.{zips.FileName.Split('.')[zips.FileName.Split('.').Length - 1]}";
                    zips.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            #region 註解
            //foreach (ZipEntry e in unzip)
            //{
            //    e.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently);
            //}
            //using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFileName)))
            //{
            //    // 若目的路徑不存在，則先建立路徑
            //    DirectoryInfo di = new DirectoryInfo(targetPath);

            //    if (!di.Exists)
            //        di.Create();

            //    ZipEntry theEntry;

            //    // 逐一取出壓縮檔內的檔案(解壓縮)
            //    while ((theEntry = s.GetNextEntry()) != null)
            //    {
            //        int size = 2048;
            //        byte[] data = new byte[2048];

            //        //Console.WriteLine("正在解壓縮: " + GetBasename(theEntry.FileName));

            //        // 寫入檔案
            //        using (FileStream fs = new FileStream(di.FullName + "\\" + GetBasename(theEntry.FileName), FileMode.Create))
            //        {
            //            while (true)
            //            {
            //                size = s.Read(data, 0, data.Length);

            //                if (size > 0)
            //                    fs.Write(data, 0, size);
            //                else
            //                    break;
            //            }

            //        }
            //    }
            //}
            #endregion

        }
    }
}