set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


create TRIGGER [tr_SRV_UPDATE] --触发器名称
ON [dbo].[Srv]  --表名
after UPDATE  --触发时间点
AS
IF UPDATE (SRV_RMT_STATUS)  --当前字段被修改时
BEGIN
DECLARE @SRV_IDtmp VARCHAR (19)  --定义变量

DECLARE @SRV_RMT_STATUS1 VARCHAR (19)  --定义变量
DECLARE @SRV_RMT_STATUS2 VARCHAR (19)  --定义变量

SET @SRV_IDtmp = (
	SELECT
		d.SRV_ID
	FROM
		deleted d,
		inserted i
	WHERE
		d.SRV_ID = i.SRV_ID
) --变量赋值


SET @SRV_RMT_STATUS1 = (
	SELECT
		d.SRV_RMT_STATUS
	FROM
		deleted d
	WHERE
		d.SRV_ID = @SRV_IDtmp
) --变量赋值


SET @SRV_RMT_STATUS2 = (
	SELECT
		i.SRV_RMT_STATUS
	FROM
		inserted i
	WHERE
		i.SRV_ID = @SRV_IDtmp
) --变量赋值

IF (@SRV_RMT_STATUS1<>@SRV_RMT_STATUS2)
begin
    --数据修改
UPDATE SRV
SET SRV_FLAG2 = '1'
WHERE
	SRV_ID = @SRV_IDtmp
  end


END




