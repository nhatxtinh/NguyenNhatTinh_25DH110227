using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Warehouse_25DH110227.Models;
namespace Warehouse_25DH110227.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WarehouseEntities db = new WarehouseEntities();
        public ActionResult Index()
        {
            return View(db.Products.OrderBy(p => p.ProductID).ToList());
        }
        // Câu 2: GET chỉ hiển thị xác nhận, POST mới xóa dữ liệu.
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue) return Missing();
            var product = db.Products.Find(id.Value);
            if (product == null) return Missing();
            return View(product);
        }
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                if (!id.HasValue) return Missing();
            var product = db.Products.Find(id.Value);
                if (product == null) return Missing();
                if (product.Quantity > 0)
                {
                    ModelState.AddModelError("", "Không thể xóa sản phẩm còn hàng.");
                    return View("Delete", product);
                }
                db.Products.Remove(product);
                db.SaveChanges();
                transaction.Commit();
            }
            TempData["Message"] = "Đã xóa sản phẩm hết hàng.";
            return RedirectToAction("Index");
        }
        // Câu 3: kiểm tra dữ liệu trước khi chạy truy vấn lọc.
        public ActionResult Filter(FilterForm model)
        {
            ViewBag.Categories = new SelectList(db.Products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList(), model.Category);
            model.Products = new System.Collections.Generic.List<Product>();
            if (model.From.HasValue && model.To.HasValue && model.From > model.To)
                ModelState.AddModelError("", "Từ số lượng không được lớn hơn Đến số lượng.");
            if (!ModelState.IsValid) return View(model);
            var query = db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(model.Category)) query = query.Where(p => p.Category == model.Category);
            if (model.From.HasValue) query = query.Where(p => p.Quantity >= model.From.Value);
            if (model.To.HasValue) query = query.Where(p => p.Quantity <= model.To.Value);
            model.Products = model.Sort == "desc"
                ? query.OrderByDescending(p => p.Quantity).ThenBy(p => p.ProductID).ToList()
                : query.OrderBy(p => p.Quantity).ThenBy(p => p.ProductID).ToList();
            return View(model);
        }
        public ActionResult Receive() { LoadProducts(); return View("Stock", new StockForm()); }
        public ActionResult Damage() { LoadProducts(); return View("Stock", new StockForm()); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Receive(StockForm model) { return ChangeStock(model, false); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Damage(StockForm model) { return ChangeStock(model, true); }
        // Câu 4: dùng chung phần kiểm tra cho nhập kho và ghi nhận hỏng.
        private ActionResult ChangeStock(StockForm model, bool damage)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var product = db.Products.Find(model.ProductID.Value);
                    if (product == null) ModelState.AddModelError("", "Sản phẩm không tồn tại.");
                    else if (damage && model.Amount.Value > product.Quantity)
                        ModelState.AddModelError("", "Số hỏng lớn hơn tồn kho. Không cập nhật.");
                    else if (!damage && (long)product.Quantity + model.Amount.Value > int.MaxValue)
                        ModelState.AddModelError("", "Số lượng sau nhập vượt giới hạn kiểu int.");
                    else
                    {
                        int old = product.Quantity;
                        product.Quantity = damage ? old - model.Amount.Value : old + model.Amount.Value;
                        db.SaveChanges();
                        transaction.Commit();
                        TempData["Message"] = product.ProductName + ": tồn cũ " + old +
                            (damage ? ", hỏng " : ", nhập ") + model.Amount + ", tồn mới " + product.Quantity + ".";
                        return RedirectToAction("Index");
                    }
                }
            }
            LoadProducts();
            return View("Stock", model);
        }
        public ActionResult Transfer() { LoadProducts(); return View(new TransferForm()); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Transfer(TransferForm model)
        {
            if (ModelState.IsValid)
            {
                // Câu 5: đọc, kiểm tra và cập nhật cả hai sản phẩm trong cùng transaction.
                // Nếu return trước Commit, Dispose sẽ rollback transaction.
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                {
                    var source = db.Products.Find(model.SourceID.Value);
                    var destination = db.Products.Find(model.DestinationID.Value);
                    if (source == null || destination == null)
                        ModelState.AddModelError("", "Sản phẩm nguồn hoặc đích không tồn tại.");
                    else if (source.ProductID == destination.ProductID)
                        ModelState.AddModelError("", "Phải chọn hai sản phẩm khác nhau.");
                    else if (source.Category != destination.Category)
                        ModelState.AddModelError("", "Hai sản phẩm phải cùng danh mục.");
                    else if (source.Quantity < model.Amount.Value)
                        ModelState.AddModelError("", "Sản phẩm nguồn không đủ hàng. Cả hai tồn kho giữ nguyên.");
                    else if ((long)destination.Quantity + model.Amount.Value > int.MaxValue)
                        ModelState.AddModelError("", "Tồn kho đích vượt giới hạn kiểu int.");
                    else
                    {
                        source.Quantity -= model.Amount.Value;
                        destination.Quantity += model.Amount.Value;
                        db.SaveChanges();
                        transaction.Commit();
                        TempData["Message"] = "Đã chuyển " + model.Amount + " đơn vị. Nguồn còn " +
                            source.Quantity + ", đích có " + destination.Quantity + ".";
                        return RedirectToAction("Index");
                    }
                }
            }
            LoadProducts();
            return View(model);
        }
        public ActionResult Create() { return View(new ProductForm()); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(ProductForm model)
        {
            if (model.Price.HasValue && decimal.Round(model.Price.Value, 2) != model.Price.Value)
                ModelState.AddModelError("Price", "Giá chỉ được có tối đa 2 chữ số thập phân.");
            if (!ModelState.IsValid) return View(model);
            var product = new Product {
                ProductName = model.ProductName.Trim(), Category = model.Category.Trim(),
                Price = model.Price.Value, Quantity = model.Quantity.Value
            };
            db.Products.Add(product);
            db.SaveChanges();
            TempData["Message"] = "Đã thêm sản phẩm có mã " + product.ProductID + ".";
            return RedirectToAction("Index");
        }
        public ActionResult Statistics()
        {
            var model = new StatisticsViewModel {
                ProductCount = db.Products.Count(),
                TotalQuantity = db.Products.Sum(p => (long?)p.Quantity) ?? 0,
                LowStockCount = db.Products.Count(p => p.Quantity >= 1 && p.Quantity <= 4)
            };
            return View(model);
        }
        private void LoadProducts()
        {
            ViewBag.Products = db.Products.OrderBy(p => p.ProductID).ToList().Select(p => new SelectListItem {
                Value = p.ProductID.ToString(),
                Text = p.ProductID + " - " + p.ProductName + " | " + p.Category + " | Tồn: " + p.Quantity
            }).ToList();
        }
        private ActionResult Missing()
        {
            TempData["Message"] = "Không tìm thấy ProductID. Dữ liệu không thay đổi.";
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
