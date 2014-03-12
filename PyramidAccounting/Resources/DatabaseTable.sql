CREATE TABLE T_BOOKS (									--���ױ�
    ID                TEXT PRIMARY KEY,					--����ID
    BOOK_NAME         TEXT,								--��������
    CREATE_DATE       DATE,								--������������
    ACCOUNTING_SYSTEM TEXT								--����ƶ�
);
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
    REVIEWER          TEXT,								--���
	REVIEWER          INTEGER,							--���˱��  0��δ��ˣ�1�������
	BOOK_ID			  TEXT								--����ID  DEFAULT
);
CREATE TABLE T_VOUCHER_DETAIL (							--ƾ֤��ϸ��
    ID            INTEGER PRIMARY KEY,					--ID
    PARENTID      TEXT,									--����ID����ƾ֤��VOUCHER_NO���
    ABSTRACT      TEXT,									--ժҪ
    SUBJECT_ID    TEXT,									--��Ŀ���
    DETAIL        TEXT,									--��ϸĿ
    BOOKKEEP_MARK INTEGER,								--����
    DEBIT         DECIMAL,								--�跽
    CREDIT        DECIMAL,    							--����
	BOOK_ID			  TEXT								--����ID  DEFAULT
);
CREATE TABLE T_SUBJECT (								--��Ŀ��
    ID           INTEGER PRIMARY KEY,					--ID
    SUBJECT_ID   TEXT,									--��Ŀ���
    SUBJECT_NAME TEXT,									--��Ŀ����
    PARENT_ID    INTEGER DEFAULT ( 0 )					--���ڵ�ID���������ֿ�Ŀ����ϸĿ  0��ʾ��Ŀ  ��Ϊ0��Ϊ��Ŀ�ı��
);
CREATE TABLE T_USER (									--�û���
	USERID INTEGER PRIMARY KEY,							--USERID
	USER_NAME TEXT NOT NULL UNIQUE,						--�û���
	PASSWORD TEXT DEFAULT (123456),						--����
	PHONE_NO TEXT,										--�ֻ�����
	AUTHORITY INTEGER DEFAULT (0)						--Ȩ��     0����ʾ����  1�����   2���������
);