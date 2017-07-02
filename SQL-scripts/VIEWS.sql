USE [p1g3]

GO
CREATE VIEW DML.LAST_PROJECTS AS
			SELECT		TOP(5) ProjectID, PrjName, PrjDescription, ClassID, ClassName, ClDescription
			FROM		DML.Project LEFT JOIN DML.Class ON Class=ClassID
			ORDER BY	ProjectID DESC;

GO
CREATE VIEW DML.LAST_REQUISITIONS AS
			SELECT		TOP (5) DML.Project.ProjectID, PrjName, PrjDescription,
						ClassID, ClassName, ClDescription, DML.Requisition.RequisitionID, ReqDate,
						NumMec, FirstName, LastName, Email, PathToImage
			FROM		((DML.Project LEFT JOIN DML.Class ON Class=ClassID) JOIN DML.Requisition
							ON DML.Project.ProjectID=DML.Requisition.ProjectID) JOIN DML.DMLUser ON UserID=NumMec
			ORDER BY	DML.Project.ProjectID DESC;

GO
CREATE VIEW DML.ALL_ELECTRONIC_RESOURCES AS
			SELECT		ProductName, Manufacturer, Model, ResDescription, DML.ElectronicResource.PathToImage, 
						DML.Staff.EmployeeNum, Email, FirstName, LastName, DML.Staff.PathToImage AS StaffImage
			FROM		DML.ElectronicResource JOIN DML.Staff ON DML.ElectronicResource.EmployeeNum = DML.Staff.EmployeeNum;

GO
CREATE VIEW DML.ALL_ELECTRONIC_UNITS AS
			SELECT		ResourceID, Supplier, DML.ElectronicResource.ProductName, DML.ElectronicResource.Manufacturer, DML.ElectronicResource.Model, 
						ResDescription, DML.ElectronicResource.PathToImage, DML.Staff.EmployeeNum, Email, FirstName, LastName, DML.Staff.PathToImage AS StaffImage
			FROM		DML.ElectronicUnit RIGHT JOIN (DML.ElectronicResource JOIN DML.Staff ON DML.ElectronicResource.EmployeeNum=DML.Staff.EmployeeNum)
						ON DML.ElectronicUnit.ProductName=DML.ElectronicResource.ProductName AND DML.ElectronicResource.Model=DML.ElectronicUnit.Model AND DML.ElectronicResource.Manufacturer=DML.ElectronicUnit.Manufacturer;

GO
CREATE VIEW DML.REQUESTED_RESOURCES AS
			SELECT		ResourceID, COUNT(ResourceID) AS Num
			FROM		(DML.ResourceRequisition JOIN DML.Requisition ON DML.ResourceRequisition.RequisitionID=DML.Requisition.RequisitionID)
			GROUP BY	ResourceID;

GO
CREATE VIEW DML.DELIVERED_RESOURCES AS
			SELECT		ResourceID, COUNT(ResourceID) AS Num
			FROM		(DML.ResourceDelivery JOIN DML.Delivery ON DML.ResourceDelivery.DeliveryID=DML.Delivery.DeliveryID)
			GROUP BY	ResourceID;

GO
CREATE VIEW DML.ACTIVE_RESOURCES_REQS AS
			SELECT		DML.REQUESTED_RESOURCES.ResourceID
			FROM		DML.REQUESTED_RESOURCES LEFT JOIN DML.DELIVERED_RESOURCES ON DML.REQUESTED_RESOURCES.ResourceID=DML.DELIVERED_RESOURCES.ResourceID
			WHERE		DML.DELIVERED_RESOURCES.Num IS NULL OR DML.REQUESTED_RESOURCES.Num - DML.DELIVERED_RESOURCES.Num > 0;

GO
CREATE VIEW DML.PROJECT_INFO AS
			SELECT		DML.Project.ProjectID, PrjName, PrjDescription, ClassID, ClassName, ClDescription
			FROM		DML.Project LEFT JOIN DML.Class ON Class=ClassID;

GO
CREATE VIEW DML.ELECTRONIC_RESOURCES_INFO AS
			SELECT		ProductName, Manufacturer, Model, ResDescription, DML.ElectronicResource.PathToImage AS ResImage, 
						DML.Staff.EmployeeNum, FirstName, LastName, Email, DML.Staff.PathToImage AS StaffImage
			FROM		DML.ElectronicResource JOIN DML.Staff ON DML.ElectronicResource.EmployeeNum = DML.Staff.EmployeeNum;