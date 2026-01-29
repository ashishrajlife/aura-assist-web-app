
-- User Table 

CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(15),
    Role NVARCHAR(20) NOT NULL, -- "User", "Influencer", "Agency"
    IsApproved BIT DEFAULT 0,
    City NVARCHAR(50) NULL, -- user location
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- InfluencerProfiles Table

CREATE TABLE InfluencerProfiles (
    InfluencerProfileId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    InstagramHandle NVARCHAR(100),
    TwitterHandle NVARCHAR(100),
    WhatsappHandle NVARCHAR(100),
    FollowersCount INT DEFAULT 0,
    Category NVARCHAR(50),
    BasePrice DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- AgencyProfiles Table

CREATE TABLE AgencyProfiles (
    AgencyProfileId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    AgencyName NVARCHAR(100),
    InstagramHandle NVARCHAR(100),
    TwitterHandle NVARCHAR(100),
    WhatsappHandle NVARCHAR(100),
    Services NVARCHAR(200),
    City NVARCHAR(50),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

------- Alter Table ---------------

-- Make Category column nullable
ALTER TABLE InfluencerProfiles 
ALTER COLUMN Category NVARCHAR(50) NULL;

-- Also make other columns nullable if needed
ALTER TABLE InfluencerProfiles 
ALTER COLUMN InstagramHandle NVARCHAR(100) NULL;

ALTER TABLE InfluencerProfiles 
ALTER COLUMN TwitterHandle NVARCHAR(100) NULL;

ALTER TABLE InfluencerProfiles 
ALTER COLUMN WhatsappHandle NVARCHAR(100) NULL;