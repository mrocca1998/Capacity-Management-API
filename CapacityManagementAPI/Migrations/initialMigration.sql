IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Employee] (
    [Id] int NOT NULL IDENTITY,
    [name] varchar(20) NULL,
    [Role] varchar(10) NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Project] (
    [Id] int NOT NULL IDENTITY,
    [Title] varchar(40) NULL,
    [StartDate] date NULL,
    [EndDate] date NULL,
    [TotalPoints] int NULL,
    [BaPoints] int NULL,
    [QaPoints] int NULL,
    [DevPoints] int NULL,
    [baEndDate] datetime2 NULL,
    [qaEndDate] datetime2 NULL,
    [devEndDate] datetime2 NULL,
    [calcEndDate] datetime2 NULL,
    [isUpdate] bit NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Allocation] (
    [Id] int NOT NULL IDENTITY,
    [ProjectID] int NULL,
    [EmployeeID] int NULL,
    [StartDate] date NULL,
    [EndDate] date NULL,
    [Allocation] float NULL,
    [WorkWeight] float NULL,
    [Role] varchar(15) NULL,
    [isUpdate] bit NULL,
    CONSTRAINT [PK_Allocation] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Allocatio__Emplo__15502E78] FOREIGN KEY ([EmployeeID]) REFERENCES [Employee] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK__Allocatio__Proje__145C0A3F] FOREIGN KEY ([ProjectID]) REFERENCES [Project] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_Allocation_EmployeeID] ON [Allocation] ([EmployeeID]);

GO

CREATE INDEX [IX_Allocation_ProjectID] ON [Allocation] ([ProjectID]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200727182416_init', N'3.1.5');

GO

ALTER TABLE [Allocation] DROP CONSTRAINT [FK__Allocatio__Emplo__15502E78];

GO

ALTER TABLE [Allocation] DROP CONSTRAINT [FK__Allocatio__Proje__145C0A3F];

GO

ALTER TABLE [Allocation] ADD CONSTRAINT [FK__Allocatio__Emplo__15502E78] FOREIGN KEY ([EmployeeID]) REFERENCES [Employee] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [Allocation] ADD CONSTRAINT [FK__Allocatio__Proje__145C0A3F] FOREIGN KEY ([ProjectID]) REFERENCES [Project] ([Id]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200803203110_init2', N'3.1.5');

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200803203914_added-collapse', N'3.1.5');

GO


