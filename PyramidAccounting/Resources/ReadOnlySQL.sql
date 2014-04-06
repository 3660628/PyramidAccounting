CREATE TABLE T_VOUCHER (								--凭证表
    ID                TEXT  PRIMARY KEY,				--凭证ID
	PERIOD            INTEGER,							--当前期
    OP_TIME           DATETIME,							--制表时间
    SUBSIDIARY_COUNTS INTEGER,							--附属单证数
    FEE_DEBIT         DECIMAL,							--合计借方总额
    FEE_CREDIT        DECIMAL,							--合计贷方总额
    ACCOUNTANT        TEXT,								--会计主管
    BOOKEEPER         TEXT,								--记账
    REVIEWER          TEXT,								--审核
	REVIEW_MARK       INTEGER,							--复核标记  0：未审核，1：已审核
	DELETE_MARK		  INTEGER DEFAULT ( 0 )             --删除标志   -1表示已删除  -2表示修改删除
);
CREATE TABLE T_VOUCHERDETAIL (							--凭证明细表
    ID            INTEGER PRIMARY KEY,					--ID
	VID			  INTEGER,								--序号，记录当前第几条
    PARENTID      TEXT,									--父节ID，与凭证表ID相等
	WORD		  TEXT,									--凭证字
	VOUCHER_NO    TEXT,									--凭证号
    ABSTRACT      TEXT,									--摘要
    SUBJECT_ID    TEXT,									--科目编号
    DETAIL        TEXT,									--子细目
    BOOKKEEP_MARK INTEGER,								--记账
    DEBIT         DECIMAL,								--借方
    CREDIT        DECIMAL    							--贷方
);
CREATE TABLE T_SUBJECT (								--科目表
    ID           INTEGER PRIMARY KEY NOT NULL,			--ID
	SID		     TEXT,									--序号
    SUBJECT_ID   TEXT UNIQUE,							--科目编号
	SUBJECT_TYPE INTEGER DEFAULT ( 999 ),				--科目类别   999表示未知 100二级 1000三级子细目
    SUBJECT_NAME TEXT,									--科目名称
    PARENT_ID    INTEGER DEFAULT ( 0 ),					--父节点ID，用于区分科目和子细目  0表示科目  不为0是为科目的编号
	USED_MARK	 INTEGER DEFAULT ( 0 ),					--使用标志，0表示正使用，1表示停用
	Borrow_Mark	 INTEGER DEFAULT ( 1 )					--借贷标记 1借 -1贷
);
CREATE TABLE T_RECORD (									--操作日志表
    ID          INTEGER  PRIMARY KEY,					--ID
    OP_TIME     DATETIME,								--日期
    USERNAME    TEXT,									--用户名
    REALNAME    TEXT,									--姓名
    LOG         TEXT									--日志
);
CREATE TABLE T_FEE(										--期末数表
    ID			INTEGER PRIMARY KEY NOT NULL,			--ID
	OP_TIME     DATETIME,								--操作时间
	PERIOD		INTEGER,								--当前期
    SUBJECT_ID  TEXT,									--科目编号
	VOUCHER_NUMS TEXT,									--凭单编号
	COMMENTS    TEXT,									--摘要
	DEBIT       DECIMAL DEFAULT(0),						--借方金额
	CREDIT      DECIMAL DEFAULT(0),						--贷方金额
	FEE         DECIMAL DEFAULT(0),						--期末数
	MARK		INTEGER DEFAULT(1),						--借或贷
	DELETE_MARK INTEGER DEFAULT(0)						--删除标志
);
