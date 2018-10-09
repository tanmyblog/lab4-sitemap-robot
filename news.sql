create database news
go

use news
go

create table Category
(
	Id int not null primary key identity(1,1),
	Name nvarchar(255) not null,
	Description nvarchar(max),
	Alias varchar(255)
)

create table Article
(
	Id int not null primary key identity(1,1),
	Title nvarchar(255) not null,
	Alias varchar(255) not null,
	Author nvarchar(255),
	Description nvarchar(max),
	DatePublished date,
	CategoryId int
)

alter table Article
add constraint FK_Article_Category foreign key (CategoryId) references Category (Id)

set dateformat DMY

insert into Category values (N'Thời sự', N'Mô tả', 'thoi-su')
insert into Category values (N'Gia đình', N'Mô tả', 'gia-dinh')
insert into Category values (N'Sức khỏe', N'Mô tả', 'suc-khoe')

insert into Article values (N'Đón không khí lạnh yếu, miền bắc','don-khong-khi-lanh-yeu-mien-bac', N'VnExpress', N'Từ 24/5 balbla cái gì đó', '01/10/2018', 1)
insert into Article values (N'Phó thủ tướng chỉ đạo không cần','pho-thu-tuong-chi-dao-khong-can', N'VnExpress', N'Từ 24/5 balbla cái gì đó', '01/10/2018', 2)
insert into Article values (N'Phó thủ tướng Vũ Đức Đam','pho-thu-tuong-vu-duc-dam', N'VnExpress', N'Từ 24/5 balbla cái gì đó', '01/10/2018', 3)
