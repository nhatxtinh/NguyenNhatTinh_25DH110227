using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace Warehouse_25DH110227.Models
{
    public partial class WarehouseEntities : DbContext
    {
        public WarehouseEntities() : base("name=WarehouseEntities") { }
        public virtual DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    }
}
