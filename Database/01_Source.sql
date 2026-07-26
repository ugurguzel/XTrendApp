USE [XTA]
GO

/******************************************************************************
*
* TABLE : Source
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Source]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [Name] NVARCHAR(100) NOT NULL,

    [DisplayName] NVARCHAR(200) NOT NULL,

    [Website] NVARCHAR(500) NULL,

    [IsActive] BIT NOT NULL
        CONSTRAINT DF_Source_IsActive DEFAULT(1),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_Source_CreatedAt DEFAULT(SYSDATETIME()),

    [UpdatedAt] DATETIME2(0) NULL,

    CONSTRAINT PK_Source
        PRIMARY KEY CLUSTERED (Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE UNIQUE INDEX UX_Source_Name
ON Source(Name);
GO

CREATE INDEX IX_Source_IsActive
ON Source(IsActive);
GO