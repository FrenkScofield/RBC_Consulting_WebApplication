using Microsoft.EntityFrameworkCore;
using RBC_Consulting_WebApplication.Models;
using System.Diagnostics.Metrics;

namespace RBC_Consulting_WebApplication.DAL
{
    public class RBC_Context : DbContext
    {
        public RBC_Context(DbContextOptions<RBC_Context> options) : base(options) { }


        public DbSet<CustomerInfo> CustomerInfos { get; set; }
    }

}
