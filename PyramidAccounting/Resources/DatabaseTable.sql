CREATE TABLE T_BOOKS (									--���ױ�
    ID                TEXT PRIMARY KEY,					--����ID
    BOOK_NAME         TEXT,								--��������
	COMPANY_NAME	  TEXT,								--��λ����
	BOOK_TIME		  TEXT,								--���������ڼ�
    CREATE_DATE       DATE,								--��������
    ACCOUNTING_SYSTEM TEXT,								--����ƶ�
	PERIOD            INTEGER,							--��ǰ��
	BOOK_INDEX		  INTEGER,							--��������
	DELETE_MARK		  INTEGER DEFAULT ( 0 )				--ɾ����־    -1��ʾ��ɾ��
);
CREATE TABLE T_YEARFEE (								--��Ŀ���������ñ�
	SUBJECT_ID   TEXT,									--��Ŀ���
	FEE			 DECIMAL,								--������
	PARENTID     TEXT,									--���ڵ�ID
	BOOKID		 TEXT									--����ID	
);
CREATE TABLE T_SUBJECTTYPE (							--��Ŀ����ά��
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
	COMMENTS TEXT,										--�û�˵��	
	DELETE_MARK INTEGER DEFAULT (0)						--ͣ�ñ�־  0�������� 1����ͣ��	
);
CREATE TABLE T_FIXEDASSETS(								--�̶��ʲ���
	ID INTEGER PRIMARY KEY,								--���
	NAME TEXT,											--���Ƽ����
	UNIT TEXT,											--��λ
	AMOUNT	DECIMAL,									--����
	PRICE   DEICMAL,									--ԭ�ۻ����ü۸�
	USED_YEAR	INTEGER,								--ʹ������
	BUY_DATE	DATE,									--��������
	DEPARMENT	TEXT,									--ʹ�ò���
	CLEAR_DATE	DATE,									--��������
	VOUCHER_NO	TEXT,									--ƾ֤���
	COMMENTS    TEXT,									--��ע
	DELETE_MARK INTEGER DEFAULT(0)						--ɾ����־ 0  1��ʾ�Ѿ�ɾ��
);
CREATE TABLE T_SYSTEMINFO(								--��Ϣ��
	ID INTEGER PRIMARY KEY,								--����
	OP_TIME DATETIME,									--��������
	RKEY TEXT,											--��������
	VALUE TEXT,											--ֵ
	COMMENTS TEXT										--��ע
);
CREATE INDEX idx_T_YEARFEE ON T_YEARFEE ( 
    BOOKID 
);