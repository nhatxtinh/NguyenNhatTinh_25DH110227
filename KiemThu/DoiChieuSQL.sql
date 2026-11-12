USE Warehouse_25DH110227;
SELECT ProductID, ProductName, Category, Price, Quantity,
CASE WHEN Quantity = 0 THEN N'Hết hàng'
     WHEN Quantity BETWEEN 1 AND 4 THEN N'Sắp hết hàng'
     ELSE N'Còn hàng' END AS TrangThai
FROM dbo.Products ORDER BY ProductID;
SELECT COUNT(*) AS TongSoSanPham,
       COALESCE(SUM(CAST(Quantity AS bigint)), 0) AS TongDonViTonKho,
       COALESCE(SUM(CASE WHEN Quantity BETWEEN 1 AND 4 THEN 1 ELSE 0 END), 0) AS SoSanPhamSapHet
FROM dbo.Products;
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION,
       NUMERIC_SCALE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Products';
