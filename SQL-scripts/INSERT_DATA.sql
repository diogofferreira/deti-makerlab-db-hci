USE [p1g3]

DECLARE @hash AS VARBINARY(200) = ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', 'password');

EXEC DML.REGISTER_STUDENT @FirstName='Pedro', @LastName='Martins', @Email='pbmartins@ua.pt', @PasswordHash='password', @PathToImage='C:\Users\Diogo Ferreira\Desktop\DETI-MakerLab@DB_HCI\DETI-MakerLab\images\default-profile.png', @Course='ECT', @userID=76551;
EXEC DML.REGISTER_STUDENT @FirstName='Ricardo', @LastName='Jesus', @Email='ricardojesus@ua.pt', @PasswordHash='password', @PathToImage='C:\Users\Diogo Ferreira\Desktop\DETI-MakerLab@DB_HCI\DETI-MakerLab\images\default-profile.png', @Course='ECT', @userID=76613;

EXEC DML.REGISTER_PROFESSOR @FirstName='Carlos', @LastName='Costa', @Email='carlos.costa@ua.pt', @PasswordHash='password', @PathToImage='C:\Users\Diogo Ferreira\Desktop\DETI-MakerLab@DB_HCI\DETI-MakerLab\images\default-profile.png', @ScientificArea='Informática', @userID=84523;
EXEC DML.REGISTER_PROFESSOR @FirstName='Joaquim', @LastName='Madeira', @Email='jmadeira@ua.pt', @PasswordHash='password', @PathToImage='C:\Users\Diogo Ferreira\Desktop\DETI-MakerLab@DB_HCI\DETI-MakerLab\images\default-profile.png', @ScientificArea='Informática', @userID=12345;
EXEC DML.REGISTER_PROFESSOR @FirstName='Beatriz', @LastName='Santos', @Email='bss@ua.pt', @PasswordHash='password', @PathToImage='C:\Users\Diogo Ferreira\Desktop\DETI-MakerLab@DB_HCI\DETI-MakerLab\images\default-profile.png', @ScientificArea='Informática', @userID=54321;

INSERT INTO DML.Staff (EmployeeNum, FirstName, LastName, Email, PasswordHash) 
	VALUES (1, 'Manuel', 'Arez', 'manuel.arez@ua.pt', ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', 'password'));

INSERT INTO DML.Class (ClassName, ClDescription) VALUES ('IHC', 'Interação Humano Computador');
INSERT INTO DML.Class (ClassName, ClDescription) VALUES ('BD', 'Bases de Dados');
INSERT INTO DML.Class (ClassName, ClDescription) VALUES ('PEI', 'Projecto em Engenharia Informatica');
INSERT INTO DML.Class (ClassName, ClDescription) VALUES ('PEE', 'Projecto em Engenharia Electrotecnica');

INSERT INTO DML.ClassManager(ClassID, Professor) VALUES (1, 12345);
INSERT INTO DML.ClassManager(ClassID, Professor) VALUES (2, 54321);

INSERT INTO DML.Roles (RoleDescription) VALUES ('Project Manager');
INSERT INTO DML.Roles (RoleDescription) VALUES ('Product Manager');
INSERT INTO DML.Roles (RoleDescription) VALUES ('Infrastructure Manager');
INSERT INTO DML.Roles (RoleDescription) VALUES ('Documentation Manager');
INSERT INTO DML.Roles (RoleDescription) VALUES ('Developer');
INSERT INTO DML.Roles (RoleDescription) VALUES ('Supervisor');

INSERT INTO DML.OS (OSName) VALUES ('Arch Linux');
INSERT INTO DML.OS (OSName) VALUES ('Ubuntu 16:04');
INSERT INTO DML.OS (OSName) VALUES ('Debian Jessie');
