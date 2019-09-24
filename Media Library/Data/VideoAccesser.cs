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
    class VideoAccesser
    {
        #region Common

        private static SQLiteConnection CreateConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Primary"].ToString();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            return connection;
        }

        public static SQLiteTransaction CreateTransaction()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["Primary"].ToString();
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            return connection.BeginTransaction();
        }

        #endregion

        #region Getters

        public static SearchEntityCollection GetSearchEntities()
        {
            var entities = new SearchEntityCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Text] From [VideoTags] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Tag", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Series] From [VideoRecords] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Series", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Alt_Series] From [VideoRecords] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Alt Series", reader.GetNullableString(0)));
                    }
                }
            }

            return entities;
        }

        public static SearchEntityCollection GetSearchEntities(VideoSeries _series)
        {
            var entities = new SearchEntityCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"Select distinct [Text] From [VideoTags] t1 Inner Join [VideoRecords] t2 on t1.[Vid] = t2.[Vid] Where t1.[Deleted] = 0 and t2.[Series] = @series;";
                    command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series.Series });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Tag", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Series] From [VideoRecords] Where [Deleted] = 0 and [Series] = @series;";
                    command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series.Series });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Series", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Alt_Series] From [VideoRecords] Where [Deleted] = 0 and [Series] = @series;";
                    command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series.Series });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Alt_Series", reader.GetNullableString(0)));
                    }
                }
            }

            return entities;
        }

        public static List<string> GetVideoSeriesAutoComplete()
        {
            var result = new List<string>();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Series] From [VideoRecords] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return result;

                        while (reader.Read())
                            result.Add(reader.GetString(0));
                    }
                }
            }

            return result;
        }

        public static List<string> GetVideoAltSeriesAutoComplete()
        {
            var result = new List<string>();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Alt_Series] From [VideoRecords] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return result;

                        while (reader.Read())
                            result.Add(reader.GetNullableString(0));
                    }
                }
            }

            return result;
        }

        public static List<string> GetVideoTagsAutoComplete()
        {
            var result = new List<string>();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Text] From [VideoTags] Where [Deleted] = 0;";
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return result;

                        while (reader.Read())
                            result.Add(reader.GetNullableString(0));
                    }
                }
            }

            return result;
        }

        public static VideoSeriesCollection GetVideoSeries()
        {
            var collection = new VideoSeriesCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        Select [Series], [Alt_Series], [Icon], [Inserted]
                        From (
	                        Select
		                         Row_Number() Over (Partition by [Series] Order by Random()) as n
                                ,[Series]
                                ,[Alt_Series]
                                ,[Icon]
                                ,[Inserted]
                            From [VideoRecords]
                            Where Deleted = 0
                        ) t1
                        Where n = 1;";

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return collection;

                        while (reader.Read())
                        {
                            VideoSeries series = new VideoSeries();
                            
                            series.Series = reader.GetString(0);
                            series.Alt_Series = reader.GetNullableString(1);
                            series.Icon = reader.GetBitmap(2);
                            series.Inserted = reader.GetDateTime(3);

                            collection.Add(series);
                        }
                    }
                }
            }

            return collection;
        }

        public static VideoRecordCollection GetVideoRecords(VideoSeries _series)
        {
            var records = new VideoRecordCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Vid],[File_Path],[File_Name],[File_Extention],[File_Size],[Alias],[Alt_Alias],[Series],[Alt_Series],[Icon],[Score],[Favorite],[Duration],[Intensity],[Last_playback],[Format],[Resolution],[Checksum],[Inserted] From [VideoRecords] Where [Deleted] = 0 and [Series] = @series;";
                    command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series.Series });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return records;

                        while (reader.Read())
                        {
                            VideoRecord record = new VideoRecord();

                            record.Vid = reader.GetInt32(0);
                            record.File_Path = reader.GetString(1);
                            record.File_Name = reader.GetString(2);
                            record.File_Extention = reader.GetString(3);
                            record.File_Size = reader.GetInt64(4);
                            record.Alias = reader.GetString(5);
                            record.Alt_Alias = reader.GetNullableString(6);
                            record.Series = reader.GetString(7);
                            record.Alt_Series = reader.GetNullableString(8);
                            record.Icon = reader.GetBitmap(9);
                            record.Score = reader.GetInt32(10);
                            record.Favorite = reader.GetBoolean(11);
                            record.Duration = reader.GetTimeSpan(12);
                            record.Intensity = reader.GetNullableString(13);
                            record.Last_playback = reader.GetDateTime(14);
                            record.Format = reader.GetNullableString(15);
                            record.Resolution = reader.GetNullableString(16);
                            record.Checksum = reader.GetNullableString(17);
                            record.Inserted = reader.GetDateTime(18);

                            records.Add(record);
                        }
                    }
                }
            }

            return records;
        }

        public static VideoTagCollection GetVideoTags(VideoSeries _series)
        {
            var tags = new VideoTagCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Id], [Vid], [Text], [Intensity] From [VideoTags] Where [Deleted] = 0 and [Series] = @series;";
                    command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series.Series });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return tags;

                        while (reader.Read())
                        {
                            VideoTag tag = new VideoTag(_series);

                            tag.Id = reader.GetInt32(0);
                            tag.Vid = reader.GetInt32(1);
                            tag.Text = reader.GetString(2);
                            tag.Intensity = reader.GetIntensity(3);

                            tags.Add(tag);
                        }
                    }
                }
            }

            return tags;
        }

        public static VideoTagCollection GetVideoTags(VideoRecord _record)
        {
            var tags = new VideoTagCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Id], [Vid], [Text], [Intensity] From [VideoTags] Where [Deleted] = 0 and [Vid] = @vid;";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int32, Value = _record.Vid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return tags;

                        while (reader.Read())
                        {
                            VideoTag tag = new VideoTag(_record);

                            tag.Id = reader.GetInt32(0);
                            tag.Vid = reader.GetInt32(1);
                            tag.Text = reader.GetString(2);
                            tag.Intensity = reader.GetIntensity(3);

                            tags.Add(tag);
                        }
                    }
                }
            }

            return tags;
        }

        public static VideoScreenlist GetVideoScreenlist(VideoRecord _record)
        {
            var screenlist = new VideoScreenlist(_record);

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Id], [Vid], [Screenlist] From [VideoScreenlists] Where [Deleted] = 0 and [Vid] = @vid;";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int32, Value = _record.Vid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return screenlist;
                        else
                        {
                            reader.Read();
                            screenlist.Id = reader.GetInt32(0);
                            screenlist.Vid = reader.GetInt32(1);
                            screenlist.Screenlist = reader.GetBitmap(2);
                        }
                    }
                }
            }
            return screenlist;
        }

        public static VideoPlaylistCollection GetVideoPlaylist()
        {
            var result = new VideoPlaylistCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Pid], [Vid], [Alias], [Series], [Inserted] From [VideoPlaylist];";

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return result;

                        while(reader.Read())
                        {
                            var record = new VideoPlaylist();

                            record.Pid = reader.GetInt32(0);
                            record.Vid = reader.GetInt32(1);
                            record.Alias = reader.GetString(2);
                            record.Series = reader.GetString(3);
                            record.Inserted = reader.GetDateTime(4);

                            result.Add(record);
                        }
                    }
                }
            }

            return result;
        }

        public static bool CheckIfInPlaylist(VideoRecord _videoRecord)
        {
            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select [Pid] From [VideoPlaylist] Where [Vid] = @vid Limit 1;";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int32, Value = _videoRecord.Vid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return true;
                        else
                            return false;
                    }
                }
            }
        }

        #endregion

        #region Setters
        public static long CreateNewRecord(SQLiteTransaction _transaction)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Insert into [VideoRecords] ([Inserted],[Deleted]) Values (@inserted, @deleted);";
                command.Parameters.Add(new SQLiteParameter("@inserted") { DbType = DbType.String, Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffffff") });
                command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Int32, Value = 0 });

                command.ExecuteNonQuery();
            }

            return _transaction.Connection.LastInsertRowId;
        }

        public static void UpdateFilePath(SQLiteTransaction _transaction, long _vid, string _path)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [File_Path] = @path Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@path") { DbType = DbType.String, Value = _path });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateFileName(SQLiteTransaction _transaction, long _vid, string _name)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [File_Name] = @name Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@name") { DbType = DbType.String, Value = _name });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateFileExtention(SQLiteTransaction _transaction, long _vid, string _extention)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [File_Extention] = @extention Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@extention") { DbType = DbType.String, Value = _extention });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateFileSize(SQLiteTransaction _transaction, long _vid, long _size)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [File_Size] = @size Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@size") { DbType = DbType.Int64, Value = _size });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }

        public static void UpdateAlias(SQLiteTransaction _transaction, long _vid, string _alias)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Alias] = @alias Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@alias") { DbType = DbType.String, Value = _alias });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateAltAlias(SQLiteTransaction _transaction, long _vid, string _altAlias)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Alt_Alias] = @altAlias Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@altAlias") { DbType = DbType.String, Value = _altAlias });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateSeries(SQLiteTransaction _transaction, long _vid, string _series)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Series] = @series Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@series") { DbType = DbType.String, Value = _series });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateAltSeries(SQLiteTransaction _transaction, long _vid, string _altSeries)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Alt_Series] = @altSeries Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@altSeries") { DbType = DbType.String, Value = _altSeries });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateIcon(SQLiteTransaction _transaction, long _vid, BitmapSource _icon)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Icon] = @icon Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@icon") { DbType = DbType.Binary, Value = _icon.GetByteArray() });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateScore(SQLiteTransaction _transaction, long _vid, int _score)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Score] = @score Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@score") { DbType = DbType.Int32, Value = _score });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateFavorite(SQLiteTransaction _transaction, long _vid, bool _favorite)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Favorite] = @favorite Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@favorite") { DbType = DbType.Int32, Value = _favorite.GetInt() });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateDuration(SQLiteTransaction _transaction, long _vid, TimeSpan _duration)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Duration] = @duration Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@duration") { DbType = DbType.String, Value = _duration.ToString("G") });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateIntensity(SQLiteTransaction _transaction, long _vid, string _intensity)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Intensity] = @intensity Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@intensity") { DbType = DbType.String, Value = _intensity });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateLastPlayback(SQLiteTransaction _transaction, long _vid, DateTime _lastPlayback)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Last_Playback] = @lastPlayback Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@lastPlayback") { DbType = DbType.String, Value = _lastPlayback.ToString("yyyy-MM-dd hh:mm:ss.fffffff") });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateFormat(SQLiteTransaction _transaction, long _vid, string _format)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Format] = @format Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@format") { DbType = DbType.String, Value = _format });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateResolution(SQLiteTransaction _transaction, long _vid, string _resolution)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Resolution] = @resolution Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@resolution") { DbType = DbType.String, Value = _resolution });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateChecksum(SQLiteTransaction _transaction, long _vid, string _checksum)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Checksum] = @checksum Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@checksum") { DbType = DbType.String, Value = _checksum });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateInserted(SQLiteTransaction _transaction, long _vid, DateTime _inserted)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Inserted] = @inserted Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@inserted") { DbType = DbType.String, Value = _inserted.ToString("yyyy-MM-dd hh:mm:ss.fffffff") });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }
        public static void UpdateDeleted(SQLiteTransaction _transaction, long _vid, bool _deleted)
        {
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Update [VideoRecords] Set [Deleted] = @deleted Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Int32, Value = _deleted.GetInt() });
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                command.ExecuteNonQuery();
            }
        }

        public static void UpsertTag(SQLiteTransaction _transaction, long _vid, string _text, Intensity _intensity, bool _deleted)
        {
            long tid = 0;
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Select [Id] From [VideoTags] Where [Vid] = @vid and [Text] = @text;";
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });
                command.Parameters.Add(new SQLiteParameter("@text") { DbType = DbType.String, Value = _text });

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        tid = reader.GetInt64(0);
                    }
                }
            }

            if (tid == 0)
            {
                using (var command = _transaction.Connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = @"Insert into [VideoTags] ([Vid], [Text], [Intensity], [Deleted]) Values (@vid, @text, @intensity, @deleted)";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });
                    command.Parameters.Add(new SQLiteParameter("@text") { DbType = DbType.String, Value = _text });
                    command.Parameters.Add(new SQLiteParameter("@intensity") { DbType = DbType.String, Value = Intensity.Neutral.ToString() });
                    command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Boolean, Value = 0 });

                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using (var command = _transaction.Connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = @"Update [VideoTags] Set [Text] = @text, [Intensity] = @intensity, [Deleted] = @deleted Where [Id] = @tid;";
                    command.Parameters.Add(new SQLiteParameter("@tid") { DbType = DbType.Int64, Value = tid });
                    command.Parameters.Add(new SQLiteParameter("@text") { DbType = DbType.String, Value = _text });
                    command.Parameters.Add(new SQLiteParameter("@intensity") { DbType = DbType.String, Value = _intensity.ToString() });
                    command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Boolean, Value = _deleted });

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpsertScreenlist(SQLiteTransaction _transaction, long _vid, BitmapSource _screenlist)
        {
            long id = 0;
            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Select [Id] From [VideoScreenlists] Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        id = reader.GetInt64(0);
                    }
                }
            }

            if (id == 0)
            {
                using (var command = _transaction.Connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = @"Insert into [VideoScreenlists] ([Vid], [Screenlist], [Deleted]) Values (@vid, @screenlist, @deleted)";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _vid });
                    command.Parameters.Add(new SQLiteParameter("@screenlist") { DbType = DbType.Binary, Value = _screenlist.GetByteArray() });
                    command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Boolean, Value = 0 });

                    command.ExecuteNonQuery();
                }
            }
            else
            {
                using (var command = _transaction.Connection.CreateCommand())
                {
                    command.Transaction = _transaction;
                    command.CommandText = @"Update [VideoScreenlists] Set [Screenlist] = @screenlist Where [Id] = @id;";
                    command.Parameters.Add(new SQLiteParameter("@id") { DbType = DbType.Int64, Value = id });
                    command.Parameters.Add(new SQLiteParameter("@screenlist") { DbType = DbType.Binary, Value = _screenlist.GetByteArray() });

                    command.ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
