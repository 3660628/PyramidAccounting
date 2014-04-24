CREATE TABLE T_BOOKS (									--账套表
    ID                TEXT PRIMARY KEY,					--账套ID
    BOOK_NAME         TEXT,								--账套名称
	COMPANY_NAME	  TEXT,								--单位名称
	BOOK_TIME		  TEXT,								--账套启用期间
    CREATE_DATE       DATE,								--创建日期
    ACCOUNTING_SYSTEM TEXT,								--会计制度
	PERIOD            INTEGER,							--当前期
	BOOK_INDEX		  INTEGER,							--账套索引
	DELETE_MARK		  INTEGER DEFAULT ( 0 )				--删除标志    -1表示已删除
);
CREATE TABLE T_YEARFEE (								--科目年初金额设置表
	SUBJECT_ID   TEXT,									--科目编号
	FEE			 DECIMAL,								--年初金额
	PARENTID     TEXT,									--父节点ID
	BOOKID		 TEXT									--账套ID	
);
CREATE TABLE T_SUBJECTTYPE (							--科目类型维表
    TYPE_ID   INTEGER,									--科目类别
    TYPE_NAME TEXT										--类别名称
);
CREATE TABLE T_USER (									--用户表
	USERID INTEGER PRIMARY KEY,							--USERID
	USERNAME TEXT NOT NULL UNIQUE,						--用户名
	REALNAME TEXT,										--用户姓名
	PASSWORD TEXT DEFAULT (123456),						--密码
	PHONE_NO TEXT,										--手机号码
	AUTHORITY INTEGER DEFAULT (0),						--权限     0：表示记账  1：审核   2：会计主管 3：管理员 4：超级管理员
	CREATE_TIME DATETIME,								--创建时间
	COMMENTS TEXT,										--用户说明	
	DELETE_MARK INTEGER DEFAULT (0)						--停用标志  0：正是用 1：已停用	
);
CREATE TABLE T_FIXEDASSETS(								--固定资产表
	ID INTEGER PRIMARY KEY,								--编号
	NAME TEXT,											--名称及规格
	UNIT TEXT,											--单位
	AMOUNT	DECIMAL,									--数量
	PRICE   DEICMAL,									--原价或重置价格
	USED_YEAR	INTEGER,								--使用年限
	BUY_DATE	DATE,									--购置日期
	DEPARMENT	TEXT,									--使用部门
	CLEAR_DATE	DATE,									--清理日期
	VOUCHER_NO	TEXT,									--凭证编号
	COMMENTS    TEXT,									--备注
	DELETE_MARK INTEGER DEFAULT(0)						--删除标志 0  1表示已经删除
);
CREATE TABLE T_SYSTEMINFO(								--信息表
	ID INTEGER PRIMARY KEY,								--主键
	OP_TIME DATETIME,									--操作日期
	RKEY TEXT,											--参数代码
	VALUE TEXT,											--值
	COMMENTS TEXT										--备注
);
CREATE INDEX idx_T_YEARFEE ON T_YEARFEE ( 
    BOOKID 
);