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
                    command.CommandText = "Select distinct [Text] From [VideoTags] Where [Deleted] = 0 and [sid] = @sid;";
                    command.Parameters.Add(new SQLiteParameter("@sid") { DbType = DbType.Int32, Value = _series.Sid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Tag", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Series] From [VideoRecords] Where [Deleted] = 0 and [sid] = @sid;";
                    command.Parameters.Add(new SQLiteParameter("@sid") { DbType = DbType.Int32, Value = _series.Sid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            while (reader.Read())
                                entities.Add(new SearchEntity("Series", reader.GetString(0)));
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "Select distinct [Alt_Series] From [VideoRecords] Where [Deleted] = 0 and [sid] = @sid;";
                    command.Parameters.Add(new SQLiteParameter("@sid") { DbType = DbType.Int32, Value = _series.Sid });

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

        public static VideoSeriesCollection GetVideoSeries()
        {
            var collection = new VideoSeriesCollection();

            using (var connection = CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        Select [Sid], [Series], [Alt_Series], [Icon], [Inserted]
                        From (
	                        Select
		                         Row_Number() Over (Partition by Sid Order by Random()) as n
		                        ,[Sid]
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

                            series.Sid = reader.GetInt32(0);
                            series.Series = reader.GetString(1);
                            series.Alt_Series = reader.GetNullableString(2);
                            series.Icon = reader.GetBitmap(3);
                            series.Inserted = reader.GetDateTime(4);

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
                    command.CommandText = "Select [Vid],[File_Path],[File_Name],[File_Extention],[File_Size],[Alias],[Alt_Alias],[Series],[Alt_Series],[Icon],[Score],[Favorite],[Duration],[Last_playback],[Format],[Resolution],[Checksum],[Inserted] From [VideoRecords] Where [Deleted] = 0 and [Sid] = @sid;";
                    command.Parameters.Add(new SQLiteParameter("@sid") { DbType = DbType.Int32, Value = _series.Sid });

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
                            record.Last_playback = reader.GetDateTime(13);
                            record.Format = reader.GetNullableString(14);
                            record.Resolution = reader.GetNullableString(15);
                            record.Checksum = reader.GetNullableString(16);
                            record.Inserted = reader.GetDateTime(17);

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
                    command.CommandText = "Select [Id], [Sid], [Vid], [Text], [Intensity] From [VideoTags] Where [Deleted] = 0 and [Sid] = @sid;";
                    command.Parameters.Add(new SQLiteParameter("@sid") { DbType = DbType.Int32, Value = _series.Sid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return tags;

                        while (reader.Read())
                        {
                            VideoTag tag = new VideoTag(_series);

                            tag.Id = reader.GetInt32(0);
                            tag.Sid = reader.GetInt32(1);
                            tag.Vid = reader.GetInt32(2);
                            tag.Text = reader.GetString(3);
                            tag.Intensity = reader.GetIntensity(4);

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
                    command.CommandText = "Select [Id], [Sid], [Vid], [Text], [Intensity] From [VideoTags] Where [Deleted] = 0 and [Vid] = @vid;";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int32, Value = _record.Vid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return tags;

                        while (reader.Read())
                        {
                            VideoTag tag = new VideoTag(_record);

                            tag.Id = reader.GetInt32(0);
                            tag.Sid = reader.GetInt32(1);
                            tag.Vid = reader.GetInt32(2);
                            tag.Text = reader.GetString(3);
                            tag.Intensity = reader.GetIntensity(4);

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
                    command.CommandText = "Select [Id], [Sid], [Vid], [Screenlist] From [VideoScreenlists] Where [Deleted] = 0 and [Vid] = @vid;";
                    command.Parameters.Add(new SQLiteParameter("@vid") { DbType = DbType.Int32, Value = _record.Vid });

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return screenlist;
                        else
                        {
                            reader.Read();
                            screenlist.Id = reader.GetInt32(0);
                            screenlist.Sid = reader.GetInt32(1);
                            screenlist.Vid = reader.GetInt32(2);
                            screenlist.Screenlist = reader.GetBitmap(3);
                        }
                    }
                }
            }
            return screenlist;
        }

        #endregion


    }
}
