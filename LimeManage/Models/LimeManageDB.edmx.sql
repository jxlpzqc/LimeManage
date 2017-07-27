
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/27/2017 15:55:45
-- Generated from EDMX file: D:\LimeManage\LimeManage\Models\LimeManageDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ProjectManagement];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_City_Province]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[City] DROP CONSTRAINT [FK_City_Province];
GO
IF OBJECT_ID(N'[dbo].[FK_Cost_PartyB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cost] DROP CONSTRAINT [FK_Cost_PartyB];
GO
IF OBJECT_ID(N'[dbo].[FK_Cost_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cost] DROP CONSTRAINT [FK_Cost_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_County_City]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[County] DROP CONSTRAINT [FK_County_City];
GO
IF OBJECT_ID(N'[dbo].[FK_Invoice_PartyB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoice] DROP CONSTRAINT [FK_Invoice_PartyB];
GO
IF OBJECT_ID(N'[dbo].[FK_Invoice_Project1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Invoice] DROP CONSTRAINT [FK_Invoice_Project1];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_County]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_County];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_PartyB]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_PartyB];
GO
IF OBJECT_ID(N'[dbo].[FK_User_UserClass]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User] DROP CONSTRAINT [FK_User_UserClass];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[City]', 'U') IS NOT NULL
    DROP TABLE [dbo].[City];
GO
IF OBJECT_ID(N'[dbo].[Cost]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cost];
GO
IF OBJECT_ID(N'[dbo].[County]', 'U') IS NOT NULL
    DROP TABLE [dbo].[County];
GO
IF OBJECT_ID(N'[dbo].[Invoice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Invoice];
GO
IF OBJECT_ID(N'[dbo].[PartyB]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PartyB];
GO
IF OBJECT_ID(N'[dbo].[Project]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Project];
GO
IF OBJECT_ID(N'[dbo].[Province]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Province];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[UserClass]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserClass];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'City'
CREATE TABLE [dbo].[City] (
    [ID] int  NOT NULL,
    [Name] nvarchar(20)  NOT NULL,
    [ProvinceID] int  NOT NULL
);
GO

-- Creating table 'County'
CREATE TABLE [dbo].[County] (
    [ID] int  NOT NULL,
    [Name] nvarchar(20)  NOT NULL,
    [CityID] int  NOT NULL
);
GO

-- Creating table 'PartyB'
CREATE TABLE [dbo].[PartyB] (
    [ID] int  NOT NULL,
    [Name] varchar(50)  NOT NULL
);
GO

-- Creating table 'Project'
CREATE TABLE [dbo].[Project] (
    [ID] int  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [CountyID] int  NOT NULL,
    [PartyAName] varchar(50)  NOT NULL,
    [PartyBID] int  NOT NULL,
    [ResponsiblePerson] varchar(20)  NULL,
    [ContactPhone] varchar(20)  NULL,
    [Date] datetime  NOT NULL,
    [Money] int  NOT NULL
);
GO

-- Creating table 'Province'
CREATE TABLE [dbo].[Province] (
    [ID] int  NOT NULL,
    [Name] nvarchar(10)  NOT NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [UserName] nvarchar(20)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [UserClassID] int  NOT NULL
);
GO

-- Creating table 'UserClass'
CREATE TABLE [dbo].[UserClass] (
    [ID] int  NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Permission] varchar(max)  NOT NULL
);
GO

-- Creating table 'Invoice'
CREATE TABLE [dbo].[Invoice] (
    [ID] varchar(20)  NOT NULL,
    [ProjectID] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [PartyBID] int  NOT NULL,
    [Money] int  NOT NULL,
    [Type] nvarchar(10)  NULL,
    [TaxPayed] int  NOT NULL,
    [PayMethod] nvarchar(10)  NULL
);
GO

-- Creating table 'Cost'
CREATE TABLE [dbo].[Cost] (
    [ID] int  NOT NULL,
    [ProjectID] int  NOT NULL,
    [PartyBID] int  NOT NULL,
    [Money] int  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'City'
ALTER TABLE [dbo].[City]
ADD CONSTRAINT [PK_City]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'County'
ALTER TABLE [dbo].[County]
ADD CONSTRAINT [PK_County]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PartyB'
ALTER TABLE [dbo].[PartyB]
ADD CONSTRAINT [PK_PartyB]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [PK_Project]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Province'
ALTER TABLE [dbo].[Province]
ADD CONSTRAINT [PK_Province]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [UserName] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([UserName] ASC);
GO

-- Creating primary key on [ID] in table 'UserClass'
ALTER TABLE [dbo].[UserClass]
ADD CONSTRAINT [PK_UserClass]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Invoice'
ALTER TABLE [dbo].[Invoice]
ADD CONSTRAINT [PK_Invoice]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Cost'
ALTER TABLE [dbo].[Cost]
ADD CONSTRAINT [PK_Cost]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ProvinceID] in table 'City'
ALTER TABLE [dbo].[City]
ADD CONSTRAINT [FK_City_Province]
    FOREIGN KEY ([ProvinceID])
    REFERENCES [dbo].[Province]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_City_Province'
CREATE INDEX [IX_FK_City_Province]
ON [dbo].[City]
    ([ProvinceID]);
GO

-- Creating foreign key on [CityID] in table 'County'
ALTER TABLE [dbo].[County]
ADD CONSTRAINT [FK_County_City]
    FOREIGN KEY ([CityID])
    REFERENCES [dbo].[City]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_County_City'
CREATE INDEX [IX_FK_County_City]
ON [dbo].[County]
    ([CityID]);
GO

-- Creating foreign key on [CountyID] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [FK_Project_County]
    FOREIGN KEY ([CountyID])
    REFERENCES [dbo].[County]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_County'
CREATE INDEX [IX_FK_Project_County]
ON [dbo].[Project]
    ([CountyID]);
GO

-- Creating foreign key on [PartyBID] in table 'Project'
ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [FK_Project_PartyB]
    FOREIGN KEY ([PartyBID])
    REFERENCES [dbo].[PartyB]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_PartyB'
CREATE INDEX [IX_FK_Project_PartyB]
ON [dbo].[Project]
    ([PartyBID]);
GO

-- Creating foreign key on [UserClassID] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [FK_User_UserClass]
    FOREIGN KEY ([UserClassID])
    REFERENCES [dbo].[UserClass]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_User_UserClass'
CREATE INDEX [IX_FK_User_UserClass]
ON [dbo].[User]
    ([UserClassID]);
GO

-- Creating foreign key on [PartyBID] in table 'Invoice'
ALTER TABLE [dbo].[Invoice]
ADD CONSTRAINT [FK_Invoice_PartyB]
    FOREIGN KEY ([PartyBID])
    REFERENCES [dbo].[PartyB]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Invoice_PartyB'
CREATE INDEX [IX_FK_Invoice_PartyB]
ON [dbo].[Invoice]
    ([PartyBID]);
GO

-- Creating foreign key on [ProjectID] in table 'Invoice'
ALTER TABLE [dbo].[Invoice]
ADD CONSTRAINT [FK_Invoice_Project1]
    FOREIGN KEY ([ProjectID])
    REFERENCES [dbo].[Project]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Invoice_Project1'
CREATE INDEX [IX_FK_Invoice_Project1]
ON [dbo].[Invoice]
    ([ProjectID]);
GO

-- Creating foreign key on [PartyBID] in table 'Cost'
ALTER TABLE [dbo].[Cost]
ADD CONSTRAINT [FK_Cost_PartyB]
    FOREIGN KEY ([PartyBID])
    REFERENCES [dbo].[PartyB]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Cost_PartyB'
CREATE INDEX [IX_FK_Cost_PartyB]
ON [dbo].[Cost]
    ([PartyBID]);
GO

-- Creating foreign key on [ProjectID] in table 'Cost'
ALTER TABLE [dbo].[Cost]
ADD CONSTRAINT [FK_Cost_Project]
    FOREIGN KEY ([ProjectID])
    REFERENCES [dbo].[Project]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Cost_Project'
CREATE INDEX [IX_FK_Cost_Project]
ON [dbo].[Cost]
    ([ProjectID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------