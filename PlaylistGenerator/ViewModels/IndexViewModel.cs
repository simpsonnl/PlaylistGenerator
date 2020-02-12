using PlaylistGenerator.Models;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlaylistGenerator.ViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            search = new Search();
        }
        public Search search { get; set; }

        public FullTrack searchTrack { get; set; }


    }
}