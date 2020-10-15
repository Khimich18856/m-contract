alter table dbo.Users add Created datetime not null default GetDate()

go

alter table dbo.Users add EmailConfirmed bit not null default 0