CREATE TABLE [dbo].[Suppliers] (
    [SupplierId]    INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX) NOT NULL,
    [ContactPerson] NVARCHAR (MAX) NOT NULL,
    [Email]         NVARCHAR (MAX) NOT NULL,
    [Phone]         NVARCHAR (MAX) NOT NULL,
    [ContactEmail]  NVARCHAR (MAX) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY CLUSTERED ([SupplierId] ASC)
);






