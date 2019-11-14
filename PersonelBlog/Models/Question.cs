namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Question")]
    public partial class Question
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Question()
        {
            QuestionComment = new HashSet<QuestionComment>();
        }

        public int QuestionId { get; set; }

        public string QuestionSummary { get; set; }

        public string QuestionContext { get; set; }

        public DateTime? QuestionPostTime { get; set; }

        public DateTime? QuestionUpdateTime { get; set; }

        public int SiteUserId { get; set; }

        public virtual SiteUser SiteUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionComment> QuestionComment { get; set; }
    }
}
