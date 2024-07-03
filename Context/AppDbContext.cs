using AppAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<Class> Classes{ get; set; }
        public DbSet<TeacherClass> TeacherClasses{ get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<TeacherClassDiscipline> TeacherClassDisciplines { get; set; }
        public DbSet<Semester> Semesters { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");

            modelBuilder.Entity<TeacherClassDiscipline>()
                .HasKey(x => new { x.TeacherId, x.ClassId, x.SubjectId });

            modelBuilder.Entity<TeacherSubject>()
                .HasKey(x => new { x.TeacherId,x.SubjectId });

            modelBuilder.Entity<TeacherClass>()
            .HasKey(x => new { x.TeacherId, x.ClassId});
            // Configuring the one-to-many relationship between Student and Class
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassID);

            // Configuring the one-to-many relationship between Grade and Student
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId);

            // Configuring the one-to-many relationship between Grade and Subject
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SubjectId);

            // Configuring the one-to-many relationship between Attendance and Student
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            // Configuring the one-to-many relationship between Attendance and Subject
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Subject)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SubjectId);

        }
    }
}
