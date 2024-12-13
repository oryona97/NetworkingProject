CREATE TABLE [User] (
  [id] int PRIMARY KEY,
  [userName] string,
  [password] string,
  [firstName] string,
  [lastName] string,
  [email] string,
  [type] string,
  [phoneNumber] string,
  [createdAt] date
)
GO

CREATE TABLE [PersonalLibrary] (
  [id] int,
  [userId] int,
  [bookId] int,
  [primary] key(id,userId,bookId),
  [createdAt] date
)
GO

CREATE TABLE [Book] (
  [id] int PRIMARY KEY,
  [publisherId] int,
  [genreId] int,
  [amountOfCopies] int,
  [title] string,
  [borrowPrice] real,
  [buyingPrice] real,
  [pubDate] date,
  [ageLimit] int,
  [priceHistory] int,
  [onSale] bool,
  [canBorrow] bool,
  [starRate] real,
  [createdAt] date
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
  [name] string,
  [createdAt] date
)
GO

CREATE TABLE [Publisher] (
  [id] int,
  [name] string,
  [primary] key(id,name),
  [createdAt] date
)
GO

CREATE TABLE [HistoryPurchases] (
  [id] int PRIMARY KEY,
  [bookId] int,
  [bookPrice] real,
  [userId] int,
  [purchaseDate] date,
  [createdAt] date
)
GO

CREATE TABLE [Rating] (
  [starRating] int,
  [userId] int,
  [bookId] int,
  [primary] key(userId,bookId),
  [createdAt] date
)
GO

CREATE TABLE [Feedback] (
  [userId] int,
  [bookId] int,
  [primary] key(userId,bookId),
  [comment] string,
  [createdAt] date
)
GO

CREATE TABLE [GeneralRating] (
  [id] int,
  [starRating] int,
  [userId] int,
  [primary] key(userId,id),
  [createdAt] date
)
GO

CREATE TABLE [GeneralFeedback] (
  [id] int PRIMARY KEY,
  [userId] int,
  [comment] string,
  [createdAt] date
)
GO

CREATE TABLE [BookRentQueue] (
  [id] int,
  [bookId] int,
  [userId] int,
  [createdAt] date,
  [primary] key(bookId,userId)
)
GO

CREATE TABLE [Auther] (
  [id] int PRIMARY KEY,
  [name] string,
  [bookId] int,
  [createdAt] date
)
GO

CREATE TABLE [Cover] (
  [id] int PRIMARY KEY,
  [imgName] string,
  [bookId] int,
  [createdAt] date
)
GO

CREATE TABLE [HistoryBookPrice] (
  [id] int PRIMARY KEY,
  [datePrice] date,
  [price] real,
  [bookId] int,
  [createdAt] date
)
GO

CREATE TABLE [ShoppingCart] (
  [userId] int PRIMARY KEY,
  [bookId] int,
  [createdAt] date
)
GO

CREATE TABLE [BookShoppingCart] (
  [bookId] int,
  [bookShoppingCartId] int,
  [primary] key(bookId,bookShoppingCartId),
  [format] enum(epub,f2b,mobi,PDF)
)
GO

CREATE TABLE [Reciept] (
  [id] int PRIMARY KEY,
  [userId] int,
  [bookId] int,
  [total] real,
  [createdAt] date
)
GO

ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO

ALTER TABLE [PersonalLibrary] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO

ALTER TABLE [Book] ADD FOREIGN KEY ([publisherId]) REFERENCES [Publisher] ([id])
GO

ALTER TABLE [Genre] ADD FOREIGN KEY ([id]) REFERENCES [Book] ([genreId])
GO

ALTER TABLE [HistoryBookPrice] ADD FOREIGN KEY ([id]) REFERENCES [Book] ([priceHistory])
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

ALTER TABLE [HistoryBookPrice] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO

ALTER TABLE [ShoppingCart] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO

CREATE TABLE [Book_ShoppingCart] (
  [Book_id] int,
  [ShoppingCart_bookId] int,
  PRIMARY KEY ([Book_id], [ShoppingCart_bookId])
);
GO

ALTER TABLE [Book_ShoppingCart] ADD FOREIGN KEY ([Book_id]) REFERENCES [Book] ([id]);
GO

ALTER TABLE [Book_ShoppingCart] ADD FOREIGN KEY ([ShoppingCart_bookId]) REFERENCES [ShoppingCart] ([bookId]);
GO


ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO

ALTER TABLE [BookShoppingCart] ADD FOREIGN KEY ([bookShoppingCartId]) REFERENCES [ShoppingCart] ([bookId])
GO

ALTER TABLE [Reciept] ADD FOREIGN KEY ([userId]) REFERENCES [User] ([id])
GO

ALTER TABLE [Reciept] ADD FOREIGN KEY ([bookId]) REFERENCES [Book] ([id])
GO
