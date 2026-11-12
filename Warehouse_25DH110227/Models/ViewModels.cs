using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Warehouse_25DH110227.Models
{
    public class ProductForm
    {
        [Required(ErrorMessage = "Nhập tên sản phẩm.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự.")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Nhập danh mục.")]
        [StringLength(100, ErrorMessage = "Danh mục tối đa 100 ký tự.")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Nhập giá.")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Giá từ 0.01 đến 9999999999999999.99.")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Nhập số lượng.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng là số nguyên không âm.")]
        public int? Quantity { get; set; }
    }
    public class StockForm
    {
        [Required(ErrorMessage = "Chọn sản phẩm.")]
        public int? ProductID { get; set; }
        [Required(ErrorMessage = "Nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải là số nguyên dương.")]
        public int? Amount { get; set; }
    }
    public class TransferForm
    {
        [Required(ErrorMessage = "Chọn sản phẩm nguồn.")]
        public int? SourceID { get; set; }
        [Required(ErrorMessage = "Chọn sản phẩm đích.")]
        public int? DestinationID { get; set; }
        [Required(ErrorMessage = "Nhập số lượng chuyển.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số chuyển phải là số nguyên dương.")]
        public int? Amount { get; set; }
    }
    public class FilterForm
    {
        public string Category { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Từ số lượng phải không âm.")]
        public int? From { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Đến số lượng phải không âm.")]
        public int? To { get; set; }
        public string Sort { get; set; }
        public List<Product> Products { get; set; }
    }
    public class StatisticsViewModel
    {
        public int ProductCount { get; set; }
        public long TotalQuantity { get; set; }
        public int LowStockCount { get; set; }
    }
}
