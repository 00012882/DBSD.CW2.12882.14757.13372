CREATE PROCEDURE ImportFromXml
AS
BEGIN
DECLARE @XmlSample nvarchar(max) = '<Employees>
									  <Employees EmployeeID="1" FirstName="Saidakbar" LastName="Akhmedov" Phone="90-027-0400" Email="saidka.akhmedov@gmail.com" HireDate="2024-03-24T21:36:37.513" FullTimeEmployee="1" />
									  <Employees EmployeeID="2" FirstName="Timur" LastName="Radjapov" Phone="97-120-65-15" Email="timur.radjapov@gmail.com" HireDate="2024-03-24T21:36:37.513" FullTimeEmployee="0" />
									</Employees>' 
DECLARE @DocHandle int 
	EXEC sp_xml_preparedocument @DocHandle OUT, @XmlSample

	INSERT INTO Employees(EmployeeID, FirstName, LastName, Phone, Email, HireDate, EmployeeImage, FullTimeEmployee)
	OUTPUT INSERTED.*
	SELECT * 
	FROM OPENXML (@DocHandle, '/Employees/Employee', 1)
	WITH 
	(
		EmployeeID int,
		FirstName varchar(50),
		LastName varchar(50),
		Phone varchar(30),
		Email varchar(100),
		HireDate datetime,
		EmployeeImage varbinary(max),
		FullTimeEmployee bit
	)

	EXEC sp_xml_preparedocument @DocHandle
END