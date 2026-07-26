USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductAttribute
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductAttribute]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductId] BIGINT NOT NULL,

    [AttributeGroup] NVARCHAR(100) NULL,

    [AttributeName] NVARCHAR(200) NOT NULL,

    [AttributeValue] NVARCHAR(MAX) NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_ProductAttribute_DisplayOrder DEFAULT(0),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductAttribute_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ProductAttribute
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductAttribute_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE INDEX IX_ProductAttribute_ProductId
ON ProductAttribute(ProductId);
GO

CREATE INDEX IX_ProductAttribute_Group
ON ProductAttribute(AttributeGroup);
GO

CREATE INDEX IX_ProductAttribute_Name
ON ProductAttribute(AttributeName);
GO

CREATE UNIQUE INDEX UX_ProductAttribute_Product_Group_Name
ON ProductAttribute(ProductId, AttributeGroup, AttributeName);
GO