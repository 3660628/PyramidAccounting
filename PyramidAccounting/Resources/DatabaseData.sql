INSERT INTO t_subjecttype
VALUES
	(999, '未知'),
	(1, '一、资产类'),
	(2, '二、负债类'),
	(
		3,
		'三、净资产类'
	),
	(4, '四、收入类'),
	(5, '五、支出类');
INSERT INTO t_user (
	username,
	realname,
	password,
	authority
)
VALUES
	('admin','石蚁科技', '123', 3);