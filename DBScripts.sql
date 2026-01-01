
-- User Table
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Gender NVARCHAR(10),
    DateOfBirth DATE NOT NULL,
    TimeOfBirth TIME NOT NULL,
    PlaceOfBirth NVARCHAR(100) NOT NULL,
    Latitude DECIMAL(9,6),
    Longitude DECIMAL(9,6),
    CreatedOn DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);


-- Astrology Master Table 

CREATE TABLE ZodiacSigns (
    ZodiacId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    StartMonth INT NOT NULL,
    StartDay INT NOT NULL,
    EndMonth INT NOT NULL,
    EndDay INT NOT NULL,
    Symbol NVARCHAR(20)
);

-- Planets 

CREATE TABLE Planets (
    PlanetId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Symbol NVARCHAR(10)
);

--Houses 
CREATE TABLE Houses (
    HouseId INT IDENTITY PRIMARY KEY,
    HouseNumber INT NOT NULL,
    Description NVARCHAR(255)
);

--HororScope type 
CREATE TABLE HoroscopeTypes (
    TypeId INT IDENTITY PRIMARY KEY,
    TypeName NVARCHAR(20) -- Daily, Weekly, Monthly
);

-- Horor scopes 
CREATE TABLE Horoscopes (
    HoroscopeId INT IDENTITY PRIMARY KEY,
    ZodiacId INT NOT NULL,
    TypeId INT NOT NULL,
    HoroscopeDate DATE NOT NULL,
    PredictionText NVARCHAR(MAX) NOT NULL,
    CreatedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ZodiacId) REFERENCES ZodiacSigns(ZodiacId),
    FOREIGN KEY (TypeId) REFERENCES HoroscopeTypes(TypeId)
);

-- Predection
CREATE TABLE Predictions (
    PredictionId INT IDENTITY PRIMARY KEY,
    ZodiacId INT NOT NULL,
    Category NVARCHAR(50), -- Love, Career, Health
    Content NVARCHAR(MAX),
    ValidFrom DATE,
    ValidTo DATE,
    FOREIGN KEY (ZodiacId) REFERENCES ZodiacSigns(ZodiacId)
);

--Rashi
CREATE TABLE Rashis (
    RashiId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50)
);

--Nakshatra 
CREATE TABLE Nakshatras (
    NakshatraId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50),
    StartDegree DECIMAL(5,2),
    EndDegree DECIMAL(5,2)
);

-- User Planet Position 
CREATE TABLE UserPlanetPositions (
    PositionId INT IDENTITY PRIMARY KEY,
    UserId INT,
    PlanetId INT,
    RashiId INT,
    HouseId INT,
    Degree DECIMAL(5,2),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PlanetId) REFERENCES Planets(PlanetId),
    FOREIGN KEY (RashiId) REFERENCES Rashis(RashiId),
    FOREIGN KEY (HouseId) REFERENCES Houses(HouseId)
);

-- User Kundli meta Data 
CREATE TABLE Kundli (
    KundliId INT IDENTITY PRIMARY KEY,
    UserId INT,
    CreatedOn DATETIME DEFAULT GETDATE(),
    LagnaRashiId INT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (LagnaRashiId) REFERENCES Rashis(RashiId)
);

-- Admin Users 
CREATE TABLE AdminUsers (
    AdminId INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50),
    PasswordHash NVARCHAR(255),
    Role NVARCHAR(20)
);

-- Indexing 
CREATE INDEX IX_Horoscope_Zodiac_Date 
ON Horoscopes (ZodiacId, HoroscopeDate);

CREATE INDEX IX_UserPlanetPositions_UserId 
ON UserPlanetPositions (UserId);

