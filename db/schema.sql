-- Create Database
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'eBook')
CREATE DATABASE eBook;
GO

USE eBook;
GO

-- Drop existing tables if they exist (in reverse order of creation)
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
IF OBJECT_ID('dbo.Genre', 'U') IS NOT NULL DROP TABLE dbo.Genre;
IF OBJECT_ID('dbo.Publisher', 'U') IS NOT NULL DROP TABLE dbo.Publisher;


GO

-- Recreate tables with original schema
CREATE TABLE [User] (
  [id] int PRIMARY KEY,
  [userName] nvarchar(50),
  [password] nvarchar(100),
  [firstName] nvarchar(50),
  [lastName] nvarchar(50),
  [email] nvarchar(100),
  [type] nvarchar(20),
  [phoneNumber] nvarchar(20),
  [createdAt] datetime
)
GO

CREATE TABLE [PersonalLibrary] (
  [id] int,
  [userId] int,
  [bookId] int,
  PRIMARY KEY (id, userId, bookId),
  [createdAt] datetime
)
GO

CREATE TABLE [Book] (
  [id] int PRIMARY KEY,
  [publisherId] int,
  [genreId] int,
  [amountOfCopies] int,
  [title] nvarchar(200),
  [borrowPrice] decimal(10,2),
  [buyingPrice] decimal(10,2),
  [pubDate] date,
  [ageLimit] int,
  [priceHistory] int,
  [onSale] bit,
  [canBorrow] bit,
  [starRate] decimal(3,2),
  [createdAt] datetime
  
)
GO

CREATE TABLE [BorrowedBooks] (
  [id] int PRIMARY KEY,
  [userId] int,
  [bookId] int
)
GO

CREATE TABLE [Genre] (
  [id] int PRIMARY KEY,
  [name] nvarchar(50),
  [createdAt] datetime
)
GO

CREATE TABLE [Publisher] (
  [id] int PRIMARY KEY ,
  [name] nvarchar(100) UNIQUE,
  [createdAt] datetime,
  
)
GO

CREATE TABLE [HistoryPurchases] (
  [id] int PRIMARY KEY,
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
  [id] int,
  [starRating] int,
  [userId] int,
  PRIMARY KEY (userId, id),
  [createdAt] datetime
)
GO

CREATE TABLE [GeneralFeedback] (
  [id] int PRIMARY KEY,
  [userId] int,
  [comment] nvarchar(max),
  [createdAt] datetime
)
GO

CREATE TABLE [BookRentQueue] (
  [id] int,
  [bookId] int,
  [userId] int,
  [createdAt] datetime,
  PRIMARY KEY (bookId, userId)
)
GO

