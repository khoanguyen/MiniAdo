﻿CREATE TABLE [dbo].[Students]
(
	[StudentId] INT NOT NULL PRIMARY KEY IDENTITY,
	[FirstName] NVARCHAR(255) NOT NULL,
	[LastName] NVARCHAR(255) NOT NULL,
	[DateOfBirth] DATETIME NOT NULL,
	[Photo] VARBINARY(MAX),	
	[Email] NVARCHAR(500),		
	[Phone] VARCHAR(15),
	[Address] NVARCHAR(500),
	[Status] TINYINT DEFAULT 0 NOT NULL,
)
