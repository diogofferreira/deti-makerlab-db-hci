USE [p1g3]

GO
CREATE SCHEMA [DML]

GO
CREATE TYPE DML.UsersList AS TABLE (
	UserID	DECIMAL(5,0),
	RoleID	INT
);

GO
CREATE TYPE DML.ResourcesList AS TABLE (
	ResourceID INT
);

GO
CREATE TABLE DML.DMLUser (
	NumMec			DECIMAL(5,0)    NOT NULL,
	FirstName		VARCHAR(15),
	LastName		VARCHAR(15),
	Email			VARCHAR(50)     NOT NULL,
	PasswordHash	VARBINARY(128)	NOT NULL,
	PathToImage	    VARCHAR(200),
	PRIMARY KEY (NumMec),
    UNIQUE (Email)
);

CREATE TABLE DML.Professor (
	NumMec			DECIMAL(5,0)    NOT NULL,
	ScientificArea	VARCHAR(50),
	PRIMARY KEY (NumMec),
	FOREIGN KEY (NumMec) REFERENCES DML.DMLUser(NumMec)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.Student (
	NumMec			DECIMAL(5,0)    NOT NULL,
	Course			VARCHAR(50)		NOT NULL,
	PRIMARY KEY (NumMec),
	FOREIGN KEY (NumMec) REFERENCES DML.DMLUser(NumMec)
        ON UPDATE CASCADE   
		ON DELETE CASCADE
);

CREATE TABLE DML.Staff (
	EmployeeNum		DECIMAL(5,0)    NOT NULL,
	FirstName		VARCHAR(15),
	LastName		VARCHAR(15),
	Email			VARCHAR(50)     NOT NULL,
	PasswordHash	VARBINARY(128)  NOT NULL,
	PathToImage		VARCHAR(200),
	PRIMARY KEY (EmployeeNum),
    UNIQUE (Email)
);

CREATE TABLE DML.Class (
	ClassID			INT             IDENTITY(1,1),
	ClassName		VARCHAR(50)		NOT NULL,
	ClDescription	VARCHAR(max),
	PRIMARY KEY (ClassID),
	UNIQUE(ClassName)
);

CREATE TABLE DML.ClassManager (
	ClassID			INT             NOT NULL,
	Professor		DECIMAL(5,0)    NOT NULL,
	PRIMARY KEY (ClassID),
    FOREIGN KEY (ClassID) REFERENCES DML.Class (ClassID)
        ON UPDATE CASCADE   
		ON DELETE CASCADE,
	FOREIGN KEY (Professor) REFERENCES DML.Professor(NumMec)
);

CREATE TABLE DML.Roles (
	RoleID			INT             IDENTITY(1,1),
	RoleDescription	VARCHAR(50)     NOT NULL,
	PRIMARY KEY (RoleID),
	UNIQUE (RoleDescription)
);

CREATE TABLE DML.Project (
	ProjectID		INT             IDENTITY(1,1),
	PrjName			VARCHAR(50)     NOT NULL,
	PrjDescription	VARCHAR(max),
	Class 			INT,
	PRIMARY KEY (ProjectID),
	FOREIGN KEY (Class) REFERENCES DML.ClassManager(ClassID)
);

CREATE TABLE DML.WorksOn (
	UserNMec		DECIMAL(5,0)    NOT NULL,
	ProjectID		INT			    NOT NULL,
	UserRole		INT			    NOT NULL,
	PRIMARY KEY (UserNMec, ProjectID),
	FOREIGN KEY (UserNMec) REFERENCES DML.DMLUser(NumMec)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ProjectID) REFERENCES DML.Project(ProjectID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (UserRole) REFERENCES DML.Roles(RoleID)
);

CREATE TABLE DML.Requisition (
	RequisitionID	INT             IDENTITY(1,1),
	ProjectID		INT             NOT NULL,
	UserID			DECIMAL(5,0)    NOT NULL,
	ReqDate			DATE            NOT NULL,
	PRIMARY KEY (RequisitionID),
	FOREIGN KEY (ProjectID) REFERENCES DML.Project(ProjectID),
	FOREIGN KEY (UserID) REFERENCES DML.DMLUser(NumMec)
);

CREATE TABLE DML.Delivery (
	DeliveryID		INT             IDENTITY(1,1),
	ProjectID		INT             NOT NULL,
	UserID			DECIMAL(5,0)    NOT NULL,
	DelDate			DATE            NOT NULL,
	PRIMARY KEY (DeliveryID),
	FOREIGN KEY (ProjectID) REFERENCES DML.Project(ProjectID),
	FOREIGN KEY (UserID) REFERENCES DML.DMLUser(NumMec)
);

CREATE TABLE DML.AvailableResource (
	ResourceID		INT             IDENTITY(1,1),
	EmployeeNum		DECIMAL(5,0)    NOT NULL,
	PRIMARY KEY (ResourceID),
	FOREIGN KEY (EmployeeNum) REFERENCES DML.Staff(EmployeeNum)
);

CREATE TABLE DML.ResourceRequisition (
	RequisitionID	INT             NOT NULL,
	ResourceID		INT             NOT NULL,
	PRIMARY KEY (RequisitionID, ResourceID),
	FOREIGN KEY (RequisitionID) REFERENCES DML.Requisition(RequisitionID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ResourceID) REFERENCES DML.AvailableResource(ResourceID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.ResourceDelivery (
	DeliveryID		INT             NOT NULL,
	ResourceID		INT             NOT NULL,
	PRIMARY KEY (DeliveryID, ResourceID),
	FOREIGN KEY (DeliveryID) REFERENCES DML.Delivery(DeliveryID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ResourceID) REFERENCES DML.AvailableResource(ResourceID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.Kit (
	ResourceID		INT             NOT NULL,
	KitDescription	VARCHAR(100)	NOT NULL,
	PRIMARY KEY (ResourceID),
	FOREIGN KEY (ResourceID) REFERENCES DML.AvailableResource(ResourceID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.ElectronicResource (
	ProductName		VARCHAR(50)     NOT NULL,
	Manufacturer	VARCHAR(50)     NOT NULL,
	Model			VARCHAR(50)     NOT NULL,
	ResDescription	VARCHAR(max),
	EmployeeNum		DECIMAL(5,0)    NOT NULL,
	PathToImage		VARCHAR(200),
	PRIMARY KEY (ProductName, Manufacturer, Model),
	FOREIGN KEY (EmployeeNum) REFERENCES DML.Staff(EmployeeNum)
);

CREATE TABLE DML.ElectronicUnit (
	ResourceID		INT             NOT NULL,
	ProductName		VARCHAR(50)     NOT NULL,
	Manufacturer	VARCHAR(50)     NOT NULL,
	Model			VARCHAR(50)     NOT NULL,
	Supplier		VARCHAR(50)     NOT NULL,
	PRIMARY KEY (ResourceID),
	FOREIGN KEY (ResourceID) REFERENCES DML.AvailableResource(ResourceID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ProductName, Manufacturer, Model) REFERENCES DML.ElectronicResource(ProductName, Manufacturer, Model)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.KitUnits (
	KitID			INT             NOT NULL,
	ResourceID		INT				NOT NULL,
	PRIMARY KEY (KitID, ResourceID),
	FOREIGN KEY (KitID) REFERENCES DML.Kit (ResourceID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
	FOREIGN KEY (ResourceID) REFERENCES DML.ElectronicUnit (ResourceID)
        ON UPDATE NO ACTION
		ON DELETE NO ACTION
);

CREATE TABLE DML.OS (
	OSID		    INT             IDENTITY(1,1),
	OSName			VARCHAR(50)		NOT NULL,
	PRIMARY KEY (OSID),
	UNIQUE (OSName)
);

CREATE TABLE DML.NetworkResource (
	NetResID		INT             IDENTITY(1,1),
	ReqProject		INT             NOT NULL,
	PRIMARY KEY (NetResID),
	FOREIGN KEY (ReqProject) REFERENCES DML.Project(ProjectID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.VirtualMachine (
	NetResID		INT             NOT NULL,
	IP				VARCHAR(15)     NOT NULL,
	PasswordHash	VARBINARY(128)  NOT NULL,
	DockerID		VARCHAR(50)     NOT NULL,
	OSID			INT             NOT NULL,
	PRIMARY KEY (NetResID),
    UNIQUE (DockerID),
	FOREIGN KEY (NetResID) REFERENCES DML.NetworkResource(NetResID)
        ON UPDATE CASCADE
		ON DELETE CASCADE,
    FOREIGN KEY (OSID) REFERENCES DML.OS(OSID)
);

CREATE TABLE DML.EthernetSocket (
	NetResID		INT             NOT NULL,
	SocketNum		DECIMAL(5,0)    NOT NULL,
	PRIMARY KEY (NetResID),
    UNIQUE (SocketNum),
	FOREIGN KEY (NetResID) REFERENCES DML.NetworkResource(NetResID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);

CREATE TABLE DML.WirelessLAN (
	NetResID		INT             NOT NULL,
	SSID			VARCHAR(50)     NOT NULL,
	PasswordHash	VARBINARY(128)  NOT NULL,
	PRIMARY KEY (NetResID),
    UNIQUE (SSID),
	FOREIGN KEY (NetResID) REFERENCES DML.NetworkResource(NetResID)
        ON UPDATE CASCADE
		ON DELETE CASCADE
);
