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
/// DBCon의 요약 설명입니다.
/// </summary>
public class DBCon
{
    public SqlConnection SqlCon = null;
    
    public DBCon()
    {
        try
        {
            //SqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["MODEWARE3"].ConnectionString);
            SqlCon = new SqlConnection("");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DBCon(string v연결값)
    {
        try
        {
            SqlCon = new SqlConnection(v연결값);



        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataSet FillDataSet(string 쿼리, SqlParameter[] 파라미터, CommandType 타입)
    {
        DataSet ds = new DataSet();

        try
        {
            SqlDataAdapter dsAdapter = new SqlDataAdapter(쿼리, SqlCon);
            dsAdapter.SelectCommand.CommandTimeout = 300; // 300초
            dsAdapter.SelectCommand.CommandType = 타입;

            ds = new DataSet();

            if (파라미터 != null)
            {
                foreach (SqlParameter param in 파라미터)
                {
                    dsAdapter.SelectCommand.Parameters.Add(param);
                }
            }

            SqlCon.Open();

            dsAdapter.Fill(ds);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
        }

        return ds;

    }

    public void ExecuteNonQuery(string 쿼리, SqlParameter[] 파라미터, CommandType 타입)
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

            SqlCon.Open();

            nRnt = sqlcmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
        }

    }

    public Hashtable ExecuteNonQuery(string 쿼리, SqlParameter[] 파라미터, CommandType 타입, params string[] 아웃파라미터)
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

            SqlCon.Open();

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
        finally
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
        }

    }

    public object ExecuteScalar(string 쿼리, SqlParameter[] 파라미터, CommandType 타입)
    {
        object QueryResultData = null;

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

            SqlCon.Open();

            QueryResultData = sqlcmd.ExecuteScalar();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (SqlCon.State == ConnectionState.Open)
            {
                SqlCon.Close();
            }

            SqlCon.Dispose();
        }

        return QueryResultData;

    }
}