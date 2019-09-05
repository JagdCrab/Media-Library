using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Media_Library.Data
{
    #region Video Series

    public class VideoSeriesCollection : List<VideoSeries>
    {
        public VideoSeries this[string series]
        {
            get { return this.Where(x => x.Series == series || x.Alt_Series == series).First(); }
        }

        public VideoSeriesCollection() : base ()
        {

        }
    }

    public class VideoSeries
    {
        private VideoRecordCollection videoRecords;
        private VideoTagCollection videoTags;
        private SearchEntityCollection searchEntities;
        
        public string Series { get; set; }
        public string Alt_Series { get; set; }
        public BitmapSource Icon { get; set; }
        public DateTime Inserted { get; set; }

        public VideoRecordCollection VideoRecords
        {
            get
            {
                if (videoRecords == null)
                    videoRecords = VideoAccesser.GetVideoRecords(this);

                return videoRecords;
            }
            set { videoRecords = value; }
        }

        public VideoTagCollection VideoTags
        {
            get
            {
                if (videoTags == null)
                    videoTags = VideoAccesser.GetVideoTags(this);

                return videoTags;
            }
            set { videoTags = value; }
        }

        public SearchEntityCollection SearchEntities
        {
            get
            {
                if (searchEntities == null)
                    searchEntities = VideoAccesser.GetSearchEntities(this);

                return searchEntities;
            }
        }

        public VideoSeries()
        {

        }
    }
    #endregion

    #region Video Record
    public class VideoRecordCollection : List<VideoRecord>
    {

        new public VideoRecord this[int vid]
        {
            get { return this.Where(x => x.Vid == vid).First(); }
        }

        public VideoRecordCollection()
        {

        }
    }

    public class VideoRecord
    {
        private VideoScreenlist screenlist;
        private VideoTagCollection tags;

        public int? Vid { get; set; }
        public string File_Path { get; set; }
        public string File_Name { get; set; }
        public string File_Extention { get; set; }
        public long File_Size { get; set; }
        public string Alias { get; set; }
        public string Alt_Alias { get; set; }
        public string Series { get; set; }
        public string Alt_Series { get; set; }
        public BitmapSource Icon { get; set; }
        public int Score { get; set; }
        public bool Favorite { get; set; }
        public TimeSpan Duration { get; set; }
        public string Intensity { get; set; }
        public DateTime Last_playback { get; set; }
        public string Format { get; set; }
        public string Resolution { get; set; }
        public string Checksum { get; set; }
        public DateTime Inserted { get; set; }
        public bool Deleted { get; set; }

        public VideoScreenlist Screenlist
        {
            get
            {
                if (screenlist == null)
                    screenlist = VideoAccesser.GetVideoScreenlist(this);
                return screenlist;
            }
            set { screenlist = value; }
        }

        public VideoTagCollection Tags
        {
            get
            {
                if (tags == null)
                    tags = VideoAccesser.GetVideoTags(this);
                return tags;
            }
            set { tags = value; }
        }

        public VideoRecord()
        {

        }
    }

    public class VideoTagCollection : List<VideoTag>
    {
        public VideoTagCollection()
        {

        }
    }
    #endregion

    #region VideoTag
    public class VideoTag
    {
        public int? Id { get; set; }
        public int? Vid { get; set; }
        public string Text { get; set; }
        public Intensity Intensity { get; set; }
        public bool Deleted { get; set; }

        public VideoSeries VideoSeries { get; private set; }
        public VideoRecord VideoRecord { get; private set; }

        public VideoTag(VideoSeries _series)
        {
            this.VideoSeries = _series;
        }

        public VideoTag(VideoRecord _record)
        {
            this.VideoRecord = _record;
        }
    }
    #endregion

    #region Video Screenlist

    public class VideoScreenlist
    {
        public int? Id { get; set; }
        public int? Vid { get; set; }
        public BitmapSource Screenlist { get; set; }
        public bool Deleted { get; set; }

        public VideoRecord VideoRecord { get; private set; }

        public VideoScreenlist()
        {

        }

        public VideoScreenlist(VideoRecord _record)
        {
            this.VideoRecord = _record;
        }
    }
    #endregion

    #region Playlist

    public class VideoPlaylistCollection : List<VideoPlaylist>
    {

    }

    public class VideoPlaylist
    {
        public int? Pid { get; set; }
        public int Vid { get; set; }
        public string Alias { get; set; }
        public string Series { get; set; }
        public DateTime Inserted { get; set; }

        public VideoPlaylist()
        {

        }
    }

    #endregion
}

