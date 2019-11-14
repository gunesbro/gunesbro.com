namespace PersonelBlog.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbModel : DbContext
    {
        public DbModel()
            : base("name=DbModel")
        {
        }

        public virtual DbSet<AboutMe> AboutMe { get; set; }
        public virtual DbSet<ADS> ADS { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Content> Content { get; set; }
        public virtual DbSet<ContentWithTags> ContentWithTags { get; set; }
        public virtual DbSet<FollowUs> FollowUs { get; set; }
        public virtual DbSet<InstagramEmbed> InstagramEmbed { get; set; }
        public virtual DbSet<Pages> Pages { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuestionComment> QuestionComment { get; set; }
        public virtual DbSet<Questionnaire> Questionnaire { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SiteUser> SiteUser { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UsersQuestionnaireSelection> UsersQuestionnaireSelection { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Content)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Content>()
                .HasMany(e => e.ContentWithTags)
                .WithRequired(e => e.Content)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pages>()
                .HasMany(e => e.Content)
                .WithRequired(e => e.Pages)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Question>()
                .HasMany(e => e.QuestionComment)
                .WithRequired(e => e.Question)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Questionnaire>()
                .HasMany(e => e.UsersQuestionnaireSelection)
                .WithRequired(e => e.Questionnaire)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.User)
                .WithRequired(e => e.Role)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SiteUser>()
                .HasMany(e => e.Question)
                .WithRequired(e => e.SiteUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SiteUser>()
                .HasMany(e => e.QuestionComment)
                .WithRequired(e => e.SiteUser)
                .HasForeignKey(e => e.QuestionUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SiteUser>()
                .HasMany(e => e.UsersQuestionnaireSelection)
                .WithRequired(e => e.SiteUser)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tags>()
                .HasMany(e => e.ContentWithTags)
                .WithRequired(e => e.Tags)
                .HasForeignKey(e => e.TagsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Content)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserInfo>()
                .HasMany(e => e.User)
                .WithRequired(e => e.UserInfo)
                .WillCascadeOnDelete(false);
        }
    }
}
