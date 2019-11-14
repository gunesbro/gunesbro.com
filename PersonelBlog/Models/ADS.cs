namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ADS
    {
        public int AdsId { get; set; }

        public string AdCompany { get; set; }

        public string AdImage { get; set; }

        public string AdLink { get; set; }
    }
}
