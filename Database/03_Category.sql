USE [XTA]
GO

/******************************************************************************
*
* TABLE : Category
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Category]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ParentId] BIGINT NULL,

    [Name] NVARCHAR(200) NOT NULL,

    [Description] NVARCHAR(500) NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_Category_DisplayOrder DEFAULT(0),

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_Category_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_Category_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    CONSTRAINT PK_Category
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_Category_Parent
        FOREIGN KEY (ParentId)
        REFERENCES Category(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE UNIQUE INDEX UX_Category_Parent_Name
ON Category(ParentId, Name);
GO

CREATE INDEX IX_Category_ParentId
ON Category(ParentId);
GO

CREATE INDEX IX_Category_IsActive
ON Category(IsActive);
GO

CREATE INDEX IX_Category_DisplayOrder
ON Category(DisplayOrder);
GO