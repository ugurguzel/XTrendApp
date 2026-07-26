USE [XTA]
GO

/******************************************************************************
*
* TABLE : ScanJob
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScanJob]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [SourceId] BIGINT NOT NULL,

    [Keyword] NVARCHAR(300) NULL,

    [Status] NVARCHAR(30) NOT NULL,

    [StartedAt] DATETIME2(0) NOT NULL,

    [FinishedAt] DATETIME2(0) NULL,

    [TotalProducts] INT NOT NULL
        CONSTRAINT DF_ScanJob_TotalProducts DEFAULT(0),

    [InsertedProducts] INT NOT NULL
        CONSTRAINT DF_ScanJob_InsertedProducts DEFAULT(0),

    [UpdatedProducts] INT NOT NULL
        CONSTRAINT DF_ScanJob_UpdatedProducts DEFAULT(0),

    [FailedProducts] INT NOT NULL
        CONSTRAINT DF_ScanJob_FailedProducts DEFAULT(0),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ScanJob_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ScanJob
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ScanJob_Source
        FOREIGN KEY (SourceId)
        REFERENCES Source(Id)
);
GO

CREATE INDEX IX_ScanJob_SourceId
ON ScanJob(SourceId);

GO

CREATE INDEX IX_ScanJob_Status
ON ScanJob(Status);

GO

CREATE INDEX IX_ScanJob_StartedAt
ON ScanJob(StartedAt);

GO