CREATE TABLE T_BOOKS (									--账套表
    ID                TEXT PRIMARY KEY,					--账套ID
    BOOK_NAME         TEXT,								--账套名称
	COMPANY_NAME	  TEXT,								--单位名称
	MONEY_TYPE		  TEXT,								--本位币
    CREATE_DATE       DATE,								--账套启用日期
    ACCOUNTING_SYSTEM TEXT,								--会计制度
	DELETE_MARK		  INTEGER DEFAULT ( 0 )				--删除标志    -1表示已删除
);
CREATE TABLE T_VOUCHER (								--凭证表
    ID                INTEGER  PRIMARY KEY,             --凭证ID
    VOUCHER_NO        TEXT,								--凭证号
    OP_TIME           DATETIME,							--制表时间
    WORD              TEXT,								--字
    NUMBER            INTEGER,							--号
    SUBSIDIARY_COUNTS INTEGER,							--附属单证数
    FEE_DEBIT         DECIMAL,							--合计借方总额
    FEE_CREDIT        DECIMAL,							--合计贷方总额
    ACCOUNTANT        TEXT,								--会计主管
    BOOKEEPER         TEXT,								--记账
    REVIEWER          TEXT,								--审核
	REVIEW_MARK       INTEGER,							--复核标记  0：未审核，1：已审核
	DELETE_MARK		  INTEGER DEFAULT ( 0 )             --删除标志   -1表示已删除
);
CREATE TABLE T_VOUCHER_DETAIL (							--凭证明细表
    ID            INTEGER PRIMARY KEY,					--ID
	VID			  INTEGER,								--序号，记录当前第几条
    PARENTID      TEXT,									--父节ID，与凭证表VOUCHER_NO相等
    ABSTRACT      TEXT,									--摘要
    SUBJECT_ID    TEXT,									--科目编号
    DETAIL        TEXT,									--子细目
    BOOKKEEP_MARK INTEGER,								--记账
    DEBIT         DECIMAL,								--借方
    CREDIT        DECIMAL    							--贷方
);
CREATE TABLE T_SUBJECT (								--科目表
    ID           INTEGER PRIMARY KEY,					--ID
	SID		     TEXT,									--序号
    SUBJECT_ID   TEXT,									--科目编号
	SUBJECT_TYPE INTEGER DEFAULT ( 999 ),				--科目类别   999表示未知
    SUBJECT_NAME TEXT,									--科目名称
    PARENT_ID    INTEGER DEFAULT ( 0 ),					--父节点ID，用于区分科目和子细目  0表示科目  不为0是为科目的编号
	USED_MARK	 INTEGER DEFAULT ( 0 )					--使用标志，0表示正使用，1表示停用
);
CREATE TABLE T_YEAR_FEE (								--科目年初金额设置表
	SUBJECT_ID   TEXT PRIMARY KEY,						--科目编号
	FEE			 DECIMAL								--年初金额
);
CREATE TABLE T_SUBJECT_TYPE (							--科目类型维表
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
CREATE TABLE T_RECORD (									--操作日志表
    ID          INTEGER  PRIMARY KEY,					--ID
    OP_TIME     DATETIME,								--日期
    USERNAME    TEXT,									--用户名
    REALNAME    TEXT,									--姓名
    OP_TYPE     TEXT,									--操作类型
    LOG         TEXT									--日志
);
CREATE INDEX idx_T_SUBJECT ON T_SUBJECT (SUBJECT_ID, PARENT_ID,USED_MARK);
CREATE INDEX idx_T_VOUCHER_DETAIL ON T_VOUCHER_DETAIL (PARENTID);