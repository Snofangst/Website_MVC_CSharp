select us.IdUser, img.FileImage,pro.NameProduct,orddl.Quantity,pro.Price,pro.IdProduct
from dbo.[User] us,dbo.[Order] ord,dbo.[OrderDetails] orddl,dbo.[Image] img,dbo.[ImageDetails] imgdl,dbo.[Product] pro 
where us.IdUser=ord.IdUser and ord.IdOrder=orddl.IdOrder and orddl.IdProduct=pro.IdProduct and pro.IdProduct=imgdl.IdProduct and img.IdImage=imgdl.IdImage and imgdl.ImageType=N'ẢNH BÌA'
group by us.IdUser, img.FileImage,pro.NameProduct,orddl.Quantity,pro.Price,pro.IdProduct