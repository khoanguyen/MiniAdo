/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET IDENTITY_INSERT Majors ON;


INSERT INTO Majors(MajorId, MajorName) VALUES(1, 'Computer Science'),(2, 'Mathematic'),(3, 'Psychology');


SET IDENTITY_INSERT Majors OFF;

SET IDENTITY_INSERT Programs ON;

INSERT INTO Programs(ProgramId, ProgramCode,Name,[Level],[Year],MajorId)
VALUES
(1,'BCS0102', 'Bachelor of Computer Science', 'Bachelor', '2001-2002', 1),
(2,'BCS0203', 'Bachelor of Computer Science', 'Bachelor', '2002-2003', 1),
(3,'MCS0203', 'Master of Computer Science', 'Master', '2001-2002', 1),
(4,'BMA0102', 'Bachelor of Mathematic', 'Bachelor', '2001-2002', 2),
(5,'BMA0203', 'Bachelor of Mathematic', 'Bachelor', '2002-2003', 2),
(6,'MMA0203', 'Master of Mathematic', 'Master', '2001-2002', 2),
(7,'BPY0102', 'Bachelor of Psychology', 'Bachelor', '2001-2002', 3),
(8,'BPY0203', 'Bachelor of Psychology', 'Bachelor', '2002-2003', 3),
(9,'MPY0203', 'Master of Psychology', 'Master', '2001-2002', 3);

SET IDENTITY_INSERT Programs OFF;

SET IDENTITY_INSERT Students ON;

INSERT INTO Students(StudentId, FirstName, LastName, DateOfBirth)
VALUES
(1,'Charles', 'Josbeck', '8-12-1983'),
(2,'Kanye', 'West', '1-2-1983'),
(3,'Luke', 'Skywalker', '7-4-1984'),
(4,'Anakin', 'Skywalker', '3-23-1983'),
(5,'Han', 'Solo', '12-12-1982'),
(6,'Kylo', 'Ren', '11-25-1983'),
(7,'Darth', 'Maul', '11-1-1984'),
(8,'Obiwan', 'Kenobi', '10-3-1983'),
(9,'Leia', 'Organa', '4-2-1981');

SET IDENTITY_INSERT Students OFF;

INSERT INTO Enrollments(ProgramId, StudentId, EnrollmentDate)
VALUES
(1, 1, '1-2-2001'),
(1, 5, '2-15-2001'),
(2, 8, '2-2-2002'),
(2, 3, '2-5-2002'),
(2, 9, '1-12-2001'),
(3, 7, '1-21-2001'),
(3, 1, '2-3-2002'),
(4, 6, '3-7-2001'),
(6, 8, '1-2-2001'),
(7, 8, '2-15-2001'),
(7, 4, '1-22-2001'),
(9, 4, '2-12-2001'),
(9, 2, '1-3-2001');

