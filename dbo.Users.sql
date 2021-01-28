CREATE TABLE [dbo].[Users] (
    [UserID]    INT             IDENTITY (1, 1) NOT NULL,
    [Email]     NVARCHAR (100)  NOT NULL,
    [FirstName] NVARCHAR (100)  NOT NULL,
    [LastName]  NVARCHAR (100)  NOT NULL,
    [Password]  NVARCHAR (100)  NOT NULL,
    [Birthday]  DATE            NOT NULL,
    [UserName]  NVARCHAR (100)  NOT NULL,
    [Comment]   VARCHAR (150)   NULL,
    [Role]      NVARCHAR (50)   NOT NULL,
    [IV]        VARBINARY (200) NULL,
    [Key]       VARBINARY (200) NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC)
);

