CREATE TABLE T_VOUCHER (								--凭证表
    ID                INTEGER  PRIMARY KEY,             --凭证ID
    VOUCHER_NO        TEXT,								--凭证号
    OP_TIME           DATETIME,							--制表时间
    WORD              TEXT,								--字
    NUMBER            TEXT,								--号
    SUBSIDIARY_COUNTS INTEGER,							--附属单证数
    FEE_DEBIT         DECIMAL,							--合计借方总额
    FEE_CREDIT        DECIMAL,							--合计贷方总额
    ACCOUNTANT        TEXT,								--会计主管
    BOOKEEPER         TEXT,								--记账
    REVIEWER          TEXT,								--复核
    DOCUMENTMARKER    TEXT, 							--制单
	REVIEWER          INTEGER,							--复核标记  0：未审核，1：已审核
);
CREATE TABLE T_VOUCHER_DETAIL (							--凭证明细表
    ID            INTEGER PRIMARY KEY,					--ID
    PARENTID      TEXT,									--父节ID，与凭证表VOUCHER_NO相等
    ABSTRACT      TEXT,									--摘要
    SUBJECT_ID    TEXT,									--科目编号
    DETAIL        TEXT,									--子细目
    BOOKKEEP_MARK INTEGER,								--记账
    DEBIT         DECIMAL,								--借方
    CREDIT        DECIMAL    							--贷方
);