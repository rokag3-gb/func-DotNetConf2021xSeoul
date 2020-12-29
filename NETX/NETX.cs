using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetConf2021xSeoul
{
    public class NETX
    {
        private SqlDbType DB타입(string 타입값)
        {
            SqlDbType 타입;

            if (타입값 == "SqlDbType.BigInt")
            {
                타입 = SqlDbType.BigInt;
            }
            else if (타입값 == "SqlDbType.Binary")
            {
                타입 = SqlDbType.Binary;
            }
            else if (타입값 == "SqlDbType.Bit")
            {
                타입 = SqlDbType.Bit;
            }
            else if (타입값 == "SqlDbType.Char")
            {
                타입 = SqlDbType.Char;
            }
            else if (타입값 == "SqlDbType.Date")
            {
                타입 = SqlDbType.Date;
            }
            else if (타입값 == "SqlDbType.DateTime")
            {
                타입 = SqlDbType.DateTime;
            }
            else if (타입값 == "SqlDbType.DateTime2")
            {
                타입 = SqlDbType.DateTime2;
            }
            else if (타입값 == "SqlDbType.Decimal")
            {
                타입 = SqlDbType.Decimal;
            }
            else if (타입값 == "SqlDbType.Float")
            {
                타입 = SqlDbType.Float;
            }
            else if (타입값 == "SqlDbType.Image")
            {
                타입 = SqlDbType.Image;
            }
            else if (타입값 == "SqlDbType.Int")
            {
                타입 = SqlDbType.Int;
            }
            else if (타입값 == "SqlDbType.Money")
            {
                타입 = SqlDbType.Money;
            }
            else if (타입값 == "SqlD bType.NChar")
            {
                타입 = SqlDbType.NChar;
            }
            else if (타입값 == "SqlDbType.NText")
            {
                타입 = SqlDbType.NText;
            }
            else if (타입값 == "SqlDbType.NVarChar")
            {
                타입 = SqlDbType.NVarChar;
            }
            else if (타입값 == "SqlDbType.Real")
            {
                타입 = SqlDbType.Real;
            }
            else if (타입값 == "SqlDbType.VarChar")
            {
                타입 = SqlDbType.VarChar;
            }
            else
            {
                타입 = SqlDbType.Int;
            }

            return 타입;
        }

        public void ExecuteNonQuery(string 프로시저명, DataSet 파라미터)
        {
            int i = 0;
            SqlParameter[] sqlParam = null;

            DBCon dbCon = null;

            try
            {
                dbCon = new DBCon(DB연결문자());

                sqlParam = new SqlParameter[파라미터.Tables[0].Rows.Count];
                //sqlParam = new SqlParameter[파라미터.Tables[0].Rows.Count - 2];
                //sqlParam = new SqlParameter[파라미터.Tables[0].Rows.Count - 1];
                    
                foreach (DataRow dr in 파라미터.Tables[0].Rows)
                {
                    if (dr["컬럼명"].ToString() != "@IP추적")
                    {
                        if (dr["컬럼명"].ToString() != "@사용자추적")
                        {

                            sqlParam[i] = new SqlParameter();
                            sqlParam[i].ParameterName = dr["컬럼명"].ToString();
                            sqlParam[i].SqlDbType = DB타입(dr["타입"].ToString());
                            sqlParam[i].Size = Convert.ToInt32(dr["사이즈"].ToString());
                            if (dr["값"].ToString().ToUpper() == "NULL")
                            {
                                sqlParam[i].Value = DBNull.Value;
                            }
                            else
                            {
                                sqlParam[i].Value = dr["값"].ToString();
                            }
                            i++;
                        }
                    }
                }

                dbCon.ExecuteNonQuery(프로시저명, sqlParam, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet FillDataSet(string 프로시저명, DataSet 파라미터)
        {
            int i = 0;
            DBCon dbCon = null;
            DataSet ds = new DataSet();
            SqlParameter[] sqlParam = null;

            string vIP추적 = "";
            int v사용자추적 = 0;

            Exception _Ex = null;
            SqlException _SqlEx = null;

            try
            {
                dbCon = new DBCon(DB연결문자());

                InfoMessage = "";
                dbCon.SqlCon.InfoMessage += new SqlInfoMessageEventHandler(SqlConnection_InfoMessage);

                sqlParam = new SqlParameter[파라미터.Tables[0].Rows.Count - 2];

                foreach (DataRow dr in 파라미터.Tables[0].Rows)
                {

                    if (dr["컬럼명"].ToString() != "@IP추적")
                    {
                        if (dr["컬럼명"].ToString() != "@사용자추적")
                        {
                            sqlParam[i] = new SqlParameter();
                            sqlParam[i].ParameterName = dr["컬럼명"].ToString();
                            sqlParam[i].SqlDbType = DB타입(dr["타입"].ToString());
                            sqlParam[i].Size = Convert.ToInt32(dr["사이즈"].ToString());

                            if (dr["값"].ToString().ToUpper() == "NULL")
                            {
                                sqlParam[i].Value = DBNull.Value;
                            }
                            else
                            {
                                sqlParam[i].Value = dr["값"].ToString();
                            }

                            i++;
                        }
                        else
                        {
                            v사용자추적 = Convert.ToInt32(dr["값"].ToString());
                        }
                    }
                    else
                    {
                        vIP추적 = dr["값"].ToString();
                    }
                }

                ds = dbCon.FillDataSet(프로시저명, sqlParam, CommandType.StoredProcedure);

                return ds;
            }
            catch (SqlException SqlEx)
            {
                Console.WriteLine(InfoMessage);
                _SqlEx = SqlEx;
                throw _SqlEx;
            }
            catch (Exception ex)
            {
                _Ex = new Exception();
                _Ex = ex;
                throw _Ex;
            }
            finally
            {
                Console.WriteLine(InfoMessage);
            }
        }

        string InfoMessage = "";

        private void SqlConnection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            InfoMessage += e.Message;
        }

        private string DB연결문자()
        {
            string v = "";

            v = "Initial Catalog = DotNet2021xSeoul"
                    + "; Data Source = 20.194.33.2,10063"
                    + "; User id = user_dba"
                    + "; Password = Cm!202012"
                    + "; Connect Timeout = " + Convert.ToString(20) // 20sec
                    + "; Application Name = " + "DotNet2021xSeoul" // "Application_Name"
                    + "; TrustServerCertificate = true"
                    + "; Encrypt = true";

            return v;
        }
    }

    public class NETX_parameter
    {
        public DataTable dt = null;

        public NETX_parameter()
        {
            dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("컬럼명"),
                                                   new DataColumn("타입"),
                                                   new DataColumn("사이즈"),
                                                   new DataColumn("값"),
                                                   new DataColumn("값2"),
                                                   new DataColumn("값3") }
                               );
            dt.Columns["값2"].DataType = Type.GetType("System.Byte[]");
            dt.Columns["값3"].DataType = Type.GetType("System.Object");
            //ADD("@IP추적", DataType.VarChar, 100, Global.GetIP2());
            //ADD("@사용자추적", DataType.Int, 0, UserInfo.직원번호.ToString());
        }

        public void ADD(string 파라미터명, string 타입, int 사이즈, string 값)
        {
            dt.Rows.Add(파라미터명, 타입, 사이즈, 값, null, null);
        }
        public void ADD(string 파라미터명, string 타입, int 사이즈, byte[] 값)
        {
            dt.Rows.Add(파라미터명, 타입, 사이즈, null, 값, null); // 값2
        }
        public void ADD(string 파라미터명, string 타입, int 사이즈, object 값)
        {
            if (값 is DataTable)
            {
                DataTable temp = (DataTable)값;
                byte[] bin = ConvDataTableToBin(temp);

                dt.Rows.Add(파라미터명, 타입, 사이즈, null, null, bin); // 값3
            }
            else
            {
                dt.Rows.Add(파라미터명, 타입, 사이즈, null, null, 값); // 값3
            }
        }

        public DataSet GetDS()
        {
            DataSet ds = new DataSet();

            ds.Tables.Add(dt);

            return ds;
        }

        private byte[] ConvDataTableToBin(DataTable dt)
        {
            byte[] result = null;

            using (MemoryStream stm = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //dt.RemotingFormat = SerializationFormat.Binary;
                bf.Serialize(stm, dt);
                result = stm.ToArray();
            }

            return result;
        }
    }

    public class DataType
    {
        //private static int vBigInt = 0;
        //private static int vBinary = 1;
        //private static int vBit = 2;
        //private static int vChar = 3;
        //private static int vDate = 31;
        //private static int vDateTime = 4;
        //private static int vDateTime2 = 33;
        //private static int vDecimal = 5;
        //private static int vFloat = 6;
        //private static int vImage = 7;
        //private static int vInt = 8;
        //private static int vMoney = 9;
        //private static int vNChar = 10;
        //private static int vNText = 11;
        //private static int vNVarChar =12;
        //private static int vReal = 13;
        //private static int vVarChar = 22;

        private static string vBigInt = "SqlDbType.BigInt";
        private static string vBinary = "SqlDbType.Binary";
        private static string vVarBinary = "SqlDbType.VarBinary";
        private static string vBit = "SqlDbType.Bit";
        private static string vChar = "SqlDbType.Char";
        private static string vDate = "SqlDbType.Date";
        private static string vDateTime = "SqlDbType.DateTime";
        private static string vDateTime2 = "SqlDbType.DateTime2";
        private static string vDecimal = "SqlDbType.Decimal";
        private static string vFloat = "SqlDbType.Float";
        private static string vImage = "SqlDbType.Image";
        private static string vInt = "SqlDbType.Int";
        private static string vMoney = "SqlDbType.Money";
        private static string vNChar = "SqlD bType.NChar";
        private static string vNText = "SqlDbType.NText";
        private static string vNVarChar = "SqlDbType.NVarChar";
        private static string vReal = "SqlDbType.Real";
        private static string vVarChar = "SqlDbType.VarChar";
        private static string vTinyInt = "SqlDbType.TinyInt";
        private static string vText = "SqlDbType.Text";
        private static string vStructured = "SqlDbType.Structured";
        private static string vXml = "SqlDbType.Xml";

        public static string Structured
        {
            get { return vStructured; }
        }
        public static string Image
        {
            get { return vImage; }
        }
        public static string Int
        {
            get { return vInt; }
        }
        public static string Money
        {
            get { return vMoney; }
        }
        public static string NChar
        {
            get { return vNChar; }
        }

        public static string NText
        {
            get { return vNText; }
        }
        public static string NVarChar
        {
            get { return vNVarChar; }
        }
        public static string Real
        {
            get { return vReal; }
        }
        public static string VarChar
        {
            get { return vVarChar; }
        }

        public static string DateTime2
        {
            get { return vDateTime2; }
        }

        public static string Decimal
        {
            get { return vDecimal; }
        }

        public static string Float
        {
            get { return vFloat; }
        }

        public static string BigInt
        {
            get { return vBigInt; }
        }

        public static string Binary
        {
            get { return vBinary; }
        }

        public static string VarBinary
        {
            get { return vVarBinary; }
        }

        public static string Bit
        {
            get { return vBit; }
        }

        public static string Char
        {
            get { return vChar; }
        }

        public static string Date
        {
            get { return vDate; }
        }

        public static string DateTime
        {
            get { return vDateTime; }
        }

        public static string TinyInt
        {
            get { return vTinyInt; }
        }

        public static string Text
        {
            get { return vText; }
        }

        public static string Xml
        {
            get { return vXml; }
        }
    }
}