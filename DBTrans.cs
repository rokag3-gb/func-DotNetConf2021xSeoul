using System;
using System.Data;
using System.Configuration;
using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections;

/// <summary>
/// DBTrans의 요약 설명입니다.
/// </summary>
public class DBTrans
{
    private SqlConnection SqlCon = null;
    private SqlTransaction SqlTrans = null;

    public DBTrans()
    {
        try
        {
            //SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MODEWARE3"].ConnectionString);
            SqlCon = new SqlConnection("");
            SqlCon.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DBTrans(string v연결값)
    {
        try
        {
            SqlCon = new SqlConnection(v연결값);

            SqlCon.Open();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void 트랜잭션시작()
    {
        try
        {
            if (null != SqlCon)
                SqlTrans = SqlCon.BeginTransaction();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void 커밋()
    {
        try
        {
            if (null != SqlTrans)
                SqlTrans.Commit();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void 롤백()
    {
        try
        {
            if (null != SqlTrans)
                SqlTrans.Rollback();
        }
        catch (Exception ex)
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
            throw ex;
        }
    }

    public void 연결끊기()
    {
        try
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void 쿼리실행(string 쿼리, SqlParameter[] 파라미터, CommandType 타입)
    {
        int nRnt = -1;

        try
        {
            SqlCommand sqlcmd = new SqlCommand(쿼리, SqlCon);
            sqlcmd.CommandTimeout = 300;
            sqlcmd.CommandType = 타입;

            if (파라미터 != null)
            {
                foreach (SqlParameter param in 파라미터)
                {
                    sqlcmd.Parameters.Add(param);
                }
            }

            sqlcmd.Transaction = SqlTrans;

            nRnt = sqlcmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public Hashtable 쿼리실행(string 쿼리, SqlParameter[] 파라미터, CommandType 타입, params string[] 아웃파라미터)
    {
        int nRnt = -1;

        Hashtable ht = new Hashtable();

        try
        {
            SqlCommand sqlcmd = new SqlCommand(쿼리, SqlCon);
            sqlcmd.CommandTimeout = 300;
            sqlcmd.CommandType = 타입;

            if (파라미터 != null)
            {
                foreach (SqlParameter param in 파라미터)
                {
                    sqlcmd.Parameters.Add(param);
                }
            }

            sqlcmd.Transaction = SqlTrans;

            nRnt = sqlcmd.ExecuteNonQuery();


            if (아웃파라미터.Length > 0)
            {
                for (int lCnt = 0; lCnt < 아웃파라미터.Length; lCnt++)
                {
                    ht.Add(아웃파라미터[lCnt].ToString(), sqlcmd.Parameters[아웃파라미터[lCnt].ToString()].Value.ToString());
                }
            }

            return ht;

        }
        catch (Exception ex)
        {
            throw ex;
        }


    }
}