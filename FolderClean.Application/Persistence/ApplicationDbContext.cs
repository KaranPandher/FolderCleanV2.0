using FolderClean.Application.Core.Domains;
using Microsoft.EntityFrameworkCore;

namespace FolderClean.Application.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<RenalDocumentStat> RenalDocumentStats { get; set; }
        public DbSet<RenalBatchStat> RenalBatchStats { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}