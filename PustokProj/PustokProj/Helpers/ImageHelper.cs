using PustokProj.Models;
using System.Reflection;

namespace PustokProj.Helpers
{
    public static class ImageHelper
    {
        public static (bool, string?) Checker(IFormFile file)
        {
            bool check = true;
            string msg = null;
            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                check = false;
                msg = "Image type must be PNG or JPG/JPEG";
            }
            else if (file.Length > 5242880)
            {
                check = false;
                msg = "Image size must be less than 5 MB";
            }
            return (check, msg);
        }

        public static string SaveFile(string webPath, string folderPath, IFormFile file)
        {
            string oldName = file.FileName;
            if (oldName.Length > 64) oldName = oldName.Substring(oldName.Length - 64, 64);
            string name = Guid.NewGuid().ToString() + oldName;
            string path = Path.Combine(webPath, folderPath, name);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fs);
            }
            return name;
        }

        public static void DeleteFile(string webPath, string folderPath, string name)
        {
            string path = Path.Combine(webPath, folderPath, name);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        }
    }
}
