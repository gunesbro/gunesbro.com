namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuestionComment")]
    public partial class QuestionComment
    {
        public int QuestionCommentId { get; set; }

        public int QuestionUserId { get; set; }

        public int QuestionId { get; set; }

        public string QuestionAnswerContext { get; set; }

        public virtual Question Question { get; set; }

        public virtual SiteUser SiteUser { get; set; }
    }
}
