using System;
using System.Collections.Generic;

namespace Wecond.Models
{
    public class Photo
    {
        public string id { get; set; }
        public string owner { get; set; }
        public string secret { get; set; }
        public string server { get; set; }
        public int farm { get; set; }
        public string title { get; set; }
        public int ispublic { get; set; }
        public int isfriend { get; set; }
        public int isfamily { get; set; }
        public string ImageAddress { get; set; }
        public string ThumbnailAddress { get; set; }
        public string OwnerUsername { get; set; }
        public string OwnerProfileLink { get; set; }
    }

    public class Photos
    {
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public string total { get; set; }
        public List<Photo> photo { get; set; }
    }

    public class FlickrImageResults
    {
        public Photos photos { get; set; }
        public string stat { get; set; }
    }




    /*public class FlickrUserResult
    {
        public Query query { get; set; }
        public int code { get; set; }
    }
    public class Timezone
    {
        public string label { get; set; }
        public string offset { get; set; }
        public string timezone_id { get; set; }
    }
    public class Person
    {
        public string can_buy_pro { get; set; }
        public string has_stats { get; set; }
        public string iconfarm { get; set; }
        public string iconserver { get; set; }
        public string id { get; set; }
        public string ispro { get; set; }
        public string nsid { get; set; }
        public string path_alias { get; set; }
        public string username { get; set; }
        public string realname { get; set; }
        public string location { get; set; }
        public Timezone timezone { get; set; }
        public string description { get; set; }
        public string photosurl { get; set; }
        public string profileurl { get; set; }
        public string mobileurl { get; set; }
        public string buddyiconurl { get; set; }
    }*/
}
