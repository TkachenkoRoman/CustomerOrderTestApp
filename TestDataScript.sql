USE [CustomersOrdersDB]
DELETE FROM Customer;
INSERT INTO Customer (CustomerId, Name, Address, PhoneNum) VALUES (1,'John','W. Augusta Blvd.Chicago, IL 60622','773-972-6745');
INSERT INTO Customer (CustomerId, Name, Address, PhoneNum) VALUES (2,'Jane','Vine Street Philadelphia, PA 19107','(215) 557-0455');
INSERT INTO Customer (CustomerId, Name, Address, PhoneNum) VALUES (3,'Chandler','Weaver Street, San Diego CA 92114 ','(619) 527-2816');

DELETE FROM [dbo].[Order];
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (1,'0001', 10);
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (1,'0002', 2);
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (2,'0003', 15);
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (2,'0004', 53);
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (3,'0005', 32);
INSERT INTO [dbo].[Order] (CustomerId, Number, Amount) VALUES (3,'0006', 19);