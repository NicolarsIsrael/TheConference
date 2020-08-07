using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Services
{
    public class FileService
    {
        public static string SaveDoc(IFormFile file, string folderName)
        {
            try
            {
                string ext = Path.GetExtension(file.FileName);
                if (!CheckIfFileIsADoc(ext.ToLower()))
                    throw new Exception();

                string uniqueFileName = "";
                string uploadsFolder = "wwwroot/Files/" + folderName;
                Directory.CreateDirectory(uploadsFolder);
                uniqueFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + ext;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));

                return filePath;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void DeleteFile(string path)
        {
            //string path = Request.MapPath(filePath);
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

        private static bool CheckIfFileIsADoc(string extenstion)
        {
            if (extenstion == ".doc" || extenstion == ".docx" || extenstion == ".pdf" || extenstion == ".txt" || extenstion == ".xls"
                || extenstion == ".xlsx" || extenstion == ".ppt" || extenstion == ".pptx")
                return true;

            return false;
        }

        private static bool CheckFileSize(IFormFile file)
        {
            if (file.Length > (4 * 1000 * 1024))
                return false;

            return true;

        }

    }
}
