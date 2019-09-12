using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static VideoRecord CreateNewVideoRecord(SQLiteTransaction _transaction)
        {
            var record = new VideoRecord();

            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Insert into [VideoRecords] ([Inserted],[Deleted]) Values (@inserted, @deleted);";
                command.Parameters.Add(new SQLiteParameter("@inserted") { DbType = DbType.String, Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fffffff") });
                command.Parameters.Add(new SQLiteParameter("@deleted") { DbType = DbType.Int32, Value = 0 });

                command.ExecuteNonQuery();
            }

            using (var command = _transaction.Connection.CreateCommand())
            {
                command.Transaction = _transaction;
                command.CommandText = @"Select [Vid], [Inserted], [Deleted] From [VideoRecords] Where [Vid] = @vid;";
                command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int64, Value = _transaction.Connection.LastInsertRowId });

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                        throw new KeyNotFoundException();

                    reader.Read();

                    record.Vid = reader.GetInt32(0);
                    record.Inserted = reader.GetDateTime(1);
                    record.Deleted = Convert.ToBoolean(reader.GetInt32(2));
                }
            }

            return record;
        }

        public static void UpdateVRFileAttr(SQLiteTransaction _transaction, )
        #endregion


    }
}
