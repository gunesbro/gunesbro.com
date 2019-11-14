namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        public int CommentId { get; set; }

        [StringLength(30)]
        public string from { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public string CommentText { get; set; }

        public int ContentId { get; set; }

        public DateTime? CommentDate { get; set; }

        public virtual Content Content { get; set; }
    }
}
