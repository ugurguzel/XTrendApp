USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductDocument
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductDocument]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductId] BIGINT NOT NULL,

    [DocumentType] NVARCHAR(100) NOT NULL,

    [Title] NVARCHAR(300) NULL,

    [DocumentUrl] NVARCHAR(1000) NOT NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_ProductDocument_DisplayOrder DEFAULT(0),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductDocument_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ProductDocument
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductDocument_Product
        FOREIGN KEY (ProductId)
        REFERENCES Product(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE INDEX IX_ProductDocument_ProductId
ON ProductDocument(ProductId);
GO

CREATE INDEX IX_ProductDocument_DocumentType
ON ProductDocument(DocumentType);
GO

CREATE INDEX IX_ProductDocument_DisplayOrder
ON ProductDocument(DisplayOrder);
GO