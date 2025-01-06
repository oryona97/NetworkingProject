-- Create Database
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'eBook')
CREATE DATABASE eBook;
GO

-- Drop all foreign keys first
DECLARE @sql NVARCHAR(MAX) = ''

SELECT @sql += 'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' +
               QUOTENAME(OBJECT_NAME(parent_object_id)) + 
               ' DROP CONSTRAINT ' + QUOTENAME(name) + ';'

FROM sys.foreign_keys

EXEC sp_executesql @sql

-- Drop existing tables if they exist (in reverse order of creation)
IF OBJECT_ID('dbo.UserNotifications', 'U') IS NOT NULL DROP TABLE dbo.UserNotifications;
IF OBJECT_ID('dbo.BookDiscount', 'U') IS NOT NULL DROP TABLE dbo.BookDiscount;
IF OBJECT_ID('dbo.Reciept', 'U') IS NOT NULL DROP TABLE dbo.Reciept;
IF OBJECT_ID('dbo.BookShoppingCart', 'U') IS NOT NULL DROP TABLE dbo.BookShoppingCart;
IF OBJECT_ID('dbo.Book_ShoppingCart', 'U') IS NOT NULL DROP TABLE dbo.Book_ShoppingCart;
IF OBJECT_ID('dbo.ShoppingCart', 'U') IS NOT NULL DROP TABLE dbo.ShoppingCart;
IF OBJECT_ID('dbo.HistoryBookPrice', 'U') IS NOT NULL DROP TABLE dbo.HistoryBookPrice;
IF OBJECT_ID('dbo.Cover', 'U') IS NOT NULL DROP TABLE dbo.Cover;
IF OBJECT_ID('dbo.Auther', 'U') IS NOT NULL DROP TABLE dbo.Auther;
IF OBJECT_ID('dbo.BookRentQueue', 'U') IS NOT NULL DROP TABLE dbo.BookRentQueue;
IF OBJECT_ID('dbo.GeneralFeedback', 'U') IS NOT NULL DROP TABLE dbo.GeneralFeedback;
IF OBJECT_ID('dbo.GeneralRating', 'U') IS NOT NULL DROP TABLE dbo.GeneralRating;
IF OBJECT_ID('dbo.Feedback', 'U') IS NOT NULL DROP TABLE dbo.Feedback;
IF OBJECT_ID('dbo.Rating', 'U') IS NOT NULL DROP TABLE dbo.Rating;
IF OBJECT_ID('dbo.HistoryPurchases', 'U') IS NOT NULL DROP TABLE dbo.HistoryPurchases;
IF OBJECT_ID('dbo.BorrowedBooks', 'U') IS NOT NULL DROP TABLE dbo.BorrowedBooks;
IF OBJECT_ID('dbo.PersonalLibrary', 'U') IS NOT NULL DROP TABLE dbo.PersonalLibrary;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.Book', 'U') IS NOT NULL DROP TABLE dbo.Book;
IF OBJECT_ID('dbo.Publisher', 'U') IS NOT NULL DROP TABLE dbo.Publisher;
IF OBJECT_ID('dbo.Genre', 'U') IS NOT NULL DROP TABLE dbo.Genre;
GO

-- Recreate tables with original schema
CREATE TABLE [User] (
  [id] int IDENTITY(50,1) PRIMARY KEY ,
  [userName] nvarchar(50) UNIQUE,
  [password] nvarchar(100),
  [firstName] nvarchar(50),
  [lastName] nvarchar(50),
  [email] nvarchar(100),
  [type] nvarchar(20) CHECK(type IN('user','admin')),
  [phoneNumber] nvarchar(20),
  [createdAt] datetime
)


GO

CREATE TABLE [PersonalLibrary] (
  [userId] int,
  [bookId] int ,
  PRIMARY KEY (userId,bookId),
  [createdAt] datetime
)
GO

CREATE TABLE [Book] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [genreId] int,
  [amountOfCopies] int CHECK(amountOfCopies >= 0 and amountOfCopies <= 3),
  [title] nvarchar(200),
  [borrowPrice] decimal(10,2),
  [buyingPrice] decimal(10,2),
  [pubDate] date,
  [ageLimit] int,
  [priceHistory] int,
  [onSale] bit,
  [canBorrow] bit,
  [starRate] decimal(3,2),
  [createdAt] datetime,
  [publisherId] int,

)
GO

