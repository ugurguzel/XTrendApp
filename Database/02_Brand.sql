USE [XTA]
GO

/******************************************************************************
*
* TABLE : Brand
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Brand]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [Name] NVARCHAR(200) NOT NULL,

    [Description] NVARCHAR(500) NULL,

    [Website] NVARCHAR(500) NULL,

    [LogoUrl] NVARCHAR(1000) NULL,

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_Brand_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_Brand_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    CONSTRAINT PK_Brand
        PRIMARY KEY CLUSTERED (Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE UNIQUE INDEX UX_Brand_Name
ON Brand(Name);
GO

CREATE INDEX IX_Brand_IsActive
ON Brand(IsActive);
GO