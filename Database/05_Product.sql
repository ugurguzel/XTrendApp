USE [XTA]
GO

/******************************************************************************
*
* TABLE : Product
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Product]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [SourceId] BIGINT NOT NULL,

    [BrandId] BIGINT NOT NULL,

    [CollectionId] BIGINT NULL,

    [CategoryId] BIGINT NULL,

    -- Marketplace ana ürün kimliði
    -- Amazon : Parent ASIN
    -- Wayfair : ProductId
    -- Walmart : ProductId
    [SourceProductId] NVARCHAR(100) NOT NULL,

    [Name] NVARCHAR(500) NOT NULL,

    [Description] NVARCHAR(MAX) NULL,

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_Product_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_Product_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT PK_Product
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_Product_Source
        FOREIGN KEY (SourceId)
        REFERENCES Source(Id),

    CONSTRAINT FK_Product_Brand
        FOREIGN KEY (BrandId)
        REFERENCES Brand(Id),

    CONSTRAINT FK_Product_Collection
        FOREIGN KEY (CollectionId)
        REFERENCES Collection(Id),

    CONSTRAINT FK_Product_Category
        FOREIGN KEY (CategoryId)
        REFERENCES Category(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

-- Ayný marketplace içerisinde ayný ürün tekrar oluþamaz.
CREATE UNIQUE INDEX UX_Product_Source_SourceProduct
ON Product(SourceId, SourceProductId);
GO

CREATE INDEX IX_Product_BrandId
ON Product(BrandId);
GO

CREATE INDEX IX_Product_CollectionId
ON Product(CollectionId);
GO

CREATE INDEX IX_Product_CategoryId
ON Product(CategoryId);
GO

CREATE INDEX IX_Product_SourceId
ON Product(SourceId);
GO

CREATE INDEX IX_Product_IsActive
ON Product(IsActive);
GO

CREATE INDEX IX_Product_Name
ON Product(Name);
GO