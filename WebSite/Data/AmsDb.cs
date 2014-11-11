namespace WebSite.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AmsDb : DbContext
    {
        public AmsDb()
            : base("name=AmsDb")
        {
        }

        public virtual DbSet<CheckIn> CheckIns { get; set; }
        public virtual DbSet<GroupMembership> GroupMemberships { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Invitation> Invitations { get; set; }
        public virtual DbSet<Person> People { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheckIn>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<GroupMembership>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Group>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Invitation>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<Person>()
                .Property(e => e.Version)
                .IsFixedLength();
        }
    }
}
