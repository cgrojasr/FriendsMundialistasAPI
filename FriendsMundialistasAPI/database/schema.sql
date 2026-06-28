IF OBJECT_ID('dbo.Grupos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Grupos
    (
        IdGrupo INT NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL
    );
END
GO

IF OBJECT_ID('dbo.Equipos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Equipos
    (
        IdEquipo INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        IdGrupo INT NULL,
        FechaCreacion DATETIMEOFFSET NOT NULL,
        CONSTRAINT FK_Equipos_Grupos FOREIGN KEY (IdGrupo) REFERENCES dbo.Grupos(IdGrupo)
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Equipos_Nombre'
      AND object_id = OBJECT_ID('dbo.Equipos')
)
BEGIN
    CREATE UNIQUE INDEX UX_Equipos_Nombre ON dbo.Equipos(Nombre);
END
GO
