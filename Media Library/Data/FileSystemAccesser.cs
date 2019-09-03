using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Library.Data
{
    class FileSystemAccesser
    {
        public static List<FileInfo> ScanFolderForVideoFiles(string _folder)
        {
            var result = new List<FileInfo>();

            if (!Directory.Exists(_folder))
                throw new DirectoryNotFoundException();

            var rawFilesList = Directory.EnumerateFiles(_folder,"*.*",SearchOption.AllDirectories);
            //var rawFilesList = Directory.EnumerateFiles(_folder);

            foreach (var file in rawFilesList)
            {
                if(new string[] { ".mp4", ".m4v", ".mov", ".avi", ".wmv", ".mkv" }.Contains(Path.GetExtension(file)))
                    result.Add(new FileInfo(file));
            }

            return result;
        }
    }
}