CREATE TABLE [Auther] (
  [id] int PRIMARY KEY,
  [name] nvarchar(100),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [Cover] (
  [id] int PRIMARY KEY,
  [imgName] nvarchar(200),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [HistoryBookPrice] (
  [id] int PRIMARY KEY,
  [datePrice] date,
  [price] decimal(10,2),
  [bookId] int,
  [createdAt] datetime
)
GO

CREATE TABLE [ShoppingCart] (
  [userId] int,
  [bookId] int,
  [createdAt] datetime,
  PRIMARY key(userId , bookId)
)
GO


CREATE TABLE [BookShoppingCart] (
  [bookId] int,
  [bookShoppingCartId] int,
  Format NVARCHAR(10) CHECK (Format IN ('epub', 'f2b', 'mobi', 'PDF')),
  PRIMARY KEY (bookId, bookShoppingCartId)
)
GO

CREATE TABLE [Reciept] (
  [id] int PRIMARY KEY,
  [userId] int,
  [bookId] int,
  [total] decimal(10,2),
  [createdAt] datetime
)
GO

-- Add Foreign Key Constraints
ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [Book] ADD FOREIGN KEY ([publisherId]) REFERENCES [Publisher] ([id])
GO
ALTER TABLE [Book] ADD FOREIGN KEY ([genreId]) REFERENCES [Genre] ([id])
GO
ALTER TABLE [HistoryBookPrice] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [BorrowedBooks] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [BorrowedBooks] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [HistoryPurchases] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [HistoryPurchases] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [Rating] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [Rating] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [Feedback] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [Feedback] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [GeneralRating] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [GeneralFeedback] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [BookRentQueue] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [BookRentQueue] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [Auther] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [Cover] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [ShoppingCart] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([bookShoppingCartId], [bookId]) REFERENCES [ShoppingCart] ([userId], [bookId])
GO
ALTER TABLE [Reciept] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO
ALTER TABLE [Reciept] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO

-- Insert comprehensive dummy data
-- Users
INSERT INTO [User] ([id], [userName], [password], [firstName], [lastName], [email], [type], [phoneNumber], [createdAt])
VALUES 
(1, 'johndoe', 'hashed_password_1', 'John', 'Doe', 'john.doe@example.com', 'reader', '1234567890', GETDATE()),
(2, 'janesmith', 'hashed_password_2', 'Jane', 'Smith', 'jane.smith@example.com', 'premium', '9876543210', GETDATE()),
(3, 'mikebrown', 'hashed_password_3', 'Mike', 'Brown', 'mike.brown@example.com', 'admin', '5551234567', GETDATE()),
(4, 'sarahlee', 'hashed_password_4', 'Sarah', 'Lee', 'sarah.lee@example.com', 'reader', '7778889999', GETDATE()),
(5, 'davidwang', 'hashed_password_5', 'David', 'Wang', 'david.wang@example.com', 'premium', '3334445555', GETDATE());

-- Publishers
INSERT INTO [Publisher] ([id], [name], [createdAt])
VALUES 
(1, 'Penguin Random House', GETDATE()),
(2, 'HarperCollins', GETDATE()),
(3, 'Simon & Schuster', GETDATE()),
(4, 'Macmillan Publishers', GETDATE()),
(5, 'Hachette Book Group', GETDATE());

-- Genres
INSERT INTO [Genre] ([id], [name], [createdAt])
VALUES 
(1, 'Fiction', GETDATE()),
(2, 'Non-Fiction', GETDATE()),
(3, 'Science Fiction', GETDATE()),
(4, 'Mystery', GETDATE()),
(5, 'Romance', GETDATE());

-- Books
INSERT INTO [Book] ([id], [publisherId], [genreId], [amountOfCopies], [title], [borrowPrice], [buyingPrice], 
                    [pubDate], [ageLimit], [priceHistory], [onSale], [canBorrow], [starRate], [createdAt])
VALUES 
(1, 1, 1, 50, 'The Great Adventure', 2.99, 15.99, '2023-01-15', 12, 1, 1, 1, 4.5, GETDATE()),
(2, 2, 3, 30, 'Starship Chronicles', 3.50, 19.99, '2022-11-20', 16, 2, 0, 1, 4.7, GETDATE()),
(3, 3, 4, 25, 'Murder in the Library', 2.50, 14.50, '2023-05-10', 18, 3, 1, 1, 4.2, GETDATE()),
(4, 4, 5, 40, 'Love Across Continents', 2.75, 16.50, '2023-02-28', 16, 4, 1, 1, 4.6, GETDATE()),
(5, 5, 2, 35, 'The Rise of Technology', 3.25, 18.99, '2022-09-05', 14, 5, 0, 1, 4.4, GETDATE());

-- Authors
INSERT INTO [Auther] ([id], [name], [bookId], [createdAt])
VALUES 
(1, 'Emily Johnson', 1, GETDATE()),
(2, 'Michael Chang', 2, GETDATE()),
(3, 'Sarah Rodriguez', 3, GETDATE()),
(4, 'David Kim', 4, GETDATE()),
(5, 'Rachel Green', 5, GETDATE());

-- Covers
INSERT INTO [Cover] ([id], [imgName], [bookId], [createdAt])
VALUES 
(1, 'great_adventure_cover.jpg', 1, GETDATE()),
(2, 'starship_chronicles_cover.png', 2, GETDATE()),
(3, 'murder_library_cover.jpg', 3, GETDATE()),
(4, 'love_continents_cover.jpg', 4, GETDATE()),
(5, 'tech_rise_cover.jpg', 5, GETDATE());

-- Personal Library
INSERT INTO [PersonalLibrary] ([id], [userId], [bookId], [createdAt])
VALUES 
(1, 1, 1, GETDATE()),
(2, 2, 2, GETDATE()),
(3, 3, 3, GETDATE()),
(4, 4, 4, GETDATE()),
(5, 5, 5, GETDATE());

-- Borrowed Books
INSERT INTO [BorrowedBooks] ([id], [userId], [bookId])
VALUES 
(1, 1, 2),
(2, 2, 3),
(3, 3, 4),
(4, 4, 5),
(5, 5, 1);

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
INSERT INTO [BookRentQueue] ([id], [bookId], [userId], [createdAt])
VALUES 
(1, 1, 2, GETDATE()),
(2, 2, 3, GETDATE()),
(3, 3, 4, GETDATE()),
(5, 5, 1, GETDATE());

-- History Purchases
INSERT INTO [HistoryPurchases] ([id], [bookId], [bookPrice], [userId], [purchaseDate], [createdAt])
VALUES 
(1, 1, 15.99, 1, GETDATE(), GETDATE()),
(2, 2, 19.99, 2, GETDATE(), GETDATE()),
(3, 3, 14.50, 3, GETDATE(), GETDATE()),
(4, 4, 16.50, 4, GETDATE(), GETDATE()),
(5, 5, 18.99, 5, GETDATE(), GETDATE());

-- History Book Price
INSERT INTO [HistoryBookPrice] ([id], [datePrice], [price], [bookId], [createdAt])
VALUES 
(1, '2023-01-01', 14.99, 1, GETDATE()),
(2, '2022-11-01', 18.99, 2, GETDATE()),
(3, '2023-05-01', 13.50, 3, GETDATE()),
(4, '2023-02-15', 15.50, 4, GETDATE()),
(5, '2022-08-20', 17.99, 5, GETDATE());

-- Shopping Cart
INSERT INTO [ShoppingCart] ([userId], [bookId], [createdAt])
VALUES 
(1, 1, GETDATE()),
(2, 2, GETDATE()),
(3, 3, GETDATE()),
(4, 4, GETDATE()),
(5, 5, GETDATE());

-- Book Shopping Cart
INSERT INTO [BookShoppingCart] ([bookId], [bookShoppingCartId], [format])
VALUES 
(1, 1, 'epub'),
(2, 2, 'PDF'),
(3, 3, 'mobi'),
(4, 4, 'f2b'),
(5, 5, 'PDF');

-- Receipts
INSERT INTO [Reciept] ([id], [userId], [bookId], [total], [createdAt])
VALUES 
(1, 1, 2, 19.99, GETDATE()),
(2, 2, 3, 14.50, GETDATE()),
(3, 3, 4, 16.50, GETDATE()),
(4, 4, 5, 18.99, GETDATE()),
(5, 5, 1, 15.99, GETDATE());

-- General Rating
INSERT INTO [GeneralRating] ([id], [starRating], [userId], [createdAt])
VALUES 
(1, 4, 1, GETDATE()),
(2, 5, 2, GETDATE()),
(3, 4, 3, GETDATE()),
(4, 5, 4, GETDATE()),
(5, 4, 5, GETDATE());

-- General Feedback
INSERT INTO [GeneralFeedback] ([id], [userId], [comment], [createdAt])
VALUES 
(1, 1, 'Enjoying the platform so far!', GETDATE()),
(2, 2, 'Great selection of books', GETDATE()),
(3, 3, 'User interface is very intuitive', GETDATE()),
(4, 4, 'Love the recommendation system', GETDATE()),
(5, 5, 'Excellent customer support', GETDATE());

-- Print success message
PRINT 'eBook Database created and populated successfully!';
GO