CREATE TABLE T_VOUCHER (								--ƾ֤��
    ID                INTEGER  PRIMARY KEY,             --ƾ֤ID
    VOUCHER_NO        TEXT,								--ƾ֤��
    OP_TIME           DATETIME,							--�Ʊ�ʱ��
    WORD              TEXT,								--��
    NUMBER            TEXT,								--��
    SUBSIDIARY_COUNTS INTEGER,							--������֤��
    FEE_DEBIT         DECIMAL,							--�ϼƽ跽�ܶ�
    FEE_CREDIT        DECIMAL,							--�ϼƴ����ܶ�
    ACCOUNTANT        TEXT,								--�������
    BOOKEEPER         TEXT,								--����
    REVIEWER          TEXT,								--����
    DOCUMENTMARKER    TEXT, 							--�Ƶ�
	REVIEWER          INTEGER,							--���˱��  0��δ��ˣ�1�������
);
CREATE TABLE T_VOUCHER_DETAIL (							--ƾ֤��ϸ��
    ID            INTEGER PRIMARY KEY,					--ID
    PARENTID      TEXT,									--����ID����ƾ֤��VOUCHER_NO���
    ABSTRACT      TEXT,									--ժҪ
    SUBJECT_ID    TEXT,									--��Ŀ���
    DETAIL        TEXT,									--��ϸĿ
    BOOKKEEP_MARK INTEGER,								--����
    DEBIT         DECIMAL,								--�跽
    CREDIT        DECIMAL    							--����
);