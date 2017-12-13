CREATE TABLE [dbo].[Enrollments]
(
	[EnrollmentId] INT NOT NULL PRIMARY KEY IDENTITY,
	[ProgramId] INT NOT NULL,
	[StudentId] INT NOT NULL,
	[EnrollmentDate] DATETIME NOT NULL,
	[Status] TINYINT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_Enrollment_ProgramId_StudentId] UNIQUE ([ProgramId], [StudentId]), 
    CONSTRAINT [FK_Enrollments_Programs] FOREIGN KEY ([ProgramId]) REFERENCES [Programs]([ProgramId]),
	CONSTRAINT [FK_Enrollments_Students] FOREIGN KEY ([StudentId]) REFERENCES [Students]([StudentId])
)
