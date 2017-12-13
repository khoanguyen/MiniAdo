CREATE TABLE [dbo].[Programs]
(
	[ProgramId] INT NOT NULL PRIMARY KEY IDENTITY,	
	[ProgramCode] CHAR(7) NOT NULL UNIQUE,
	[Name] NVARCHAR(255) NOT NULL,
	[MajorId] INT NOT NULL,	
	[Level] VARCHAR(15) NOT NULL,
	[Year] VARCHAR(9) NOT NULL,
	[Status] TINYINT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Programs_Majors] FOREIGN KEY ([MajorId]) REFERENCES [Majors]([MajorId])
);
