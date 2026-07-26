USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductImage
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductImage]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductId] BIGINT NOT NULL,

    [ImageUrl] NVARCHAR(1000) NOT NULL,

    [ImageType] NVARCHAR(50) NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_ProductImage_DisplayOrder DEFAULT(0),

    [IsPrimary] BIT NOT NULL
        CONSTRAINT DF_ProductImage_IsPrimary DEFAULT(0),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductImage_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ProductImage
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductImage_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE INDEX IX_ProductImage_ProductId
ON ProductImage(ProductId);
GO

CREATE INDEX IX_ProductImage_DisplayOrder
ON ProductImage(DisplayOrder);
GO

CREATE INDEX IX_ProductImage_IsPrimary
ON ProductImage(IsPrimary);
GO