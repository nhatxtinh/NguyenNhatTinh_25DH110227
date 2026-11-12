namespace Warehouse_25DH110227.Models
{
    // Entity tương ứng bảng Products trong mô hình EDMX Database First.
    public partial class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
