using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data.SQLite;


namespace Media_Library.Data
{
    static class AccesserExtentions
    {
        internal static int GetInt(this bool b)
        {
            if (b)
                return 1;
            else
                return 0;
        }

        internal static BitmapSource GetBitmap(this SQLiteDataReader reader, int colIndex)
        {
            using (var stream = reader.GetStream(colIndex))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                bitmap.Freeze();

                return bitmap;
            }
        }

        internal static byte[] GetByteArray(this BitmapSource bitmapSource)
        {
            byte[] bytes;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder() { QualityLevel = 100 };
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
                bytes = stream.ToArray();
                stream.Close();
            }
            return bytes;
        }

        internal static TimeSpan GetTimeSpan(this SQLiteDataReader reader, int colIndex)
        {
            string seconds = reader.GetString(colIndex);
            return TimeSpan.Parse(seconds);
        }

        internal static string GetNullableString(this SQLiteDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            else
                return string.Empty;
        }

        internal static MediaType GetMediaType(this SQLiteDataReader reader, int colIndex)
        {
            string value = reader.GetString(colIndex);
            return (MediaType)Enum.Parse(typeof(MediaType), value);
        }

        internal static Intensity GetIntensity(this SQLiteDataReader reader, int colIndex)
        {
            string value = reader.GetString(colIndex);
            return (Intensity)Enum.Parse(typeof(Intensity), value);
        }
    }
}
