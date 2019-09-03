using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Media_Library.Components;

namespace Media_Library.Data
{
    public class SearchEntityCollection : List<SearchEntity>
    {
        public SearchEntity this[string _type, string _text] { get { return this.Where(x => x.Type == _type && x.Text == _text).FirstOrDefault(); } }

        public SearchEntity GetSearchEntity(string _type, string _text)
        {
            return this.Where(x => x.Type == _type && x.Text == _text).FirstOrDefault();
        }
    }

    public class SearchEntity
    {
        public string Type { get; }
        public string Text { get; }

        public SearchEntity(string _type, string _text)
        {
            Type = _type;
            Text = _text;
        }
    }
    
    public enum MediaType { Video, Iwara, Game }
    public enum Intensity : int { Lowest = -2328737, Low = -138553, Neutral = -723467, High = -5783088, Highest = -9003082 }
    // Lowest: dc775f Low: fde2c7 Neutral: F4F5F5 High: a7c1d0 Highest: 769fb6
}
