
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


--Master Data-----------

INSERT INTO ZodiacSigns (Name, StartMonth, StartDay, EndMonth, EndDay, Symbol) VALUES
('Aries', 3, 21, 4, 19, '♈'),
('Taurus', 4, 20, 5, 20, '♉'),
('Gemini', 5, 21, 6, 20, '♊'),
('Cancer', 6, 21, 7, 22, '♋'),
('Leo', 7, 23, 8, 22, '♌'),
('Virgo', 8, 23, 9, 22, '♍'),
('Libra', 9, 23, 10, 22, '♎'),
('Scorpio', 10, 23, 11, 21, '♏'),
('Sagittarius', 11, 22, 12, 21, '♐'),
('Capricorn', 12, 22, 1, 19, '♑'),
('Aquarius', 1, 20, 2, 18, '♒'),
('Pisces', 2, 19, 3, 20, '♓');

INSERT INTO HoroscopeTypes (TypeName) VALUES
('Daily'),
('Weekly'),
('Monthly');

INSERT INTO Planets (Name, Symbol) VALUES
('Sun', '☉'),
('Moon', '☽'),
('Mars', '♂'),
('Mercury', '☿'),
('Jupiter', '♃'),
('Venus', '♀'),
('Saturn', '♄'),
('Rahu', '☊'),
('Ketu', '☋');

INSERT INTO Houses (HouseNumber, Description) VALUES
(1, 'Self, Appearance, Personality'),
(2, 'Wealth, Family, Speech'),
(3, 'Courage, Communication, Siblings'),
(4, 'Home, Mother, Comfort'),
(5, 'Creativity, Children, Intelligence'),
(6, 'Disease, Enemies, Service'),
(7, 'Marriage, Partnerships'),
(8, 'Longevity, Transformation'),
(9, 'Luck, Dharma, Higher Knowledge'),
(10, 'Career, Status, Karma'),
(11, 'Gains, Income, Fulfillment'),
(12, 'Loss, Moksha, Isolation');

INSERT INTO Rashis (Name) VALUES
('Mesha'),
('Vrishabha'),
('Mithuna'),
('Karka'),
('Simha'),
('Kanya'),
('Tula'),
('Vrischika'),
('Dhanu'),
('Makara'),
('Kumbha'),
('Meena');

INSERT INTO Nakshatras (Name, StartDegree, EndDegree) VALUES
('Ashwini', 0.00, 13.20),
('Bharani', 13.20, 26.40),
('Krittika', 26.40, 40.00),
('Rohini', 40.00, 53.20),
('Mrigashira', 53.20, 66.40),
('Ardra', 66.40, 80.00),
('Punarvasu', 80.00, 93.20),
('Pushya', 93.20, 106.40),
('Ashlesha', 106.40, 120.00),
('Magha', 120.00, 133.20),
('Purva Phalguni', 133.20, 146.40),
('Uttara Phalguni', 146.40, 160.00),
('Hasta', 160.00, 173.20),
('Chitra', 173.20, 186.40),
('Swati', 186.40, 200.00),
('Vishakha', 200.00, 213.20),
('Anuradha', 213.20, 226.40),
('Jyeshtha', 226.40, 240.00),
('Mula', 240.00, 253.20),
('Purva Ashadha', 253.20, 266.40),
('Uttara Ashadha', 266.40, 280.00),
('Shravana', 280.00, 293.20),
('Dhanishta', 293.20, 306.40),
('Shatabhisha', 306.40, 320.00),
('Purva Bhadrapada', 320.00, 333.20),
('Uttara Bhadrapada', 333.20, 346.40),
('Revati', 346.40, 360.00);


-- Sample Data 

INSERT INTO Horoscopes 
(ZodiacId, TypeId, HoroscopeDate, PredictionText)
VALUES 
(5, 1, CAST(GETDATE() AS DATE),
'Today brings confidence and leadership opportunities.');
