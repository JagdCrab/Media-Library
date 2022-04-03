using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Media_Library.Data
{

    public class ClipRecordCollection : List<ClipRecord>
    {
        public ClipRecordCollection() : base()
        {

        }
    }

    public class ClipRecord
    {
        private ClipTagCollection clipTagCollection;
        private ClipScreenlist clipScreenlist;
        private SearchEntityCollection searchEntities;

        public string Cid { get; set; }
        public string File_Path { get; set; }
        public string File_Name { get; set; }
        public string File_Extention { get; set; }
        public long File_Size { get; set; }
        public string Alias { get; set; }
        public string Source_Name { get; set; }
        public string Music { get; set; }
        public string Source_Code { get; set; }
        public string Author { get; set; }
        public DateTime Submit_Date { get; set; }
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

        public ClipTagCollection ClipTags 
        { 
            get {
                if (clipTagCollection == null)
                    clipTagCollection = ClipAccesser.GetClipTags(this);
                return clipTagCollection; 
            } 
            set { clipTagCollection = value; } 
        }

        public ClipScreenlist ClipScreenlist
        {
            get
            {
                if (clipScreenlist == null)
                    clipScreenlist = ClipAccesser.GetClipScreenlist(this);
                return clipScreenlist;
            }
            set { clipScreenlist = value; }
        }

        public SearchEntityCollection SearchEntities { get {
                if (searchEntities == null)
                    searchEntities = ClipAccesser.GetSearchEntities(this);
                return searchEntities; 
            } 
        }
    }

    public class ClipTagCollection : List<ClipTag>
    {
        public ClipTagCollection()
        {

        }
    }

    public class ClipTag
    {
        public int? Id { get; set; }
        public string Cid { get; set; }
        public string Text { get; set; }
        public Intensity Intensity { get; set; }
        public bool Deleted { get; set; }

        public ClipRecord Clip { get; private set; }

        public ClipTag(ClipRecord _clip)
        {
            this.Clip = _clip;
        }
    }

    public class ClipScreenlist
    {
        public int? Id { get; set; }
        public string Cid { get; set; }
        public BitmapSource Screenlist { get; set; }
        public bool Deleted { get; set; }

        public ClipRecord Clip { get; private set; }
        
        public ClipScreenlist()
        {

        }

        public ClipScreenlist(ClipRecord _clip)
        {
            this.Clip = _clip;
        }
    }
}
