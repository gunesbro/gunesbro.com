namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Content")]
    public partial class Content
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Content()
        {
            Comment = new HashSet<Comment>();
            ContentWithTags = new HashSet<ContentWithTags>();
        }

        public int ContentId { get; set; }

        public string ContentHeader { get; set; }

        public string ContentText { get; set; }

        public string ContentImage { get; set; }

        public int PageId { get; set; }

        public int UserId { get; set; }

        public int? HitCounter { get; set; }

        public DateTime? PostDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        public virtual Pages Pages { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContentWithTags> ContentWithTags { get; set; }
    }
}
