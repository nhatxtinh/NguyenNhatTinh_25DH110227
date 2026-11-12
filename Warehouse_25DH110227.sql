USE Warehouse_25DH110227;

SELECT COUNT(*) AS TongSoSanPham
FROM dbo.Products;

SELECT COUNT(*) AS SoSanPhamSapHet
FROM dbo.Products
WHERE Quantity BETWEEN 1 AND 4;