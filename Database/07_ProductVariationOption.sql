USE [XTA]
GO

/******************************************************************************
*
* TABLE : ProductVariationOption
*
******************************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductVariationOption]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,

    [ProductVariationId] BIGINT NOT NULL,

    [OptionName] NVARCHAR(100) NOT NULL,

    [OptionValue] NVARCHAR(300) NOT NULL,

    [DisplayOrder] INT NOT NULL
        CONSTRAINT DF_ProductVariationOption_DisplayOrder DEFAULT(0),

    [CreatedAt] DATETIME2(0) NOT NULL
        CONSTRAINT DF_ProductVariationOption_CreatedAt DEFAULT(SYSDATETIME()),

    CONSTRAINT PK_ProductVariationOption
        PRIMARY KEY CLUSTERED (Id),

    CONSTRAINT FK_ProductVariationOption_ProductVariation
        FOREIGN KEY (ProductVariationId)
        REFERENCES ProductVariation(Id)
);
GO

/******************************************************************************
*
* INDEXES
*
******************************************************************************/

CREATE INDEX IX_ProductVariationOption_ProductVariationId
ON ProductVariationOption(ProductVariationId);
GO

CREATE INDEX IX_ProductVariationOption_OptionName
ON ProductVariationOption(OptionName);
GO

-- Ayný varyasyon için ayný OptionName yalnýzca bir kez bulunabilir.
CREATE UNIQUE INDEX UX_ProductVariationOption_Variation_Option
ON ProductVariationOption(ProductVariationId, OptionName);
GO