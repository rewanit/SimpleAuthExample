using Microsoft.EntityFrameworkCore;
using SimpleAuthExample.DB.Model;

namespace SimpleAuthExample.DB
{
    public class SimpleAuthContext:DbContext
    {
        public SimpleAuthContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

       

    }
}
