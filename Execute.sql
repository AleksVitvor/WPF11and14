create database TODO
create table Users
(
	id int primary key identity(0,1),
	Login nvarchar(50),
	Password nvarchar(100),
	Surname nvarchar(50),
	Name nvarchar(50)
)
create table TODOList
(
	id int primary key identity(0,1),
	Userid int foreign key references Users(id),
	ShortDescription nvarchar(50),
	FullDescription nvarchar(500),
	Start datetime,
	Finish datetime,
	Priority nvarchar(50),
	Category nvarchar(50),
	State nvarchar(50),
	Image varchar(MAX)
)