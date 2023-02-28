--Create if not exists clients table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Clients' and xtype='U')
    CREATE TABLE Clients (
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(256) not null,
		Address nvarchar(256) not null,
		PostCode nvarchar(16)
    )
GO

--Create if not exists clients audit table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ClientsAudit' and xtype='U')
	CREATE TABLE [dbo].[ClientsAudit](
		Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY ,
		ClientId INT,
		Name varchar(256),
		Address nvarchar(256),
		PostCode nvarchar(16),
		EventDate datetime NOT NULL DEFAULT (GETDATE()),
		LoginName nvarchar(256),
		Operation  nvarchar(16)
	) 
GO

--Create ir alter trigger for audit table. All INSERT, UPDATE, DELETE events with client state will be logged into clients audit table
CREATE OR ALTER TRIGGER ClientsAudit_tr 
ON Clients 
AFTER INSERT, UPDATE, DELETE 
AS
BEGIN

    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        -- Insert new records
        INSERT INTO ClientsAudit (
            ClientId,
            Name,
            Address,
            PostCode,
            LoginName,
            Operation
        )
        SELECT
            id,
            Name,
            Address,
            PostCode,
            SUSER_NAME(),
            'INSERT'
        FROM inserted;
    END;

    IF EXISTS (SELECT * FROM deleted) AND NOT EXISTS (SELECT * FROM inserted)
    BEGIN
        -- Delete records
        INSERT INTO ClientsAudit (
            ClientId,
            Name,
            Address,
            PostCode,
            LoginName,
            Operation
        )
        SELECT
            id,
            Name,
            Address,
            PostCode,
            SUSER_NAME(),
            'DELETE'
        FROM deleted;
    END;

    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        -- Update records
        INSERT INTO ClientsAudit (
            ClientId,
            Name,
            Address,
            PostCode,
            LoginName,
            Operation
        )
        SELECT
            i.id,
            i.Name,
            i.Address,
            i.PostCode,
            SUSER_NAME(),
            'UPDATE'
        FROM inserted i
        INNER JOIN deleted d ON i.id = d.id;
    END;
END;

ENABLE TRIGGER ClientsAudit_tr ON Clients;

GO