CREATE TABLE [BorrowedBooks] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [userId] int,
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [Genre] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [name] nvarchar(50),
  [createdAt] datetime
)
GO

CREATE TABLE [Publisher] (
    [id] int IDENTITY(50,1) PRIMARY KEY,
    [bookId] int,
    [name] nvarchar(100) UNIQUE,
    [createdAt] datetime,
    CONSTRAINT PK_Publisher UNIQUE ([id], [bookId])
)
GO

CREATE TABLE [HistoryPurchases] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [bookId] int,
  [bookPrice] decimal(10,2),
  [userId] int,
  [purchaseDate] date,
  [createdAt] datetime
)
GO

CREATE TABLE [Rating] (
  [starRating] int,
  [userId] int,
  [bookId] int,
  PRIMARY KEY (userId, bookId),
  [createdAt] datetime
)
GO

CREATE TABLE [Feedback] (
  [userId] int,
  [bookId] int,
  [comment] nvarchar(max),
  PRIMARY KEY (userId, bookId),
  [createdAt] datetime
)
GO

CREATE TABLE [GeneralRating] (
  [id] int IDENTITY(50,1),
  [starRating] int,
  [userId] int,
  PRIMARY KEY (userId, id),
  [createdAt] datetime
)
GO

CREATE TABLE [GeneralFeedback] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [userId] int,
  [comment] nvarchar(max),
  [createdAt] datetime
)
GO

CREATE TABLE [BookRentQueue] (
  [id] int IDENTITY(50,1),
  [bookId] int,
  [userId] int,
  [createdAt] datetime,
  PRIMARY KEY (bookId, userId)
)
GO

CREATE TABLE [Auther] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [name] nvarchar(100),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [Cover] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [imgName] nvarchar(200),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [HistoryBookPrice] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [price] decimal(10,2),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [ShoppingCart] (
  [userId] int,
  [createdAt] datetime,
  PRIMARY key(userId )
)
GO


CREATE TABLE [BookShoppingCart] (
  [bookId] int,
  [userId] int,
  Format NVARCHAR(10) CHECK (Format IN ('epub', 'f2b', 'mobi', 'PDF')),
  [createdAt] datetime,
  PRIMARY KEY (bookId, userId)

)
GO

CREATE TABLE [Reciept] (
  [id] int IDENTITY(50,1) PRIMARY KEY,
  [userId] int,
  [bookId] int,
  [total] decimal(10,2),
  [createdAt] datetime
)
GO

CREATE TABLE [BookDiscount] (
    bookId int PRIMARY KEY REFERENCES [Book](id) ON DELETE CASCADE,
    discountPercentage decimal(5, 2) NOT NULL CHECK (discountPercentage >= 0.0 AND discountPercentage <= 1.0),
    saleStartDate datetime DEFAULT GETDATE(),
    saleEndDate datetime NOT NULL 
);
GO
CREATE TABLE [UserNotifications] (
    id INT IDENTITY(1,1) PRIMARY KEY, 
    userId INT NOT NULL REFERENCES [User](id) ON DELETE CASCADE, 
    message NVARCHAR(MAX) NOT NULL, 
    createdAt DATETIME DEFAULT GETDATE()
);
GO


-- Add Foreign Key Constraints with ON DELETE CASCADE
ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Book] ADD FOREIGN KEY ([genreId]) REFERENCES [Genre] ([id]) ON DELETE CASCADE;
GO
            -- this constraint is to ensure that the borrow price is less than the buying price
ALTER TABLE [Book] ADD CONSTRAINT CHK_Book_BorrowPrice_LessThan_BuyingPrice CHECK (borrowPrice < buyingPrice);
GO
ALTER TABLE [Publisher] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE NO ACTION;
GO
ALTER TABLE [HistoryBookPrice] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BorrowedBooks] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BorrowedBooks] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [HistoryPurchases] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [HistoryPurchases] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Rating] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Rating] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Feedback] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Feedback] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [GeneralRating] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [GeneralFeedback] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BookRentQueue] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BookRentQueue] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Auther] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Cover] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [ShoppingCart] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([userId]) REFERENCES [ShoppingCart] ([userId]) ON DELETE CASCADE;
GO
ALTER TABLE [Reciept] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id]) ON DELETE CASCADE;
GO
ALTER TABLE [Reciept] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id]) ON DELETE CASCADE;
GO

