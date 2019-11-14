namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ContentWithTags
    {
        public int Id { get; set; }

        public int ContentId { get; set; }

        public int TagsId { get; set; }

        public virtual Content Content { get; set; }

        public virtual Tags Tags { get; set; }
    }
}
