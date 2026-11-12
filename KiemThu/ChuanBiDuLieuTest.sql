-- CHỈ dùng trên CSDL bài tập. Script XÓA mọi sản phẩm và tạo lại dữ liệu mẫu.
-- Dừng website khi chuẩn bị dữ liệu, chạy xong mới mở trang cần test.
-- Chọn mã test, đổi @ChoPhepTaoLai = 1 rồi chạy TOÀN BỘ script.
-- Không chạy lại script giữa bước thao tác và bước chụp kết quả của cùng test.
USE Warehouse_25DH110227;
SET XACT_ABORT ON;
DECLARE @ChoPhepTaoLai bit = 0;
DECLARE @MaTest varchar(10) = '1.1';
IF @ChoPhepTaoLai <> 1
    THROW 50002, N'Chưa cho phép tạo lại dữ liệu. Đọc cảnh báo đầu file.', 1;
IF @MaTest NOT IN ('1.1','1.2','2.1','2.2','3.1','3.2','3.3','3.4',
'4.1','4.2','4.3','4.4','4.5','5.1','5.2','5.3','6.1','6.2','6.3','7.1','7.2','7.3')
    THROW 50003, N'Mã test không hợp lệ.', 1;
BEGIN TRANSACTION;
TRUNCATE TABLE dbo.Products;
INSERT INTO dbo.Products(ProductName, Category, Price, Quantity) VALUES
(N'Bút bi Nguyễn Nhất Tính A', N'Văn phòng phẩm Nguyễn Nhất Tính', 5000, 10),
(N'Bút bi Nguyễn Nhất Tính B', N'Văn phòng phẩm Nguyễn Nhất Tính', 6000, 4),
(N'Vở Nguyễn Nhất Tính C', N'Văn phòng phẩm Nguyễn Nhất Tính', 15000, 0),
(N'Thước Nguyễn Nhất Tính D', N'Văn phòng phẩm Nguyễn Nhất Tính', 7000, 3),
(N'Chuột Nguyễn Nhất Tính E', N'Điện tử Nguyễn Nhất Tính', 150000, 5),
(N'Cáp USB Nguyễn Nhất Tính F', N'Điện tử Nguyễn Nhất Tính', 50000, 2),
(N'Bàn phím Nguyễn Nhất Tính G', N'Điện tử Nguyễn Nhất Tính', 250000, 8),
(N'Tai nghe Nguyễn Nhất Tính H', N'Điện tử Nguyễn Nhất Tính', 200000, 5);
IF @MaTest = '4.3' UPDATE dbo.Products SET Quantity = 8 WHERE ProductID = 1;
IF @MaTest IN ('4.4','4.5') UPDATE dbo.Products SET Quantity = 5 WHERE ProductID = 1;
IF @MaTest IN ('7.2','7.3')
BEGIN
    UPDATE dbo.Products SET Quantity = 0;
    UPDATE dbo.Products SET Quantity = 2 WHERE ProductID = 1;
    UPDATE dbo.Products SET Quantity = 5 WHERE ProductID = 2;
END;
COMMIT TRANSACTION;
SELECT * FROM dbo.Products ORDER BY ProductID;
