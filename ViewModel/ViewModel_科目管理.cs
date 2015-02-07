using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PA.Model.DataGrid;
using PA.Helper.DataBase;
using System.Data;
using PA.Model.Database;
using PA.Helper.DataDefind;

namespace PA.ViewModel
{
    class ViewModel_科目管理
    {
        DataBase db = new DataBase();
       
        public List<Model_科目管理> GetSujectData(int type)
        {
            string sql = "select a.fee,b.* from " + DBTablesName.T_SUBJECT 
                + " b left join t_yearfee a on a.bookid='" + CommonInfo.账薄号 
                + "' and a.subject_id=b.subject_id where  b.subject_type=" + type 
                + " order by b.id,b.used_mark";
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[1].ToString());
                m.序号 = "" + (i + 1);
                m.科目编号 = d[2].ToString();
                m.科目名称 = d[4].ToString();
                m.年初金额 = d[0].ToString();
                m.Used_mark = Convert.ToInt32(d[6].ToString());
                m.是否启用 = m.Used_mark == 0 ? true : false;
                int 借贷标记result = 1;
                int.TryParse(d[7].ToString(), out 借贷标记result);
                m.借贷标记 = (借贷标记result == 1) ? true : false;
                list.Add(m);
            }
            return list;
        }
        public List<Model_科目管理> GetChildSubjectData(string parent_id)
        {
            string sql = "select a.*,b.fee from " + DBTablesName.T_SUBJECT 
                + " a left join t_yearfee b on a.subject_id=b.subject_id  where b.bookid='"
                + CommonInfo.账薄号 + "' "
                +"and a.SUBJECT_TYPE in ('100','1000','10000') AND a.parent_id LIKE '" + parent_id + "%'"
                +" order by a.subject_id";
            DataTable dt = db.Query(sql).Tables[0];
            List<Model_科目管理> list = new List<Model_科目管理>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Model_科目管理 m = new Model_科目管理();
                DataRow d = dt.Rows[i];
                m.ID = Convert.ToInt32(d[0].ToString());
                m.科目编号 = d[1].ToString();
                m.类别 = d[2].ToString();
                m.科目名称 = d[3].ToString();
                m.父ID = d[4].ToString();
                m.借贷标记 = (d["Borrow_Mark"].ToString()=="1");
                m.年初金额 = d["FEE"].ToString();
                list.Add(m);
            }
            return list;
        }
       
        public bool UpdateUsedMark(Model_科目管理 m)
        {
            string sql = "update " + DBTablesName.T_SUBJECT + " set used_mark="
                + m.Used_mark + " where SUBJECT_ID=" + m.科目编号 + " or PARENT_ID=" + m.科目编号 + " OR PARENT_ID LIKE '" + m.科目编号 + "%'";
            return db.Excute(sql);
        }

        public bool UpdateBorrowMark(Model_科目管理 m)
        {
            string sql = "update " + DBTablesName.T_SUBJECT + " set Borrow_Mark="
                + ((m.借贷标记) ? 1 : -1) + " where SUBJECT_ID=" + m.科目编号 + " or PARENT_ID=" + m.科目编号 + " OR PARENT_ID LIKE '" + m.科目编号 + "%'";
            return db.Excute(sql);
        }

        //public void UpdateChildSubject(Model_科目管理 m)
        //{
        //    List<string> sqlList = new List<string>();
        //    string sql2 = "update " + DBTablesName.T_YEAR_FEE + " set fee='"
        //           + m.年初金额 + "',subject_id='" + m.科目编号 + "' where parentid='"
        //           + m.父ID + "' and subject_id='" + m.科目编号 + "' and bookid='" + CommonInfo.账薄号 + "'";
        //    sqlList.Add(sql2);
        //    //统计主科目的年初额
        //    string sql3 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid="
        //        + m.父ID + ") where subject_id=" + m.父ID;
        //    string sql1 = "update " + DBTablesName.T_SUBJECT + " set subject_id='" 
        //        + m.科目编号 + "',subject_name='" + m.科目名称 + "' where id=" + m.ID;
        //    sqlList.Add(sql1);
            
        //    sqlList.Add(sql3);
        //    db.BatchOperate(sqlList);
        //}
        /// <summary>
        /// 插入新二三级细目
        /// Lugia
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool Insert(List<Model_科目管理> list)
        {
            bool flag = false;
            string parentid = "";
            flag = db.InsertPackage(DBTablesName.T_SUBJECT, list.OfType<object>().ToList());
            if(!flag)
            {
                return false;
            }
            List<string> sqlList = new List<string>();
            foreach(Model_科目管理 m in list)
            {
                string sql = "insert into t_yearfee values ('" + m.科目编号 + "','" 
                    + m.年初金额 + "','" + m.父ID + "','" + CommonInfo.账薄号 + "')";
                sqlList.Add(sql);
                parentid = m.父ID;
            }
            //刷新上级科目金额
            string sql3 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid='" + parentid + "' and bookid='" + CommonInfo.账薄号 + "') "
                        + " where subject_id='" + parentid + "' and bookid='" + CommonInfo.账薄号 + "'";
            sqlList.Add(sql3);
            if (parentid.Length == 7)//四级科目
            {
                string parentid2 = parentid.Substring(0, 5);
                string sql4 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid='" + parentid2 + "' and bookid='" + CommonInfo.账薄号 + "') "
                            + " where subject_id='" + parentid2 + "' and bookid='" + CommonInfo.账薄号 + "'";
                sqlList.Add(sql4);
            }
            return db.BatchOperate(sqlList);
        }

        public bool Delete(List<Model_科目管理> list)
        {
            string parentid = "";
            List<string> sqlList = new List<string>();
            foreach (Model_科目管理 m in list)
            {
                string sql = "delete from " + DBTablesName.T_SUBJECT + " where id=" + m.ID + " OR parent_id LIKE '" + m.科目编号 + "%'";
                sqlList.Add(sql);
                string sql2 = "delete from " + DBTablesName.T_YEAR_FEE + " where "
                    + " (subject_id='" + m.科目编号 + "' and parentid='" + m.父ID + "') OR parentid LIKE '" + m.科目编号 + "%' and bookid='" + CommonInfo.账薄号 + "'";
                sqlList.Add(sql2);
                parentid = m.父ID;
            }
            string sql3 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid='" + parentid + "' and bookid='" + CommonInfo.账薄号 + "') "
                + " where subject_id='" + parentid + "' and bookid='" + CommonInfo.账薄号 + "'";
            sqlList.Add(sql3);
            return db.BatchOperate(sqlList);
        }

        public string GetSubjectID(string name)
        {
            string sql = "select subject_id from " + DBTablesName.T_SUBJECT + " where subject_name='" 
                + name + "' and parent_id = 0";
            return db.GetAllData(sql).Split('\t')[0].Split(',')[0];
        }

        /// <summary>
        /// 更新子细目内容，DataGridCell编辑后更新
        /// Lugia
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool UpdateChildSubject(string id, string key, string value)
        {
            List<string> sqlList = new List<string>();
            string sqlparentid = "(select parentid from " + DBTablesName.T_YEAR_FEE + " where subject_id='" + id + "')";
            string sql = "";
            if(key == "年初数")
            {
                sql = "update " + DBTablesName.T_YEAR_FEE + " set FEE=" + value + " where bookid='" + CommonInfo.账薄号 + "' AND subject_id='" + id + "'";
                sqlList.Add(sql);
                string sql3 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid=" + sqlparentid + " and bookid='" + CommonInfo.账薄号 + "') "
                + " where subject_id=" + sqlparentid + " and bookid='" + CommonInfo.账薄号 + "'";
                sqlList.Add(sql3);
                if (id.Length == 9)//四级科目
                {
                    sqlparentid = "(select parentid from " + DBTablesName.T_YEAR_FEE + " where subject_id='" + id.Substring(0,7) + "')";
                    string sql4 = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid=" + sqlparentid + " and bookid='" + CommonInfo.账薄号 + "') "
                                + " where subject_id=" + sqlparentid + " and bookid='" + CommonInfo.账薄号 + "'";
                    Console.WriteLine(sql4);
                    sqlList.Add(sql4);
                }
            }
            else if(key == "子细目名称")
            {
                sql = "update " + DBTablesName.T_SUBJECT + " set subject_name='" + value + "' where id=" + id;
                sqlList.Add(sql);
            }
            return db.BatchOperate(sqlList);
        }
        /// <summary>
        /// 关闭子科目管理时，刷新主科目年初额
        /// Lugia
        /// </summary>
        /// <param name="SubjectId"></param>
        /// <returns></returns>
        public bool UpdateMainSubjectsFee(string SubjectId)
        {
            List<string> sqlList = new List<string>();
            string sql = "update T_YEARFEE set fee = (select total(fee) from T_YEARFEE where parentid=" + SubjectId + " and bookid='" + CommonInfo.账薄号 + "') "
                + " where subject_id=" + SubjectId + " and bookid='" + CommonInfo.账薄号 + "'";
            sqlList.Add(sql);
            return db.BatchOperate(sqlList);
        }

        public bool UpdateMainSubjectsFeeManual(string SubjectId, string NewFee)
        {
            List<string> sqlList = new List<string>();
            string sql = "update " + DBTablesName.T_YEAR_FEE + " set FEE= " + NewFee + " where subject_id='" + SubjectId + "' AND bookid='" + CommonInfo.账薄号 + "'";
            sqlList.Add(sql);
            return db.BatchOperate(sqlList);
        }


        public List<string> GetIncomeAndOutSubjectList()
        {
            List<string> list = new List<string>();
            switch (CommonInfo.制度索引)
            {
                case "0":
                    list = new List<string>() { "40101", "40102", "40401", "40402" };
                    break;
                case "1":
                    list = new List<string>() { "40101", "40102", "40501", "30601", "30602" };
                    break;
            }
            return list;
        }

        public List<string> GetOneSubjectList()
        {
            List<string> list = new List<string>();
            switch (CommonInfo.制度索引)
            {
                case "0":
                    list = new List<string>() { "303","401", "403","404", "407", "501", "502", "505" };
                    break;
                case "1":
                    list = new List<string>() { "401", "403", "412", "405", "413", "404", "502", "516", "517", "504", "520", "512", "503", "306" };
                    break;
            }
            return list;
        }

        public string GetMaxSubjectID(string parentID)
        {
            string sql = "select max(subject_id) from " + DBTablesName.T_SUBJECT + " where subject_id like '" + parentID + "%'";
            string result = db.GetSelectValue(sql);
            if (result.Length == parentID.Length)
            {
                return parentID + "01";
            }
            int c = 0;
            int.TryParse(result.Substring(result.Length - 2, 2), out c);
            return result.Substring(0, result.Length - 2) + (100 + c + 1).ToString().Substring(1, 2);
        }
    }
}
