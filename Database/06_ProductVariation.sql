USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductVariation
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductVariation]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductId] BIGINT NOT NULL,

    -- Marketplace varyasyon kimliði
    -- Amazon : Child ASIN
    -- Wayfair : SKU / Variation Id
    [SourceVariationId] NVARCHAR(100) NOT NULL,

    [Name] NVARCHAR(500) NULL,

    [SKU] NVARCHAR(100) NULL,

    [UPC] NVARCHAR(50) NULL,

    [EAN] NVARCHAR(50) NULL,

    [GTIN] NVARCHAR(50) NULL,

    [ProductUrl] NVARCHAR(1000) NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_ProductVariation_DisplayOrder DEFAULT(0),

    [IsDefault] BIT NOT NULL
        CONSTRAINT DF_ProductVariation_IsDefault DEFAULT(0),

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_ProductVariation_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductVariation_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT PK_ProductVariation
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductVariation_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

-- Ayný Product altýnda ayný marketplace varyasyonu tekrar oluþamaz.
CREATE UNIQUE INDEX UX_ProductVariation_Product_SourceVariation
ON ProductVariation(ProductId, SourceVariationId);
GO

CREATE INDEX IX_ProductVariation_ProductId
ON ProductVariation(ProductId);
GO

CREATE INDEX IX_ProductVariation_IsDefault
ON ProductVariation(IsDefault);
GO

CREATE INDEX IX_ProductVariation_IsActive
ON ProductVariation(IsActive);
GO

CREATE INDEX IX_ProductVariation_DisplayOrder
ON ProductVariation(DisplayOrder);
GO