--Triggers--
CREATE TRIGGER trg_DeleteExpiredBorrowedBooks
ON BorrowedBooks
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    --temp table to store deleted records
    DECLARE @DeletedBooks TABLE (
        bookId INT,
        userId INT,
        createdAt DATETIME
    );

    -- delete expired borrowed books
    DELETE FROM BorrowedBooks
    OUTPUT DELETED.bookId, DELETED.userId, DELETED.createdAt INTO @DeletedBooks
    WHERE DATEDIFF(DAY, createdAt, GETDATE()) > 30;

    --match the deleted books with the personal library and delete them
    DELETE PL
    FROM PersonalLibrary PL
    WHERE EXISTS (
        SELECT 1
        FROM @DeletedBooks DB
        WHERE DB.bookId = PL.bookId
          AND DB.userId = PL.userId
    );

    -- upfate amoutOfCoppies in the book table
    UPDATE B
    SET amountOfCopies = CASE 
                            WHEN amountOfCopies < 3 THEN amountOfCopies + 1 
                            ELSE amountOfCopies 
                         END
    FROM Book B
    WHERE EXISTS (
        SELECT 1
        FROM @DeletedBooks DB
        WHERE DB.bookId = B.id
    );

    PRINT 'Expired borrowed books have been deleted, and inventory updated successfully.';
END;
GO



CREATE TRIGGER trg_DeleteInvalidSaleEndDate
ON [BookDiscount]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [BookDiscount]
    WHERE saleEndDate < GETDATE();

    PRINT 'Rows with past saleEndDate have been deleted.';
END;
GO

