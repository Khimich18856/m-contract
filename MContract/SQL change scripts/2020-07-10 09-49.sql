alter table Ads add IsSendedExpiredMessage bit not null default (0)

GO

alter table Offers add IsSendedExpiredMessage bit not null default (0)