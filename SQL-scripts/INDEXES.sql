USE [p1g3]

CREATE INDEX IxKitDescription ON DML.Kit(KitDescription) WITH (FILLFACTOR = 85);
CREATE INDEX IxElectronicUnit ON DML.ElectronicUnit(ProductName, Manufacturer, Model) WITH (FILLFACTOR = 85);
