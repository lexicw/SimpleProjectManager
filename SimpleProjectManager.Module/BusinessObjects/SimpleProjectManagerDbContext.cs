using DevExpress.ExpressApp.EFCore.Updating;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.EFCore.DesignTime;
using DomainComponents.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Drawing;
using SimpleProjectManager.Module.Controllers;

namespace SimpleProjectManager.Module.BusinessObjects;

// This code allows our Model Editor to get relevant EF Core metadata at design time.
// For details, please refer to https://supportcenter.devexpress.com/ticket/details/t933891.
public class SimpleProjectManagerContextInitializer : DbContextTypesInfoInitializerBase
{
    protected override DbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<SimpleProjectManagerEFCoreDbContext>()
            .UseSqlServer(";")
            .UseChangeTrackingProxies()
            .UseObjectSpaceLinkProxies();
        return new SimpleProjectManagerEFCoreDbContext(optionsBuilder.Options);
    }
}
//This factory creates DbContext for design-time services. For example, it is required for database migration.
public class SimpleProjectManagerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SimpleProjectManagerEFCoreDbContext>
{
    public SimpleProjectManagerEFCoreDbContext CreateDbContext(string[] args)
    {
        // Throw new InvalidOperationException("Make sure that the database connection string and connection provider are correct. After that, uncomment the code below and remove this exception.");
        var optionsBuilder = new DbContextOptionsBuilder<SimpleProjectManagerEFCoreDbContext>();
        optionsBuilder.UseSqlServer("Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=SimpleProjectManager");
        // Automatically implements the INotifyPropertyChanged interface in the business objects
        optionsBuilder.UseChangeTrackingProxies();
        optionsBuilder.UseObjectSpaceLinkProxies();
        return new SimpleProjectManagerEFCoreDbContext(optionsBuilder.Options);
    }
}
[TypesInfoInitializer(typeof(SimpleProjectManagerContextInitializer))]
public class SimpleProjectManagerEFCoreDbContext : DbContext
{
    public SimpleProjectManagerEFCoreDbContext(DbContextOptions<SimpleProjectManagerEFCoreDbContext> options) : base(options)
    {
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotificationsWithOriginalValues);
        modelBuilder.UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

        var colorConverter = new ValueConverter<Color?, int?>(
            c => c.HasValue ? c.Value.ToArgb() : (int?)null,
            v => v.HasValue ? Color.FromArgb(v.Value) : (Color?)null
        );

        modelBuilder.Entity<Employee>()
            .Property(e => e.Color)
            .HasConversion(colorConverter);

    }
}
