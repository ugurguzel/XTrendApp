USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductSnapshot
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductSnapshot]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductVariationId] BIGINT NOT NULL,

    [Price] DECIMAL(18,2) NULL,

    [ListPrice] DECIMAL(18,2) NULL,

    [SalePrice] DECIMAL(18,2) NULL,

    [ShippingPrice] DECIMAL(18,2) NULL,

    [CurrencyCode] NVARCHAR(10) NULL,

    [Rating] DECIMAL(3,2) NULL,

    [ReviewCount] INT NULL,

    [StockQuantity] INT NULL,

    [IsInStock] BIT NOT NULL
        CONSTRAINT DF_ProductSnapshot_IsInStock DEFAULT(1),

    [IsPrime] BIT NOT NULL
        CONSTRAINT DF_ProductSnapshot_IsPrime DEFAULT(0),

    [HasBuyBox] BIT NOT NULL
        CONSTRAINT DF_ProductSnapshot_HasBuyBox DEFAULT(0),

    [SellerName] NVARCHAR(300) NULL,

    [BoughtLastMonthText] NVARCHAR(200) NULL,

    [BoughtLastMonthCount] INT NULL,

    [CouponText] NVARCHAR(500) NULL,

    [DeliveryText] NVARCHAR(500) NULL,

    [CapturedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductSnapshot_CapturedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ProductSnapshot
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductSnapshot_ProductVariation
        FOREIGN KEY (ProductVariationId)
        REFERENCES ProductVariation(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE INDEX IX_ProductSnapshot_ProductVariation_CapturedAt
ON ProductSnapshot(ProductVariationId, CapturedAt DESC);
GO

CREATE INDEX IX_ProductSnapshot_CapturedAt
ON ProductSnapshot(CapturedAt);
GO