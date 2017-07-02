//USE [p1g3]
USE [DML]
GO
CREATE PROCEDURE DML.REGISTER_STUDENT (@FirstName VARCHAR(15), @LastName VARCHAR(15), @Email VARCHAR(50), 
	@PasswordHash VARCHAR(50), @PathToImage VARCHAR(200), @Course VARCHAR(15), @userID DECIMAL(5,0)) 
	WITH ENCRYPTION
	AS
	BEGIN
        BEGIN TRAN
			INSERT INTO DML.DMLUser (NumMec, FirstName, LastName, Email, PasswordHash, PathToImage) 
				VALUES (@userID, @FirstName, @LastName, @Email, ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', @PasswordHash), @PathToImage);
			INSERT INTO DML.Student VALUES (@userID, @Course); 
		COMMIT TRAN;
	END

GO
CREATE PROCEDURE DML.REGISTER_PROFESSOR (@FirstName VARCHAR(15), @LastName VARCHAR(15), @Email VARCHAR(50), 
	@PasswordHash VARCHAR(50), @PathToImage VARCHAR(200), @ScientificArea VARCHAR(15), @userID DECIMAL(5,0)) 
	WITH ENCRYPTION
	AS
	BEGIN
        BEGIN TRAN
			INSERT INTO DML.DMLUser (NumMec, FirstName, LastName, Email, PasswordHash, PathToImage)
				VALUES (@userID, @FirstName, @LastName, @Email, ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', @PasswordHash), @PathToImage);
			INSERT INTO DML.Professor VALUES (@userID, @ScientificArea);
        COMMIT TRAN;
	END

GO
CREATE PROCEDURE DML.CREATE_EQUIPMENT (@ProductName VARCHAR(50), @Manufacturer VARCHAR(50), @Model VARCHAR(50), 
	@ResDescription VARCHAR(max), @EmployeeNum DECIMAL(5,0), @PathToImage VARCHAR(200)) AS
	BEGIN
		INSERT INTO DML.ElectronicResource (ProductName, Manufacturer, Model, ResDescription, EmployeeNum, PathToImage)
                VALUES (@ProductName, @Manufacturer, @Model, @ResDescription, @EmployeeNum, @PathToImage)
	END

GO
CREATE PROCEDURE DML.USERS_INFO AS
	BEGIN
			SELECT		*
			FROM		DML.DMLUser JOIN DML.Student ON DML.DMLUser.NumMec=DML.Student.NumMec;

			SELECT		*
			FROM		DML.DMLUser JOIN DML.Professor ON DML.DMLUser.NumMec=DML.Professor.NumMec;
	END

GO
CREATE PROCEDURE DML.REQUISITION_UNITS (@reqID INT) AS
	BEGIN
			SELECT		DML.ResourceRequisition.ResourceID, DML.ElectronicResource.ProductName, DML.ElectronicResource.Model, DML.ElectronicResource.Manufacturer, ResDescription, PathToImage, Supplier
			FROM		(DML.ResourceRequisition JOIN DML.Requisition ON DML.ResourceRequisition.RequisitionID=DML.Requisition.RequisitionID) JOIN (DML.ElectronicUnit JOIN DML.ElectronicResource 
							ON DML.ElectronicResource.ProductName=DML.ElectronicUnit.ProductName AND DML.ElectronicResource.Model=DML.ElectronicUnit.Model AND DML.ElectronicResource.Manufacturer=DML.ElectronicUnit.Manufacturer) 
							ON DML.ResourceRequisition.ResourceID=DML.ElectronicUnit.ResourceID
			WHERE		DML.Requisition.RequisitionID = @reqID;

			SELECT		DML.ResourceRequisition.ResourceID, KitDescription
			FROM		(DML.ResourceRequisition JOIN DML.Requisition ON DML.ResourceRequisition.RequisitionID=DML.Requisition.RequisitionID)
							JOIN Kit ON DML.ResourceRequisition.ResourceID=DML.Kit.ResourceID
			WHERE		DML.Requisition.RequisitionID = @reqID;
	END
GO
CREATE PROCEDURE DML.ADD_PROJECT_USERS (@projectID INT, @WorkersList AS DML.UsersList READONLY) AS
	BEGIN
		INSERT INTO DML.WorksOn (UserNMec, ProjectID, UserRole)
		SELECT UserID, @projectID, RoleID FROM @WorkersList;
	END

GO
CREATE PROCEDURE DML.PROJECT_USERS (@pID INT) AS
	BEGIN
			SELECT		NumMec, FirstName, LastName, Email, PasswordHash, PathToImage, UserRole
			INTO		#Temp
			FROM		DML.WorksOn JOIN DML.DMLUser ON UserNMec=NumMec
			WHERE		ProjectID=@pID;

			SELECT * FROM #Temp JOIN DML.Student ON #Temp.NumMec=DML.Student.NumMec;
			SELECT * FROM #Temp JOIN DML.Professor ON #Temp.NumMec=DML.Professor.NumMec;
	END

GO
CREATE PROCEDURE DML.UPDATE_USER_ROLE (@UserRole INT, @UserNMec DECIMAL(5,0), @ProjectID INT) AS
	UPDATE WorksOn SET UserRole=@UserRole 
    WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID;

GO
CREATE PROCEDURE DML.DELETE_PROJECT_USER (@UserNMec DECIMAL(5,0), @ProjectID INT) AS
	DELETE FROM WorksOn
    WHERE UserNMec=@UserNMec AND ProjectID=@ProjectID;

GO
CREATE PROCEDURE DML.PROJECT_ACTIVE_REQS (@pID INT) AS
	BEGIN
			SELECT		Reqs.ResourceID, DML.ElectronicResource.ProductName, DML.ElectronicResource.Model, DML.ElectronicResource.Manufacturer, ResDescription, PathToImage, Supplier
			FROM		(DML.PROJECT_COUNT_REQS (@pID) AS Reqs LEFT JOIN DML.PROJECT_COUNT_DELS (@pID) AS Dels ON Reqs.ResourceID=Dels.ResourceID) 
							JOIN (DML.ElectronicUnit JOIN DML.ElectronicResource 
							ON DML.ElectronicResource.ProductName=DML.ElectronicUnit.ProductName AND DML.ElectronicResource.Model=DML.ElectronicUnit.Model AND DML.ElectronicResource.Manufacturer=DML.ElectronicUnit.Manufacturer) 
							ON Reqs.ResourceID=DML.ElectronicUnit.ResourceID
			WHERE		Dels.Num IS NULL OR Reqs.Num - Dels.Num > 0;

			SELECT		Reqs.ResourceID, KitDescription
			FROM		(DML.PROJECT_COUNT_REQS (@pID) AS Reqs LEFT JOIN DML.PROJECT_COUNT_DELS (@pID) AS Dels ON Reqs.ResourceID=Dels.ResourceID)
							JOIN DML.Kit ON Reqs.ResourceID=Kit.ResourceID
			WHERE		Dels.Num IS NULL OR Reqs.Num - Dels.Num > 0;
	END

GO
CREATE PROCEDURE DML.RESOURCES_TO_REQUEST AS
	BEGIN
			SELECT		DML.ElectronicResource.ProductName, DML.ElectronicResource.Model, DML.ElectronicResource.Manufacturer, ResDescription, PathToImage, DML.ElectronicUnit.ResourceID, Supplier
			FROM		DML.ElectronicResource JOIN (DML.ElectronicUnit JOIN ((SELECT ResourceID FROM DML.AvailableResource) EXCEPT (SELECT * FROM DML.ACTIVE_RESOURCES_REQS) EXCEPT (SELECT ResourceID FROM DML.KitUnits)) As res 
							ON DML.ElectronicUnit.ResourceID=res.ResourceID) 
							ON DML.ElectronicResource.ProductName=DML.ElectronicUnit.ProductName AND DML.ElectronicResource.Model=DML.ElectronicUnit.Model AND DML.ElectronicResource.Manufacturer=DML.ElectronicUnit.Manufacturer;

			SELECT		KitDescription, DML.Kit.ResourceID
			FROM		DML.Kit JOIN ((SELECT ResourceID FROM DML.AvailableResource) EXCEPT (SELECT * FROM DML.ACTIVE_RESOURCES_REQS)) As res ON DML.Kit.ResourceID=res.ResourceID
			GROUP BY	KitDescription, DML.Kit.ResourceID;
	END

GO
CREATE PROCEDURE DML.ADD_UNITS (@ProductName VARCHAR(50), @Manufacturer VARCHAR(50), @Model VARCHAR(50), @Supplier VARCHAR(50), @Units INT, @EmployeeID DECIMAL(5,0)) AS
	BEGIN
		DECLARE @i INT = 0;
		DECLARE @resID INT;
		WHILE @i < @Units
			BEGIN
				BEGIN TRAN
					INSERT INTO DML.AvailableResource (EmployeeNum) VALUES (@EmployeeID);
					SELECT @resID = SCOPE_IDENTITY();
					INSERT INTO DML.ElectronicUnit VALUES (@resID, @ProductName, @Manufacturer, @Model, @Supplier);
					SELECT @i = @i + 1;
				COMMIT TRAN;
			END;
	END

GO
CREATE PROCEDURE DML.CREATE_PROJECT (@PrjName VARCHAR(50), @PrjDescription VARCHAR(max), @ClassID INT, @ProjectID INT OUTPUT) AS
	BEGIN
		INSERT INTO DML.Project (PrjName, PrjDescription, Class) VALUES (@PrjName, @PrjDescription, @ClassID);
		SELECT @ProjectID = SCOPE_IDENTITY();
	END

GO
CREATE PROCEDURE DML.CREATE_REQUISITION (@ProjectID INT, @UserID INT, @RequisitionID INT OUTPUT) AS
	BEGIN
		INSERT INTO DML.Requisition (ProjectID, UserID, ReqDate) VALUES (@ProjectID, @UserID, GETDATE());
		SELECT @RequisitionID = SCOPE_IDENTITY();
	END

GO

GO
CREATE PROCEDURE DML.REQUEST_RESOURCES (@UnitsList AS DML.ResourcesList READONLY, @reqID INT) AS
	BEGIN
		INSERT INTO	DML.ResourceRequisition SELECT @reqID, * FROM @UnitsList;
	END

GO
CREATE PROCEDURE DML.CREATE_DELIVERY (@ProjectID INT, @UserID INT, @DeliveryID INT OUTPUT) AS
	BEGIN
		INSERT INTO DML.Delivery(ProjectID, UserID, DelDate) VALUES (@ProjectID, @UserID, GETDATE());
		SELECT @DeliveryID = SCOPE_IDENTITY();
	END

GO
CREATE PROCEDURE DML.DELIVER_RESOURCES (@UnitsList AS DML.ResourcesList READONLY, @delID INT) AS
	BEGIN
		INSERT INTO	DML.ResourceDelivery SELECT @delID, * FROM @UnitsList;
	END

GO
CREATE PROCEDURE DML.DELIVER_NET_RESOURCES (@UnitsList AS DML.ResourcesList READONLY) AS
	BEGIN
		DELETE FROM DML.NetworkResource WHERE NetResID IN (SELECT ResourceID FROM @UnitsList);
	END

GO
CREATE PROCEDURE DML.REQUEST_VM (@ProjectID INT, @IP VARCHAR(15), @PasswordHash VARCHAR(50), @DockerID VARCHAR(50), @OSID INT, @resID INT OUTPUT) 
WITH ENCRYPTION
AS
	BEGIN
		BEGIN TRAN
			INSERT INTO DML.NetworkResource (ReqProject) VALUES (@ProjectID);
			SELECT @resID = SCOPE_IDENTITY();
			INSERT INTO DML.VirtualMachine VALUES (@resID, @IP, ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', @PasswordHash), @DockerID, @OSID);
		COMMIT TRAN;
	END

GO
CREATE PROCEDURE DML.REQUEST_SOCKETS (@ProjectID INT, @UnitsList AS DML.ResourcesList READONLY) AS
	BEGIN
		DECLARE @num INT, @resID INT, @socketID INT, @i INT = 0;
		SELECT @num = COUNT(*) FROM @UnitsList;
		SELECT * INTO #Temp FROM @UnitsList;

		WHILE @i < @num
		BEGIN
			BEGIN TRAN
				INSERT INTO DML.NetworkResource (ReqProject) VALUES (@ProjectID);
				SELECT @resID = SCOPE_IDENTITY();
				SELECT TOP (1) @socketID = ResourceID FROM #Temp;
				INSERT INTO DML.EthernetSocket VALUES (@resID, @socketID);
				DELETE TOP(1) FROM #Temp;
				SELECT @i = @i + 1;
			COMMIT TRAN;
		END
		SELECT TOP (@num) * FROM DML.EthernetSocket ORDER BY SocketNum DESC;
	END

GO
CREATE PROCEDURE DML.REQUEST_WLAN (@ProjectID INT, @SSID VARCHAR(50), @PasswordHash VARCHAR(50), @resID INT OUTPUT) 
WITH ENCRYPTION
AS
	BEGIN
		BEGIN TRAN
			INSERT INTO DML.NetworkResource (ReqProject) VALUES (@ProjectID);
			SELECT @resID = SCOPE_IDENTITY();
			INSERT INTO DML.WirelessLAN VALUES (@resID, @SSID, ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', @PasswordHash));
		END TRAN;
	END

GO
CREATE PROCEDURE DML.UPDATE_WLAN (@resID INT, @PasswordHash VARCHAR(50)) 
WITH ENCRYPTION
AS
	BEGIN
		UPDATE		DML.WirelessLAN 
		SET			PasswordHash=ENCRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', @PasswordHash)
		WHERE		NetResID=@resID;
	END

GO
CREATE PROCEDURE DML.CREATE_KIT (@StaffID INT, @KitDescription VARCHAR(100), @UnitsList AS DML.ResourcesList READONLY, @KitID INT OUTPUT) AS
	BEGIN
		BEGIN TRAN
			INSERT INTO DML.AvailableResource (EmployeeNum) VALUES (@StaffID);
			SELECT @KitID = SCOPE_IDENTITY();
			INSERT INTO DML.Kit VALUES (@KitID, @KitDescription);
			INSERT INTO DML.KitUnits SELECT @KitID, ResourceID FROM @UnitsList;
		COMMIT TRAN;
		SELECT		*
		FROM		DML.ElectronicUnit JOIN DML.ElectronicResource 
					ON DML.ElectronicUnit.ProductName=DML.ElectronicResource.ProductName
						AND DML.ElectronicUnit.Model=DML.ElectronicResource.Model
						AND DML.ElectronicUnit.Manufacturer=DML.ElectronicResource.Manufacturer
		WHERE		DML.ElectronicUnit.ResourceID IN (SELECT ResourceID FROM @UnitsList);
	END

GO
CREATE PROCEDURE DML.KIT_UNITS (@KitID INT) AS
	BEGIN
		SELECT		*
		FROM		DML.KitUnits JOIN (DML.ElectronicUnit JOIN DML.ElectronicResource 
					ON DML.ElectronicUnit.ProductName=DML.ElectronicResource.ProductName
						AND DML.ElectronicUnit.Model=DML.ElectronicResource.Model
						AND DML.ElectronicUnit.Manufacturer=DML.ElectronicResource.Manufacturer)
					ON DML.KitUnits.ResourceID=DML.ElectronicUnit.ResourceID
		WHERE		KitID = @KitID;
	END
