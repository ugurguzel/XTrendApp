USE [XTA]
GO

/*
    XTrendApp
    ScanExecution Table

    Purpose:
    Stores each actual execution of a ScanJob.

    ScanJob   = What should run?
    ScanExecution = When did it actually run?
*/

CREATE TABLE ScanExecution
(
    Id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_ScanExecution PRIMARY KEY,

    ScanJobId BIGINT NOT NULL,

    JobType NVARCHAR(30) NOT NULL,

    Status NVARCHAR(20) NOT NULL,

    StartedAt DATETIME2 NOT NULL,

    FinishedAt DATETIME2 NULL,

    TotalProducts INT NOT NULL
        CONSTRAINT DF_ScanExecution_TotalProducts DEFAULT (0),

    InsertedProducts INT NOT NULL
        CONSTRAINT DF_ScanExecution_InsertedProducts DEFAULT (0),

    UpdatedProducts INT NOT NULL
        CONSTRAINT DF_ScanExecution_UpdatedProducts DEFAULT (0),

    FailedProducts INT NOT NULL
        CONSTRAINT DF_ScanExecution_FailedProducts DEFAULT (0),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_ScanExecution_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT FK_ScanExecution_ScanJob
        FOREIGN KEY (ScanJobId)
        REFERENCES ScanJob(Id)
);
GO


/*
    Indexes
*/

CREATE INDEX IX_ScanExecution_ScanJobId
ON ScanExecution(ScanJobId);
GO

CREATE INDEX IX_ScanExecution_Status
ON ScanExecution(Status);
GO

CREATE INDEX IX_ScanExecution_StartedAt
ON ScanExecution(StartedAt DESC);
GO