using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Utility.KodiMovieRenamer.Model.VideoLibrary
{
    public class GetMoviesResponse
    {
        public string id { get; set; }
        public string jsonrpc { get; set; }
        public GetMoviesResponseResult result { get; set; }

        public class GetMoviesResponseResult
        {
            public Limits limits { get; set; }
            public List<Movie> movies { get; set; }

            public class Limits
            {
                public int start { get; set; }
                public int end { get; set; }
                public int total { get; set; }
            }
        }
        public class Movie
        {
            public string file { get; set; }
            public string label { get; set; }
            public int movieid { get; set; }
            public int year { get; set; }

        }
    }


}
