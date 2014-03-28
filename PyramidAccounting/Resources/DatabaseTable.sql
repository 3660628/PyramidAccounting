CREATE TABLE T_BOOKS (									--���ױ�
    ID                TEXT PRIMARY KEY,					--����ID
    BOOK_NAME         TEXT,								--��������
	COMPANY_NAME	  TEXT,								--��λ����
	BOOK_TIME		  TEXT,								--���������ڼ�
    CREATE_DATE       DATE,								--��������
    ACCOUNTING_SYSTEM TEXT,								--����ƶ�
	PERIOD            INTEGER,							--��ǰ��
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