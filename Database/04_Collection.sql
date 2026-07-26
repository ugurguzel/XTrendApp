USE [XTA]
GO

/******************************************************************************
*
* TABLE : Collection
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Collection]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [BrandId] BIGINT NOT NULL,

    [Name] NVARCHAR(200) NOT NULL,

    [Description] NVARCHAR(500) NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_Collection_DisplayOrder DEFAULT(0),

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_Collection_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_Collection_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    CONSTRAINT PK_Collection
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_Collection_Brand
        FOREIGN KEY (BrandId)
        REFERENCES Brand(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE UNIQUE INDEX UX_Collection_Brand_Name
ON Collection(BrandId, Name);
GO

CREATE INDEX IX_Collection_BrandId
ON Collection(BrandId);
GO

CREATE INDEX IX_Collection_IsActive
ON Collection(IsActive);
GO

CREATE INDEX IX_Collection_DisplayOrder
ON Collection(DisplayOrder);
GO