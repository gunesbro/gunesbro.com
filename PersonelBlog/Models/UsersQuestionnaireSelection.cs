namespace PersonelBlog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UsersQuestionnaireSelection")]
    public partial class UsersQuestionnaireSelection
    {
        public int Id { get; set; }

        public int SiteUserId { get; set; }

        public int QuestionnaireId { get; set; }

        public string Selection { get; set; }

        public virtual Questionnaire Questionnaire { get; set; }

        public virtual SiteUser SiteUser { get; set; }
    }
}
