using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialNumber.EntityFrameworkCore
{
    public class SerialNumberDbContext : DbContext
    {
        public SerialNumberDbContext(DbContextOptions<SerialNumberDbContext> options) : base(options)
        {
        }

        public DbSet<EntityFrameworkCore.SerialNumber> SerialNumbers { get; set; }
    }
}
