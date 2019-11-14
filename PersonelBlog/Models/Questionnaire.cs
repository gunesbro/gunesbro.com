namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Questionnaire")]
    public partial class Questionnaire
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Questionnaire()
        {
            UsersQuestionnaireSelection = new HashSet<UsersQuestionnaireSelection>();
        }

        public int Id { get; set; }

        public string Question { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersQuestionnaireSelection> UsersQuestionnaireSelection { get; set; }
    }
}
