USE [p1g3]

GO
CREATE TRIGGER DML.CHECK_PROFESSOR ON DML.Professor 
AFTER INSERT, UPDATE 
AS	
	IF (EXISTS(SELECT NumMec FROM DML.Student WHERE NumMec in (SELECT NumMec FROM inserted)))
		BEGIN
			RAISERROR ('Professor not updated/inserted - a student with same Mec. Num. exists.', 16,1);
			ROLLBACK TRAN;
		END

GO
CREATE TRIGGER DML.CHECK_STUDENT ON DML.Student 
AFTER INSERT, UPDATE 
AS	
	IF (EXISTS(SELECT NumMec FROM DML.Professor WHERE NumMec in (SELECT NumMec FROM inserted)))
		BEGIN
			RAISERROR ('Student not updated/inserted - a professor with same Mec. Num. exists.', 16,1);
			ROLLBACK TRAN;
		END

GO
CREATE TRIGGER DML.CHECK_ELECTRONIC_UNIT ON DML.ElectronicUnit 
AFTER INSERT, UPDATE 
AS	
	IF (EXISTS(SELECT ResourceID FROM DML.Kit WHERE ResourceID in (SELECT ResourceID FROM inserted)))
		BEGIN
			RAISERROR ('Electronic Unit not updated/inserted - a Kit with same ID exists.', 16,1);
			ROLLBACK TRAN;
		END

GO
CREATE TRIGGER DML.CHECK_KIT ON DML.Kit 
AFTER INSERT, UPDATE 
AS	
	IF (EXISTS(SELECT ResourceID FROM DML.ElectronicUnit WHERE ResourceID in (SELECT ResourceID FROM inserted)))
		BEGIN
			RAISERROR ('Kit not updated/inserted - an Electronic Unit with same ID exists.', 16,1);
			ROLLBACK TRAN;
		END