--this trigger for sand massege to user when the book is due in 5 days
IF OBJECT_ID('dbo.sp_NotifyUsersOnBorrowedBooks', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_NotifyUsersOnBorrowedBooks;
GO

CREATE PROCEDURE sp_NotifyUsersOnBorrowedBooks
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [UserNotifications] (userId, message)
    SELECT 
        BB.userId,
        CONCAT('The book with ID ', BB.bookId, ' is due in 5 days. Please return it on time.')
    FROM [BorrowedBooks] BB
    WHERE 
        DATEDIFF(DAY, BB.createdAt, GETDATE()) = 25
        AND NOT EXISTS (
            SELECT 1
            FROM [UserNotifications] UN
            WHERE UN.userId = BB.userId 
              AND UN.message LIKE CONCAT('%book with ID ', BB.bookId, '%')
        );

    PRINT 'Notifications have been added for users.';
END;
GO


CREATE TRIGGER trg_ValidateSaleEndDate
ON [BookDiscount]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [BookDiscount]
    WHERE DATEDIFF(DAY, saleStartDate, saleEndDate) > 7;

    PRINT 'Rows with invalid saleEndDate (more than 7 days after saleStartDate) have been deleted.';
END;
GO

SET IDENTITY_INSERT [User] ON;
-- Insert comprehensive dummy data
-- Users
INSERT INTO [User] ([id], [userName], [password], [firstName], [lastName], [email], [type], [phoneNumber], [createdAt])
VALUES 
(1, 'johndoe', 'hashed_password_1', 'John', 'Doe', 'john.doe@example.com', 'user', '1234567890', GETDATE()),
(2, 'janesmith', 'hashed_password_2', 'Jane', 'Smith', 'jane.smith@example.com', 'user', '9876543210', GETDATE()),
(3, 'mikebrown', 'hashed_password_3', 'Mike', 'Brown', 'mike.brown@example.com', 'admin', '5551234567', GETDATE()),
(4, 'sarahlee', 'hashed_password_4', 'Sarah', 'Lee', 'sarah.lee@example.com', 'user', '7778889999', GETDATE()),
(5, 'davidwang', 'hashed_password_5', 'David', 'Wang', 'david.wang@example.com', 'user', '3334445555', GETDATE());

SET IDENTITY_INSERT [User] OFF;

-- Genres
SET IDENTITY_INSERT [Genre] ON;
INSERT INTO [Genre] ([id], [name], [createdAt])
VALUES 
(1, 'Fiction', GETDATE()),
(2, 'Non-Fiction', GETDATE()),
(3, 'Science Fiction', GETDATE()),
(4, 'Mystery', GETDATE()),
(5, 'Romance', GETDATE());
SET IDENTITY_INSERT [Genre] OFF;

-- Books
SET IDENTITY_INSERT [Book] ON;

INSERT INTO [Book] ([id], [genreId], [amountOfCopies], [title], [borrowPrice], [buyingPrice], 
                    [pubDate], [ageLimit], [priceHistory], [onSale], [canBorrow], [starRate], [createdAt])
VALUES 
(1, 1, 3, 'The Great Adventure', 2.99, 15.99, '2023-01-15', 12, 1, 0, 1, 4.5, GETDATE()),
(2, 3, 2, 'Starship Chronicles', 3.50, 19.99, '2022-11-20', 16, 2, 0, 0, 4.7, GETDATE()),
(3, 4, 3, 'Murder in the Library', 2.50, 14.50, '2023-05-10', 18, 3, 0, 1, 4.2, GETDATE()),
(4, 5, 3, 'Love Across Continents', 2.75, 16.50, '2023-02-28', 16, 4, 0, 1, 4.6, GETDATE()),
(5, 2, 3, 'The Rise of Technology', 3.25, 18.99, '2022-09-05', 14, 5, 0, 1, 4.4, GETDATE());

SET IDENTITY_INSERT [Book] OFF;

-- Publishers
SET IDENTITY_INSERT [Publisher] ON;
INSERT INTO [Publisher] ([id], [bookId], [name], [createdAt])
VALUES 
(1, 1, 'Penguin Random House', GETDATE()),
(2, 2, 'HarperCollins', GETDATE()),
(3, 3, 'Simon & Schuster', GETDATE()),
(4, 4, 'Macmillan Publishers', GETDATE()),
(5, 5, 'Hachette Book Group', GETDATE());
SET IDENTITY_INSERT [Publisher]OFF;

-- Authors
SET IDENTITY_INSERT [Auther] ON;
INSERT INTO [Auther] ([id], [name], [bookId], [createdAt])
VALUES 
(1, 'Emily Johnson', 1, GETDATE()),
(2, 'Michael Chang', 2, GETDATE()),
(3, 'Sarah Rodriguez', 3, GETDATE()),
(4, 'David Kim', 4, GETDATE()),
(5, 'Rachel Green', 5, GETDATE());
SET IDENTITY_INSERT [Auther] OFF;

-- Covers
SET IDENTITY_INSERT [Cover] ON;
INSERT INTO [Cover] ([id], [imgName], [bookId], [createdAt])
VALUES 
(1, 'great_adventure_cover.jpg', 1, GETDATE()),
(2, 'starship_chronicles_cover.png', 2, GETDATE()),
(3, 'murder_library_cover.jpg', 3, GETDATE()),
(4, 'love_continents_cover.jpg', 4, GETDATE()),
(5, 'tech_rise_cover.jpg', 5, GETDATE());

SET IDENTITY_INSERT [Cover] OFF;

-- Personal Library


INSERT INTO [PersonalLibrary] ([userId], [bookId], [createdAt])
VALUES 
( 1, 2, GETDATE()),
( 2, 2, GETDATE()),
( 3, 3, GETDATE()),
( 4, 4, GETDATE()),
( 5, 5, GETDATE());


-- Borrowed Books
SET IDENTITY_INSERT [BorrowedBooks] ON;
INSERT INTO [BorrowedBooks] ([id], [userId], [bookId],[createdAt])
VALUES 
(1, 1, 2,GETDATE()),
(2, 2, 3,GETDATE()),
(3, 3, 4,GETDATE()),
(4, 4, 5,GETDATE()),
(5, 5, 1,GETDATE());
SET IDENTITY_INSERT [BorrowedBooks] OFF;

-- Ratings

INSERT INTO [Rating] ([starRating], [userId], [bookId], [createdAt])
VALUES 
(4, 1, 1, GETDATE()),
(5, 2, 2, GETDATE()),
(4, 3, 3, GETDATE()),
(5, 4, 4, GETDATE()),
(4, 5, 5, GETDATE());


-- Feedback

INSERT INTO [Feedback] ([userId], [bookId], [comment], [createdAt])
VALUES 
(1, 1, 'Great book, really enjoyed the adventure!', GETDATE()),
(2, 2, 'Exciting sci-fi, couldn''t put it down', GETDATE()),
(3, 3, 'Thrilling mystery with unexpected twists', GETDATE()),
(4, 4, 'A beautiful love story that spans continents', GETDATE()),
(5, 5, 'Insightful look into technological innovations', GETDATE());

-- Book Rent Queue
SET IDENTITY_INSERT [BookRentQueue] ON;
INSERT INTO [BookRentQueue] ([id], [bookId], [userId], [createdAt])
VALUES 
(1, 1, 2, GETDATE()),
(2, 2, 3, GETDATE()),
(3, 3, 4, GETDATE()),
(5, 5, 1, GETDATE());
SET IDENTITY_INSERT [BookRentQueue] OFF;

-- History Purchases
SET IDENTITY_INSERT [HistoryPurchases] ON;
INSERT INTO [HistoryPurchases] ([id], [bookId], [bookPrice], [userId], [purchaseDate], [createdAt])
VALUES 
(1, 1, 15.99, 1, GETDATE(), GETDATE()),
(2, 2, 19.99, 2, GETDATE(), GETDATE()),
(3, 3, 14.50, 3, GETDATE(), GETDATE()),
(4, 4, 16.50, 4, GETDATE(), GETDATE()),
(5, 5, 18.99, 5, GETDATE(), GETDATE());
SET IDENTITY_INSERT [HistoryPurchases] OFF;

-- History Book Price
SET IDENTITY_INSERT [HistoryBookPrice] ON;
INSERT INTO [HistoryBookPrice] ([id],  [price], [bookId], [createdAt])
VALUES 
(1,  14.99, 1, GETDATE()),
(2,  18.99, 2, GETDATE()),
(3,  13.50, 3, GETDATE()),
(4,  15.50, 4, GETDATE()),
(5,  17.99, 5, GETDATE());
SET IDENTITY_INSERT [HistoryBookPrice] OFF;

-- Shopping Cart
INSERT INTO [ShoppingCart] ([userId],  [createdAt])
VALUES 
(1,  GETDATE()),
(2,  GETDATE()),
(3,  GETDATE()),
(4,  GETDATE()),
(5,  GETDATE());

-- Book Shopping Cart
INSERT INTO [BookShoppingCart] ([bookId], [userId], [format])
VALUES 
(1, 1, 'epub'),
(2, 2, 'PDF'),
(3, 3, 'mobi'),
(4, 4, 'f2b'),
(5, 5, 'PDF');

-- Receipts
SET IDENTITY_INSERT [Reciept] ON;

INSERT INTO [Reciept] ([id], [userId], [bookId], [total], [createdAt])
VALUES 
(1, 1, 2, 19.99, GETDATE()),
(2, 2, 3, 14.50, GETDATE()),
(3, 3, 4, 16.50, GETDATE()),
(4, 4, 5, 18.99, GETDATE()),
(5, 5, 1, 15.99, GETDATE());
SET IDENTITY_INSERT [Reciept] OFF;
-- General Rating

SET IDENTITY_INSERT [GeneralRating] ON;
INSERT INTO [GeneralRating] ([id], [starRating], [userId], [createdAt])
VALUES 
(1, 4, 1, GETDATE()),
(2, 5, 2, GETDATE()),
(3, 4, 3, GETDATE()),
(4, 5, 4, GETDATE()),
(5, 4, 5, GETDATE());
SET IDENTITY_INSERT [GeneralRating] OFF;
-- General Feedback
SET IDENTITY_INSERT [GeneralFeedback] ON;
INSERT INTO [GeneralFeedback] ([id], [userId], [comment], [createdAt])
VALUES 
(1, 1, 'Enjoying the platform so far!', GETDATE()),
(2, 2, 'Great selection of books', GETDATE()),
(3, 3, 'User interface is very intuitive', GETDATE()),
(4, 4, 'Love the recommendation system', GETDATE()),
(5, 5, 'Excellent customer support', GETDATE());
SET IDENTITY_INSERT [GeneralFeedback] OFF;
-- Print success message
PRINT 'eBook Database created and populated successfully!';
GO


