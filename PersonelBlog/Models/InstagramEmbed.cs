namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InstagramEmbed")]
    public partial class InstagramEmbed
    {
        public int Id { get; set; }

        [Column("InstagramEmbed")]
        public string InstagramEmbed1 { get; set; }
    }
}
