namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FollowUs
    {
        public int Id { get; set; }

        public string FacebookLink { get; set; }

        public string TwitterLink { get; set; }

        public string InstagramLink { get; set; }

        public string LinkedInLink { get; set; }
    }
}
