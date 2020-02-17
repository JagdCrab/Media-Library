using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Data;
using System.Data.SQLite;
using System.Configuration;

namespace Media_Library.Data
{
    class ClipAccesser
    {
        #region Common

        private static Accesser accesser = Accesser.Instance;

        #endregion

        #region Getters

        public static SearchEntityCollection GetSearchEntities()
        {
            var entities = new SearchEntityCollection();

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Text] From [ClipTags] Where [Deleted] = 0;";
                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Tag", reader.GetString(0)));
                }
            }

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Author] From [ClipRecords] Where [Deleted] = 0;";
                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Author", reader.GetString(0)));
                }
            }

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Music] From [ClipRecords] Where [Deleted] = 0;";
                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Music", reader.GetNullableString(0)));
                }
            }

            return entities;
        }

        public static SearchEntityCollection GetSearchEntities(ClipRecord _clip)
        {
            var entities = new SearchEntityCollection();

            using (var command = new SQLiteCommand())
            {
                command.CommandText = @"Select distinct [Text] From [ClipTags] Where [Deleted] = 0 and [Cid] = @cid;";
                command.Parameters.Add(new SQLiteParameter("@cid") { DbType = DbType.String, Value = _clip.Cid });

                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Tag", reader.GetString(0)));
                }
            }

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Author] From [ClipRecords] Where [Deleted] = 0 and [Cid] = @cid;";
                command.Parameters.Add(new SQLiteParameter("@cid") { DbType = DbType.String, Value = _clip.Cid });

                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Author", reader.GetString(0)));
                }
            }

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Music] From [ClipRecords] Where [Deleted] = 0 and [Cid] = @cid;";
                command.Parameters.Add(new SQLiteParameter("@cid") { DbType = DbType.String, Value = _clip.Cid });

                using (var reader = accesser.ExecuteReader(command))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            entities.Add(new SearchEntity("Music", reader.GetNullableString(0)));
                }
            }

            return entities;
        }

        public static List<string> GetClipTagsAutoComplete()
        {
            var result = new List<string>();

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select distinct [Text] From [ClipTags] Where [Deleted] = 0;";
                using (var reader = accesser.ExecuteReader(command))
                {
                    if (!reader.HasRows)
                        return result;

                    while (reader.Read())
                        result.Add(reader.GetNullableString(0));
                }
            }

            return result;
        }


        public static ClipRecordCollection GetClipCollection()
        {
            var records = new ClipRecordCollection();

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select [Cid], [File_Path], [File_Name], [File_Size], [Alias], [Source_Name], [Music], [Source_Code], [Author], [Submit_Date], [Icon], [Score], [Favorite], [Duration], [Intensity], [Last_Playback], [Format], [Resolution], [Checksum], [Inserted] From [ClipRecords];";

                using (var reader = accesser.ExecuteReader(command))
                {

                    if (!reader.HasRows)
                        return records;

                    while (reader.Read())
                    {
                        var record = new ClipRecord();

                        record.Cid = reader.GetString(0);
                        record.File_Path = reader.GetNullableString(1);
                        record.File_Name = reader.GetNullableString(2);
                        record.File_Extention = reader.GetNullableString(3);
                        record.File_Size = reader.GetInt64(4);
                        record.Alias = reader.GetNullableString(5);
                        record.Source_Name = reader.GetNullableString(6);
                        record.Music = reader.GetNullableString(7);
                        record.Source_Code = reader.GetNullableString(8);
                        record.Author = reader.GetNullableString(9);
                        record.Submit_Date = reader.GetDateTime(10);
                        record.Icon = reader.GetBitmap(11);
                        record.Score = reader.GetInt32(12);
                        record.Favorite = reader.GetBoolean(13);
                        record.Duration = reader.GetTimeSpan(14);
                        record.Intensity = reader.GetNullableString(15);
                        record.Last_playback = reader.GetDateTime(16);
                        record.Format = reader.GetNullableString(17);
                        record.Resolution = reader.GetNullableString(18);
                        record.Checksum = reader.GetNullableString(19);
                        record.Inserted = reader.GetDateTime(20);

                        records.Add(record);
                    }
                }
            }

            return records;
        }

        public static ClipTagCollection GetClipTags(ClipRecord _clip)
        {
            var tags = new ClipTagCollection();

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select [Id], [Cid], [Text], [Intensity] From [ClipTags] Where [Deleted] = 0 and [Cid] = @cid;";
                command.Parameters.Add(new SQLiteParameter("@cid") { DbType = DbType.String, Value = _clip.Cid });

                using (var reader = accesser.ExecuteReader(command))
                {
                    if (!reader.HasRows)
                        return tags;

                    while (reader.Read())
                    {
                        var tag = new ClipTag(_clip);

                        tag.Id = reader.GetInt32(0);
                        tag.Cid = reader.GetString(1);
                        tag.Text = reader.GetString(2);
                        tag.Intensity = reader.GetIntensity(3);

                        tags.Add(tag);
                    }
                }
            }

            return tags;
        }

        public static ClipScreenlist GetClipScreenlist(ClipRecord _record)
        {
            var screenlist = new ClipScreenlist(_record);

            using (var command = new SQLiteCommand())
            {
                command.CommandText = "Select [Id], [Cid], [Screenlist] From [ClipScreenlists] Where [Deleted] = 0 and [Vid] = @cid;";
                command.Parameters.Add(new SQLiteParameter("@cid") { DbType = DbType.String, Value = _record.Cid });

                using (var reader = accesser.ExecuteReader(command))
                {
                    if (!reader.HasRows)
                        return screenlist;
                    else
                    {
                        reader.Read();
                        screenlist.Id = reader.GetInt32(0);
                        screenlist.Cid = reader.GetString(1);
                        screenlist.Screenlist = reader.GetBitmap(2);
                    }
                }
            }

            return screenlist;
        }

        #endregion

        #region Setters
        #endregion
    }
}
