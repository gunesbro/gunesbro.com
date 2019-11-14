namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AboutMe")]
    public partial class AboutMe
    {
        public int Id { get; set; }

        public string AboutMeText { get; set; }

        public string AboutMeImage { get; set; }
    }
}
