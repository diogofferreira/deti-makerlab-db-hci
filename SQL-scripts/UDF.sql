USE [p1g3]

GO
CREATE FUNCTION DML.CHECK_LOGIN(@Email VARCHAR(50), @Password VARCHAR(50)) 
RETURNS TABLE
WITH ENCRYPTION
AS RETURN (
	SELECT * FROM DML.DMLUser
	WHERE Email = @Email AND CONVERT(VARCHAR(50), DECRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', DML.DMLUser.PasswordHash))=@Password
);

GO
CREATE FUNCTION DML.CHECK_STAFF_LOGIN(@Email VARCHAR(50), @Password VARCHAR(50)) RETURNS TABLE 
WITH ENCRYPTION
AS RETURN (
	SELECT * FROM DML.Staff
	WHERE Email = @Email AND CONVERT(VARCHAR(50), DECRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', DML.Staff.PasswordHash))=@Password
);

GO
CREATE FUNCTION DML.USER_REQS (@userID INT) RETURNS TABLE AS RETURN (
			SELECT		Project.ProjectID, PrjName, PrjDescription,
						ClassID, ClassName, ClDescription, Requisition.RequisitionID, ReqDate,
						NumMec, FirstName, LastName, Email, PathToImage
			FROM		((DML.Project LEFT JOIN DML.Class ON Class=ClassID) JOIN DML.Requisition
							ON DML.Project.ProjectID = DML.Requisition.ProjectID) JOIN DML.DMLUser ON UserID=NumMec
			WHERE		UserID=@userID);

GO
CREATE FUNCTION DML.LAST_EQUIP_REQUISITIONS (@productName VARCHAR(50), @model VARCHAR(50), @manufacturer VARCHAR(50)) RETURNS TABLE AS RETURN (
			SELECT		TOP(5) DML.Project.ProjectID, PrjName, PrjDescription, Class, DML.Requisition.RequisitionID, UserID, ReqDate, 
						DML.ElectronicUnit.ResourceID, DML.ElectronicResource.ProductName, DML.ElectronicResource.Model, DML.ElectronicResource.Manufacturer, 
						DML.ElectronicResource.ResDescription, DML.ElectronicResource.PathToImage
			FROM		DML.Project JOIN (DML.Requisition JOIN (DML.ResourceRequisition JOIN (DML.ElectronicUnit JOIN DML.ElectronicResource
							ON DML.ElectronicResource.ProductName=DML.ElectronicUnit.ProductName AND DML.ElectronicResource.Model=DML.ElectronicUnit.Model AND DML.ElectronicResource.Manufacturer=DML.ElectronicUnit.Manufacturer)
							ON DML.ResourceRequisition.ResourceID=DML.ElectronicUnit.ResourceID)
							ON DML.Requisition.RequisitionID=DML.ResourceRequisition.RequisitionID) 
							ON DML.Project.ProjectID=DML.Requisition.ProjectID
			WHERE		DML.ElectronicResource.ProductName = @productName AND DML.ElectronicResource.Model = @model AND DML.ElectronicResource.Manufacturer = @manufacturer
			ORDER BY	DML.Requisition.RequisitionID DESC);

GO

CREATE FUNCTION DML.LAST_KIT_REQUISITIONS (@kitID INT) RETURNS TABLE AS RETURN (
			SELECT		TOP(5) DML.Project.ProjectID, PrjName, PrjDescription, Class, DML.Requisition.RequisitionID, UserID, ReqDate, DML.Kit.ResourceID, KitDescription
			FROM		DML.Project JOIN (DML.Requisition JOIN (DML.ResourceRequisition JOIN DML.Kit ON DML.ResourceRequisition.ResourceID=DML.Kit.ResourceID)
							ON DML.Requisition.RequisitionID=DML.ResourceRequisition.RequisitionID) 
								ON DML.Project.ProjectID=DML.Requisition.ProjectID
			WHERE		DML.Kit.ResourceID = @kitID
			ORDER BY	DML.Requisition.RequisitionID DESC);

GO
CREATE FUNCTION DML.USER_PROJECTS (@nMec DECIMAL(5,0)) RETURNS TABLE AS RETURN (
			SELECT		DML.Project.ProjectID, PrjName, PrjDescription, ClassID, ClassName, ClDescription
			FROM		(DML.Project JOIN DML.WorksOn ON DML.Project.ProjectID=DML.WorksOn.ProjectID) LEFT JOIN DML.Class ON Class=ClassID
			WHERE		DML.WorksOn.UserNMec = @nMec);

GO
CREATE FUNCTION DML.PROJECT_REQS (@pID INT) RETURNS TABLE AS RETURN (
			SELECT		DML.Project.ProjectID, PrjName, PrjDescription,
						ClassID, ClassName, ClDescription, Requisition.RequisitionID, ReqDate,
						NumMec, FirstName, LastName, Email, PathToImage
			FROM		((DML.Project LEFT JOIN DML.Class ON Class=ClassID) JOIN DML.Requisition
							ON DML.Project.ProjectID=DML.Requisition.ProjectID) JOIN DML.DMLUser ON UserID=NumMec
			WHERE		DML.Project.ProjectID=@pID);

GO
CREATE FUNCTION DML.PROJECT_COUNT_REQS (@pID INT) RETURNS TABLE AS RETURN (
			SELECT		ResourceID, COUNT(ResourceID) AS Num
			FROM		(DML.ResourceRequisition JOIN DML.Requisition ON DML.ResourceRequisition.RequisitionID=DML.Requisition.RequisitionID)
			WHERE		ProjectID=@pid
			GROUP BY	ResourceID);

GO
CREATE FUNCTION DML.PROJECT_COUNT_DELS (@pID INT) RETURNS TABLE AS RETURN (
			SELECT		ResourceID, COUNT(ResourceID) AS Num
			FROM		(DML.ResourceDelivery JOIN DML.Delivery ON DML.ResourceDelivery.DeliveryID=DML.Delivery.DeliveryID)
			WHERE		ProjectID=@pid
			GROUP BY	ResourceID);

GO
CREATE FUNCTION DML.VM_INFO (@pID INT) RETURNS TABLE 
WITH ENCRYPTION
AS RETURN (
			SELECT		DML.VirtualMachine.NetResID, IP, PasswordHash=CONVERT(VARCHAR(50), DECRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', PasswordHash)), DockerID, OSID
			FROM		DML.NetworkResource JOIN DML.VirtualMachine ON DML.NetworkResource.NetResID=DML.VirtualMachine.NetResID
			WHERE		ReqProject = @pID);

GO
CREATE FUNCTION DML.SOCKETS_INFO (@pID INT) RETURNS TABLE AS RETURN (
			SELECT		DML.EthernetSocket.NetResID, SocketNum
			FROM		DML.NetworkResource JOIN DML.EthernetSocket ON DML.NetworkResource.NetResID=DML.EthernetSocket.NetResID
			WHERE		ReqProject = @pID);

GO
CREATE FUNCTION DML.WLAN_INFO (@pID INT) RETURNS TABLE 
WITH ENCRYPTION
AS RETURN (
			SELECT		DML.WirelessLAN.NetResID, SSID, PasswordHash=CONVERT(VARCHAR(50), DECRYPTBYPASSPHRASE('IBR,44#KqfVVb$8u#k*FMf58a7id4G', PasswordHash))
			FROM		DML.NetworkResource JOIN DML.WirelessLAN ON DML.NetworkResource.NetResID=DML.WirelessLAN.NetResID
			WHERE		ReqProject = @pID);

GO
CREATE FUNCTION DML.AVAILABLE_SOCKETS () RETURNS @table TABLE (SocketNum INT NOT NULL) AS
	BEGIN
		DECLARE @i INT = 1;
		WHILE @i < 21
			BEGIN
				IF (NOT EXISTS(SELECT SocketNum FROM DML.EthernetSocket WHERE SocketNum=@i))
					INSERT INTO @table VALUES (@i);
				SELECT @i = @i + 1; 
			END 
			RETURN;
	END;
