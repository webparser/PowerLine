create table Customers
(
	Id int not null,
	Name varchar(255) not null,
    primary key (`Id`)
);

insert into Customers(Id, Name)
values
(1, 'Max'),
(2, 'Pavel'),
(3, 'Ivan'),
(4, 'Leonid');

create table `orders` (
	`Id` int not null,
	`CustomerId` int not null,
	primary key (`Id`),
	key `ix_Orders_Customer` (`CustomerId`),
	constraint `fk_Orders_Customer`
		foreign key (`CustomerId`)
		references `Customers` (`Id`)
		on delete cascade on update cascade
);

insert into Orders(Id, CustomerId)
values
(1, 2),
(2, 4);

-- Case 1 ----------------------------------------

select Name 
from Customers c
left join Orders o on o.CustomerId = c.Id
where o.Id is null
order by Name;

-- Case 2 ----------------------------------------

select Name
from Customers c
where Id not in 
(
	select CustomerId
    from Orders
)
order by Name;

-- Case 3 ----------------------------------------

select Name
from Customers
where not exists 
(
	select Id
	from Orders
	where Customers.Id = Orders.CustomerId
);
