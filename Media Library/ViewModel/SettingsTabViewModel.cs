using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Threading;

using Media_Library.Components;
using Media_Library.Data;

using System.Data.SQLite;

namespace Media_Library.ViewModel
{
    class SettingsTabViewModel
    {
        public ObservableCollection<MissingVideoFile> MissingVideoFiles { get; }

        public Observable<string> FolderToScan { get; set; }

        public Command ScanFolderForVideoFiles {
            get {
                return new Command(new Action(() => {

                    if (!Directory.Exists(FolderToScan.Value))
                        return;

                    var rawVideoList = FileSystemAccesser.ScanFolderForVideoFiles(FolderToScan.Value);
                    Task.Factory.StartNew(new Action(() => getMissingVideoFiles(rawVideoList)));
                }));
            }
        }

        public SettingsTabViewModel()
        {
            FolderToScan = new Observable<string>();
            MissingVideoFiles = new ObservableCollection<MissingVideoFile>();
        }

        private void getMissingVideoFiles(List<FileInfo> _rawVideoList)
        {
            var presentVideoFiles = new List<string>();
            var crc64 = new Crc64Iso();
            
            using (var connection = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Primary"].ToString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [File_Path] From [VideoRecords] Where [Deleted] = 0;";

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            presentVideoFiles.Add(reader.GetString(0));
                }
            }

            foreach (var file in _rawVideoList)
            {
                if (!presentVideoFiles.Contains(file.FullName))
                {
                    //string hash = string.Empty;

                    //using (var fs = File.Open(file.FullName, FileMode.Open))
                    //    foreach (var b in crc64.ComputeHash(fs))
                    //        hash += b.ToString("x2").ToLower();

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                        MissingVideoFiles.Add(new MissingVideoFile(file));
                    }));
                }
            }
        }
    }

    public class MissingVideoFile
    {
        public string Path { get; }
        public string Directory { get; }
        public string Name { get; }
        public string Size { get; }

        public Command AddVideoFile { get; }

        public MissingVideoFile(FileInfo _fileInfo)
        {
            Path = _fileInfo.FullName;
            Directory = _fileInfo.Directory.Name;
            Name = _fileInfo.Name;
            Size = _fileInfo.Length.ToString();

            AddVideoFile = new Command(new Action(() => {
                var transaction = VideoAccesser.CreateTransaction();
                var record = VideoAccesser.CreateNewVideoRecord(transaction);
                transaction.Rollback();
            }));
        }
    }
}
