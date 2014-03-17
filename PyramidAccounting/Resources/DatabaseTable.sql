CREATE TABLE T_BOOKS (									--���ױ�
    ID                TEXT PRIMARY KEY,					--����ID
    BOOK_NAME         TEXT,								--��������
    CREATE_DATE       DATE,								--������������
    ACCOUNTING_SYSTEM TEXT,								--����ƶ�
	DELETE_MARK		  INTEGER DEFAULT ( 0 )				--ɾ����־    -1��ʾ��ɾ��
);
CREATE TABLE T_VOUCHER (								--ƾ֤��
    ID                INTEGER  PRIMARY KEY,             --ƾ֤ID
    VOUCHER_NO        TEXT,								--ƾ֤��
    OP_TIME           DATETIME,							--�Ʊ�ʱ��
    WORD              TEXT,								--��
    NUMBER            INTEGER,							--��
    SUBSIDIARY_COUNTS INTEGER,							--������֤��
    FEE_DEBIT         DECIMAL,							--�ϼƽ跽�ܶ�
    FEE_CREDIT        DECIMAL,							--�ϼƴ����ܶ�
    ACCOUNTANT        TEXT,								--�������
    BOOKEEPER         TEXT,								--����
    REVIEWER          TEXT,								--���
	REVIEW_MARK       INTEGER,							--���˱��  0��δ��ˣ�1�������
	DELETE_MARK		  INTEGER DEFAULT ( 0 ),            --ɾ����־   -1��ʾ��ɾ��
	BOOK_ID			  TEXT  							--����ID  DEFAULT
);
CREATE TABLE T_VOUCHER_DETAIL (							--ƾ֤��ϸ��
    ID            INTEGER PRIMARY KEY,					--ID
	VID			  INTEGER,								--��ţ���¼��ǰ�ڼ���
    PARENTID      TEXT,									--����ID����ƾ֤��VOUCHER_NO���
    ABSTRACT      TEXT,									--ժҪ
    SUBJECT_ID    TEXT,									--��Ŀ���
    DETAIL        TEXT,									--��ϸĿ
    BOOKKEEP_MARK INTEGER,								--����
    DEBIT         DECIMAL,								--�跽
    CREDIT        DECIMAL,    							--����
	BOOK_ID		  TEXT									--����ID  DEFAULT
);
CREATE TABLE T_SUBJECT (								--��Ŀ��
    ID           INTEGER PRIMARY KEY,					--ID
	SID		     TEXT,									--���
    SUBJECT_ID   TEXT,									--��Ŀ���
	SUBJECT_TYPE INTEGER DEFAULT ( 999 ),				--��Ŀ���   999��ʾδ֪
    SUBJECT_NAME TEXT,									--��Ŀ����
	FEE			 DECIMAL,								--�ڳ����
    PARENT_ID    INTEGER DEFAULT ( 0 ),					--���ڵ�ID���������ֿ�Ŀ����ϸĿ  0��ʾ��Ŀ  ��Ϊ0��Ϊ��Ŀ�ı��
	USED_MARK	 INTEGER DEFAULT ( 0 )					--ʹ�ñ�־��0��ʾ��ʹ�ã�1��ʾͣ��
);
CREATE TABLE T_SUBJECT_TYPE (							--��Ŀ����ά��
    TYPE_ID   INTEGER,									--��Ŀ���
    TYPE_NAME TEXT										--�������
);
CREATE TABLE T_USER (									--�û���
	USERID INTEGER PRIMARY KEY,							--USERID
	USERNAME TEXT NOT NULL UNIQUE,						--�û���
	REALNAME TEXT,										--�û�����
	PASSWORD TEXT DEFAULT (123456),						--����
	PHONE_NO TEXT,										--�ֻ�����
	AUTHORITY INTEGER DEFAULT (0),						--Ȩ��     0����ʾ����  1�����   2��������� 3������Ա 4����������Ա
	CREATE_TIME DATETIME,								--����ʱ��
	COMMENTS TEXT										--�û�˵��			
);
CREATE TABLE T_RECORD (									--������־��
    ID          INTEGER  PRIMARY KEY,					--ID
    OP_TIME     DATETIME,								--����
    USERNAME    TEXT,									--�û���
    REALNAME    TEXT,									--����
    OP_TYPE     TEXT,									--��������
    LOG         TEXT									--��־
);