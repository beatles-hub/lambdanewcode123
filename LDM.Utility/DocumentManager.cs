using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Utility
{
    public static class DocumentManager
    {
        //public static string SaveDocument()
        //{

        //}
        public static string CombineFilePath(string sourcePath, string filename)
        {
            string destPath = Path.Combine(sourcePath, filename);
            return destPath;
        }
        public static bool CreateFolder(string sourcePath, string folderName, out string folderPath)
        {
            bool isSuccess = true;
            folderPath = Path.Combine(sourcePath, folderName)+Path.DirectorySeparatorChar;
            if (!CheckFolderIsExist(folderPath))
            {               
               Directory.CreateDirectory(folderPath);              
            }
            return isSuccess;
        }
        public static bool CheckFolderIsExist(string sourcePath)
        {
            bool isSuccess = false;
            if (Directory.Exists(sourcePath))
            {
                isSuccess = true;
            }
            return isSuccess;
        }

       
    }
}
