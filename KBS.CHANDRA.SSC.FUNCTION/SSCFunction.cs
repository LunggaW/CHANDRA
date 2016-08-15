using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Data;
using NLog;
using System.Configuration;
using System.Globalization;
using Oracle.DataAccess.Client;
using KBS.CHANDRA.SSC.DATAMODEL;
using System.Windows.Forms;

namespace KBS.CHANDRA.SSC.FUNCTION
{
    public class SSCFunction
    {
        private String ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private String ConnectionStringLocal = ConfigurationManager.AppSettings["ConnectionStringLocal"];
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private String ErrorString;

        private OracleConnection con;

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            try
            {
                logger.Trace("Start Starting Connection Server");
                con = new OracleConnection();
                con.ConnectionString = ConnectionString;
                logger.Debug("Connection String : " + con.ConnectionString.ToString());
                con.Open();
                logger.Debug("End Starting Connection Server");
            }
            catch (OracleException ex)
            {
                logger.Error("Connect Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public void ConnectLocal()
        {
            try
            {
                logger.Trace("Start Starting Connection Local");
                con = new OracleConnection();
                con.ConnectionString = ConnectionStringLocal;
                logger.Debug("Connection String : " + con.ConnectionString.ToString());
                con.Open();
                logger.Debug("End Starting Connection Local");
            }
            catch (OracleException ex)
            {
                logger.Error("Connect Local Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            try
            {
                logger.Debug("Closing Connection");
                con.Close();
                con.Dispose();
                logger.Debug("End Close Connection");
            }
            catch (Exception e)
            {
                logger.Error("Close Function");
                logger.Error(e.Message);
            }

        }

        /// <summary>
        /// Logins to SSC.
        /// </summary>
        /// <param name="UserID">The user id used to login.</param>
        /// <param name="Password">The password.</param>
        /// <returns></returns>
        public User Login(String UserID, String Password)
        {
            User user = new User();
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM KDSUSERSSC WHERE USERID ='" + UserID + "' AND PASSWORD = '" + Password +
                                  "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    user.UserID = (String) dr["USERID"];
                    user.Username = (String) dr["USERNAME"];
                    user.Password = (String) dr["PASSWORD"];
                    user.ProfileID = (String) dr["PROFILEID"];
                    user.Status = (User.UserStatus) ((Int16) dr["STATUS"]);
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return user;
            }
            catch (Exception e)
            {
                logger.Error("Login Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string lastUpdatedMV_ARTICLES()
        {
            string lastUpdated = null;
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select DCRE as MODIFIEDDATE from mv_articles where rownum = 1";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    lastUpdated = dr["MODIFIEDDATE"].ToString();
                }

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return lastUpdated;
            }
            catch (Exception e)
            {
                logger.Error("lastUpdatedMV_ARTICLES Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public bool CheckDataV_SITE(string strbyteFile)
        {
            string strByteV_SITE = "";
            logger.Debug("CheckDataV_SITE function");
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT CHECKSUM FROM KDSDATASYNCHSSC WHERE TABLENAME = 'V_SITE'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    strByteV_SITE = dr["CHECKSUM"].ToString();
                }

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");

                if (strByteV_SITE == strbyteFile)
                {
                    return true;
                }
                else
                {
                    logger.Debug("Hash is different, Updating V_SITE in KDSDATASYNCHSSC");
                    logger.Debug("Start Connect");
                    this.ConnectLocal();
                    logger.Debug("End Connect");

                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE KDSDATASYNCHSSC SET CHECKSUM = '" + strbyteFile +
                                      "', MODIFIEDDATE = SYSDATE WHERE TABLENAME = 'V_SITE'";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();

                    logger.Debug("Start Close Connection");
                    this.Close();
                    logger.Debug("End Close Connection");
                    return false;
                }




            }
            catch (Exception e)
            {
                logger.Error("CheckDataV_SITE Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public bool CheckDataMV_ARTICLES(string strbyteFile)
        {
            string strByteMV_Articles = "";
            logger.Debug("CheckDataMV_ARTICLES function");
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT CHECKSUM FROM KDSDATASYNCHSSC WHERE TABLENAME = 'MV_ARTICLES'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    strByteMV_Articles = dr["CHECKSUM"].ToString();
                }


                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                if (strByteMV_Articles == strbyteFile)
                {
                    return true;
                }
                else
                {
                    logger.Debug("Hash is different, Updating MV_ARTICLES in KDSDATASYNCHSSC");
                    logger.Debug("Start Connect");
                    this.ConnectLocal();
                    logger.Debug("End Connect");

                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE KDSDATASYNCHSSC SET CHECKSUM = '" + strbyteFile +
                                      "', MODIFIEDDATE = SYSDATE WHERE TABLENAME = 'MV_ARTICLES'";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();

                    logger.Debug("Start Close Connection");
                    this.Close();
                    logger.Debug("End Close Connection");
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Error("CheckDataMV_ARTICLES Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public bool CheckDataV_BRAND(string strbyteFile)
        {
            string strByteV_BRAND = "";
            logger.Debug("CheckDataV_BRAND function");
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT CHECKSUM FROM KDSDATASYNCHSSC WHERE TABLENAME = 'V_BRAND'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    strByteV_BRAND = dr["CHECKSUM"].ToString();
                }


                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");


                if (strByteV_BRAND == strbyteFile)
                {
                    return true;
                }
                else
                {
                    logger.Debug("Hash is different, Updating V_BRAND in KDSDATASYNCHSSC");
                    logger.Debug("Start Connect");
                    this.ConnectLocal();
                    logger.Debug("End Connect");

                    cmd.Connection = con;
                    cmd.CommandText = "UPDATE KDSDATASYNCHSSC SET CHECKSUM = '" + strbyteFile +
                                      "', MODIFIEDDATE = SYSDATE WHERE TABLENAME = 'V_BRAND'";
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();

                    logger.Debug("Start Close Connection");
                    this.Close();
                    logger.Debug("End Close Connection");
                    return false;
                }

            }
            catch (Exception e)
            {
                logger.Error("CheckDataV_BRAND Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public String getDiscount(Item item)
        {
            String Discount = null;
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "with PromoMsg as " +
                                  "(select decode(nvl(disc1,0),0,'',disc1||'%')|| " +
                                  "decode(nvl(disc2,0),0,'','+'||disc2||'%')|| " +
                                  " decode(nvl(disc3,0),0,'','+'||disc3||'%')|| " +
                                  "decode(nvl(discRP,0),0,'','+'||trim(to_char(discRP,'999G999G999'))) " +
                                  "msg " +
                                  "from mv_articles " +
                                  "where store = '" + GlobalVar.GlobalVarSite + "' " +
                                  "and brand = '" + item.Brand + "' " +
                                  "and barcode = '" + item.Barcode + "') " +
                                  "select trim(decode(substr(Msg,1,1),'+',substr(Msg,2,20),Msg)) as Msg " +
                                  "from PromoMsg";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Discount = (string) dr["MSG"];

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Discount;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String getPrice(String Barcode, String Site)
        {
            String Price = null;
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select PRICE " +
                                  "from mv_articles  " +
                                  "where BARCODE = :BARCODE " +
                                  "and STORE = :SITE";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = Barcode;
                cmd.Parameters.Add(new OracleParameter(":SITE", OracleDbType.Varchar2)).Value = Site;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                while (dr.Read())
                {
                    Price = (string)dr["PRICE"];

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Price;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public Int32 GetMinutesToLogout()
        {
            Int32 MinutesToLogout = 0;
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT MINUTESTOLOGOUT FROM KDSPARAMETERSSC WHERE ROWNUM = 1";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    MinutesToLogout = Convert.ToInt32(dr["MINUTESTOLOGOUT"].ToString());

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return MinutesToLogout;
            }
            catch (Exception e)
            {
                logger.Error("GetMinutesToLogout Function");
                logger.Error(e.Message);
                this.Close();
                return 0;
            }
        }

        public String SelectBrandNameByBrandCode(String BrandCode)
        {
            String BrandName = null;
            this.ConnectLocal();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT BRANDNAME FROM V_BRAND WHERE BRANDCODE ='" + BrandCode + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    BrandName = (String) dr["BRANDNAME"];
                }
                this.Close();
                return BrandName;
            }
            catch (Exception e)
            {
                logger.Error("SelectBrandNameByBrandCode Function");
                logger.Error(e.Message);
                return null;
            }
        }

        public int SelectVariantIDByBarCode(String Barcode)
        {
            int VARIANTID = 0;
            this.ConnectLocal();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT VARIANTID FROM MV_ARTICLES WHERE BARCODE ='" + Barcode + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    VARIANTID = (int) dr["VARIANTID"];
                }
                this.Close();
                return VARIANTID;
            }
            catch (Exception e)
            {
                logger.Error("SelectVariantIDByBarCode Function");
                logger.Error(e.Message);
                return 0;
            }
        }

        public decimal SelectTotalDiscountAmountByBarCode(String Barcode)
        {
            decimal TOTDISAMOUNT = 0;
            this.ConnectLocal();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT TOTDISAMOUNT FROM MV_ARTICLES WHERE BARCODE ='" + Barcode + "' AND STORE = '" +
                                  GlobalVar.GlobalVarSite + "'";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TOTDISAMOUNT = Decimal.Parse(dr["TOTDISAMOUNT"].ToString());
                }
                this.Close();
                return TOTDISAMOUNT;
            }
            catch (Exception e)
            {
                logger.Error("SelectTotalDiscountAmountByBarCode Function");
                logger.Error(e.Message);
                return 0;
            }
        }

        public DataTable SelectStockDisplay(Item item)
        {
            User user = new User();
            this.ConnectLocal();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "SELECT   ar.article, brand, ar.description, ar.color, ar.ukuran, " +
                    "TO_CHAR (ar.price, '999,999,999,999') AS price, " +
                    "NVL (findreserve ('" + GlobalVar.GlobalVarSite + "', ar.variantid),0) AS reserved, " +
                    "ar.qty AS \"G.O.L.D STOCK\", " +
                    "(qty - NVL (findreserve ('" + GlobalVar.GlobalVarSite +
                    "', ar.variantid), 0)) AS \"REMAINING STOCK\" " +
                    "FROM mv_articles ar inner join v_brand brand on Ar.Brand = Brand.Brandcode " +
                    "WHERE ar.STORE = '" + GlobalVar.GlobalVarSite + "' ";
                //"and EXISTS (SELECT 1 FROM kdsprofilebrandlinkssc WHERE profileid = '" + GlobalVar.GlobalVarProfileID + "' AND brandid = ar.brand)" +

                if (!String.IsNullOrWhiteSpace(item.Brand))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.BRAND LIKE '%" + item.Brand + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.Barcode))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.BARCODE LIKE '%" + item.Barcode + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.Article))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.ARTICLE LIKE '%" + item.Article + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.Color))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.COLOR LIKE '%" + item.Color + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.Description))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.DESCRIPTION LIKE '%" + item.Description + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.Size))
                {
                    cmd.CommandText = cmd.CommandText + "AND AR.UKURAN LIKE '%" + item.Size + "%'";
                }

                if (!String.IsNullOrWhiteSpace(item.BrandName))
                {
                    cmd.CommandText = cmd.CommandText + "AND BRAND.BRANDNAME LIKE '%" + item.BrandName + "%'";
                }

                //cmd.CommandText = cmd.CommandText + "GROUP BY AR.ARTICLE, BRAND ,Ar.Description, AR.COLOR, AR.UKURAN, ar.store, ar.qty, ar.price";


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectStockDisplay Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectMemoHeader(MemoDiscountHeader memoHeader)
        {
            this.Connect();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText = "select " +
                                  "PROMOHEADER.ermndis as  PROMOCODE, " +
                                  "TO_CHAR(PROMOHEADER.ERMDDEB, 'DD-MM-YYYY') AS STARTDATE, " +
                                  "TO_CHAR(PROMOHEADER.ERMDFIN, 'DD-MM-YYYY') AS ENDDATE, " +
                                  "NVL(PROMOHEADER.ERMHDEB,0) AS STARTTIME, " +
                                  "NVL(PROMOHEADER.ERMHFIN,0) AS ENDTIME, " +
                                  "DESCRIPTION.TERMDESC AS DESCRIPTION, " +
                                  "(CASE ( " +
                                  "NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) + " +
                                  "NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) + " +
                                  "NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) + " +
                                  "NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  ") WHEN 0 THEN 'MEMBER' " +
                                  "  ELSE 'NORMAL' END) DISC_TYPE , " +
                                  "(CASE  " +
                                  "( " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  ") WHEN 0  " +
                                  " THEN NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 0, 1),0) " +
                                  " ELSE NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) " +
                                  " END) DISCOUNT1, " +
                                  "(CASE  " +
                                  "( " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  ") WHEN 0  " +
                                  "  THEN NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 0, 2),0) " +
                                  "  ELSE NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) " +
                                  "  END) DISCOUNT2, " +
                                  "(CASE  " +
                                  "( " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  ") WHEN 0  " +
                                  "  THEN NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 0, 3),0) " +
                                  "  ELSE NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) " +
                                  "  END) DISCOUNT3, " +
                                  "(CASE  " +
                                  "( " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 1),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 2),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 3),0) + " +
                                  "  NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  ") WHEN 0  " +
                                  "  THEN NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 0, 4),0) " +
                                  "  ELSE NVL(KDSPKPROMO.GET_DISC_PERSEN(PROMOHEADER.ERMNDIS, 1, 4),0) " +
                                  "  END) DISCOUNTRP " +
                                  "FROM mixentpos promoheader, tra_mixentpos description " +
                                  "WHERE description.langue = 'GB' " +
                                  "and promoheader.ermndis = description.termndis " +
                                  "AND EXISTS (SELECT 1 FROM mixsite site WHERE promoheader.ermndis = srmndis and '" +
                                  GlobalVar.GlobalVarSite + "' = srmsite) " +
                                  "AND TRUNC (promoheader.ermddeb) <= TRUNC (to_date(:dateFromParam,'dd/mm/yy')) " +
                                  "AND TRUNC (to_date(:dateToParam,'dd/mm/yy')) <= TRUNC (promoheader.ermdfin)  ";

                //cmd.CommandText =
                //    "SELECT MV.PROMOCODE, " +
                //    "TO_CHAR (MV.STARTDATE, 'dd-mm-yyyy') as STARTDATE, " +
                //    "TO_CHAR (MV.ENDDATE, 'dd-mm-yyyy') as ENDDATE, " +
                //    "CASE WHEN MV.STARTTIME = '0' THEN '-' ELSE MV.STARTTIME END AS STARTTIME, "+
                //    "CASE WHEN MV.ENDTIME = '0' THEN '-' ELSE MV.ENDTIME END AS ENDTIME, "+
                //    "MV.DESCRIPTION, " +
                //    "MV.DISC_TYPE, " +
                //    "MV.DISCOUNT1, " +
                //    "MV.DISCOUNT2, " +
                //    "MV.DISCOUNT3 " +
                //    "FROM MV_MEMODISCOUNTHEADER MV " +
                //    "WHERE EXISTS(SELECT 1 FROM kdsprofileSITElinkssc WHERE profileid = '" + GlobalVar.GlobalVarProfileID + "' and siteid = MV.SITE)" +
                //    "AND MV.SITE = '" + GlobalVar.GlobalVarSite + "' " +
                //    "AND MV.STARTDATE BETWEEN TRUNC(:dateFromParam) AND TRUNC(:dateToParam)" +
                //    "AND MV.ENDDATE BETWEEN TRUNC(:dateFromParam) AND TRUNC(:dateToParam)";

                if (!string.IsNullOrWhiteSpace(memoHeader.PromoCode))
                {
                    cmd.CommandText = cmd.CommandText + "AND PROMOHEADER.ERMNDIS LIKE '%" + memoHeader.PromoCode + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(memoHeader.Description))
                {
                    cmd.CommandText = cmd.CommandText + "AND DESCRIPTION.TERMDESC LIKE  '%" + memoHeader.Description +
                                      "%' ";
                }

                cmd.CommandText = cmd.CommandText + "ORDER BY promoheader.ermndis";

                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new OracleParameter(":dateFromParam", OracleDbType.Date)).Value =
                    memoHeader.StartDate;
                cmd.Parameters.Add(new OracleParameter(":dateToParam", OracleDbType.Date)).Value = memoHeader.EndDate;

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectMemoHeader Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectMemoDetail(String PromoCode, MemoDiscountHeader memoHeader)
        {
            this.Connect();
            try
            {
                int PromoType = 0;
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
                    "SELECT ERMDNUM FROM MIXENTPOS WHERE ERMNDIS = '" + PromoCode + "' ";

                logger.Debug(cmd.CommandText);

                PromoType = int.Parse(cmd.ExecuteScalar().ToString());



                cmd.Connection = con;


                //1 = Disc Price, 2 = HSP
                if (PromoType == 1)
                {
                    logger.Debug("Promo Type is Disc Percent");
                    cmd.CommandText =
                        "with dt as " +
                        "( " +
                        "select distinct drmndis, drmcint " +
                        "from mixdetpos " +
                        "where " +
                        "        nvl(drmidstr,0)!=0 " +
                        "        and nvl(drmcint,0)!=0 " +
                        "        and decode('" + PromoCode + "','','1',drmndis) like '%'||decode('" + PromoCode +
                        "','','1','" + PromoCode + "')||'%' " +
                        ") " +
                        "select  " +
                        "          arvcexr Item, arvcexv SalesVariant, " +
                        "         decode(pkartattri.get_codeatt_artattri(1,arvcinr),'-1','','-2','',pkartattri.get_codeatt_artattri(1,arvcinr)) Brand, " +
                        "         pkstrucobj.get_desc(1,arvclibl,'GB') Description " +
                        "from artuv , " +
                        "            ( " +
                        "      SELECT     /*+ cardinality(e, 10)*/ " +
                        "               distinct e.objcint cint, dt.drmndis " +
                        "          FROM strucrel e, dt " +
                        "      CONNECT BY PRIOR e.objcint = e.objpere " +
                        "           AND TRUNC (CURRENT_DATE) BETWEEN e.objddeb AND e.objdfin " +
                        "      START WITH objpere = dt.drmcint " +
                        "      ) xx " +
                        "where cint=arvcinr  " +
                        "union  " +
                        "select arvcexr Item, arvcexv SalesVariant, " +
                        "        decode(pkartattri.get_codeatt_artattri(1,arvcinr),'-1','','-2','',pkartattri.get_codeatt_artattri(1,arvcinr)) Brand, " +
                        "        pkstrucobj.get_desc(1,arvclibl,'GB') Description " +
                        "from mixdetpos, artdetlist, artuv  " +
                        "where drmnlis is not null " +
                        "        and drmnlis=dlinlis " +
                        "        and arvcinv=dlicinv " +
                        "        and dliddeb >= TRUNC(:dateFromParam) and dlidfin <= TRUNC(:dateToParam) " +
                        "        and decode('" + PromoCode + "','','1',drmndis) like  '%'||decode('" + PromoCode +
                        "','','1','" + PromoCode + "')||'%' " +
                        "union " +
                        "select arvcexr Item, arvcexv SalesVariant, " +
                        "         decode(pkartattri.get_codeatt_artattri(1,arvcinr),'-1','','-2','',pkartattri.get_codeatt_artattri(1,arvcinr)) Brand, " +
                        "         pkstrucobj.get_desc(1,arvclibl,'GB') Description " +
                        "from mixdetpos, artuv  " +
                        "where drmnlis is null and drmidstr is null " +
                        "        and arvcinv=drmcinv " +
                        "        and decode('" + PromoCode + "','','1',drmndis) like '%'||decode('" + PromoCode +
                        "','','1','" + PromoCode + "')||'%' ";
                }
                else
                {
                    logger.Debug("Promo Type is HSP");
                    cmd.CommandText =
                        "WITH dt AS " +
                        "( " +
                        "SELECT DISTINCT brmndis, brmcint " +
                        "FROM mixbenpos " +
                        "WHERE NVL (brmidstr, 0) != 0 " +
                        "AND NVL (brmcint, 0) != 0 " +
                        "AND DECODE ('" + PromoCode + "', '', '1', brmndis) LIKE " +
                        "'%' || DECODE ('" + PromoCode + "', '', '1', '" + PromoCode + "') " +
                        "|| '%') " +
                        "SELECT arvcexr item, arvcexv salesvariant, " +
                        "DECODE (pkartattri.get_codeatt_artattri (1, arvcinr), " +
                        "'-1', '', " +
                        "'-2', '', " +
                        "pkartattri.get_codeatt_artattri (1, arvcinr) " +
                        ") brand, " +
                        "pkstrucobj.get_desc (1, arvclibl, 'GB') description " +
                        "FROM artuv, " +
                        "(SELECT          /*+ cardinality(e, 10)*/ " +
                        "DISTINCT e.objcint cint, dt.brmndis " +
                        "FROM strucrel e, dt " +
                        "CONNECT BY PRIOR e.objcint = e.objpere " +
                        "AND TRUNC (CURRENT_DATE) BETWEEN e.objddeb AND e.objdfin " +
                        "START WITH objpere = dt.brmcint) xx " +
                        "WHERE cint = arvcinr " +
                        "UNION " +
                        "SELECT arvcexr item, arvcexv salesvariant, " +
                        "DECODE (pkartattri.get_codeatt_artattri (1, arvcinr), " +
                        "'-1', '', " +
                        "'-2', '', " +
                        "pkartattri.get_codeatt_artattri (1, arvcinr) " +
                        ") brand, " +
                        "pkstrucobj.get_desc (1, arvclibl, 'GB') description " +
                        "FROM mixbenpos, artdetlist, artuv " +
                        "WHERE brmnlis IS NOT NULL " +
                        "AND brmnlis = dlinlis " +
                        "AND arvcinv = dlicinv " +
                        "AND dliddeb >= TRUNC (:dateFromParam) " +
                        "AND dlidfin <= TRUNC (:dateToParam) " +
                        "AND DECODE ('" + PromoCode + "', '', '1', brmndis) LIKE " +
                        "'%' || DECODE ('" + PromoCode + "', '', '1', '" + PromoCode + "') " +
                        "|| '%' " +
                        "UNION " +
                        "SELECT arvcexr item, arvcexv salesvariant, " +
                        "DECODE (pkartattri.get_codeatt_artattri (1, arvcinr), " +
                        "'-1', '', " +
                        "'-2', '', " +
                        "pkartattri.get_codeatt_artattri (1, arvcinr) " +
                        ") brand, " +
                        "pkstrucobj.get_desc (1, arvclibl, 'GB') description " +
                        "FROM mixbenpos, artuv " +
                        "WHERE brmnlis IS NULL " +
                        "AND brmidstr IS NULL " +
                        "AND arvcinv = brmcinv " +
                        "AND DECODE ('" + PromoCode + "', '', '1', brmndis) LIKE " +
                        "'%' || DECODE ('" + PromoCode + "', '', '1', '" + PromoCode + "') " +
                        "|| '%'";
                }




                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.Parameters.Add(new OracleParameter(":dateFromParam", OracleDbType.Date)).Value =
                    memoHeader.StartDate;
                cmd.Parameters.Add(new OracleParameter(":dateToParam", OracleDbType.Date)).Value = memoHeader.EndDate;

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectMemoDetail Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String getNextValSalesInput()
        {
            String NextVal = "";
            this.ConnectLocal();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select 'WH'||KDSSALESINPUTSSC_SEQ.NEXTVAL as NEXTVAL from dual";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    NextVal = (String) dr["NEXTVAL"];

                }
                this.Close();
                return NextVal;
            }
            catch (Exception e)
            {
                logger.Error("getNextValSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        /// <summary>
        /// Gets the brand data by profile id.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns> Data Table containing the data</returns>
        public DataTable GetBrandDataByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT * FROM V_BRAND WHERE BRANDCODE IN( SELECT BRANDID FROM KDSPROFILEBRANDLINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') ORDER BY BRANDNAME ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetBrandDataByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;

            }

        }


        public string insertSalesInput(Item item, String Qty, Decimal NetAmount, Decimal GrossAmount)
        {
            String ErrorString;
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSSALESINPUTSSC (NOMORNOTA, USERID, BARCODE, ARTICLE, DESCRIPTION, BRAND, UKURAN, QTY, VARIANTID, STORE, PRICE, DISCOUNT1, DISCOUNT2, NETAMOUNT, STATUS, COLOR, GROSSAMOUNT, SALESDATE, CREATEDBY, CREATEDDATE, DISCOUNT3, DISCOUNTRP, TOTALDISCAMT, FIXPRICE) VALUES " +
                    "('" + item.NomorNota + "','" + GlobalVar.GlobalVarUserID + "', '" + item.Barcode + "', '" +
                    item.Article + "', '" + item.Description + "', '" + item.Brand + "', '" + item.Size + "', '" + Qty +
                    "', '" + item.VariantID + "', '" + GlobalVar.GlobalVarSite + "', '" + item.Price + "' ," +
                    "'" + item.Discount1 + "', '" + item.Discount2 + "', '" + NetAmount + "', 1, '" + item.Color +
                    "', '" + GrossAmount + "', :dateParam, '" + GlobalVar.GlobalVarUserID + "', SYSDATE, '" +
                    item.Discount3 + "', '" + item.DiscountRP + "', '" + item.TotalDiscountAmount + "', '" +
                    item.FixPrice + "')";
                cmd.Parameters.Add(new OracleParameter(":dateParam", OracleDbType.Date)).Value = DateTime.Now;


                //cmd.CommandText = "INSERT INTO KDSSALESINPUTSSC (NOMORNOTA, USERID, BARCODE, ARTICLE, DESCRIPTION, BRAND, UKURAN, QTY, VARIANTID, STORE, PRICE, DISCOUNT1, DISCOUNT2, NETAMOUNT, STATUS, COLOR, GROSSAMOUNT, SALESDATE, CREATEDBY, CREATEDDATE, DISCOUNT3, DISCOUNTRP, TOTALDISCAMT) VALUES " +
                //                    "(:nomorNotaParam, :userIDParam, :barcodeParam, :articleParam, :descriptionParam, :brandParam, :sizeParam, :qtyParam, :variantParam, :siteParam, :priceParam, " +
                //                     ":discount1Param, :discount2Param, :netAmountParam, 1, :colorParam, :grossAmountParam, SYSDATE, :userIDParam, SYSDATE, :discount3Param, :discountrpParam, :totalDiscountAmountParam)";
                //cmd.Parameters.Add(new OracleParameter(":nomorNotaParam", OracleDbType.Varchar2)).Value = item.NomorNota;
                //cmd.Parameters.Add(new OracleParameter(":userIDParam", OracleDbType.Varchar2)).Value = UserID;
                //cmd.Parameters.Add(new OracleParameter(":barcodeParam", OracleDbType.Varchar2)).Value = item.Barcode;
                //cmd.Parameters.Add(new OracleParameter(":articleParam", OracleDbType.Varchar2)).Value = item.Article;
                //cmd.Parameters.Add(new OracleParameter(":descriptionParam", OracleDbType.Varchar2)).Value = item.Description;
                //cmd.Parameters.Add(new OracleParameter(":brandParam", OracleDbType.Varchar2)).Value = item.Brand;
                //cmd.Parameters.Add(new OracleParameter(":sizeParam", OracleDbType.Varchar2)).Value = item.Size;
                //cmd.Parameters.Add(new OracleParameter(":qtyParam", OracleDbType.Int32)).Value = Qty;
                //cmd.Parameters.Add(new OracleParameter(":variantParam", OracleDbType.Varchar2)).Value = item.VariantID;
                //cmd.Parameters.Add(new OracleParameter(":siteParam", OracleDbType.Varchar2)).Value = GlobalVar.GlobalVarSite;
                //cmd.Parameters.Add(new OracleParameter(":priceParam", OracleDbType.Decimal)).Value = item.Price;
                //cmd.Parameters.Add(new OracleParameter(":discount1Param", OracleDbType.Int16)).Value = item.Discount1;
                //cmd.Parameters.Add(new OracleParameter(":discount2Param", OracleDbType.Int16)).Value = item.Discount2;
                //cmd.Parameters.Add(new OracleParameter(":discount3Param", OracleDbType.Int16)).Value = item.Discount3;
                //cmd.Parameters.Add(new OracleParameter(":discountrpParam", OracleDbType.Decimal)).Value = item.DiscountRP;
                //cmd.Parameters.Add(new OracleParameter(":netAmountParam", OracleDbType.Decimal)).Value = NetAmount;
                //cmd.Parameters.Add(new OracleParameter(":colorParam", OracleDbType.Varchar2)).Value = item.Color;
                //cmd.Parameters.Add(new OracleParameter(":grossAmountParam", OracleDbType.Decimal)).Value = GrossAmount;
                //cmd.Parameters.Add(new OracleParameter(":priceParam", OracleDbType.Varchar2)).Value = item.Price;
                //cmd.Parameters.Add(new OracleParameter(":totalDiscountAmountParam", OracleDbType.Decimal)).Value = item.TotalDiscountAmount;

                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndNomorNotaReservedOnly(Item itemValidasi,
            String NomorNota)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT SI.NOMORNOTA AS\"NOMOR NOTA\", SI.ARTICLE, BRAND.BRANDCODE, BRAND.BRANDNAME, SI.DESCRIPTION, SI.COLOR, SI.UKURAN AS \"SIZE\", SI.BARCODE, SI.QTY AS \"QTY ORDERED\", " +
                    "TO_CHAR(SI.PRICE,'999,999,999,999') AS PRICE , " +
                    "TO_CHAR(SI.PRICE,'999,999,999,999') AS \"FIX PRICE\", " +
                    "SI.DISCOUNT1, SI.DISCOUNT2, SI.DISCOUNT3, " +
                    "TO_CHAR(SI.DISCOUNTRP,'999,999,999,999') AS \"DISCOUNT RP\", " +
                    //"TO_CHAR(SI.TOTALDISCAMT,'999,999,999,999') AS \"TOTAL DISCOUNT AMOUNT\", " +
                    "TO_CHAR(SI.GROSSAMOUNT,'999,999,999,999') AS \"GROSS AMOUNT\"," +
                    "TO_CHAR(SI.NETAMOUNT,'999,999,999,999') AS \"NET AMOUNT\" " +
                    "FROM KDSSALESINPUTSSC SI, V_BRAND BRAND " +
                    "WHERE EXISTS (SELECT 1 FROM Kdsprofilebrandlinkssc WHERE PROFILEID = '" +
                    GlobalVar.GlobalVarProfileID + "' AND BRANDID = SI.BRAND) " +
                    "AND SI.BRAND = BRAND.BRANDCODE " +
                    "AND SI.STORE = '" + GlobalVar.GlobalVarSite + "' " +
                    "AND SI.STATUS = 1 " +
                    "AND SI.SALESDATE >= TRUNC(SYSDATE) ";

                if (!string.IsNullOrWhiteSpace(itemValidasi.BrandName))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND BRAND.BRANDNAME LIKE '%" + itemValidasi.BrandName + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(itemValidasi.Brand))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.BRAND LIKE '%" + itemValidasi.Brand + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(NomorNota))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.NOMORNOTA LIKE '%" + NomorNota + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(itemValidasi.Article))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.ARTICLE LIKE '%" + itemValidasi.Article + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(itemValidasi.Color))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.COLOR LIKE '%" + itemValidasi.Color + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(itemValidasi.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.BARCODE LIKE '%" + itemValidasi.Barcode + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(itemValidasi.Size))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.UKURAN LIKE '%" + itemValidasi.Size + "%' ";
                }


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndNomorNotaReservedOnly Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription(
            SalesSearchFilter salesSearchFilter, String ProfileID, String Site)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                //cmd.CommandText = "SELECT BARCODE, ARTICLE, DESCRIPTION, BRAND, UKURAN, COLOR, QTY, STORE, DISC1, DISC2, DISC3, " +
                //                    "TO_CHAR(DISCRP,'999,999,999,999') AS DISCRP ," +
                //                  "TO_CHAR(PRICE,'999,999,999,999') AS PRICE " +
                //                  "FROM MV_ARTICLES WHERE " +
                //                   "BRAND IN (SELECT BRANDCODE FROM V_BRAND WHERE BRANDCODE IN " +
                //                    "(SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " + ProfileID + ") " +
                //                        "AND BRANDNAME LIKE '%" + BrandName + "%') " +
                //                  "AND BRAND LIKE '%" + BrandID + "%' " +
                //                  "AND STORE = '" + Site + "' " +
                //                  "AND ARTICLE LIKE '%" + Article + "%' " +
                //                  "AND DESCRIPTION LIKE '%" + Description + "%'";


                cmd.CommandText =
                    "SELECT BARCODE, ARTICLE, DESCRIPTION, BRAND, UKURAN, COLOR, QTY, STORE, DISC1, DISC2, DISC3, " +
                    "TO_CHAR(DISCRP,'999,999,999,999') AS DISCRP, " +
                    "TO_CHAR(PRICE,'999,999,999,999') AS PRICE," +
                    "TO_CHAR(FIXPRICE,'999,999,999,999') AS FIXPRICE " +
                    "FROM MV_ARTICLES " +
                    "WHERE STORE = '" + Site + "' " +
                    "AND decode(nvl('" + salesSearchFilter.BrandCode + "',''),'','-1',BRAND) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.BrandCode + "',''),'','-1','" + salesSearchFilter.BrandCode + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Article + "',''),'','-1',ARTICLE) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.Article + "',''),'','-1','" + salesSearchFilter.Article + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Description +
                    "',''),'','-1',DESCRIPTION) LIKE '%'||decode(nvl('" + salesSearchFilter.Description +
                    "',''),'','-1','" + salesSearchFilter.Description + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Size + "',''),'','-1',UKURAN) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.Size + "',''),'','-1','" + salesSearchFilter.Size + "')||'%' " +
                    //"AND exists (SELECT 1 " +
                    //    "FROM Kdsprofilebrandlinkssc " +
                    //    "WHERE PROFILEID = '" + ProfileID + "' and BRAND=BRANDID " +
                    //    "and rownum=1 " +
                    //    ") " +
                    "and exists (SELECT 1 " +
                    "FROM V_BRAND " +
                    "WHERE BRAND=BRANDCODE " +
                    "AND decode(nvl('" + salesSearchFilter.BrandName + "',''),'','-1',BRANDNAME) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.BrandName + "',''),'','-1','" + salesSearchFilter.BrandName + "')||'%' " +
                    "and rownum=1 " +
                    ")";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public int CountSelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription(
            SalesSearchFilter salesSearchFilter, String ProfileID, String Site)
        {
            int TotalRecords = 0;
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                //cmd.CommandText = "SELECT BARCODE, ARTICLE, DESCRIPTION, BRAND, UKURAN, COLOR, QTY, STORE, DISC1, DISC2, DISC3, " +
                //                    "TO_CHAR(DISCRP,'999,999,999,999') AS DISCRP ," +
                //                  "TO_CHAR(PRICE,'999,999,999,999') AS PRICE " +
                //                  "FROM MV_ARTICLES WHERE " +
                //                   "BRAND IN (SELECT BRANDCODE FROM V_BRAND WHERE BRANDCODE IN " +
                //                    "(SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " + ProfileID + ") " +
                //                        "AND BRANDNAME LIKE '%" + BrandName + "%') " +
                //                  "AND BRAND LIKE '%" + BrandID + "%' " +
                //                  "AND STORE = '" + Site + "' " +
                //                  "AND ARTICLE LIKE '%" + Article + "%' " +
                //                  "AND DESCRIPTION LIKE '%" + Description + "%'";


                cmd.CommandText =
                    "SELECT COUNT(1) " +
                    "FROM MV_ARTICLES " +
                    "WHERE STORE = '" + Site + "' " +
                    "AND decode(nvl('" + salesSearchFilter.BrandCode + "',''),'','-1',BRAND) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.BrandCode + "',''),'','-1','" + salesSearchFilter.BrandCode + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Article + "',''),'','-1',ARTICLE) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.Article + "',''),'','-1','" + salesSearchFilter.Article + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Description +
                    "',''),'','-1',DESCRIPTION) LIKE '%'||decode(nvl('" + salesSearchFilter.Description +
                    "',''),'','-1','" + salesSearchFilter.Description + "')||'%' " +
                    "AND decode(nvl('" + salesSearchFilter.Size + "',''),'','-1',UKURAN) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.Size + "',''),'','-1','" + salesSearchFilter.Size + "')||'%' " +
                    //"AND exists (SELECT 1 " +
                    //    "FROM Kdsprofilebrandlinkssc " +
                    //    "WHERE PROFILEID = '" + ProfileID + "' and BRAND=BRANDID " +
                    //    "and rownum=1 " +
                    //    ") " +
                    "and exists (SELECT 1 " +
                    "FROM V_BRAND " +
                    "WHERE BRAND=BRANDCODE " +
                    "AND decode(nvl('" + salesSearchFilter.BrandName + "',''),'','-1',BRANDNAME) LIKE '%'||decode(nvl('" +
                    salesSearchFilter.BrandName + "',''),'','-1','" + salesSearchFilter.BrandName + "')||'%' " +
                    "and rownum=1 " +
                    ")";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                TotalRecords = int.Parse(cmd.ExecuteScalar().ToString());
                this.Close();
                return TotalRecords;
            }
            catch (Exception e)
            {
                logger.Error("SelectItemByBrandCodeAndBrandNameAndProfileIDAndSiteAndArticleAndDescription Function");
                logger.Error(e.Message);
                this.Close();
                return 0;
            }

        }

        public DataTable SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteSalesInput(String Barcode,
            String ProfileID, String Site)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT MV_ARTICLES.BARCODE, MV_ARTICLES.ARTICLE, MV_ARTICLES.DESCRIPTION, MV_ARTICLES.BRAND, MV_ARTICLES.UKURAN, MV_ARTICLES.QTY, MV_ARTICLES.VARIANTID, MV_ARTICLES.STORE, MV_ARTICLES.PRICE, MV_ARTICLES.DISC1, MV_ARTICLES.DISC2, MV_ARTICLES.DISC3, MV_ARTICLES.DISCRP, MV_ARTICLES.TOTDISAMOUNT, MV_ARTICLES.COLOR, V_Brand.Brandname" +
                    ",MV_ARTICLES.FIXPRICE " +
                    " FROM MV_ARTICLES, V_BRAND" +
                    " WHERE MV_ARTICLES.BRAND IN (SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " +
                    ProfileID + ")" +
                    " AND MV_ARTICLES.BARCODE = '" + Barcode + "'" +
                    " AND MV_ARTICLES.STORE = '" + Site + "'" +
                    " AND MV_ARTICLES.BRAND = V_BRAND.BRANDCODE" +
                    " AND ROWNUM = 1";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }


        public DataTable SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(
            String Barcode, String ProfileID, String Site)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT MV_ARTICLES.BARCODE, " +
                                  "MV_ARTICLES.ARTICLE, " +
                                  "MV_ARTICLES.DESCRIPTION, " +
                                  "MV_ARTICLES.BRAND, " +
                                  "MV_ARTICLES.UKURAN, " +
                                  "MV_ARTICLES.VARIANTID, " +
                                  "MV_ARTICLES.STORE, " +
                                  "1 AS QTY, " +
                                  "TO_CHAR(MV_ARTICLES.PRICE,'999,999,999,999') AS PRICE, " +
                                  "TO_CHAR(MV_ARTICLES.FIXPRICE,'999,999,999,999') AS FIXPRICE, " +
                                  "MV_ARTICLES.DISC1, MV_ARTICLES.DISC2, MV_ARTICLES.DISC3, " +
                                  "TO_CHAR(MV_ARTICLES.DISCRP,'999,999,999,999') AS DISCRP, " +
                                  "NVL(TO_CHAR(MV_ARTICLES.TOTDISAMOUNT,'999,999,999,999'),0) AS TOTDISCAMOUNT, " +
                                  "CASE WHEN MV_ARTICLES.FIXPRICE IS NOT NULL THEN TO_CHAR((MV_ARTICLES.FIXPRICE),'999,999,999,999') " +
                                  "ELSE TO_CHAR((MV_ARTICLES.PRICE - NVL(MV_ARTICLES.TOTDISAMOUNT,0)),'999,999,999,999') " +
                                  "END " +
                                  "AS NETAMOUNT, " +
                                  "MV_ARTICLES.COLOR, " +
                                  "V_Brand.Brandname" +
                                  " FROM MV_ARTICLES, V_BRAND" +
                                  " WHERE MV_ARTICLES.BRAND IN (SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " +
                                  ProfileID + ")" +
                                  " AND ROWNUM = 1" +
                                  " AND MV_ARTICLES.BARCODE = '" + Barcode + "'" +
                                  " AND MV_ARTICLES.STORE = '" + Site + "'" +
                                  " AND MV_ARTICLES.BRAND = V_BRAND.BRANDCODE";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectItemByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public Item SelectItemPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(
            String Barcode, String ProfileID, String Site)
        {
            Item itemContainer = new Item();
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT " +
                                  "MV_ARTICLES.PRICE, " +
                                  "MV_ARTICLES.FIXPRICE, " +
                                  "MV_ARTICLES.DISCRP, " +
                                  "MV_ARTICLES.TOTDISAMOUNT " +
                                  " FROM MV_ARTICLES" +
                                  " WHERE MV_ARTICLES.BRAND IN (SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " +
                                  ProfileID + ")" +
                                  " AND MV_ARTICLES.BARCODE = '" + Barcode + "'" +
                                  " AND MV_ARTICLES.STORE = '" + Site + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    itemContainer.FixPrice = string.IsNullOrWhiteSpace(dr["FIXPRICE"].ToString())
                        ? 0
                        : decimal.Parse(dr["FIXPRICE"].ToString());
                    itemContainer.Price = decimal.Parse(dr["PRICE"].ToString());
                    itemContainer.TotalDiscountAmount = string.IsNullOrWhiteSpace(dr["TOTDISAMOUNT"].ToString())
                        ? 0
                        : decimal.Parse(dr["TOTDISAMOUNT"].ToString());
                    itemContainer.DiscountRP = decimal.Parse(dr["DISCRP"].ToString());
                }

                this.Close();
                return itemContainer;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectItemPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public Decimal SelectPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput(String Barcode,
            String ProfileID, String Site)
        {
            try
            {
                Decimal Price = 0;
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT MV_ARTICLES.PRICE " +
                                  " FROM MV_ARTICLES, V_BRAND" +
                                  " WHERE MV_ARTICLES.BRAND IN (SELECT BRANDID FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " +
                                  ProfileID + ")" +
                                  " AND MV_ARTICLES.BARCODE = '" + Barcode + "'" +
                                  " AND MV_ARTICLES.STORE = '" + Site + "'" +
                                  " AND MV_ARTICLES.BRAND = V_BRAND.BRANDCODE";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Price = Decimal.Parse(dr["PRICE"].ToString());
                }
                this.Close();
                return Price;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPriceByBrandCodeAndBarcodeAndBrandNameAndProfileIDAndSiteMultipleSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return 0;
            }

        }

        public DataTable SelectSalesIputSalesHistory(Item item, String From, String To, String NomorNota, String UserID,
            String StatusItem)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT SI.STORE, SI.NOMORNOTA, SI.USERID, SI.ARTICLE, BRAND.BRANDCODE, BRAND.BRANDNAME, SI.DESCRIPTION, SI.COLOR, SI.UKURAN AS \"SIZE\", SI.BARCODE, SI.QTY, " +
                    "TO_CHAR(SI.PRICE,'999,999,999,999') AS PRICE,  " +
                    "DISCOUNT1, DISCOUNT2, DISCOUNT3, " +
                    "TO_CHAR(SI.DISCOUNTRP,'999,999,999,999') AS \"DISCOUNT RP\", " +
                    "TO_CHAR(SI.GROSSAMOUNT,'999,999,999,999') AS GROSSAMOUNT, " +
                    "TO_CHAR(SI.NETAMOUNT,'999,999,999,999') AS NETAMOUNT, " +
                    "DECODE(SI.STATUS, 1, 'Reserved', 2, 'Sold', 3, 'Cancelled', 'Unknown Code') STATUS, " +
                    "SI.SALESDATE, SI.CREATEDDATE, SI.MODIFIEDDATE, SI.CREATEDBY, SI.MODIFIEDBY " +
                    "FROM KDSSALESINPUTSSC SI, V_BRAND BRAND " +
                    "WHERE EXISTS (SELECT 1 FROM Kdsprofilebrandlinkssc WHERE PROFILEID = " +
                    GlobalVar.GlobalVarProfileID + " AND BRAND = Si.BRAND) " +
                    "AND ( trunc(SI.salesdate) >= trunc(TO_DATE ('" + From + "', 'dd-mm-yyyy')) " +
                    "AND trunc(SI.salesdate) <= trunc(TO_DATE ('" + To + "', 'dd-mm-yyyy'))) " +
                    "AND SI.BRAND = BRAND.BRANDCODE " +
                    "AND SI.STORE = '" + GlobalVar.GlobalVarSite + "' ";

                if (!string.IsNullOrWhiteSpace(item.BrandName))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND BRAND.BRANDNAME LIKE '%" + item.BrandName + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(item.Size))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.UKURAN LIKE '%" + item.Size + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(item.Brand))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.BRAND LIKE '%" + item.Brand + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(NomorNota))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.NOMORNOTA LIKE '%" + NomorNota + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(item.Article))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.ARTICLE LIKE '%" + item.Article + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(item.Color))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.COLOR LIKE '%" + item.Color + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(item.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.BARCODE LIKE '%" + item.Barcode + "%' ";
                }

                if (!string.IsNullOrWhiteSpace(StatusItem))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.STATUS  = '" + StatusItem + "' ";
                }

                if (!string.IsNullOrWhiteSpace(UserID))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND SI.USERID LIKE '%" + UserID + "%' ";
                }

                //AND SI.USERID LIKE '%%' 
                //AND SI.STATUS LIKE '%%' "
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectSalesIputSalesHistory Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectPrintLabelPurchaseOrder(String Site, String PONumber, String From, String To,
            String POStatus, String StatusPrint)
        {
            try
            {
                String StatusPrint2 = String.IsNullOrWhiteSpace(StatusPrint)
                    ? ""
                    : "AND decode(nvl(ecdnseq,0),0,0,1)=nvl(" + StatusPrint + ",0) ";

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT ECDSITE AS SITE, " +
                                  "ECDCEXCDE AS \"PO NUMBER\", ECDDCOM AS \"ORDER DATE\", " +
                                  "DECODE(ECDETAT, 5, 'Awaiting Delivery', 7, 'Received', 'Unknown Code') AS \"PO STATUS\", " +
                                  "DECODE(NVL(ECDNSEQ,0), 0, 'Not Printed', 'Printed') \"PRINT STATUS\" " +
                                  "FROM CDEENTCDE " +
                                  "WHERE ECDETAT IN (5,7) " +
                                  "AND ECDSITE LIKE '%" + Site + "%' " +
                                  "AND ECDCEXCDE LIKE '%" + PONumber + "%' " +
                                  "AND ECDETAT LIKE '%" + POStatus + "%' " +
                                  "AND ( trunc(ECDDCOM) >= trunc(TO_DATE ('" + From + "', 'dd-mm-yyyy')) " +
                                  "AND trunc(ECDDCOM) <= trunc(TO_DATE ('" + To + "', 'dd-mm-yyyy'))) " +
                                  StatusPrint2;
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectPrintLabel Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectPrintLabelReceivingWithoutOrder(String Site, String PONumber, String From, String To,
            String StatusPrint)
        {
            try
            {
                String StatusPrint3 = String.IsNullOrWhiteSpace(StatusPrint)
                    ? ""
                    : "AND decode(nvl(SERCCPT,0),0,0,1)=nvl(" + StatusPrint + ",0) ";


                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT SERSITE AS SITE, " +
                                  "SERNUSR AS \"PO NUMBER\", SERDCOM AS \"ORDER DATE\", " +
                                  "'Reception Without Order'  AS \"PO STATUS\", " +
                                  "DECODE(NVL(SERCCPT,0), 0, 'Not Printed', 'Printed') \"PRINT STATUS\" " +
                                  "from stoentre " +
                                  "where sercincde=999999999 " +
                                  "AND SERSITE LIKE '%" + Site + "%' " +
                                  "AND SERNUSR LIKE '%" + PONumber + "%' " +
                                  StatusPrint3 +
                                  "AND ( trunc(SERDCOM) >= trunc(TO_DATE ('" + From + "', 'dd-mm-yyyy')) " +
                                  "AND trunc(SERDCOM) <= trunc(TO_DATE ('" + To + "', 'dd-mm-yyyy')))";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectPrintLabel Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectPrintLabel(String Site, String PONumber, String From, String To, String POStatus,
            String StatusPrint)
        {
            try
            {
                String StatusPrint2 = String.IsNullOrWhiteSpace(StatusPrint)
                    ? ""
                    : "AND decode(nvl(ecdnseq,0),0,0,1)=nvl(" + StatusPrint + ",0) ";

                String StatusPrint3 = String.IsNullOrWhiteSpace(StatusPrint)
                    ? ""
                    : "AND decode(nvl(SERCCPT,0),0,0,1)=nvl(" + StatusPrint + ",0) ";
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT ECDSITE AS SITE, " +
                                  "ECDCEXCDE AS \"PO NUMBER\", ECDDCOM AS \"ORDER DATE\", " +
                                  "DECODE(ECDETAT, 5, 'Awaiting Delivery', 7, 'Received', 'Unknown Code') AS \"PO STATUS\", " +
                                  "DECODE(NVL(ECDNSEQ,0), 0, 'Not Printed', 'Printed') \"PRINT STATUS\" " +
                                  "FROM CDEENTCDE " +
                                  "WHERE ECDETAT IN (5,7) " +
                                  "AND ECDSITE LIKE '%" + Site + "%' " +
                                  "AND ECDCEXCDE LIKE '%" + PONumber + "%' " +
                                  "AND ECDETAT LIKE '%" + POStatus + "%' " +
                                  StatusPrint2 +
                                  "AND ( trunc(ECDDCOM) >= trunc(TO_DATE ('" + From + "', 'dd-mm-yyyy')) " +
                                  "AND trunc(ECDDCOM) <= trunc(TO_DATE ('" + To + "', 'dd-mm-yyyy')))" +
                                  "UNION ALL " +
                                  "SELECT SERSITE AS SITE, " +
                                  "SERNUSR AS \"PO NUMBER\", SERDCOM AS \"ORDER DATE\", " +
                                  "'Reception Without Order'  AS \"PO STATUS\", " +
                                  "DECODE(NVL(SERCCPT,0), 0, 'Not Printed', 'Printed') \"PRINT STATUS\" " +
                                  "from stoentre " +
                                  "where sercincde=999999999 " +
                                  "AND SERSITE LIKE '%" + Site + "%' " +
                                  "AND SERNUSR LIKE '%" + PONumber + "%' " +
                                  StatusPrint3 +
                                  "AND ( trunc(SERDCOM) >= trunc(TO_DATE ('" + From + "', 'dd-mm-yyyy')) " +
                                  "AND trunc(SERDCOM) <= trunc(TO_DATE ('" + To + "', 'dd-mm-yyyy')))";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectPrintLabel Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        /// <summary>
        /// Gets the brand data exclude by profile id.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public DataTable GetBrandDataExcludeByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT * FROM V_BRAND WHERE BRANDCODE NOT IN(SELECT BRANDID FROM KDSPROFILEBRANDLINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') ORDER BY BRANDNAME";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetBrandDataExcludeByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }


        /// <summary>
        /// Gets the site data by profile identifier.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public DataTable GetSiteDataByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT * FROM V_SITE WHERE SITECODE IN( SELECT SITEID FROM KDSPROFILESITELINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') ORDER BY SITENAME";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetSiteDataByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        /// <summary>
        /// Gets the site data exclude by profile id.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public DataTable GetSiteDataExcludeByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT * FROM V_SITE WHERE SITECODE NOT IN(SELECT SITEID FROM KDSPROFILESITELINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') ORDER BY SITENAME";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetSiteDataExcludeByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable GetMenuDataByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT MENUID, MENU FROM KDSMENUSSC WHERE MENUID IN( SELECT MENUID FROM KDSPROFILEMENULINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') AND MENUID != 6";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetMenuDataByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        /// <summary>
        /// Gets the site data exclude by profile id.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public DataTable GetMenuDataExcludeByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT MENUID, MENU FROM KDSMENUSSC WHERE MENUID NOT IN( SELECT MENUID FROM KDSPROFILEMENULINKSSC WHERE PROFILEID = '" +
                    ProfileID + "') AND MENUID != 6";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetMenuDataExcludeByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }


        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="NewPassword">The new password.</param>
        /// <param name="UserID">The user identifier.</param>
        /// <param name="OldPassword">The old password.</param>
        /// <returns></returns>
        public string changePassword(String NewPassword, String UserID, String OldPassword)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE KDSUSERSSC SET PASSWORD = '" + NewPassword +
                                  "', MODIFIEDDATE = SYSDATE, MODIFIEDBY = '" + GlobalVar.GlobalVarUserID +
                                  "' WHERE USERID = '" + UserID + "'";
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                ErrorString = "Success Updating Password, Password Changed to : " + NewPassword;

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("changePassword Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string changeMinutesToLogout(String MinutesToLogout)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE KDSPARAMETERSSC SET MINUTESTOLOGOUT = '" + MinutesToLogout +
                                  "' , MODIFIEDDATE = SYSDATE, MODIFIEDBY = '" + GlobalVar.GlobalVarUserID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                ErrorString = "Success Updating Minutes To Logout to " + MinutesToLogout + "";

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                ErrorString = e.Message;
                logger.Error("changeMinutesToLogout Function");
                logger.Error(e.Message);
                this.Close();
                return ErrorString;
            }
        }

        public string changeSalesInputStatusFromReservedToCancelled()
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE KDSSALESINPUTSSC SET " +
                                  "STATUS = 3, MODIFIEDDATE = SYSDATE, MODIFIEDBY = '" + GlobalVar.GlobalVarUserID +
                                  "' " +
                                  "WHERE STATUS = 1 " +
                                  "AND SALESDATE BETWEEN TRUNC(SYSDATE - 1) AND TRUNC(SYSDATE) - 1/86400";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                ErrorString = "Success Updating";

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("changeSalesInputStatusFromReservedToCancelled Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string updateSalesInputFlagbyNomorNota(String NomorNota, String Status)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE KDSSALESINPUTSSC SET STATUS = " + Status + ", MODIFIEDBY = '" +
                                  GlobalVar.GlobalVarUserID + "', MODIFIEDDATE = SYSDATE WHERE NOMORNOTA = '" +
                                  NomorNota + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                ErrorString = "Success";

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("updateSalesInputFlagbyNomorNota Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string GetFileNameFromPONumber(String PONumber)
        {
            try
            {
                String FileName = null;
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT LPAD(ECDCINCDE, 8,'0') AS FILENAME FROM CDEENTCDE WHERE ECDCEXCDE = '" +
                                  PONumber + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    FileName = (String) dr["FILENAME"];
                }


                if (string.IsNullOrWhiteSpace(FileName))
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT LPAD(SERCINREC, 8,'0') AS FILENAME FROM STOENTRE WHERE SERNUSR = '" +
                                      PONumber + "'";
                    cmd.CommandType = CommandType.Text;

                    logger.Debug(cmd.CommandText);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        FileName = (String) dr["FILENAME"];
                    }
                }



                ErrorString = "Success";

                this.Close();
                return FileName;
            }
            catch (Exception e)
            {
                logger.Error("GetFileNameFromPONumber Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string GetFilePathFromSite(String Site)
        {
            try
            {
                String FilePath = null;
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                //cmd.CommandText = "SELECT TPARCOMM FROM TRA_PARPOSTES WHERE TPARTABL = '1104' AND LANGUE = 'GB' AND TPARLIBC LIKE '%" + Site + "%'";
                cmd.CommandText = "SELECT SATVALN FROM SITATTRI WHERE " +
                                  "SATSITE = " + Site + " " +
                                  "AND SATCLA='FTP' " +
                                  "AND SATATT='LBL' " +
                                  "AND trunc(CURRENT_DATE) BETWEEN TRUNC(SATDDEB) AND TRUNC(SATDFIN)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    FilePath = (String) dr["TPARCOMM"];
                }

                ErrorString = "Success";

                this.Close();
                return FilePath;
            }
            catch (Exception e)
            {
                logger.Error("GetFilePathFromSite Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public DataTable GetFileContentByPONumber(String PONumber)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                //cmd.CommandText =
                //    "select RPAD (NVL (kdspkcustom.get_barcode (dcdcinl, dcddcom, 1), ' '),9,' ') codeOnLabel_F1, " +
                //    "RPAD (NVL (kdspkcustom.get_barcode (dcdcinl, dcddcom, 2), ' '),13,' ') f2, " +
                //    "RPAD (SUBSTR (dcdcinl, 1, 20), 20, ' ') f3, ' ' f4, '   ' f5, " +
                //    "RPAD (pkfoudgene.get_CNUF(1,dcdcfin), 7, ' ') f6, LPAD ('0', 19, ' ') f7, " +
                //    "LPAD ('0', 19, ' ') f8,  " +
                //    "LPAD (TO_CHAR (KDSPKCUSTOM.GET_PRICELBL(dcdcinl, dcdsite, TO_DATE(dcddcom, 'DD/MM/RR'))), 19, ' ') f9, " +
                //    "'1' f10, LPAD (KDSPKCUSTOM.GET_QTYLBL(dcdseqvl, dcdsite, dcdqtec, dcdcexcde), 5, ' ') f11, " +
                //    "TO_CHAR (CURRENT_DATE, 'YYYYMMDD') f12, ' 0' f13 " +
                //    "FROM cdedetcde " +
                //    "WHERE dcdcexcde = " + PONumber + " " +
                //    "and exists (select 1 from STODETRE where dcdcincde = SDRCINCDE and SDRSEQVL = dcdseqvl)" +
                //    "UNION ALL " +
                //    "select RPAD (NVL (kdspkcustom.get_barcode (sdrcinla, sdrsdrc, 1), ' '),9,' ') codeOnLabel_F1, " +
                //    "RPAD (NVL (kdspkcustom.get_barcode (sdrcinla, sdrsdrc, 2), ' '),13,' ') f2, " +
                //    "RPAD (SUBSTR (sdrcinla, 1, 20), 20, ' ') f3, ' ' f4, '   ' f5, " +
                //    "RPAD (pkfoudgene.get_CNUF(1,sdrcfin), 7, ' ') f6, LPAD ('0', 19, ' ') f7, " +
                //    "LPAD ('0', 19, ' ') f8,  " +
                //    "LPAD (TO_CHAR (KDSPKCUSTOM.GET_PRICELBL(sdrcinla, sdrsite, TO_DATE(sdrsdrc, 'DD/MM/RR'))), 19, ' ') f9, " +
                //    "'1' f10, LPAD (sdrqteo, 5, ' ') f11, " +
                //    "TO_CHAR (CURRENT_DATE, 'YYYYMMDD') f12, ' 0' f13 " +
                //    "FROM stodetre " +
                //    "WHERE exists  " +
                //    "(SELECT 1 from stoentre  " +
                //    " where sercincde=999999999  " +
                //    "  and SERNUSR=" + PONumber + " " +
                //    "  and SERCINREC=SDRCINREC " +
                //    ") ";

                cmd.CommandText =
                    "select RPAD (NVL (kdspkcustom.get_barcode (dcdcinl, dcddcom, 1), ' '),9,' ') codeOnLabel_F1, " +
                    "RPAD (NVL (kdspkcustom.get_barcode (dcdcinl, dcddcom, 2), ' '),13,' ') f2, " +
                    "RPAD (SUBSTR (PKARTUV.GET_LIBELLE_COURT(1,dcdcinl,'GB'), 1, 20), 20, ' ') f3, " +
                    "' ' f4, '   ' f5, " +
                    "RPAD (pkfoudgene.get_CNUF(1,dcdcfin), 7, ' ') f6, LPAD ('0', 19, ' ') f7, " +
                    "LPAD ('0', 19, ' ') f8,  " +
                    "LPAD (TO_CHAR (KDSPKCUSTOM.GET_PRICELBL(dcdcinl, dcdsite, TO_DATE(dcddcom, 'DD/MM/RR'))), 19, ' ') f9, " +
                    "'1' f10, LPAD (KDSPKCUSTOM.GET_QTYLBL(dcdseqvl, dcdsite, dcdqtec, dcdcexcde), 5, ' ') f11, " +
                    "TO_CHAR (CURRENT_DATE, 'YYYYMMDD') f12, ' 0' f13 " +
                    "FROM cdedetcde " +
                    "WHERE dcdcexcde =  '" + PONumber + "' " +
                    "and DCDETAT >= 5 " +
                    //"and exists (select 1 from STODETRE where dcdcincde = SDRCINCDE and SDRSEQVL = dcdseqvl) " +
                    "UNION ALL " +
                    "select RPAD (NVL (kdspkcustom.get_barcode (sdrcinla, sdrsdrc, 1), ' '),9,' ') codeOnLabel_F1, " +
                    "RPAD (NVL (kdspkcustom.get_barcode (sdrcinla, sdrsdrc, 2), ' '),13,' ') f2, " +
                    "RPAD (SUBSTR (PKARTUV.GET_LIBELLE_COURT(1,sdrcinla,'GB'), 1, 20), 20, ' ') f3, " +
                    "' ' f4, '   ' f5, " +
                    "RPAD (pkfoudgene.get_CNUF(1,sdrcfin), 7, ' ') f6, LPAD ('0', 19, ' ') f7, " +
                    "LPAD ('0', 19, ' ') f8,  " +
                    "LPAD (TO_CHAR (KDSPKCUSTOM.GET_PRICELBL(sdrcinla, sdrsite, TO_DATE(sdrsdrc, 'DD/MM/RR'))), 19, ' ') f9, " +
                    "'1' f10, LPAD (sdrqteo, 5, ' ') f11, " +
                    "TO_CHAR (CURRENT_DATE, 'YYYYMMDD') f12, ' 0' f13 " +
                    "FROM stodetre " +
                    "WHERE exists  " +
                    "(SELECT 1 from stoentre  " +
                    "where sercincde=999999999  " +
                    "and SERNUSR= '" + PONumber + "' " +
                    "and SERCINREC=SDRCINREC " +
                    ")";

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);

                ErrorString = "Success";

                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetFileContentByPONumber Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable GetSTCKBySite(String Site)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                //cmd.CommandText =
                //    "SELECT " +
                //    "rpad(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 1),9,' ') f1, " +
                //    "rpad(nvl(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 2),' '),13,' ') f2, " +
                //    "rpad(substr(PKARTUV.GET_LIBELLE_COURT(1,ARVCINV,'GB'),1,20),20,' ') f3, " +
                //    "' ' f4, " +
                //    "'   ' f5, " +
                //    "rpad(cnuf,7,' ') f6, " +
                //    "lpad('0',19,' ') f7, " +
                //    "lpad('0',19,' ') f8, " +
                //    "lpad(to_char(KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site1, TRUNC(SYSDATE))),19,' ') f9, " +
                //    "'1' f10, " +
                //    "lpad('1',5,' ') f11, " +
                //    "TO_CHAR(SYSDATE, 'YYYYMMDD') f12, " +
                //    "' 0' f13 " +
                //    "FROM " +
                //    "ARTUV, " +
                //    "( " +
                //    "		select " +
                //    "aracinl, " +
                //    "PKFOUDGENE.get_CNUF(1,ARACFIN) cnuf  " +
                //    "from artuc  " +
                //    "where  " +
                //    "	ARATFOU=1  " +
                //    "		and TRUNC(SYSDATE) between araddeb and aradfin " +
                //    "		AND PKFOUDGENE.get_FouType(1,ARACFIN)=1 " +
                //    "	group by " +
                //    "		aracinl, " +
                //    "		PKFOUDGENE.get_CNUF(1,ARACFIN) " +
                //    ") ARTUC " +
                //    "WHERE " +
                //    "	ARVCINV=ARACINL " +
                //    "	AND KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 1) is not null " +
                //    "	AND EXISTS (select 1 from artcaisse where acacinv = ARVCINV and acasite = :site2 and acaacti < 2 and acaprix > 1) " +
                //    "union " +
                //    "SELECT " +
                //    "	rpad(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 1),9,' ') f1, " +
                //    "	rpad(nvl(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 2),' '),13,' ') f2, " +
                //    "	rpad(substr(PKARTUV.GET_LIBELLE_COURT(1,ARVCINV,'GB'),1,20),20,' ') f3, " +
                //    "	' ' f4, " +
                //    "	'   ' f5, " +
                //    "	rpad(cnuf,7,' ') f6, " +
                //    "	lpad('0',19,' ') f7, " +
                //    "	lpad('0',19,' ') f8, " +
                //    "	lpad(to_char(KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site3, TRUNC(SYSDATE + 1))),19,' ') f9, " +
                //    "	'1' f10, " +
                //    "	lpad('1',5,' ') f11, " +
                //    "	TO_CHAR(SYSDATE, 'YYYYMMDD') f12, " +
                //    "	' 0' f13 " +
                //    "FROM " +
                //    "	ARTUV, " +
                //    "	( " +
                //    "		select " +
                //    "			aracinl, " +
                //    "			PKFOUDGENE.get_CNUF(1,ARACFIN) cnuf  " +
                //    "		from artuc  " +
                //    "		where  " +
                //    "			ARATFOU=1  " +
                //    "			and TRUNC(SYSDATE + 1) between araddeb and aradfin " +
                //    "			AND PKFOUDGENE.get_FouType(1,ARACFIN)=1 " +
                //    "		group by " +
                //    "			aracinl, " +
                //    "			PKFOUDGENE.get_CNUF(1,ARACFIN) " +
                //    "	) ARTUC " +
                //    "WHERE " +
                //    "ARVCINV=ARACINL " +
                //    "AND KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 1) is not null " +
                //    "AND KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site4, TRUNC(SYSDATE + 1)) > 1 " +
                //    "AND TRUNC(ARVDCRE) = TRUNC(SYSDATE) ";

                cmd.CommandText = "SELECT " +
                                  "rpad(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 1),9,' ') f1, " +
                                  "rpad(nvl(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 2),' '),13,' ') f2, " +
                                  "rpad(substr(PKARTUV.GET_LIBELLE_COURT(1,ARVCINV,'GB'),1,20),20,' ') f3, " +
                                  "' ' f4, " +
                                  "'   ' f5, " +
                                  "rpad(KDSPKCUSTOM.getCnufLabel(ARVCINV),7,' ') f6, " +
                                  "lpad('0',19,' ') f7, " +
                                  "lpad('0',19,' ') f8, " +
                                  "lpad(to_char(KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site1, TRUNC(SYSDATE))),19,' ') f9, " +
                                  "'1' f10, " +
                                  "lpad('1',5,' ') f11, " +
                                  "TO_CHAR(SYSDATE, 'YYYYMMDD') f12, " +
                                  "' 0' f13 " +
                                  "FROM " +
                                  "ARTUV " +
                                  "WHERE " +
                                  "KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE), 1) is not null " +
                                  "AND EXISTS (select 1 from artcaisse where acacinv = ARVCINV and acasite = :site2 and acaacti < 2 and acaprix > 1) " +
                                  "union " +
                                  "SELECT " +
                                  "rpad(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 1),9,' ') f1, " +
                                  "rpad(nvl(KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 2),' '),13,' ') f2, " +
                                  "rpad(substr(PKARTUV.GET_LIBELLE_COURT(1,ARVCINV,'GB'),1,20),20,' ') f3, " +
                                  "' ' f4, " +
                                  "'   ' f5, " +
                                  "rpad(KDSPKCUSTOM.getCnufLabel(ARVCINV),7,' ') f6, " +
                                  "lpad('0',19,' ') f7, " +
                                  "lpad('0',19,' ') f8, " +
                                  "lpad(to_char(KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site3, TRUNC(SYSDATE + 1))),19,' ') f9, " +
                                  "'1' f10, " +
                                  "lpad('1',5,' ') f11, " +
                                  "TO_CHAR(SYSDATE, 'YYYYMMDD') f12, " +
                                  "' 0' f13 " +
                                  "FROM " +
                                  "ARTUV " +
                                  "WHERE " +
                                  "KDSPKCUSTOM.GET_BARCODE(ARVCINV, TRUNC(SYSDATE + 1), 1) is not null " +
                                  "AND KDSPKCUSTOM.GET_PRICELBL(ARVCINV, :site4, TRUNC(SYSDATE + 1)) > 1 " +
                                  "AND TRUNC(ARVDCRE) = TRUNC(SYSDATE)";

                cmd.Parameters.Add(new OracleParameter(":site1", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":site2", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":site3", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":site4", OracleDbType.Varchar2)).Value = Site;

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);

                ErrorString = "Success";

                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("GetSTCKBySite Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string insertUser(User userInsert)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSUSERSSC (USERNAME, PASSWORD, USERID, STATUS, PROFILEID, CREATEDBY, CREATEDDATE) " +
                    "VALUES ('" + userInsert.Username + "', '" + userInsert.Password + "', '" + userInsert.UserID +
                    "', '" + (Int16) userInsert.Status + "', '" + userInsert.ProfileID + "', '" +
                    GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertUser Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string insertProfile(String ProfileName)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();

                String ProfileID_nextVal = "";
                cmd.Connection = con;

                cmd.CommandText = "select KDSPROFILESSC_SEQ.NEXTVAL as NEXTVAL from dual";
                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ProfileID_nextVal = dr["NEXTVAL"].ToString();
                    ;

                }

                cmd.CommandText = "INSERT INTO KDSPROFILESSC (PROFILEID, PROFILENAME, CREATEDBY, CREATEDDATE) " +
                                  "VALUES ('" + ProfileID_nextVal + "', '" + ProfileName + "' , '" +
                                  GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();





                cmd.CommandText =
                    "INSERT INTO KDSPROFILEMENULINKSSC (MENUID, PROFILEID, CREATEDBY, CREATEDATE) VALUES ('6', '" +
                    ProfileID_nextVal + "', '" + GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();


                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertProfile Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string GetDuplicateProfileName(String ProfileName)
        {
            try
            {

                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();

                String Name = "";
                cmd.Connection = con;

                cmd.CommandText = "SELECT PROFILENAME FROM KDSPROFILESSC WHERE PROFILENAME = '" + ProfileName + "'";


                logger.Debug(cmd.CommandText);


                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Name = dr["PROFILENAME"].ToString();
                    ;

                }


                this.Close();
                return Name;
            }
            catch (Exception e)
            {
                logger.Error("GetDuplicateProfileName Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string updateUser(User userUpdate)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE KDSUSERSSC SET " +
                                  "USERNAME = '" + userUpdate.Username + "', " +
                                  "PASSWORD = '" + userUpdate.Password + "', " +
                                  "PROFILEID = '" + userUpdate.ProfileID + "', " +
                                  "STATUS = " + (int) userUpdate.Status + ", " +
                                  "MODIFIEDBY = '" + GlobalVar.GlobalVarUserID + "', " +
                                  "MODIFIEDDATE = SYSDATE " +
                                  "WHERE USERID = '" + userUpdate.UserID + "'";

                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                ErrorString = "Success";

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("updateUser Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string updatePrintLabelPrintStatus(String PONumber, String POStatus)
        {
            int Curr_Print = 0;
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;


                if (POStatus == "5" || POStatus == "7")
                {
                    cmd.CommandText = "SELECT ECDNSEQ FROM CDEENTCDE WHERE ECDCEXCDE = '" + PONumber + "'";

                    logger.Debug(cmd.CommandText);

                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Curr_Print = String.IsNullOrWhiteSpace(dr["ECDNSEQ"].ToString())
                            ? 0
                            : Int32.Parse(dr["ECDNSEQ"].ToString());

                    }

                    Curr_Print += 1;

                    cmd.CommandText = "UPDATE CDEENTCDE SET ECDNSEQ = " + Curr_Print + " WHERE ECDCEXCDE = '" + PONumber +
                                      "'";
                    cmd.CommandType = CommandType.Text;

                    logger.Debug(cmd.CommandText);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "SELECT SERCCPT FROM STOENTRE WHERE SERNUSR = '" + PONumber + "'";

                    logger.Debug(cmd.CommandText);

                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Curr_Print = String.IsNullOrWhiteSpace(dr["SERCCPT"].ToString())
                            ? 0
                            : Int32.Parse(dr["SERCCPT"].ToString());

                    }

                    Curr_Print += 1;

                    cmd.CommandText = "UPDATE STOENTRE SET SERCCPT = " + Curr_Print + " WHERE SERNUSR = '" + PONumber +
                                      "'";
                    cmd.CommandType = CommandType.Text;

                    logger.Debug(cmd.CommandText);

                    cmd.ExecuteNonQuery();
                }


                cmd.CommandText = "";

                ErrorString = "Success";

                this.Close();
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("updatePrintLabelPrintStatus Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        /// <summary>
        /// Inserts the brand by profile identifier.
        /// </summary>
        /// <param name="BrandID">The brand identifier.</param>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns> Error String</returns>
        public string insertBrandByProfileID(String BrandID, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSPROFILEBRANDLINKSSC (BRANDID, PROFILEID, CREATEDBY, CREATEDDATE) VALUES ('" +
                    BrandID + "', '" + ProfileID + "', '" + GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertBrandByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        /// <summary>
        /// Inserts the site by profile identifier.
        /// </summary>
        /// <param name="BrandID">The brand identifier.</param>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public string insertSiteByProfileID(String SiteID, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSPROFILESITELINKSSC (SITEID, PROFILEID, CREATEDBY, CREATEDDATE) VALUES ('" + SiteID +
                    "', '" + ProfileID + "', '" + GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertSiteByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        /// <summary>
        /// Deletes the brand by profile identifier.
        /// </summary>
        /// <param name="BrandID">The brand identifier.</param>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public string DeleteBrandByProfileID(String BrandID, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM KDSPROFILEBRANDLINKSSC WHERE BRANDID = '" + BrandID +
                                  "' AND Profileid = '" + ProfileID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteBrandByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        /// <summary>
        /// Deletes promo older than 3 months.
        /// </summary>
        /// <returns>System.String.</returns>
        public string DeleteOldUploadPromo()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "delete KDS_INTMIXDETPRO where PRMDCRE <= sysdate-100";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteOldUploadPromo Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public int SearchForExistingTransactionByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                int CountData = 0;
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "SELECT COUNT(1) AS COUNTDATA FROM KDSSALESINPUTSSC " +
                                  "WHERE USERID IN " +
                                  "(SELECT USERID FROM KDSUSERSSC WHERE PROFILEID = '" + ProfileID + "')";

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CountData = Convert.ToInt32(dr["COUNTDATA"].ToString());
                }

                this.Close();
                return CountData;
            }
            catch (Exception e)
            {
                logger.Error("SearchForExistingTransactionByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = e.Message;
                return 0;
            }
        }

        public int CountExistingMenuInProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                int CountData = 0;
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "SELECT COUNT(1) AS COUNTDATA FROM KDSPROFILEMENULINKSSC " +
                                  "WHERE MENUID != 6 " +
                                  "AND PROFILEID = '" + ProfileID + "'";

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    CountData = Convert.ToInt32(dr["COUNTDATA"].ToString());
                }

                this.Close();
                return CountData;
            }
            catch (Exception e)
            {
                logger.Error("CountExistingMenuInProfileID Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = e.Message;
                return 0;
            }
        }

        public string DeleteProfileByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                //cmd.CommandText = "DELETE FROM KDSPROFILEMENULINKSSC " +
                //                  "WHERE MENUID = 6 " +
                //                  "AND PROFILEID = '"+ProfileID+"'";
                //cmd.ExecuteNonQuery();


                cmd.CommandText = "DELETE FROM KDSUSERSSC " +
                                  "WHERE PROFILEID = '" + ProfileID + "'";

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();


                cmd.CommandText = "DELETE FROM KDSPROFILESSC WHERE PROFILEID = '" + ProfileID + "'";
                cmd.ExecuteNonQuery();

                logger.Debug(cmd.CommandText);


                this.Close();
                ErrorString = "Success Deleting Profile";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteProfileByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = e.Message;
                return ErrorString;
            }
        }

        public string insertMenuByProfileID(String MenuId, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSPROFILEMENULINKSSC (MENUID, PROFILEID, CREATEDBY, CREATEDDATE) VALUES ('" + MenuId +
                    "', '" + ProfileID + "', '" + GlobalVar.GlobalVarUserID + "', SYSDATE)";
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertMenuByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string DeleteMenuByProfileID(String MenuID, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM KDSPROFILEMENULINKSSC WHERE MENUID = '" + MenuID + "' AND Profileid = '" +
                                  ProfileID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteMenuByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string DeleteUserByUserID(String UserID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM KDSUSERSSC WHERE USERID = '" + UserID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteUserByUserID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string DeleteSiteByProfileID(String SiteID, String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM KDSPROFILESITELINKSSC WHERE SITEID = '" + SiteID + "' AND Profileid = '" +
                                  ProfileID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("DeleteSiteByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        /// <summary>
        /// Selects the menu by profile identifier.
        /// </summary>
        /// <param name="ProfileID">The profile identifier.</param>
        /// <returns></returns>
        public DataTable SelectMenuByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT MENUID FROM Kdsprofilemenulinkssc WHERE PROFILEID = '" + ProfileID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectMenuByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String SelectDuplicateUserID(String UserID)
        {
            String UserIDReturn = "";
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT USERID FROM KDSUSERSSC WHERE USERID = '" + UserID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    UserIDReturn = (String) dr["USERID"];
                }
                this.Close();
                return UserIDReturn;
            }
            catch (Exception e)
            {
                logger.Error("SelectDuplicateUserID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectAllProfile()
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT PROFILEID, PROFILENAME FROM KDSPROFILESSC";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectAllProfile Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable selectinvkodeH()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " SELECT '' IDKODE,'ALL' KODE from DUAL " +
                                  " UNION ALL " +
                                  " SELECT KODE, KODE  FROM KDSFAKTURPAJAK WHERE STATUS in ('CREATED','CONFIRM EDIT') ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Invoice Filter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable selectinvUsahaH()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " SELECT '' IDPENG,'ALL' IDPENGUSAHA from DUAL " +
                                  " UNION ALL " +
                                  " SELECT IDPENGUSAHA, (SELECT   foulibl from foudgene  where foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA) IDPENGUSAHA FROM KDSFAKTURPAJAK where STATUS in ('CREATED','CONFIRM EDIT') group by IDPENGUSAHA";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Invoice Filter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable selectinvbeliH()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " SELECT '' IDPEM,'ALL' IDPEMBELI from DUAL " +
                                  " UNION ALL " +
                                  "SELECT IDPEMBELI ,(select LONGDESC from KDSPARAM where ID = KDSFAKTURPAJAK.IDPEMBELI) IDPEMBELI FROM KDSFAKTURPAJAK where STATUS in ('CREATED','CONFIRM EDIT') GROUP BY IDPEMBELI ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Invoice Filter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectParameterPajak(String Type)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT  ID, LONGDESC FROM KDSPARAM where PRMVAR1 = '"+ Type +"'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectAll Param Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        

        public DataTable InvoiceData(String Supply)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select efarfou InvNUm FROM CFDENFAC where  nvl(EFAARCH,0) not in ('1','2') and PKFOUDGENE.get_CNUF(1,EFACFIN) IN (select FOUCNUF from foudgene  where FOULIBL = '" + Supply + "')";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectPembeli()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select ID, LONGDESC from KDSPARAM where PRMVAR1 = 'Pembeli'";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectKodeSeriFakturPajak()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select KODE from KDSFAKTURPAJAK";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select SelectKodeSeriFakturPajak");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectSupplier()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT   foucnuf, foulibl from foudgene order by foulibl";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable SelectSupplierSlip()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select '' ID, 'ALL' KET from Dual UNION ALL select * from (SELECT foucnuf as ID, foulibl as KET from foudgene order by foulibl)";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectPembeliSlip()
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select 9999 ID, 'ALL' KET from Dual UNION ALL select ID, LONGDESC as KET from KDSPARAM where PRMVAR1 = 'Pembeli'";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public SupplierPembeli SelectDataManualPenerimaDetailSlip(String ID)
        {
            SupplierPembeli Pengusaha = new SupplierPembeli();

            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT DATATUJUAN, ADPENERIMA, ANTUJUAN, BANKTUJUAN, REKTUJUAN from KDSMSTBAYAR where ID = '" + ID + "'";


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pengusaha.CompanyName = dr["DATATUJUAN"].ToString();
                    Pengusaha.Address = dr["ADPENERIMA"].ToString();
                    Pengusaha.NoRek = dr["REKTUJUAN"].ToString();
                    Pengusaha.AN = dr["ANTUJUAN"].ToString();
                    Pengusaha.Bank = dr["BANKTUJUAN"].ToString();
                }

                this.Close();
                return Pengusaha;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public SupplierPembeli SelectDataPenerimaDetailSlip(String ID, String Rek)
        {
            SupplierPembeli Pengusaha = new SupplierPembeli();           
              
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT " +
                    "fadrais AS CompanyName, " +
                    "fadrue1      AS AddressPengirim, " +
                    //"fadrue2      AS AddressPengirim2, " +
                    //"fadvill      AS KotaPengirim, " +
                    //"fadregn      AS ProvinsiPengirim, " +
                    //"FADIDEN      AS NPWP, " +
                    "decode(nvl(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount) NoRek , " +
                    "kdspkinvoice.get_AN(efaccin) AtasNama, " +
                    //"(select BNKNAME from BANKS where BNKCODE = 'efacbank') Bank " +
                    " (SELECT bbrname FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) Bank " +
                    "FROM fouadres, foudgene, cfdenfac " +
                    "WHERE foudgene.foucnuf = '" + ID + "' and fouadres.FADCFIN    = foudgene.foucfin and cfdenfac.EFACFIN = foudgene.foucfin and cfdenfac.EFARFOU " +
                    "in (Select SKUID from kdstrxinvoice where IDH in (select IDH from kdsfakturpajak where IDPENGUSAHA = '" + ID + "')) " +
                    " and decode(nvl(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount) = '" + Rek + "' " +
                    "Group By foudgene.foucfin, " +
                    "fadrais, fadrue1, fadrue2, fadvill, fadregn, FADIDEN, EFACBRANCH,  " +
                    "efaccin,efaacount, efacbank, EFARFOU";

                
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                 while (dr.Read())
                {
                    Pengusaha.CompanyName = dr["CompanyName"].ToString();
                    Pengusaha.Address = dr["AddressPengirim"].ToString();                   
                    Pengusaha.NoRek = dr["NoRek"].ToString();
                    Pengusaha.AN = dr["AtasNama"].ToString();
                    Pengusaha.Bank = dr["Bank"].ToString();
                }

                this.Close();
                return Pengusaha;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable SelectDataPenerimaSlip(String ID)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT " +
                    //"fadrais AS CompanyName, " +
                    //"fadrue1      AS AddressPengirim, " +
                    //"fadrue2      AS AddressPengirim2, " +
                    //"fadvill      AS KotaPengirim, " +
                    //"fadregn      AS ProvinsiPengirim, " +
                    //"FADIDEN      AS NPWP, " +
                    " NoRek from (SELECT nvl(DECODE(NVL(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount),'Not Found') NoRek " +
                    //"kdspkinvoice.get_AN(efaccin) AtasNama, " + 
                    //"(select BNKNAME from BANKS where BNKCODE = 'efacbank') Bank " +
                    "FROM fouadres, foudgene, cfdenfac " +
                    "WHERE foudgene.foucnuf = '" + ID + "' and fouadres.FADCFIN    = foudgene.foucfin and cfdenfac.EFACFIN = foudgene.foucfin and cfdenfac.EFARFOU " +
                    "in (Select SKUID from kdstrxinvoice where IDH in (select IDH from kdsfakturpajak where IDPENGUSAHA = '" + ID + "')) " +
                    "Group By foudgene.foucfin, " +
                    //"fadrais, fadrue1, fadrue2, fadvill, fadregn, FADIDEN,  "+
                    "efaccin,efaacount, efacbank, EFARFOU )  RData GROUP BY NoRek";


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectInvoiceDetailDetail(String SupplierCode, String InvoiceNo)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select " +
                                  "kdspkconsignment.get_Coca(kctmcinl, kctmdmvt) Barcode, " +
                                  "kctmbrand Brand, " +
                                  "pkartuv.get_libelle_long(1,kctmcinl,'GB') LongDesc, " +
                                  "trim(to_char(sum(kctmqty),'999G999G999G999')) Qty, " +
                                  "trim(to_char(sum(kctmgsls),'999G999G999G999D99')) Penjualan, " +
                                  "decode(sum(decode(kctmmotf,50,0,51,0,54,0,56,0,kctndisvalamt)),0,'',trim(to_char(sum(kctndisvalamt),'999G999G999G999D99'))) DiscCust, " +
                                  "decode(sum(decode(kctmmotf,50,0,51,0,54,0,56,0,kctmmara)),0,'',trim(to_char(sum(kctmgsls)-sum(kctndisvalamt),'999G999G999G999D99'))) NetSales, " +
                                  "decode(kctmmarg,0,'',to_char(kctmmarg,'99D99')) MarginPct, " +
                                  "decode(sum(kctmmara),0,'',trim(to_char(sum(kctmmara),'999G999G999G999D99'))) MarginRp, " +
                                  "decode(kctmpart,0,'',to_char(kctmpart,'99D99')) DiscPct, " +
                                  "decode(sum(kctmmarp),0,'',trim(to_char(sum(kctmmarp),'999G999G999G999D99'))) DiscRp, " +
                                  "decode(sum(kctmnvpa),0,'',trim(to_char(sum(-kctmnvpa),'999G999G999G999D99'))) Total " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode " +
                                  "and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kdspkconsignment.get_Coca(kctmcinl, kctmdmvt), pkartuv.get_libelle_long(1,kctmcinl,'GB') " +
                                  "order by kctmbrand, kdspkconsignment.get_Coca(kctmcinl, kctmdmvt)";

                cmd.Parameters.Add(new OracleParameter(":SupplierCode", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo", OracleDbType.Varchar2)).Value = InvoiceNo;

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectInvoiceDetailSummary(String SupplierCode, String InvoiceNo)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select " +
                                  "to_char(min(kctmdmvt),'dd-MON')||' - '|| to_char(max(kctmdmvt),'dd-MON')||decode(nvl(kctndisvalpct,0),0,'',' ('||to_char(kctndisvalpct)||'%)') Keterangan, " +
                                  "kctmbrand Brand, " +
                                  "decode(sum(kctmgsls),0,'',trim(to_char(sum(kctmgsls),'999G999G999G999D99'))) Penjualan, " +
                                  "decode(kctmmarg,0,'',to_char(kctmmarg,'99D99')) MarginPct, " +
                                  "decode(sum(kctmmara),0,'',trim(to_char(sum(kctmmara),'999G999G999G999D99'))) MarginRp, " +
                                  "decode(kctmpart,0,'',to_char(kctmpart,'99D99')) DiscPct, " +
                                  "decode(sum(kctmmarp),0,'',trim(to_char(sum(kctmmarp),'999G999G999G999D99'))) DiscRp, " +
                                  "trim(to_char(sum(-kctmnvpa),'999G999G999G999D99')) Total " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode and kctmmotf!=55 " +
                                  "and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kctndisvalpct " +
                                  "union all " +
                                  "select " +
                                  "to_char(min(kctmdmvt),'dd-MON')||' - '|| to_char(min(kctmdmvt),'dd-MON')||' HSP' Keterangan, " +
                                  "kctmbrand Brand, " +
                                  "decode(sum(kctmgsls),0,'',trim(to_char(sum(kctmgsls),'999G999G999G999D99'))) Penjualan, " +
                                  "decode(kctmmarg,0,'',to_char(kctmmarg,'99D99')) MarginPct, " +
                                  "decode(sum(kctmmara),0,'',trim(to_char(sum(kctmmara),'999G999G999G999D99'))) MarginRp, " +
                                  "decode(kctmpart,0,'',to_char(kctmpart,'99D99')) DiscPct, " +
                                  "decode(sum(kctmmarp),0,'',trim(to_char(sum(kctmmarp),'999G999G999G999D99'))) DiscRp, " +
                                  "trim(to_char(sum(-kctmnvpa),'999G999G999G999D99')) Total " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode2 and kctmmotf=55 " +
                                  "and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo2,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kctndisvalpct";

                cmd.Parameters.Add(new OracleParameter(":SupplierCode", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo", OracleDbType.Varchar2)).Value = InvoiceNo;
                cmd.Parameters.Add(new OracleParameter(":SupplierCode2", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo2", OracleDbType.Varchar2)).Value = InvoiceNo;

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select All Supplier");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumInvoiceDetailDetail(String SupplierCode, String InvoiceNo)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select sum(total) as TOTAL from ( " +
                                  "select " +
                                  "       sum(-kctmnvpa) as TOTAL " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode " +
                                  "       and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kdspkconsignment.get_Coca(kctmcinl, kctmdmvt), pkartuv.get_libelle_long(1,kctmcinl,'GB') " +
                                  "order by kctmbrand, kdspkconsignment.get_Coca(kctmcinl, kctmdmvt))";

                cmd.Parameters.Add(new OracleParameter(":SupplierCode", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo", OracleDbType.Varchar2)).Value = InvoiceNo;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = String.IsNullOrWhiteSpace(dr["TOTAL"].ToString())
                        ? (decimal?) null
                        : decimal.Parse(dr["TOTAL"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumInvoiceDetailDetail Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumInvoiceDetailSummary(String SupplierCode, String InvoiceNo)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select sum(total) as TOTAL from ( " +
                                  "select " +
                                  "sum(-kctmnvpa) as  total " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode and kctmmotf!=55 " +
                                  "and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kctndisvalpct " +
                                  "union all " +
                                  "select " +
                                  "sum(-kctmnvpa) as  Total " +
                                  "from   kdscnsmvt " +
                                  "where  pkfoudgene.get_cnuf(1,KCTMCFIN)=:SupplierCode2 and kctmmotf=55 " +
                                  "and KDSPKINVOICE.CEK_DETAILINV(:InvoiceNo2,kctminvnum,kctmsite,kctmccin)=1 " +
                                  "group by kctmmarg, kctmpart, kctmbrand, kctndisvalpct)";

                cmd.Parameters.Add(new OracleParameter(":SupplierCode", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo", OracleDbType.Varchar2)).Value = InvoiceNo;
                cmd.Parameters.Add(new OracleParameter(":SupplierCode2", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":InvoiceNo2", OracleDbType.Varchar2)).Value = InvoiceNo;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = String.IsNullOrWhiteSpace(dr["TOTAL"].ToString())
                        ? (decimal?)null
                        : decimal.Parse(dr["TOTAL"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumInvoiceDetailDetail Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? HistorySumFakturPajak(String IDH)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select SUM(BRUTO) as Total from HKDSTRXINVOICE where IDH = :IDH";

                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumFakturPajak(String IDH)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select SUM(BRUTO) as Total from KDSTRXINVOICE where IDH = :IDH";

                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumExpenseFakturPajak(String IDH)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select nvl(SUM(VALUE),0) as Total from KDSTRXEXPINVOICE where IDD IN (select IDD from KDSTRXINVOICE where IDH =  :IDH)";

                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    String A = dr["Total"].ToString();
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumExpenseFakturPajakHistorical(String IDH)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select nvl(SUM(VALUE),0) as Total from HKDSTRXEXPINVOICE where IDD IN (select IDD from HKDSTRXINVOICE where IDH =  :IDH)";

                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public decimal? SumExpenseTrxTotalInvoice(String IDD)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select SUM(BRUTO) as Total from KDSTRXINVOICE where IDD = :IDD";

                cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumExpenseTrxTotalInvoiceHistorical(String IDD)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select SUM(BRUTO) as Total from HKDSTRXEXPINVOICE where IDD = :IDD";

                cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Total = Decimal.Parse(dr["Total"].ToString());
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseTrxInvoiceHistorical Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumExpenseTrxInvoice(String IDD)
        {
            decimal? Total = null;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select nvl(SUM(VALUE),0) as Total from KDSTRXEXPINVOICE where IDD = :IDD";

                cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {

                    string ABC = dr["Total"].ToString();
                        Total = Decimal.Parse(dr["Total"].ToString());
                    
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseFakturPajak Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public decimal? SumExpenseTrxInvoiceHistorical(String IDD)
        {
            decimal? Total = 0;
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select nvl(SUM(VALUE),0) as Total from HKDSTRXEXPINVOICE where IDD = :IDD";

                cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    
                        Total = Decimal.Parse(dr["Total"].ToString());
                    
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Total;
            }
            catch (Exception e)
            {
                logger.Error("SumExpenseTrxInvoiceHistorical Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvoiceDetailDetailHeader SelectInvoiceDetailDetailHeader(String Site)
        {
            InvoiceDetailDetailHeader header = new InvoiceDetailDetailHeader();
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT " +
                    "'OUTLET : '||pksitdgene.get_sitedescription(1,:site) as Outlet, " +
                    "upper(adrrue1)||' '||adrvill as Address " +
                    "from cliadres " +
                    "where adrncli=:Site and adradre=1";

                cmd.Parameters.Add(new OracleParameter(":Site", OracleDbType.Varchar2)).Value = Site;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    header.OUTLET = (String)dr["Outlet"];
                    header.ADDRESS = (String)dr["Address"];
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return header;
            }
            catch (Exception e)
            {
                logger.Error("SelectInvoiceDetailSummaryHeader Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvoiceDetailDetailHeaderData SelectInvoiceDetailDetailHeaderData(DateTime StartDate, DateTime EndDate, String Site, String SupplierCode)
        {
            InvoiceDetailDetailHeaderData headerData = new InvoiceDetailDetailHeaderData();
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT pkfoudgene.get_DescriptionCnuf(1,:SupplierCode) as SupplierName, " +
                    "to_char(to_date(:startDate,'DD/MM/YY'),'dd-MON-YY')||' to '||to_char(to_date(:endDate,'DD/MM/YY'),'dd-MON-YY') as PeriodePenjualan, " +
                    "pksitdgene.get_sitedescription(1,:Site)||' - '||upper(adrrue1) as Cabang, " +
                    "upper(adrrue1), " +
                    "adrvill " +
                    "from cliadres " +
                    "where adrncli=:Site and adradre=1";

                cmd.Parameters.Add(new OracleParameter(":SupplierCode", OracleDbType.Varchar2)).Value = SupplierCode;
                cmd.Parameters.Add(new OracleParameter(":startDate", OracleDbType.Date)).Value = StartDate;
                cmd.Parameters.Add(new OracleParameter(":endDate", OracleDbType.Date)).Value = EndDate;
                cmd.Parameters.Add(new OracleParameter(":Site1", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":Site2", OracleDbType.Varchar2)).Value = Site;

                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    headerData.Cabang = (String)dr["Cabang"];
                    headerData.PeriodePenjualan = (String)dr["PeriodePenjualan"];
                    headerData.SupplierName = (String)dr["SupplierName"];

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return headerData;
            }
            catch (Exception e)
            {
                logger.Error("SelectInvoiceDetailSummaryHeader Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable SelectFakturPajakConfirmSlip(FakturPajakSearch FPSearch)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    " SELECT  KDSMSTBAYAR.ID  , Description, foudgene.foulibl as Pengusaha,PENGUSAHA as IDPENGUSAHA, KDSPARAM.LongDesc as Pembeli, PEMBELI as IDPEMBELI, StartDate, EndDate, Total, nvl(BIAYA,'0')  as TRANSFER , Total - nvl(BIAYA,'0') as TotalAkhir, " +
                    " RekTujuan, ANTujuan, BankTujuan, DataPengirim, NPWP,MANUAL from KDSMSTBAYAR, foudgene, KDSPARAM where "+
                    " foucnuf = PENGUSAHA and PRMVAR1 = 'Pembeli' and KDSPARAM.ID = KDSMSTBAYAR.PEMBELI" +
                    " and STARTDATE >= :FromDate " +
                    " and ENDDATE <= :ToDate and KDSMSTBAYAR.CREATEDBY = '" + GlobalVar.GlobalVarUsername + "' ";

                cmd.Parameters.Add(new OracleParameter(":FromDate", OracleDbType.Date)).Value = FPSearch.StartDate.Date;
                cmd.Parameters.Add(new OracleParameter(":ToDate", OracleDbType.Date)).Value = FPSearch.EndDate.Date;


                if (!string.IsNullOrWhiteSpace(FPSearch.IDPENGUSAHA))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and PENGUSAHA = :PENGUSAHA ";
                    cmd.Parameters.Add(new OracleParameter(":PENGUSAHA", OracleDbType.Varchar2)).Value =
                        FPSearch.IDPENGUSAHA;
                }

                if (!string.IsNullOrWhiteSpace(FPSearch.IDPEMBELI))
                {
                    cmd.CommandText = cmd.CommandText +
                                       " and PEMBELI = :PEMBELI ";
                    cmd.Parameters.Add(new OracleParameter(":PEMBELI", OracleDbType.Varchar2)).Value =
                        FPSearch.IDPEMBELI;
                }

                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select KDS Master Bayar");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public DataTable SelectFakturPajakConfirm(FakturPajakSearch FPSearch, bool IncludeHist)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT Kode, foudgene.foulibl as Pengusaha, KDSPARAM.LongDesc as Pembeli, StartDate, EndDate, Status " +
                    "from KDSFAKTURPAJAK " +
                    "inner join foudgene on foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA " +
                    "inner join KDSPARAM on ID = KDSFAKTURPAJAK.IDPEMBELI " +
                    "where STARTDATE >= :FromDate " +
                    "and ENDDATE <= :ToDate and KDSFAKTURPAJAK.CREATEDBY = '" + GlobalVar.GlobalVarUsername + "' " +
                    "and (STATUS = 'CONFIRM' " +
                    "or STATUS = 'CONFIRM EDIT') ";

                cmd.Parameters.Add(new OracleParameter(":FromDate", OracleDbType.Date)).Value = FPSearch.StartDate.Date;
                cmd.Parameters.Add(new OracleParameter(":ToDate", OracleDbType.Date)).Value = FPSearch.EndDate.Date;

                if (!string.IsNullOrWhiteSpace(FPSearch.KODE))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and KODE like '%' || :Kode  || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":Kode", OracleDbType.Varchar2)).Value = FPSearch.KODE;
                }

                if (!string.IsNullOrWhiteSpace(FPSearch.IDPENGUSAHA))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and foulibl like '%' || :PENGUSAHA || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":PENGUSAHA", OracleDbType.Varchar2)).Value =
                        FPSearch.IDPENGUSAHA;
                }

                if (!string.IsNullOrWhiteSpace(FPSearch.IDPEMBELI))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and LongDesc like '%' || :PEMBELI || '%'  ";
                    cmd.Parameters.Add(new OracleParameter(":PEMBELI", OracleDbType.Varchar2)).Value =
                        FPSearch.IDPEMBELI;
                }

                if (!string.IsNullOrWhiteSpace(FPSearch.InvoiceNo))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and exists(select 1 from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH and SKUID like '%' || :Invoice || '%' )";
                    cmd.Parameters.Add(new OracleParameter(":Invoice", OracleDbType.Varchar2)).Value =
                        FPSearch.InvoiceNo;
                }

                if (IncludeHist)
                {
                    cmd.CommandText = cmd.CommandText +
                                      "union all " +
                                      "SELECT Kode, foudgene.foulibl as Pengusaha, KDSPARAM.LongDesc as Pembeli, StartDate, EndDate, Status " +
                                      "from HKDSFAKTURPAJAK " +
                                      "inner join foudgene on foucnuf = HKDSFAKTURPAJAK.IDPENGUSAHA " +
                                      "inner join KDSPARAM on ID = HKDSFAKTURPAJAK.IDPEMBELI " +
                                      "where STARTDATE >= :FromDate2 " +
                                      "and ENDDATE <= :ToDate2 ";

                    cmd.Parameters.Add(new OracleParameter(":FromDate2", OracleDbType.Date)).Value = FPSearch.StartDate;
                    cmd.Parameters.Add(new OracleParameter(":ToDate2", OracleDbType.Date)).Value = FPSearch.EndDate;

                    if (!string.IsNullOrWhiteSpace(FPSearch.KODE))
                    {
                        cmd.CommandText = cmd.CommandText +
                                          "and KODE like '%' || :Kode2  || '%' ";
                        cmd.Parameters.Add(new OracleParameter(":Kode2", OracleDbType.Varchar2)).Value = FPSearch.KODE;
                    }

                    if (!string.IsNullOrWhiteSpace(FPSearch.IDPENGUSAHA))
                    {
                        cmd.CommandText = cmd.CommandText +
                                          "and foulibl like '%' || :PENGUSAHA2 || '%' ";
                        cmd.Parameters.Add(new OracleParameter(":PENGUSAHA2", OracleDbType.Varchar2)).Value =
                            FPSearch.IDPENGUSAHA;
                    }

                    if (!string.IsNullOrWhiteSpace(FPSearch.IDPEMBELI))
                    {
                        cmd.CommandText = cmd.CommandText +
                                          "and LongDesc like '%' || :PEMBELI2 || '%'  ";
                        cmd.Parameters.Add(new OracleParameter(":PEMBELI2", OracleDbType.Varchar2)).Value =
                            FPSearch.IDPEMBELI;
                    }

                    if (!string.IsNullOrWhiteSpace(FPSearch.InvoiceNo))
                    {
                        cmd.CommandText = cmd.CommandText +
                                          "and exists(select 1 from HKDSTRXINVOICE where IDH = HKDSFAKTURPAJAK.IDH and SKUID like '%' || :Invoice2 || '%' )";
                        cmd.Parameters.Add(new OracleParameter(":Invoice2", OracleDbType.Varchar2)).Value =
                            FPSearch.InvoiceNo;
                    }
                }



            cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select FakturPajak");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

       public DataTable selectTrxInvoiceByKODEForReport(String KODE)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select SKUID, " +
                                  "TO_CHAR ((BRUTO - nvl((SELECT SUM(VALUE) FROM KDSTRXEXPINVOICE WHERE IDD = KDSTRXINVOICE.IDD),0)), '999,999,999,999.99') as total " +
                                  "from KDSTRXINVOICE " +
                                  "where IDH = (SELECT IDH FROM KDSFAKTURPAJAK WHERE KODE = :KODE)";

                cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select selectTrxInvoiceByID");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

       public DataTable selectTrxInvoiceByKODEForReportHistory(String KODE)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select SKUID, " +
                                 "TO_CHAR ((BRUTO - nvl((SELECT SUM(VALUE) FROM HKDSTRXEXPINVOICE WHERE IDD = HKDSTRXINVOICE.IDD),0)), '999,999,999,999.99') as total " +
                                 "from HKDSTRXINVOICE " +
                                 "where IDH = (SELECT IDH FROM HKDSFAKTURPAJAK WHERE KODE = :KODE)";

               cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectTrxInvoiceByID");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectTrxInvoiceByKODE(String KODE)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText =
                   "select SKUID, " +
                   "TO_CHAR (BRUTO, '999,999,999,999.99') as BRUTO, " +
                  // "TO_CHAR (NETTO, '999,999,999,999.99') as NETTO, " +
                   "COMMENTDETAIL as \"COMMENT DETAIL\" " +
                  // "TO_CHAR (DISCBRUTO, '999,999,999,999.99') as \"DISCOUNT BRUTO\", " +
                  // "TO_CHAR (DISCNETTO, '999,999,999,999.99') as \"DISCOUNT NETTO\" " +
                   "from kdstrxinvoice " +
                   "where  " +
                   "idh = (SELECT IDH FROM KDSFAKTURPAJAK WHERE KODE = :KODE)";

               cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectTrxInvoiceByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectTrxInvoiceByKODEHistory(String KODE)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText =
                   "select SKUID, " +
                   "TO_CHAR (BRUTO, '999,999,999,999.99') as BRUTO, " +
                  // "TO_CHAR (NETTO, '999,999,999,999.99') as NETTO, " +
                   "COMMENTDETAIL as \"COMMENT DETAIL\" " +
                 //  "TO_CHAR (DISCBRUTO, '999,999,999,999.99') as \"DISCOUNT BRUTO\", " +
                  // "TO_CHAR (DISCNETTO, '999,999,999,999.99') as \"DISCOUNT NETTO\" " +
                   "from Hkdstrxinvoice " +
                   "where  " +
                   "idh = (SELECT IDH FROM HKDSFAKTURPAJAK WHERE KODE = :KODE)";

               cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectTrxInvoiceByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectExpenseFakturPajakByKODE(String KODE)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select KDSPARAM.LONGDESC as KETERANGAN, " +
                                 "TO_CHAR (KDSTRXEXPINVOICE.VALUE, '999,999,999,999.99') as TOTAL " +
                                 "from KDSTRXEXPINVOICE inner join KDSPARAM on KDSPARAM.id = KDSTRXEXPINVOICE.PARAMID " +
                                 "where IDD in (SELECT IDD FROM KDSTRXINVOICE, KDSFAKTURPAJAK WHERE KDSFAKTURPAJAK.KODE = :KODE and KDSFAKTURPAJAK.IDH = KDSTRXINVOICE.IDH)";

               cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectExpenseFakturPajakByKODEHistorical(String KODE)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select KDSPARAM.LONGDESC as KETERANGAN, " +
                                "TO_CHAR (HKDSTRXEXPINVOICE.VALUE, '999,999,999,999.99') as TOTAL " +
                                "from HKDSTRXEXPINVOICE inner join KDSPARAM on KDSPARAM.id = HKDSTRXEXPINVOICE.PARAMID " +
                                "where IDD in (SELECT IDD FROM HKDSTRXINVOICE, HKDSFAKTURPAJAK WHERE HKDSFAKTURPAJAK.KODE = :KODE and HKDSFAKTURPAJAK.IDH = HKDSTRXINVOICE.IDH)";

               cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectExpenseFakturPajakByIDD(String IDD)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = " select KDSPARAM.LONGDESC as KETERANGAN, " +
                                 "TO_CHAR (KDSTRXEXPINVOICE.VALUE, '999G999G999G999D99') as TOTAL " +
                                 "from KDSTRXEXPINVOICE inner join KDSPARAM on KDSPARAM.id = KDSTRXEXPINVOICE.PARAMID " +
                                 "where IDD = :IDD";

               cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public DataTable selectExpenseFakturPajakByIDDHistorical(String IDD)
       {
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = " select KDSPARAM.LONGDESC as KETERANGAN, " +
                                 "TO_CHAR (HKDSTRXEXPINVOICE.VALUE, '999G999G999G999D99') as TOTAL " +
                                 "from HKDSTRXEXPINVOICE inner join KDSPARAM on KDSPARAM.id = HKDSTRXEXPINVOICE.PARAMID " +
                                 "where IDD = :IDD";

               cmd.Parameters.Add(new OracleParameter(":IDD", OracleDbType.Varchar2)).Value = IDD;


               cmd.CommandType = CommandType.Text;
               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               DataTable DT = new DataTable();
               DT.Load(dr);
               this.Close();
               return DT;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public ExpenseFakturPajak selectExpenseFakturPajakTotalTerbilangByKODE(String KODE)
       {
           ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select " +
                                 "TO_CHAR (sum(value), '999,999,999,999.99') as TOTAL, " +
                                 "kdspkinvoice.terbilang_indo(sum(value)) as TERBILANG " +
                                 "from KDSTRXEXPINVOICE " +
                                 "where IDD in (SELECT IDD FROM KDSTRXINVOICE WHERE IDH = :Code)";

               cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;

               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               while (dr.Read())
               {

                   expenseFP.Total = dr["TOTAL"].ToString();
                   expenseFP.Terbilang = dr["TERBILANG"].ToString();
               }

               this.Close();
               return expenseFP;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public ExpenseFakturPajak selectExpenseFakturPajakTotalTerbilangByKODEHistorical(String KODE)
       {
           ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select " +
                                 "TO_CHAR (sum(value), '999,999,999,999.99') as TOTAL, " +
                                 "kdspkinvoice.terbilang_indo(sum(value)) as TERBILANG " +
                                 "from HKDSTRXEXPINVOICE " +
                                 "where IDD in (SELECT IDD FROM HKDSTRXINVOICE WHERE IDH = :Code)";

               cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;

               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               while (dr.Read())
               {

                   expenseFP.Total = dr["TOTAL"].ToString();
                   expenseFP.Terbilang = dr["TERBILANG"].ToString();
               }

               this.Close();
               return expenseFP;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }


       public ExpenseFakturPajak selectExpenseFakturPajakTotalTerbilangByKODEPembayaran(String KODE)
       {
           ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select TO_CHAR ((BRUTO- nvl(Jml,0) - nvl(jmlheader,0)) , '999,999,999,999.99') as TOTAL, " +
                                "kdspkinvoice.terbilang_indo((BRUTO - nvl(Jml,0)-nvl(jmlheader,0)) ) as TERBILANG  " +
                                "from ( select (select SUM(BRUTO) from KDSTRXINVOICE where IDH = :Code) BRUTO, (select nvl(sum(value),0) from KDSEXPENSEFAKTURPAJAK where IDH = :Code) jmlheader, " +
                                "(select nvl(sum(value),0) from KDSTRXEXPINVOICE where IDD in ( select IDD from KDSTRXINVOICE where IDH = :Code)) Jml from dual) a";                                


               cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;

               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               while (dr.Read())
               {

                   expenseFP.Total = dr["TOTAL"].ToString();
                   expenseFP.Terbilang = dr["TERBILANG"].ToString();
               }

               this.Close();
               return expenseFP;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }

       public ExpenseFakturPajak selectExpenseFakturPajakTotalTerbilangByKODEPembayaranHistorical(String KODE)
       {
           ExpenseFakturPajak expenseFP = new ExpenseFakturPajak();
           try
           {
               this.Connect();
               OracleCommand cmd = new OracleCommand();
               cmd.Connection = con;
               cmd.CommandText = "select TO_CHAR ((BRUTO- nvl(Jml,0) - nvl(jmlheader,0)) , '999,999,999,999.99') as TOTAL, " +
                                "kdspkinvoice.terbilang_indo((BRUTO - nvl(Jml,0)-nvl(jmlheader,0)) ) as TERBILANG  " +
                                "from ( select (select SUM(BRUTO) from HKDSTRXINVOICE where IDH = :Code) BRUTO, (select nvl(sum(value),0) from HKDSEXPENSEFAKTURPAJAK where IDH = :Code) jmlheader, " +                                 
                                "(select sum(value) from HKDSTRXEXPINVOICE where IDD in ( select IDD from HKDSTRXINVOICE where IDH = :Code)) Jml from dual) a";                                


               cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = KODE;


               cmd.CommandType = CommandType.Text;

               logger.Debug(cmd.CommandText);

               OracleDataReader dr = cmd.ExecuteReader();

               while (dr.Read())
               {

                   expenseFP.Total = dr["TOTAL"].ToString();
                   expenseFP.Terbilang = dr["TERBILANG"].ToString();
               }

               this.Close();
               return expenseFP;
           }
           catch (Exception e)
           {
               logger.Error("Select selectExpenseFakturPajakByKODE");
               logger.Error(e.Message);
               this.Close();
               return null;
           }
       }


        public DataTable selectCommentFakturPajakByKODE(String KODE)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select COMMENTHEADER, COMMENTFOOTER " +
                                  "from KDSFAKTURPAJAK " +
                                  "where IDH = (SELECT IDH FROM KDSFAKTURPAJAK WHERE KODE = :KODE)";

                cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select selectTrxInvoiceByID");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable selectCommentFakturPajakByKODEHistorical(String KODE)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select COMMENTHEADER, COMMENTFOOTER " +
                                  "from HKDSFAKTURPAJAK " +
                                  "where IDH = (SELECT IDH FROM HKDSFAKTURPAJAK WHERE KODE = :KODE)";

                cmd.Parameters.Add(new OracleParameter(":KODE", OracleDbType.Varchar2)).Value = KODE;


                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select selectTrxInvoiceByID");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public FakturPajakSearch SelectFakturPajakByCode(String Code)
        {
            FakturPajakSearch FPSearch = new FakturPajakSearch();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select KODE, IDPENGUSAHA, " +
                                  "IDPEMBELI, STATUS, COMMENTHEADER, COMMENTFOOTER, STARTDATE, ENDDATE , MODIFIEDDATE " +
                                  "from kdsfakturpajak " +
                                  "where KODE = :Code";

                cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = Code;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {

                    FPSearch.KODE = dr["KODE"].ToString();
                    FPSearch.IDPENGUSAHA = dr["IDPENGUSAHA"].ToString();
                    FPSearch.IDPEMBELI = dr["IDPEMBELI"].ToString();
                    FPSearch.STATUS = dr["STATUS"].ToString();
                    FPSearch.COMMENTHEADER = dr["COMMENTHEADER"].ToString();
                    FPSearch.COMMENTFOOTER = dr["COMMENTFOOTER"].ToString();
                    FPSearch.StartDate = DateTime.Parse(dr["STARTDATE"].ToString());
                    FPSearch.EndDate = DateTime.Parse(dr["ENDDATE"].ToString());
                    FPSearch.LastModified = DateTime.Parse(dr["MODIFIEDDATE"].ToString());
                }

                this.Close();
                return FPSearch;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectFakturPajakByCode Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public FakturPajakSearch SelectFakturPajakByCodeHistorical(String Code)
        {
            FakturPajakSearch FPSearch = new FakturPajakSearch();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select KODE, IDPENGUSAHA, " +
                                  "IDPEMBELI, STATUS, COMMENTHEADER, COMMENTFOOTER, STARTDATE, ENDDATE , MODIFIEDDATE " +
                                  "from hkdsfakturpajak " +
                                  "where KODE = :Code";

                cmd.Parameters.Add(new OracleParameter(":Code", OracleDbType.Varchar2)).Value = Code;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {

                    FPSearch.KODE = dr["KODE"].ToString();
                    FPSearch.IDPENGUSAHA = dr["IDPENGUSAHA"].ToString();
                    FPSearch.IDPEMBELI = dr["IDPEMBELI"].ToString();
                    FPSearch.STATUS = dr["STATUS"].ToString();
                    FPSearch.COMMENTHEADER = dr["COMMENTHEADER"].ToString();
                    FPSearch.COMMENTFOOTER = dr["COMMENTFOOTER"].ToString();
                    FPSearch.StartDate = DateTime.Parse(dr["STARTDATE"].ToString());
                    FPSearch.EndDate = DateTime.Parse(dr["ENDDATE"].ToString());
                    FPSearch.LastModified = DateTime.Parse(dr["MODIFIEDDATE"].ToString());
                }

                this.Close();
                return FPSearch;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectFakturPajakByCode Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public SupplierPembeli SelectSupplierByID(String ID, String IDH)
        {
            SupplierPembeli Pengusaha = new SupplierPembeli();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT fadrais AS CompanyName, " +
                    "fadrue1      AS AddressPengirim, " +
                    "fadrue2      AS AddressPengirim2, " +
                    "fadvill      AS KotaPengirim, " +
                    "fadregn      AS ProvinsiPengirim, " +
                    "FADIDEN      AS NPWP, " +
                    "decode(nvl(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount) NoRek, " +
                    "kdspkinvoice.get_AN(efaccin) AtasNama, " +
                    " (SELECT bbrname FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) Bank, " +
                    " (SELECT Substr(bbradr1 || bbradr2 , 1,50) FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) BankAddress " +
                    "FROM fouadres, foudgene, cfdenfac " +
                    "WHERE foudgene.foucnuf = :ID and fouadres.FADCFIN    = foudgene.foucfin and cfdenfac.EFACFIN = foudgene.foucfin and cfdenfac.EFARFOU " +
                    "in (Select SKUID from kdstrxinvoice where IDH = :IDH) " +
                    "Group By foudgene.foucfin, fadrais, fadrue1, fadrue2, fadvill, fadregn, FADIDEN, efaccin,efaacount, efacbank, EFARFOU , EFACBRANCH";                    

                cmd.Parameters.Add(new OracleParameter(":ID", OracleDbType.Varchar2)).Value = ID;
                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pengusaha.CompanyName = dr["CompanyName"].ToString();
                    Pengusaha.Address = dr["AddressPengirim"].ToString();
                    Pengusaha.NPWP = dr["NPWP"].ToString();
                    Pengusaha.NoRek = dr["NoRek"].ToString();
                    Pengusaha.AN = dr["AtasNama"].ToString();
                    Pengusaha.Bank = dr["Bank"].ToString();
                    Pengusaha.BankAddress = dr["BankAddress"].ToString();
                }

                this.Close();
                return Pengusaha;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPengusahaByID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public SupplierPembeli SelectSupplierByIDSlip(String ID, String Kode)
        {
            SupplierPembeli Pengusaha = new SupplierPembeli();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT fadrais AS CompanyName, " +
                    "fadrue1      AS AddressPengirim, " +
                    "fadrue2      AS AddressPengirim2, " +
                    "fadvill      AS KotaPengirim, " +
                    "fadregn      AS ProvinsiPengirim, " +
                    "FADIDEN      AS NPWP, " +
                    "decode(nvl(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount) NoRek, " +
                    "kdspkinvoice.get_AN(efaccin) AtasNama, " +
                    //"(select BNKNAME from BANKS where BNKCODE = efacbank) Bank " +
                    " (SELECT bbrname FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) Bank " +
                    "FROM fouadres, foudgene, cfdenfac " +
                    "WHERE foudgene.foucnuf = :ID and fouadres.FADCFIN    = foudgene.foucfin and cfdenfac.EFACFIN = foudgene.foucfin and cfdenfac.EFARFOU " +
                    "in (Select SKUID from kdstrxinvoice where IDH in (select IDH from kdsfakturpajak where kode in (" + Kode + "))) " +
                    "Group By foudgene.foucfin, fadrais, fadrue1, fadrue2, fadvill, fadregn, FADIDEN, efaccin,efaacount, efacbank,EFACBRANCH, EFARFOU ";

                cmd.Parameters.Add(new OracleParameter(":ID", OracleDbType.Varchar2)).Value = ID;
             //   cmd.Parameters.Add(new OracleParameter(":Kode", OracleDbType.Varchar2)).Value = Kode;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pengusaha.CompanyName = dr["CompanyName"].ToString();
                    Pengusaha.Address = dr["AddressPengirim"].ToString();
                    Pengusaha.NPWP = dr["NPWP"].ToString();
                    Pengusaha.NoRek = dr["NoRek"].ToString();
                    Pengusaha.AN = dr["AtasNama"].ToString();
                    Pengusaha.Bank = dr["Bank"].ToString();
                }

                this.Close();
                return Pengusaha;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPengusahaByID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public SupplierPembeli HistorySelectSupplierByID(String ID, String IDH)
        {
            SupplierPembeli Pengusaha = new SupplierPembeli();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT fadrais AS CompanyName, " +
                    "fadrue1      AS AddressPengirim, " +
                    "fadrue2      AS AddressPengirim2, " +
                    "fadvill      AS KotaPengirim, " +
                    "fadregn      AS ProvinsiPengirim, " +
                    "FADIDEN      AS NPWP, " +
                    "decode(nvl(efaacount,'0'),'0',kdspkinvoice.get_NoRek(efaccin),efaacount) NoRek, " +
                    "kdspkinvoice.get_AN(efaccin) AtasNama, " +
                     " (SELECT bbrname FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) Bank, " +
                    " (SELECT Substr(bbradr1 || bbradr2 , 1,50) FROM bankbranch  WHERE  bbrcbranch = EFACBRANCH) BankAddress " +
                    "FROM fouadres, foudgene, cfdenfac " +
                    "WHERE foudgene.foucnuf = :ID and fouadres.FADCFIN    = foudgene.foucfin and cfdenfac.EFACFIN = foudgene.foucfin and cfdenfac.EFARFOU " +
                    "in (Select SKUID from Hkdstrxinvoice where IDH = :IDH) " +
                    "Group By foudgene.foucfin, fadrais, fadrue1, fadrue2, fadvill, fadregn, FADIDEN, EFACBRANCH, efaccin,efaacount, efacbank, EFARFOU ";

                cmd.Parameters.Add(new OracleParameter(":ID", OracleDbType.Varchar2)).Value = ID;
                cmd.Parameters.Add(new OracleParameter(":IDH", OracleDbType.Varchar2)).Value = IDH;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pengusaha.CompanyName = dr["CompanyName"].ToString();
                    Pengusaha.Address = dr["AddressPengirim"].ToString();
                    Pengusaha.NPWP = dr["NPWP"].ToString();
                    Pengusaha.NoRek = dr["NoRek"].ToString();
                    Pengusaha.AN = dr["AtasNama"].ToString();
                    Pengusaha.Bank = dr["Bank"].ToString();
                    Pengusaha.BankAddress = dr["BankAddress"].ToString();
                }

                this.Close();
                return Pengusaha;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPengusahaByID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public FakturPajakSearch PrintSlipPembayaran(String ID)
        {
            
            try
            {
                FakturPajakSearch Faktur = new FakturPajakSearch();
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    " select ID,DESCRIPTION,PEMBELI,PENGUSAHA,Total - nvl(BIAYA,'0') as TOTAL2, kdspkinvoice.terbilang_indo(Total - nvl(BIAYA,'0')) TERBILANG, " +
                    " DATATUJUAN,ANTUJUAN,BANKTUJUAN,DATAPENGIRIM,ADPENGIRIM, " +
                    " NPWP,STARTDATE,ENDDATE,ADPENERIMA,REKTUJUAN, MODIFIEDDATE FROM KDSMSTBAYAR WHERE ID = '"+ ID +"'";                

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Faktur.KODE = dr["DESCRIPTION"].ToString();                
                    Faktur.Total = dr["TOTAL2"].ToString();
                    Faktur.DataPenerima = dr["DATATUJUAN"].ToString();
                    Faktur.ANPenerima = dr["ANTUJUAN"].ToString();
                    Faktur.BankPenerima = dr["BANKTUJUAN"].ToString();
                    Faktur.DataPengirim = dr["DATAPENGIRIM"].ToString();
                    Faktur.AdPengirim = dr["ADPENGIRIM"].ToString();
                    Faktur.NPWP = dr["NPWP"].ToString();
                    Faktur.StartDate = DateTime.Parse(dr["STARTDATE"].ToString());
                    Faktur.EndDate = DateTime.Parse(dr["ENDDATE"].ToString());
                    Faktur.AdPenerima = dr["ADPENERIMA"].ToString();
                    Faktur.NoRekPenerima = dr["REKTUJUAN"].ToString();
                    Faktur.TotalTerbilang = dr["TERBILANG"].ToString();
                    Faktur.LastModified = DateTime.Parse(dr["MODIFIEDDATE"].ToString());
                }

                this.Close();
                return Faktur;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPengusahaByID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public SupplierPembeli SelectPembeliByID(String ID)
        {
            SupplierPembeli Pembeli = new SupplierPembeli();
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "select LONGDESC as CompanyName, " +
                    "PRMVAR2 as NPWP, " +
                    "PRMVAR3 as Address " +
                    "from kdsParam " +
                    "where id = :ID";

                cmd.Parameters.Add(new OracleParameter(":ID", OracleDbType.Varchar2)).Value = ID;


                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Pembeli.CompanyName = dr["CompanyName"].ToString();
                    Pembeli.Address = dr["Address"].ToString();
                    Pembeli.NPWP = dr["NPWP"].ToString();
                }

                this.Close();
                return Pembeli;
            }
            catch (Exception e)
            {
                logger.Error(
                    "SelectPengusahaByID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectAllUser()
        {
            try
            {
                this.ConnectLocal();
                logger.Debug("Start Select All User");
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT KDSUSERSSC.USERID, KDSUSERSSC.USERNAME, " +
                                  "DECODE(KDSUSERSSC.STATUS, 1, 'Active', " +
                                  "2, 'Frozen', " +
                                  "3, 'Delete', " +
                                  "'Unknown Code') STATUS, " +
                                  "KDSPROFILESSC.PROFILENAME " +
                                  "FROM KDSUSERSSC, KDSPROFILESSC " +
                                  "WHERE KDSUSERSSC.PROFILEID = KDSPROFILESSC.PROFILEID";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectAllUser Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public User SelectUserByUserID(String UserID)
        {
            try
            {
                this.ConnectLocal();

                logger.Debug("Start SelectUserByUserID");
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " SELECT * FROM KDSUSERSSC WHERE USERID = '" + UserID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                User user = new User();

                while (dr.Read())
                {
                    user.UserID = (String) dr["USERID"];
                    user.Username = (String) dr["USERNAME"];
                    user.Password = (String) dr["PASSWORD"];
                    user.ProfileID = (String) dr["PROFILEID"];
                    user.Status = (User.UserStatus) ((Int16) dr["STATUS"]);
                }


                this.Close();
                return user;
            }
            catch (Exception e)
            {
                logger.Error("SelectUserByUserID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }


        public String getProfileNameByProfileID(String ProfileID)
        {
            try
            {
                this.ConnectLocal();
                logger.Debug("Start getProfileNameByProfileID");
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " SELECT PROFILENAME FROM KDSPROFILESSC WHERE PROFILEID = '" + ProfileID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                String ProfileName = "";

                while (dr.Read())
                {
                    ProfileName = (String) dr["PROFILENAME"];
                }

                this.Close();
                return ProfileName;
            }
            catch (Exception e)
            {
                logger.Error("getProfileNameByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String getImageFooterMemoDiscount()
        {
            try
            {
                return ConfigurationManager.AppSettings["ImageFooterMemoDiscount"];
            }
            catch (Exception e)
            {
                logger.Error("getImageFooterMemoDiscount Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String getNextValUploadPromo()
        {
            String NextVal = "";

            try
            {
                this.Connect();
                logger.Debug("Start getNextValUploadPromo");

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select PKUPLOADPROMO.get_Sequence as NEXTVAL from dual";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    NextVal = dr["NEXTVAL"].ToString();

                }
                this.Close();
                return NextVal;
            }
            catch (Exception e)
            {
                logger.Error("getNextValUploadPromo Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string insertUploadPromo(UploadPromo uploadPromo)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Upload Promo");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDS_INTMIXDETPRO (PRMSEQN, PRMTYPE, PRMNUMB, PRMCEXR, PRMCEXV, PRMTILL, PRMDESC, PRMPRIX, PRMACTI, PRMUTIL, PRMFTRT, PRMNERR, PRMMESS, PRMDCRE, PRMDMAJ) VALUES " +
                    "('" + uploadPromo.Sequence + "', " +
                    "'" + uploadPromo.Reference + "', " +
                    "'" + uploadPromo.Number + "', " +
                    "'" + uploadPromo.Article + "', " +
                    "'" + uploadPromo.SU + "', " +
                    "'" + uploadPromo.TILLCode + "', " +
                    "'" + uploadPromo.SUDescription + "', " +
                    "'" + uploadPromo.HSP + "', " +
                    "'" + uploadPromo.ActionCode + "', " +
                    "'" + GlobalVar.GlobalVarUserID + "', " +
                    "'0', " +
                    "'', " +
                    "'', " +
                    "SYSDATE, " +
                    "SYSDATE)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Uploading Data";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertUploadPromo Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string processUploadInvent(UploadInventory uploadInvent)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Upload Inventory");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = "INSERT INTO intinv " +
                                  "(IVFCEXINV, IVFSITE, IVFTINV, IVFLIBL, IVFDINV, IVFTPOS, IVFCODE, IVFQTER, IVFPDSINV, IVFEMPL, IVFNORDRE, IVFORIGCEXINV, IVFLGFI, IVFTRT, IVFDTRT, IVFDCRE, IVFDMAJ, IVFUTIL, IVFFICH, IVFNLIG, IVFNERR, IVFMESS, IVFCEXV, IVFPV, IVFIDSTR, IVFNODE, IVFNLIS, IVFCACT, IVFGRPS, IVFMODE, IVFCEXVL, IVFNPORT, IVFDCPPREV) " +
                                  "VALUES " +
                                  "(:IVFCEXINV,:IVFSITE,:IVFTINV,:IVFLIBL,:IVFDINV,:IVFTPOS,:IVFCODE,:IVFQTER,:IVFPDSINV,:IVFEMPL,:IVFNORDRE,:IVFORIGCEXINV,:IVFLGFI,:IVFTRT,:IVFDTRT,:IVFDCRE,:IVFDMAJ,:IVFUTIL,:IVFFICH,:IVFNLIG,:IVFNERR,:IVFMESS," +
                                  "(DECODE(:IVFCEXV, '', (SELECT DISTINCT " +
                                  "PKARTUV.GET_ARVCEXV(1, ARCCINV) cexv  " +
                                  "FROM " +
                                  "ARTCOCA,  " +
                                  "ARTUL " +
                                  "WHERE  " +
                                  "ARCCINR = ARUCINR " +
                                  "AND ARCCINV = ARUCINL " +
                                  "AND TRUNC(SYSDATE) BETWEEN TRUNC(ARCDDEB) AND TRUNC(ARCDFIN) " +
                                  "AND ARCETAT = 1 " +
                                  "AND ARCCODE = :IVFCODE1), " +
                                  ":IVFCEXV1) " +
                                  ")," +
                                  ":IVFPV,:IVFIDSTR,:IVFNODE,:IVFNLIS,:IVFCACT,:IVFGRPS,:IVFMODE," +
                                  "(DECODE(:IVFCEXVL, '', (SELECT DISTINCT " +
                                  "PKARTVL.GET_ARLCEXVL(1, ARUSEQVL) cexvl  " +
                                  "FROM " +
                                  "ARTCOCA,  " +
                                  "ARTUL " +
                                  "WHERE  " +
                                  "ARCCINR = ARUCINR " +
                                  "AND ARCCINV = ARUCINL " +
                                  "AND TRUNC(SYSDATE) BETWEEN TRUNC(ARCDDEB) AND TRUNC(ARCDFIN) " +
                                  "AND ARCETAT = 1 " +
                                  "AND ARCCODE = :IVFCODE2), " +
                                  ":IVFCEXVL1) " +
                                  "), " +
                                  ":IVFNPORT,:IVFDCPPREV)";




                cmd.Parameters.Add(":IVFCEXINV", OracleDbType.Varchar2).Value = uploadInvent.IVFCEXINV;
                cmd.Parameters.Add(":IVFSITE", OracleDbType.Varchar2).Value = uploadInvent.IVFSITE;
                cmd.Parameters.Add(":IVFTINV", OracleDbType.Varchar2).Value = uploadInvent.IVFTINV;
                cmd.Parameters.Add(":IVFLIBL", OracleDbType.Varchar2).Value = uploadInvent.IVFLIBL;
                cmd.Parameters.Add(":IVFDINV", OracleDbType.Date).Value = uploadInvent.IVFDINV;
                cmd.Parameters.Add(":IVFTPOS", OracleDbType.Varchar2).Value = uploadInvent.IVFTPOS;
                cmd.Parameters.Add(":IVFCODE", OracleDbType.Varchar2).Value = uploadInvent.IVFCODE;
                cmd.Parameters.Add(":IVFQTER", OracleDbType.Varchar2).Value = uploadInvent.IVFQTER;
                cmd.Parameters.Add(":IVFPDSINV", OracleDbType.Varchar2).Value = uploadInvent.IVFPDSINV;
                cmd.Parameters.Add(":IVFEMPL", OracleDbType.Varchar2).Value = uploadInvent.IVFEMPL;
                cmd.Parameters.Add(":IVFNORDRE", OracleDbType.Varchar2).Value = uploadInvent.IVFNORDRE;
                cmd.Parameters.Add(":IVFORIGCEXINV", OracleDbType.Varchar2).Value = uploadInvent.IVFORIGCEXINV;
                cmd.Parameters.Add(":IVFLGFI", OracleDbType.Varchar2).Value = uploadInvent.IVFLGFI;
                cmd.Parameters.Add(":IVFTRT", OracleDbType.Varchar2).Value = uploadInvent.IVFTRT;
                cmd.Parameters.Add(":IVFDTRT", OracleDbType.Date).Value = uploadInvent.IVFDTRT;
                cmd.Parameters.Add(":IVFDCRE", OracleDbType.Date).Value = uploadInvent.IVFDCRE;
                cmd.Parameters.Add(":IVFDMAJ", OracleDbType.Date).Value = uploadInvent.IVFDMAJ;
                cmd.Parameters.Add(":IVFUTIL", OracleDbType.Varchar2).Value = uploadInvent.IVFUTIL;
                cmd.Parameters.Add(":IVFFICH", OracleDbType.Varchar2).Value = uploadInvent.IVFFICH;
                cmd.Parameters.Add(":IVFNLIG", OracleDbType.Varchar2).Value = uploadInvent.IVFNLIG;
                cmd.Parameters.Add(":IVFNERR", OracleDbType.Varchar2).Value = uploadInvent.IVFNERR;
                cmd.Parameters.Add(":IVFMESS", OracleDbType.Varchar2).Value = uploadInvent.IVFMESS;
                cmd.Parameters.Add(":IVFCEXV", OracleDbType.Int32).Value = uploadInvent.IVFCEXV;
                cmd.Parameters.Add(":IVFCODE1", OracleDbType.Varchar2).Value = uploadInvent.IVFCODE;
                cmd.Parameters.Add(":IVFCEXV1", OracleDbType.Int32).Value = uploadInvent.IVFCEXV;
                cmd.Parameters.Add(":IVFPV", OracleDbType.Varchar2).Value = uploadInvent.IVFPV;
                cmd.Parameters.Add(":IVFIDSTR", OracleDbType.Varchar2).Value = uploadInvent.IVFIDSTR;
                cmd.Parameters.Add(":IVFNODE", OracleDbType.Varchar2).Value = uploadInvent.IVFNODE;
                cmd.Parameters.Add(":IVFNLIS", OracleDbType.Varchar2).Value = uploadInvent.IVFNLIS;
                cmd.Parameters.Add(":IVFCACT", OracleDbType.Varchar2).Value = uploadInvent.IVFCACT;
                cmd.Parameters.Add(":IVFGRPS", OracleDbType.Varchar2).Value = uploadInvent.IVFGRPS;
                cmd.Parameters.Add(":IVFMODE", OracleDbType.Varchar2).Value = uploadInvent.IVFMODE;
                cmd.Parameters.Add(":IVFCEXVL", OracleDbType.Int32).Value = uploadInvent.IVFCEXVL;
                cmd.Parameters.Add(":IVFCODE2", OracleDbType.Varchar2).Value = uploadInvent.IVFCODE;
                cmd.Parameters.Add(":IVFCEXVL1", OracleDbType.Int32).Value = uploadInvent.IVFCEXVL;
                cmd.Parameters.Add(":IVFNPORT", OracleDbType.Varchar2).Value = uploadInvent.IVFNPORT;
                cmd.Parameters.Add(":IVFDCPPREV", OracleDbType.Varchar2).Value = uploadInvent.IVFDCPPREV;

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Uploading Data";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("processUploadInvent Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }



        public string deleteFromINTINV(String IVFCEXINV)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start delete From INTINV with IVFCEXINV : " + IVFCEXINV);
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = "DELETE FROM intinv WHERE IVFCEXINV = :IVFCEXINV";

                cmd.Parameters.Add(":IVFCEXINV", OracleDbType.Varchar2).Value = IVFCEXINV;

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Uploading Data";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("deleteFromINTINV Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public DataTable SelectUploadPromoBySeqNumber(string sequence)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT " +
                                  "PRMTYPE AS \"PROMO REFERENCE\", " +
                                  "PRMNUMB AS \"PROMO NUMBER\", " +
                                  "PRMCEXR AS \"ARTICLE CODE\", " +
                                  "PRMCEXV AS \"SU\", " +
                                  "PRMTILL AS \"TILL CODE\", " +
                                  "PRMDESC AS \"SU DESCRIPTION\", " +
                                  "PRMPRIX AS \"HSP\", " +
                                  "PRMACTI AS \"ACTION CODE\", " +
                                  "PRMNERR AS \"ERROR NUMBER\", " +
                                  "PRMMESS AS \"ERROR MESSAGE\" " +
                                  "FROM KDS_INTMIXDETPRO " +
                                  "WHERE PRMSEQN = '" + sequence + "' " +
                                  "AND PRMFTRT = 2";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectUploadPromoBySeqNumber Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public DataTable SelectUploadPromoBySeqNumberAndStatus(string sequence, bool AllData)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT " +
                                  "PRMTYPE AS \"PROMO REFERENCE\", " +
                                  "PRMNUMB AS \"PROMO NUMBER\", " +
                                  "PRMCEXR AS \"ARTICLE CODE\", " +
                                  "PRMCEXV AS \"SU\", " +
                                  "PRMTILL AS \"TILL CODE\", " +
                                  "PRMDESC AS \"SU DESCRIPTION\", " +
                                  "PRMPRIX AS \"HSP\", " +
                                  "PRMACTI AS \"ACTION CODE\", " +
                                  "PRMNERR AS \"ERROR NUMBER\", " +
                                  "PRMMESS AS \"ERROR MESSAGE\" " +
                                  "FROM KDS_INTMIXDETPRO " +
                                  "WHERE PRMSEQN LIKE '%" + sequence + "%' ";


                if (!AllData)
                {
                    cmd.CommandText = cmd.CommandText +
                                      "AND PRMFTRT = 2";
                }

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectUploadPromoBySeqNumberAndStatus Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public String validateUploadPromo(string sequence)
        {
            String NextVal = "";
            this.Connect();
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "PKUPLOADPROMO.proc_Upload";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("sequence", OracleDbType.Int32).Value = sequence;
                cmd.Parameters.Add("out-number", OracleDbType.Int32).Direction = ParameterDirection.Output;

                logger.Debug(cmd.CommandText);


                cmd.ExecuteNonQuery();

                NextVal = cmd.Parameters["out-number"].Value.ToString();
                this.Close();
                return NextVal;
            }
            catch (Exception e)
            {
                logger.Error("validateUploadPromo Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public Boolean CheckInvNumCentral(String InvNum, String Site)
        {
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "select 1 from inventinv " +
                    "where " +
                    "einsite = :SITE " +
                    "and einmode = 4 " +
                    "and EINCININV = :INVNUM " +
                    "and exists(select 1 from invetape where ietcininv = eincininv and ietsite = einsite and ietneta = 2 and IETPASS = 1) " +
                    "and exists(select 1 from invetape where ietcininv = eincininv and ietsite = einsite and ietneta = 3 and IETPASS = 0) ";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                
                logger.Debug("End Close Connection");


                if (dr.HasRows)
                {
                    this.Close();
                    return true;
                }
                this.Close();
                return false;
            }
            catch (Exception e)
            {
                logger.Error("CheckInvNumCentral Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public Boolean CheckInvNumLocal(String InvNum, String Site)
        {
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "select 1 from KDSHEADSTOCKTAKESSC " +
                    "where " +
                    "SITECODE = :SITE " +
                    "and INVNUM = :INVNUM";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                logger.Debug("End Close Connection");


                if (dr.HasRows)
                {
                    this.Close();
                    return true;
                }
                this.Close();
                return false;
            }
            catch (Exception e)
            {
                logger.Error("CheckInvNumLocal Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }

        }

        public Boolean DownloadInventoryHeader(String InvNum, String Site)
        {
            User user = new User();
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "insert into KDSHEADSTOCKTAKESSC " +
                    "(SITECODE, INVNUM, INVTYPE, INVMODE,  DESCRIPTION, INVDATE) " +
                    "select  " +
                    "  EINSITE, " +
                    "  EINCININV, " +
                    "  EINTINV, " +
                    "  EINMODE, " +
                    "  EINLIBL, " +
                    "  EINDINV " +
                    "from  " +
                    "  inventinv@LINK2SSC " +
                    "where " +
                    "  EINMODE=4 " +
                    "  AND einsite = :SITE  " +
                    "  AND EINCININV = :INVNUM  " +
                    "  and exists(select 1 from invetape@LINK2SSC where ietcininv = eincininv and ietsite = einsite and ietneta = 2 and IETPASS = 1) " +
                    " and exists(select 1 from invetape@LINK2SSC where ietcininv = eincininv and ietsite = einsite and ietneta = 3 and IETPASS = 0)";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");


                return true;
            }
            catch (Exception e)
            {
                logger.Error("DownloadInventoryHeader Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }

        }

        public Boolean DownloadInventoryDetail(String InvNum, String Site)
        {
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "insert into KDSDETSTOCKTAKESSC " +
                    "(ID, BARCODE, CINV, SITECODE, FLAG, INVNUM, DESCRIPTION, INVDATE) " +
                    "select " +
                    "KDSDETSTOCKTAKESSC_SEQ.NEXTVAL, DINCODE, DINCINL, DINSITE, 1, DINCININV, PKARTUV.GET_LIBELLE_COURT@LINK2SSC(1, DINCINL,'GB'), DINDINV " +
                    "from " +
                    "  invdetinv@LINK2SSC " +
                    "where DINSITE = :SITE " +
                    "  AND DINCININV = :INVNUM";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");

                return true;
            }
            catch (Exception e)
            {
                logger.Error("DownloadInventoryDetail Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }

        }

        public Boolean CheckStockTake(String InvNum, String Site, String Barcode)
        {
            User user = new User();
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT 1 FROM KDSDETSTOCKTAKESSC " +
                    "WHERE BARCODE = :BARCODE " +
                    "AND INVNUM = :INVNUM " +
                    "AND SITECODE = :SITE";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":BARCODE", OracleDbType.Varchar2).Value = Barcode;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;
                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                
                

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                logger.Debug("End Close Connection");


                if (dr.HasRows)
                {
                    this.Close();
                    return true;
                }
                this.Close();
                return false;
            }
            catch (Exception e)
            {
                logger.Error("CheckStockTake Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public Boolean CheckStockTakeDuplicateBarcode(String InvNum, String Site, String Barcode)
        {
            User user = new User();
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                    "SELECT 1 FROM KDSDETSTOCKTAKESSC " +
                    "WHERE BARCODE = :BARCODE " +
                    "AND INVNUM = :INVNUM " +
                    "AND SITECODE = :SITE";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":BARCODE", OracleDbType.Varchar2).Value = Barcode;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;
                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;



                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                logger.Debug("End Close Connection");


                if (dr.HasRows)
                {
                    this.Close();
                    return true;
                }
                this.Close();
                return false;
            }
            catch (Exception e)
            {
                logger.Error("CheckStockTake Function");
                logger.Error(e.Message);
                this.Close();
                return false;
            }
        }

        public DataTable SelectDetailStockTake(String Site, String Barcode, String InvNum)
        {
            try
            {
                this.ConnectLocal();

                logger.Debug("Start SelectDetailStockTake");
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT BARCODE, DESCRIPTION,  1 AS QTY FROM KDSDETSTOCKTAKESSC " +
                                  "WHERE BARCODE = :BARCODE " +
                                  "AND INVNUM = :INVNUM " +
                                  "AND SITECODE = :SITE";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":BARCODE", OracleDbType.Varchar2).Value = Barcode;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;
                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;

                logger.Debug(cmd.CommandText);

               
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;

            }
            catch (Exception e)
            {
                logger.Error("SelectDetailStockTake Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public string insertStockTake(String Site, String InvNum, StockTake stockTake, String Hostname, String user, String Location)
        {
            String ErrorString;
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText =
                     "INSERT INTO KDSSTOCKTAKESSC " +
                     "(ID, BARCODE, CREATEDBY, MODIFIEDBY, HOSTNAME, CREATEDDATE, MODIFIEDDATE, QUANTITY, CINV, SITECODE, LOCATION, FLAG, INVNUM, DESCRIPTION, INVDATE, INVTYPE ) " +
                     "SELECT " +
                     "KDSSTOCKTAKESSC_SEQ.NEXTVAL, :BARCODE, :USERID, :USERID2, :HOSTNAME, SYSDATE, SYSDATE, :QTY, CINV, :SITE, :LOCATION, 1, :INVNUM, :DESCRIPTION, :INVDATE, :INVTYPE " +
                     "FROM KDSDETSTOCKTAKESSC " +
                     "WHERE SITECODE = :SITE2 " +
                     "AND INVNUM = :INVNUM2 " +
                     "AND BARCODE = :BARCODE2";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(":BARCODE", OracleDbType.Varchar2).Value = stockTake.Barcode;
                cmd.Parameters.Add(":USERID", OracleDbType.Varchar2).Value = user;
                cmd.Parameters.Add(":USERID2", OracleDbType.Varchar2).Value = user;
                cmd.Parameters.Add(":HOSTNAME", OracleDbType.Varchar2).Value = Hostname;
                cmd.Parameters.Add(":QTY", OracleDbType.Varchar2).Value = stockTake.Qty;
                cmd.Parameters.Add(":SITE", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":LOCATION", OracleDbType.Varchar2).Value = Location;
                cmd.Parameters.Add(":INVNUM", OracleDbType.Varchar2).Value = InvNum;
                cmd.Parameters.Add(":DESCRIPTION", OracleDbType.Varchar2).Value = stockTake.Description;
                cmd.Parameters.Add(":INVDATE", OracleDbType.Date).Value = stockTake.InvDate;
                cmd.Parameters.Add(":INVTYPE", OracleDbType.Int32).Value = stockTake.Type;
                cmd.Parameters.Add(":SITE2", OracleDbType.Varchar2).Value = Site;
                cmd.Parameters.Add(":INVNUM2", OracleDbType.Varchar2).Value = InvNum;
                cmd.Parameters.Add(":BARCODE2", OracleDbType.Varchar2).Value = stockTake.Barcode;
                

                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insertSalesInput Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        

        public DataTable SelectStockTake(StockTake stockTake, String Site, String Hostname, DateTime From, DateTime To, String Location, String InvNum, Boolean isServer)
        {
            try
            {
                if (isServer)
                {
                    this.Connect();    
                }
                else
                {
                    this.ConnectLocal();
                }
                
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "SELECT BARCODE, " +
                    "LOCATION, " +
                    "INVNUM AS \"INVENTORY NUMBER\", " +
                    "DESCRIPTION, " +
                    "CREATEDBY, " +
                    "QUANTITY, " +
                    "SITECODE AS SITE, " +
                    "HOSTNAME, " +
                    "TO_CHAR(createddate, 'DD-MON-YYYY') as CREATEDDATE, " +
                    "TO_CHAR(CREATEDDATE, 'HH24:MI:SS') as TIME " +
                    "FROM KDSSTOCKTAKESSC " +
                    "WHERE TRUNC (to_date(:FROMDATE,'dd/mm/yy')) <= TRUNC(CREATEDDATE) " +
                    "AND TRUNC(CREATEDDATE) <= TRUNC (to_date(:TODATE, 'dd/mm/yy')) " +
                    "and Location = :LOCATION " + 
                    "and INVNUM = :INVNUM " +
                    "and SITECODE = :SITE ";
                

                cmd.Parameters.Add(new OracleParameter(":FROMDATE", OracleDbType.Date)).Value = From;
                cmd.Parameters.Add(new OracleParameter(":TODATE", OracleDbType.Date)).Value = To;
                cmd.Parameters.Add(new OracleParameter(":LOCATION", OracleDbType.Varchar2)).Value = Location;
                cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;
                cmd.Parameters.Add(new OracleParameter(":SITE", OracleDbType.Varchar2)).Value = Site;

                if (isServer)
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and FLAG = 1 ";
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and BARCODE like '%' || :BARCODE || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = stockTake.Barcode;
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Description))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and DESCRIPTION like '%' || :DESC || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":DESC", OracleDbType.Varchar2)).Value = stockTake.Description;
                }

                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and HOSTNAME like '%' || :HOSTNAME || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":HOSTNAME", OracleDbType.Varchar2)).Value = Hostname;
                }


                //cmd.Parameters.Add(new OracleParameter(":ProfileId", OracleDbType.Varchar2)).Value = ProfileID;





                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();


                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectStockTake Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public String UploadStockTakeToUSSCServer(StockTake stockTake, String Site, String Hostname, DateTime From, DateTime To, String Location, String InvNum)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "INSERT INTO KDSSTOCKTAKESSC@LINK2USSCSERVER " +
                    "(ID, CREATEDBY ,MODIFIEDBY, HOSTNAME, CREATEDDATE, MODIFIEDDATE, QUANTITY, CINV, SITECODE, LOCATION, FLAG, INVNUM, DESCRIPTION, BARCODE, INVDATE, INVTYPE ) " +
                    "SELECT " +
                    "KDSSTOCKTAKESSC_SEQ.NEXTVAL@LINK2USSCSERVER, CREATEDBY ,MODIFIEDBY, HOSTNAME, CREATEDDATE, MODIFIEDDATE, QUANTITY, CINV, SITECODE, LOCATION, FLAG, INVNUM, DESCRIPTION, BARCODE, INVDATE, INVTYPE " +
                    "FROM KDSSTOCKTAKESSC " +
                    "WHERE TRUNC (to_date(:FROMDATE,'dd/mm/yy')) <= TRUNC(CREATEDDATE) " +
                    "AND TRUNC(CREATEDDATE) <= TRUNC (to_date(:TODATE, 'dd/mm/yy')) ";

                cmd.Parameters.Add(new OracleParameter(":FROMDATE", OracleDbType.Date)).Value = From;
                cmd.Parameters.Add(new OracleParameter(":TODATE", OracleDbType.Date)).Value = To;

                if (!string.IsNullOrWhiteSpace(stockTake.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and BARCODE like '%' || :BARCODE || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = stockTake.Barcode;
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Description))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and DESCRIPTION like '%' || :DESC || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":DESC", OracleDbType.Varchar2)).Value = stockTake.Description;
                }

                if (!string.IsNullOrWhiteSpace(Location))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and Location like '%' || :LOCATION || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":LOCATION", OracleDbType.Varchar2)).Value = Location;
                }

                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and HOSTNAME like '%' || :HOSTNAME || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":HOSTNAME", OracleDbType.Varchar2)).Value = Hostname;
                }

                if (!string.IsNullOrWhiteSpace(InvNum))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and INVNUM like '%' || :INVNUM || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;
                }

                //cmd.Parameters.Add(new OracleParameter(":ProfileId", OracleDbType.Varchar2)).Value = ProfileID;



                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("UploadStockTakeToUSSCServer Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = "Failed";
                return ErrorString;
            }

        }

        public String UploadToINTINVFromUSSCServer(StockTake stockTake, String Site, String Hostname, DateTime From, DateTime To, String Location, String InvNum)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "INSERT INTO INTINV " +
                    "(IVFTRT, IVFLGFI, IVFSITE, IVFCEXINV, IVFNLIG, IVFDTRT, IVFUTIL, IVFCACT, IVFDMAJ, IVFDCRE, IVFFICH, IVFLIBL, IVFCODE, IVFQTER, IVFEMPL, IVFDINV, IVFNORDRE, IVFTINV, IVFTPOS, IVFMODE, IVFORIGCEXINV ) " +
                    "SELECT  0 AS IVFTRT, " +
                    "ABC + ROWNUM AS IVFLGFI, " +
                    "SITECODE AS IVFSITE, " +
                    "INVNUM AS IVFCEXINV, " +
                    "ABC2 + ROWNUM AS IVFNLIG, " +
                    "CURRENT_DATE AS IVFDTRT, " +
                    "CREATEDBY AS IVFUTIL, " +
                    "3 AS IVFCACT, " +
                    "CURRENT_DATE AS IVFDMAJ, " +
                    "CURRENT_DATE AS IVFDCRE, " +
                    "(SUBSTR(INVNUM||CURRENT_DATE, 0, 50)) AS IVFFICH, " +
                    "DESCRIPTION AS IVFLIBL, " +
                    "BARCODE AS IVFCODE, " +
                    "QTY AS IVFQTER, " +
                    "LOCATION AS IVFEMPL, " +
                    //"INVDATE AS IVFDINV, " +
                    "CURRENT_DATE AS IVFDINV, " +
                    "ABC2 + ROWNUM AS IVFNORDRE, " +
                    "INVTYPE AS IVFTINV," +
                    "0 AS IVFTPOS, " +
                    "4 AS IVFMODE, " +
                    "1 AS IVFORIGCEXINV " +
                    "FROM ( " +
                    "SELECT " +
                    "SITECODE, " +
                    "INVNUM, " +
                    "NVL((SELECT MAX(IVFLGFI) FROM  INTINV WHERE IVFCEXINV = INVNUM), 0) AS ABC, " +
                    "NVL((SELECT MAX(IVFNLIG) FROM  INTINV WHERE IVFCEXINV = INVNUM), 0) AS ABC2, " +
                    " CREATEDBY,  DESCRIPTION, BARCODE, SUM(QUANTITY) AS QTY, LOCATION, INVDATE, INVTYPE " +
                    "FROM KDSSTOCKTAKESSC " +
                    "WHERE FLAG = 1 " +
                    "AND TRUNC (to_date(:FROMDATE,'dd/mm/yy')) <= TRUNC(CREATEDDATE) " +
                    "AND TRUNC(CREATEDDATE) <= TRUNC (to_date(:TODATE, 'dd/mm/yy')) ";

                cmd.Parameters.Add(new OracleParameter(":FROMDATE", OracleDbType.Date)).Value = From;
                cmd.Parameters.Add(new OracleParameter(":TODATE", OracleDbType.Date)).Value = To;

                if (!string.IsNullOrWhiteSpace(stockTake.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and BARCODE like '%' || :BARCODE || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = stockTake.Barcode;
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Description))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and DESCRIPTION like '%' || :DESC || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":DESC", OracleDbType.Varchar2)).Value = stockTake.Description;
                }

                if (!string.IsNullOrWhiteSpace(Location))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and Location like '%' || :LOCATION || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":LOCATION", OracleDbType.Varchar2)).Value = Location;
                }

                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and HOSTNAME like '%' || :HOSTNAME || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":HOSTNAME", OracleDbType.Varchar2)).Value = Hostname;
                }

                if (!string.IsNullOrWhiteSpace(InvNum))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and INVNUM like '%' || :INVNUM || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;
                }

                //cmd.Parameters.Add(new OracleParameter(":ProfileId", OracleDbType.Varchar2)).Value = ProfileID;

                cmd.CommandText = cmd.CommandText +
                                      "GROUP BY SITECODE, INVNUM, CREATEDBY, DESCRIPTION, BARCODE, LOCATION, INVDATE, INVTYPE )";

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("UploadStockTakeToUSSCServer Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = "Failed";
                return ErrorString;
            }

        }

        public String Commit(Boolean isLocal)
        {
            try
            {
                if (isLocal)
                {
                    this.ConnectLocal();
                }
                else
                {
                    this.Connect();
                }
                
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "Commit";

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("UploadStockTakeToUSSCServer Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = "Failed";
                return ErrorString;
            }

        }

        public String UpdateUSSCServerFlagAfterUpload(StockTake stockTake, String Site, String Hostname, DateTime From, DateTime To, String Location, String InvNum)
        {
            try
            {
                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "UPDATE KDSSTOCKTAKESSC " +
                    "SET FLAG = 2 " +
                    "WHERE TRUNC (to_date(:FROMDATE,'dd/mm/yy')) <= TRUNC(CREATEDDATE) " +
                    "AND TRUNC(CREATEDDATE) <= TRUNC (to_date(:TODATE, 'dd/mm/yy')) ";

                cmd.Parameters.Add(new OracleParameter(":FROMDATE", OracleDbType.Date)).Value = From;
                cmd.Parameters.Add(new OracleParameter(":TODATE", OracleDbType.Date)).Value = To;

                if (!string.IsNullOrWhiteSpace(stockTake.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and BARCODE like '%' || :BARCODE || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = stockTake.Barcode;
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Description))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and DESCRIPTION like '%' || :DESC || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":DESC", OracleDbType.Varchar2)).Value = stockTake.Description;
                }

                if (!string.IsNullOrWhiteSpace(Location))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and Location like '%' || :LOCATION || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":LOCATION", OracleDbType.Varchar2)).Value = Location;
                }

                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and HOSTNAME like '%' || :HOSTNAME || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":HOSTNAME", OracleDbType.Varchar2)).Value = Hostname;
                }

                if (!string.IsNullOrWhiteSpace(InvNum))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and INVNUM like '%' || :INVNUM || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;
                }

                //cmd.Parameters.Add(new OracleParameter(":ProfileId", OracleDbType.Varchar2)).Value = ProfileID;

                

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("UploadStockTakeToUSSCServer Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = "Failed";
                return ErrorString;
            }

        }

        public String DeleteLocalStockTake(StockTake stockTake, String Site, String Hostname, DateTime From, DateTime To, String Location, String InvNum)
        {
            try
            {
                this.ConnectLocal();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "DELETE " +
                    "FROM KDSSTOCKTAKESSC " +
                    "WHERE TRUNC(CREATEDDATE) <= TRUNC (to_date(:TODATE, 'dd/mm/yy')) " +
                    "AND TRUNC (to_date(:FROMDATE,'dd/mm/yy')) <= TRUNC(CREATEDDATE) ";

                
                cmd.Parameters.Add(new OracleParameter(":TODATE", OracleDbType.Date)).Value = To;
                cmd.Parameters.Add(new OracleParameter(":FROMDATE", OracleDbType.Date)).Value = From;

                if (!string.IsNullOrWhiteSpace(stockTake.Barcode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and BARCODE like '%' || :BARCODE || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":BARCODE", OracleDbType.Varchar2)).Value = stockTake.Barcode;
                }

                if (!string.IsNullOrWhiteSpace(stockTake.Description))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and DESCRIPTION like '%' || :DESC || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":DESC", OracleDbType.Varchar2)).Value = stockTake.Description;
                }

                if (!string.IsNullOrWhiteSpace(Location))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and Location like '%' || :LOCATION || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":LOCATION", OracleDbType.Varchar2)).Value = Location;
                }

                if (!string.IsNullOrWhiteSpace(Hostname))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and HOSTNAME like '%' || :HOSTNAME || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":HOSTNAME", OracleDbType.Varchar2)).Value = Hostname;
                }

                if (!string.IsNullOrWhiteSpace(InvNum))
                {
                    cmd.CommandText = cmd.CommandText +
                                      "and INVNUM like '%' || :INVNUM || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;
                }

                

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("UploadStockTakeToUSSCServer Function");
                logger.Error(e.Message);
                this.Close();
                ErrorString = "Failed";
                return ErrorString;
            }

        }

        public DateTime? GetInvDateStockTake(String InvNum, String Site)
        {
            DateTime? InvDate = null;
            try
            {
                this.ConnectLocal();
                
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select invdate from KDSHEADSTOCKTAKESSC " +
                                  "where sitecode = :SITE " +
                                  "and invnum = :INVNUM";

                cmd.Parameters.Add(new OracleParameter(":SITE", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //InvDate = DateTime.ParseExact(dr["invdate"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    InvDate = (DateTime)dr["invdate"];
                }

                this.Close();
                return InvDate;
            }
            catch (Exception e)
            {
                logger.Error("SearchForExistingTransactionByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public int? GetInvTypeStockTake(String InvNum, String Site)
        {
            int? InvType = null;
            try
            {
                this.ConnectLocal();

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select invtype from KDSHEADSTOCKTAKESSC " +
                                  "where sitecode = :SITE " +
                                  "and invnum = :INVNUM";

                cmd.Parameters.Add(new OracleParameter(":SITE", OracleDbType.Varchar2)).Value = Site;
                cmd.Parameters.Add(new OracleParameter(":INVNUM", OracleDbType.Varchar2)).Value = InvNum;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //InvDate = DateTime.ParseExact(dr["invdate"].ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    InvType = Int32.Parse(dr["invtype"].ToString());
                }

                this.Close();
                return InvType;
            }
            catch (Exception e)
            {
                logger.Error("SearchForExistingTransactionByProfileID Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvDetail getIdInvoiceDetail(String SKUID)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvDetail Detail = new InvDetail();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Select IDD, IDH, SKUID, CREATEDBY, CREATEDDATE, COMMENTDETAIL, " +
                                  "BRUTO, NETTO, DISCBRUTO, DISCNETTO, SITE, SUPPLIER, COMMERCIAL "+
                                  "from KDSTRXINVOICE " +
                                  "where SKUID = :SKUID";

                cmd.Parameters.Add(new OracleParameter(":SKUID", OracleDbType.Varchar2)).Value = SKUID;
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Detail.IDH = (string)dr["IDH"].ToString();
                    Detail.IDD = (string)dr["IDD"].ToString();
                    Detail.SKUID = (string)dr["SKUID"].ToString();
                    Detail.CREATEDBY = (string)dr["CREATEDBY"];
                    Detail.BRUTO = dr["BRUTO"].ToString();
                    Detail.NETTO = dr["NETTO"].ToString();
                    Detail.DISCBRUTO = dr["DISCBRUTO"].ToString();
                    Detail.DISCNETTO = dr["DISCNETTO"].ToString();
                    Detail.SITE = dr["SITE"].ToString();
                    Detail.SUPPLIER = dr["SUPPLIER"].ToString();
                    Detail.COMMERCIAL = dr["COMMERCIAL"].ToString();
                    Detail.COMMENT = dr["COMMENTDETAIL"].ToString();


                   

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Detail;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvDetail getIdInvoiceDetailHistorical(String SKUID)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvDetail Detail = new InvDetail();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Select IDD, IDH, SKUID, CREATEDBY, CREATEDDATE, COMMENTDETAIL, " +
                                  "BRUTO, NETTO, DISCBRUTO, DISCNETTO, SITE, SUPPLIER, COMMERCIAL " +
                                  "from HKDSTRXINVOICE " +
                                  "where SKUID = :SKUID";

                cmd.Parameters.Add(new OracleParameter(":SKUID", OracleDbType.Varchar2)).Value = SKUID;
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Detail.IDH = (string)dr["IDH"].ToString();
                    Detail.IDD = (string)dr["IDD"].ToString();
                    Detail.SKUID = (string)dr["SKUID"].ToString();
                    Detail.CREATEDBY = (string)dr["CREATEDBY"];
                    Detail.BRUTO = dr["BRUTO"].ToString();
                    Detail.NETTO = dr["NETTO"].ToString();
                    Detail.DISCBRUTO = dr["DISCBRUTO"].ToString();
                    Detail.DISCNETTO = dr["DISCNETTO"].ToString();
                    Detail.SITE = dr["SITE"].ToString();
                    Detail.SUPPLIER = dr["SUPPLIER"].ToString();
                    Detail.COMMERCIAL = dr["COMMERCIAL"].ToString();




                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Detail;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public TotalInvoice GetTotalInvoice(String Data)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            TotalInvoice total = new TotalInvoice();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Select  TO_CHAR (nvl((Select nvl(sum(VALUE),0) from KDSTRXEXPINVOICE where IDD = KDSTRXINVOICE.IDD ),'0'), '999,999,999,999.99') TotalExp,  TO_CHAR (nvl((BRUTO / 1.1),'0'), '999,999,999,999.99') TotalWithTax, " +
                                  "  TO_CHAR (nvl((BRUTO - ( BRUTO / 1.1)),'0'), '999,999,999,999.99') Tax, " +
                                  " TO_CHAR ( nvl(BRUTO ,'0'), '999,999,999,999.99') Total from KDSTRXINVOICE where IDD = '" + Data + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    total.TotalWithTax = (string)dr["TotalWithTax"].ToString();
                    total.Tax = (string)dr["Tax"].ToString();
                    total.Total = (string)dr["Total"].ToString();
                    total.TotalExp = (string)dr["TotalExp"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return total;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public InvoiceParam getParameterHeader(InvoiceParam Data)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvoiceParam Param = new InvoiceParam();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select IDHP, IDH, PARAMID, VALUE from KDSEXPENSEFAKTURPAJAK where IDH = '" + Data.IDH + "' and PARAMID = (SELECT ID FROM KDSPARAM WHERE LONGDESC = '" + Data.PARAMDESC + "') ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Param.IDHP = (string)dr["IDHP"].ToString();
                    Param.IDH = (string)dr["IDH"].ToString();
                    Param.PARAMID = (string)dr["PARAMID"].ToString();
                    Param.VALUE = (string)dr["VALUE"].ToString();

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Param;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvoiceParam getParameter(InvoiceParam Data)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvoiceParam Param = new InvoiceParam();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select IDDP, IDD, PARAMID, VALUE from KDSTRXEXPINVOICE where IDD = '" + Data.IDD + "' and PARAMID = (SELECT ID FROM KDSPARAM WHERE LONGDESC = '" + Data.PARAMDESC + "') ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Param.IDDP = (string)dr["IDDP"].ToString();
                    Param.IDD = (string)dr["IDD"].ToString();
                    Param.PARAMID = (string)dr["PARAMID"].ToString();
                    Param.VALUE = (string)dr["VALUE"].ToString();

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Param;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public InvHeader getIdKodeSlip(String Kode)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvHeader Header = new InvHeader();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                /*
                cmd.CommandText = "Select IDH, Kode, IDPENGUSAHA, IDPEMBELI, STATUS, CREATEDDATE, CREATEDBY" +
                                  " from KDSFAKTURPAJAK where Kode = '"+ Kode +"'";
                */
                cmd.CommandText =   " SELECT   " +
                                    " ((SUM(TotalInv)  - SUM(EXPDETAIL)) - EXPHEADER) AS Total, " +
                                    " TO_CHAR(((SUM(TotalInv)  - SUM(EXPDETAIL)) - EXPHEADER), '999,999,999,999.99') AS LastTotal, " +
                                    " kdspkinvoice.terbilang_indo(( (((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER)))) AS TERBILANGTotalFacNT " +
                                    " FROM (SELECT NVL(SUM(B.BRUTO),0) TotalInv, " +
                                    " NVL((SELECT SUM(VALUE) FROM KDSEXPENSEFAKTURPAJAK WHERE IDH in (select IDH from KDSFAKTURPAJAK where KODE IN (" + Kode + "))),0) EXPHEADER," +
                                    " sum(NVL((SELECT SUM(VALUE) FROM KDSTRXEXPINVOICE WHERE IDD = B.IDD),0)) EXPDETAIL" +
                                    " FROM KDSFAKTURPAJAK A, KDSTRXINVOICE B " +
                                    " WHERE A.KODE IN (" + Kode + ") " +
                                    " AND A.IDH     = B.IDH " +
                                    " GROUP BY A.IDPENGUSAHA, " +
                                    " A.IDPEMBELI) GROUP BY EXPHEADER ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Header.TOTALDATAINV = (string)dr["Total"].ToString();
                    Header.LASTTOTAL = (string)dr["LastTotal"].ToString();
                    Header.TERBILANGTotalFacNT = (string)dr["TERBILANGTotalFacNT"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Header;
            }
            catch (Exception e)
            {
                logger.Error("get Data Invoice Slip");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public InvHeader getIdKode(String Kode)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvHeader Header = new InvHeader();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                /*
                cmd.CommandText = "Select IDH, Kode, IDPENGUSAHA, IDPEMBELI, STATUS, CREATEDDATE, CREATEDBY" +
                                  " from KDSFAKTURPAJAK where Kode = '"+ Kode +"'";
                */
                cmd.CommandText = " SELECT " +
                                  " IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,nvl(MODIFIEDDATE,'') MODIFIEDDATE,nvl(MODIFIEDBY,'') MODIFIEDBY, " +
                                  " SUM(TotalDataInv) TotalDataInv, SUM(TotalInv) TotalInv, SUM(EXPDETAIL) EXPDETAIL, " +
                                  " (SUM(TotalInv)) TotalInvWT, TO_CHAR ( (SUM(TotalInv)-(SUM(TotalInv)/1.1)),'999,999,999,999.99') TotalInvT, " +
                                  " TO_CHAR ( (SUM(TotalInv)/1.1), '999,999,999,999.99') TotalInvNT, EXPHEADER, " +
                                  " TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL))), '999,999,999,999.99') TotalFacWT , TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL)) -((SUM(TotalInv) - SUM(EXPDETAIL))/1.1)), '999,999,999,999.99') TotalFacT, " +
                                  "  TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL))/1.1), '999,999,999,999.99') TotalFacNT, " +
                                  " TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER), '999,999,999,999.99') AS LastTotal, " +
                                  " kdspkinvoice.terbilang_indo(( (((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER)))) AS TERBILANGTotalFacNT " +
                                  " FROM " +
                                  " (SELECT A.IDH, B.IDD, " +
                                  " A.Kode,  A.IDPENGUSAHA,  A.IDPEMBELI,  A.STATUS,  A.CREATEDDATE,  A.CREATEDBY, " +
                                  " A.MODIFIEDDATE,  A.MODIFIEDBY, count(B.IDD) TotalDataInv, " +
                                  " nvl(sum(B.BRUTO),0) TotalInv, " +
                                  " nvl((SELECT sum(VALUE) FROM KDSEXPENSEFAKTURPAJAK WHERE IDH = A.IDH ),0) EXPHEADER, " +
                                  " nvl((SELECT sum(VALUE) FROM KDSTRXEXPINVOICE WHERE IDD = B.IDD ),0) EXPDETAIL " +
                                  " from KDSFAKTURPAJAK A, KDSTRXINVOICE B " +
                                  " where    A.KODE = '" + Kode + "' and  A.IDH = B.IDH  " +
                                  " GROUP BY A.IDH,  A.Kode,  A.IDPENGUSAHA, A.IDPEMBELI, " +
                                  " A.STATUS,  A.CREATEDDATE,  A.CREATEDBY,  A.MODIFIEDDATE,  A.MODIFIEDBY, B.IDD ) " +
                                  " group BY  IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,MODIFIEDDATE,MODIFIEDBY, EXPHEADER ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Header.IDH = (string)dr["IDH"].ToString();
                    Header.STATUS = (string)dr["STATUS"].ToString();
                    Header.IDPENGUSAHA = (string)dr["IDPENGUSAHA"].ToString();
                    Header.KODE = (string)dr["Kode"].ToString();
                    Header.IDPEMBELI = (string)dr["IDPEMBELI"].ToString();
                    Header.CREATEDDATE = (string)dr["CREATEDDATE"].ToString();
                    Header.CREATEDBY = (string)dr["CREATEDBY"].ToString();
                    Header.MODIFIEDDATE = (string)dr["MODIFIEDDATE"].ToString();
                    Header.MODIFIEDBY = (string)dr["MODIFIEDBY"].ToString();
                    Header.TOTALDATAINV = (string)dr["TOTALDATAINV"].ToString();
                    Header.TOTALINV = (string)dr["TOTALINV"].ToString();
                    Header.EXPDETAIL = (string)dr["EXPDETAIL"].ToString();
                    Header.TOTALINVWT = (string)dr["TOTALINVWT"].ToString();
                    Header.TOTALINVT = (string)dr["TOTALINVT"].ToString();
                    Header.TOTALINVNT = (string)dr["TOTALINVNT"].ToString();
                    Header.EXPHEADER = (string)dr["EXPHEADER"].ToString();
                    Header.TOTALFACWT = (string)dr["TOTALFACWT"].ToString();
                    Header.TOTALFACT = (string)dr["TOTALFACT"].ToString();
                    Header.TOTALFACNT = (string)dr["TOTALFACNT"].ToString();
                    Header.LASTTOTAL = (string)dr["LastTotal"].ToString();
                    Header.TERBILANGTotalFacNT = (string)dr["TERBILANGTotalFacNT"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Header;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public InvHeader getIdKodeCekData(String Kode)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvHeader Header = new InvHeader();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                /*
                cmd.CommandText = "Select IDH, Kode, IDPENGUSAHA, IDPEMBELI, STATUS, CREATEDDATE, CREATEDBY" +
                                  " from KDSFAKTURPAJAK where Kode = '"+ Kode +"'";
                */
                cmd.CommandText = " SELECT " +
                                  " IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,nvl(MODIFIEDDATE,'') MODIFIEDDATE,nvl(MODIFIEDBY,'') MODIFIEDBY, " +
                                  " SUM(TotalDataInv) TotalDataInv, SUM(TotalInv) TotalInv, SUM(EXPDETAIL) EXPDETAIL, " +
                                  " (SUM(TotalInv)) TotalInvWT, TO_CHAR ( (SUM(TotalInv)-(SUM(TotalInv)/1.1)),'999,999,999,999.99') TotalInvT, " +
                                  " TO_CHAR ( (SUM(TotalInv)/1.1), '999,999,999,999.99') TotalInvNT, EXPHEADER, " +
                                  " TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL))), '999,999,999,999.99') TotalFacWT , TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL)) -((SUM(TotalInv) - SUM(EXPDETAIL))/1.1)), '999,999,999,999.99') TotalFacT, " +
                                  "  TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL))/1.1), '999,999,999,999.99') TotalFacNT, " +
                                  " TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER), '999,999,999,999.99') AS LastTotal, " +
                                  " kdspkinvoice.terbilang_indo(( (((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER)))) AS TERBILANGTotalFacNT " +
                                  " FROM " +
                                  " (SELECT A.IDH, B.IDD, " +
                                  " A.Kode,  A.IDPENGUSAHA,  A.IDPEMBELI,  A.STATUS,  A.CREATEDDATE,  A.CREATEDBY, " +
                                  " A.MODIFIEDDATE,  A.MODIFIEDBY, count(B.IDD) TotalDataInv, " +
                                  " nvl(sum(B.BRUTO),0) TotalInv, " +
                                  " nvl((SELECT sum(VALUE) FROM KDSEXPENSEFAKTURPAJAK WHERE IDH = A.IDH ),0) EXPHEADER, " +
                                  " nvl((SELECT sum(VALUE) FROM KDSTRXEXPINVOICE WHERE IDD = B.IDD ),0) EXPDETAIL " +
                                  " from KDSFAKTURPAJAK A, KDSTRXINVOICE B " +
                                  " where   A.IDH = '" + Kode + "' and  A.IDH = B.IDH  " +
                                  " GROUP BY A.IDH,  A.Kode,  A.IDPENGUSAHA, A.IDPEMBELI, " +
                                  " A.STATUS,  A.CREATEDDATE,  A.CREATEDBY,  A.MODIFIEDDATE,  A.MODIFIEDBY, B.IDD ) " +
                                  " group BY  IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,MODIFIEDDATE,MODIFIEDBY, EXPHEADER ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Header.IDH = (string)dr["IDH"].ToString();
                    Header.STATUS = (string)dr["STATUS"].ToString();
                    Header.IDPENGUSAHA = (string)dr["IDPENGUSAHA"].ToString();
                    Header.KODE = (string)dr["Kode"].ToString();
                    Header.IDPEMBELI = (string)dr["IDPEMBELI"].ToString();
                    Header.CREATEDDATE = (string)dr["CREATEDDATE"].ToString();
                    Header.CREATEDBY = (string)dr["CREATEDBY"].ToString();
                    Header.MODIFIEDDATE = (string)dr["MODIFIEDDATE"].ToString();
                    Header.MODIFIEDBY = (string)dr["MODIFIEDBY"].ToString();
                    Header.TOTALDATAINV = (string)dr["TOTALDATAINV"].ToString();
                    Header.TOTALINV = (string)dr["TOTALINV"].ToString();
                    Header.EXPDETAIL = (string)dr["EXPDETAIL"].ToString();
                    Header.TOTALINVWT = (string)dr["TOTALINVWT"].ToString();
                    Header.TOTALINVT = (string)dr["TOTALINVT"].ToString();
                    Header.TOTALINVNT = (string)dr["TOTALINVNT"].ToString();
                    Header.EXPHEADER = (string)dr["EXPHEADER"].ToString();
                    Header.TOTALFACWT = (string)dr["TOTALFACWT"].ToString();
                    Header.TOTALFACT = (string)dr["TOTALFACT"].ToString();
                    Header.TOTALFACNT = (string)dr["TOTALFACNT"].ToString();
                    Header.TERBILANGTotalFacNT = (string)dr["TERBILANGTotalFacNT"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Header;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public InvHeader getIdKodeHistorical(String Kode)
        {

            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvHeader Header = new InvHeader();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                /*
                cmd.CommandText = "Select IDH, Kode, IDPENGUSAHA, IDPEMBELI, STATUS, CREATEDDATE, CREATEDBY" +
                                  " from KDSFAKTURPAJAK where Kode = '"+ Kode +"'";
                */
                cmd.CommandText = " SELECT " +
                                  " IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,nvl(MODIFIEDDATE,'') MODIFIEDDATE,nvl(MODIFIEDBY,'') MODIFIEDBY, " +
                                  " SUM(TotalDataInv) TotalDataInv, SUM(TotalInv) TotalInv, SUM(EXPDETAIL) EXPDETAIL, " +
                                  " (SUM(TotalInv)) TotalInvWT, TO_CHAR ( (SUM(TotalInv)-(SUM(TotalInv)/1.1)),'999,999,999,999.99') TotalInvT, " +
                                  " TO_CHAR ( (SUM(TotalInv)/1.1), '999,999,999,999.99') TotalInvNT, EXPHEADER, " +
                                  " TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL))), '999,999,999,999.99') TotalFacWT , TO_CHAR (((SUM(TotalInv) - SUM(EXPDETAIL)) -((SUM(TotalInv) - SUM(EXPDETAIL))/1.1)), '999,999,999,999.99') TotalFacT, " +
                                  "  TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL))/1.1), '999,999,999,999.99') TotalFacNT, " +
                                  " TO_CHAR(((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER), '999,999,999,999.99') AS LastTotal, " +
                                  " kdspkinvoice.terbilang_indo(( (((SUM(TotalInv) - SUM(EXPDETAIL)) - EXPHEADER)))) AS TERBILANGTotalFacNT " +
                                  " FROM " +
                                  " (SELECT A.IDH, B.IDD, " +
                                  " A.Kode,  A.IDPENGUSAHA,  A.IDPEMBELI,  A.STATUS,  A.CREATEDDATE,  A.CREATEDBY, " +
                                  " A.MODIFIEDDATE,  A.MODIFIEDBY, count(B.IDD) TotalDataInv, " +
                                  " nvl(sum(B.BRUTO),0) TotalInv, " +
                                  " nvl((SELECT sum(VALUE) FROM HKDSEXPENSEFAKTURPAJAK WHERE IDH = A.IDH ),0) EXPHEADER, " +
                                  " nvl((SELECT sum(VALUE) FROM HKDSTRXEXPINVOICE WHERE IDD = B.IDD ),0) EXPDETAIL " +
                                  " from HKDSFAKTURPAJAK A, HKDSTRXINVOICE B " +
                                  " where    A.KODE = '" + Kode + "' and  A.IDH = B.IDH  " +
                                  " GROUP BY A.IDH,  A.Kode,  A.IDPENGUSAHA, A.IDPEMBELI, " +
                                  " A.STATUS,  A.CREATEDDATE,  A.CREATEDBY,  A.MODIFIEDDATE,  A.MODIFIEDBY, B.IDD ) " +
                                  " group BY  IDH,Kode,IDPENGUSAHA,IDPEMBELI,STATUS,CREATEDDATE,CREATEDBY,MODIFIEDDATE,MODIFIEDBY, EXPHEADER ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Header.IDH = (string)dr["IDH"].ToString();
                    Header.STATUS = (string)dr["STATUS"].ToString();
                    Header.IDPENGUSAHA = (string)dr["IDPENGUSAHA"].ToString();
                    Header.KODE = (string)dr["Kode"].ToString();
                    Header.IDPEMBELI = (string)dr["IDPEMBELI"].ToString();
                    Header.CREATEDDATE = (string)dr["CREATEDDATE"].ToString();
                    Header.CREATEDBY = (string)dr["CREATEDBY"].ToString();
                    Header.MODIFIEDDATE = (string)dr["MODIFIEDDATE"].ToString();
                    Header.MODIFIEDBY = (string)dr["MODIFIEDBY"].ToString();
                    Header.TOTALDATAINV = (string)dr["TOTALDATAINV"].ToString();
                    Header.TOTALINV = (string)dr["TOTALINV"].ToString();
                    Header.EXPDETAIL = (string)dr["EXPDETAIL"].ToString();
                    Header.TOTALINVWT = (string)dr["TOTALINVWT"].ToString();
                    Header.TOTALINVT = (string)dr["TOTALINVT"].ToString();
                    Header.TOTALINVNT = (string)dr["TOTALINVNT"].ToString();
                    Header.EXPHEADER = (string)dr["EXPHEADER"].ToString();
                    Header.TOTALFACWT = (string)dr["TOTALFACWT"].ToString();
                    Header.TOTALFACT = (string)dr["TOTALFACT"].ToString();
                    Header.TOTALFACNT = (string)dr["TOTALFACNT"].ToString();
                    Header.TERBILANGTotalFacNT = (string)dr["TERBILANGTotalFacNT"].ToString();
                    Header.LASTTOTAL = (string)dr["LastTotal"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Header;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public InvHeader getIdKodeData(String Kode)
        {
            
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            InvHeader Header = new InvHeader();
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText = "Select IDH, Kode, IDPENGUSAHA, IDPEMBELI, STATUS, CREATEDDATE, CREATEDBY, MODIFIEDDATE, MODIFIEDBY,COMMENTHEADER, COMMENTFOOTER, STARTDATE, ENDDATE " +
                                  " from KDSFAKTURPAJAK where IDH = '"+ Kode +"'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Header.IDH = (string)dr["IDH"].ToString();
                    Header.STATUS = (string)dr["STATUS"].ToString();
                    Header.IDPENGUSAHA = (string)dr["IDPENGUSAHA"].ToString();
                    Header.KODE = (string)dr["Kode"].ToString();
                    Header.IDPEMBELI = (string)dr["IDPEMBELI"].ToString();
                    Header.CREATEDDATE = (string)dr["CREATEDDATE"].ToString();
                    Header.CREATEDBY = (string)dr["CREATEDBY"].ToString();
                    Header.MODIFIEDDATE = (string)dr["MODIFIEDDATE"].ToString();
                    Header.MODIFIEDBY = (string)dr["MODIFIEDBY"].ToString();
                    Header.HComment = (string)dr["COMMENTHEADER"].ToString();
                    Header.FComment = (string)dr["COMMENTFOOTER"].ToString();
                    Header.STARTDATE = (string)dr["STARTDATE"].ToString();
                    Header.ENDDATE = (string)dr["ENDDATE"].ToString();   
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Header;
            }
            catch (Exception e)
            {
                logger.Error("getDiscount");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string InsertInvDetail(String IDH, String Invoice, String Comment, String Status, String DataSupply)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                if (Status == "Insert")
                {
                   /* 
                    cmd.CommandText =
                        "INSERT INTO KDSTRXINVOICE (IDD, IDH, SKUID, CREATEDBY, MODIFIEDBY, CREATEDDATE,MODIFIEDDATE, NOMODIFIED, BRUTO, NETTO, COMMENTDETAIL, DISCBRUTO, DISCNETTO,SITE, SUPPLIER, COMMERCIAL) " +
                        " (select (select nvl(max(IDD),0)+1 from KDSTRXINVOICE ), '" + IDH + "', '" + Invoice + "', '" + GlobalVar.GlobalVarUsername + "', '" + GlobalVar.GlobalVarUsername + "', SYSDATE, SYSDATE,0, " +
                        "EFAESCRECB,EFAESCRECN, '" + Comment + "' ," +
                        "EFAESCOB,EFAESCON,kdspkinvoice.get_InvSite(efarfou), " +
                        "PKFOUDGENE.get_CNUF(1,EFACFIN), " +
                        "pkfouccom.get_NUMContrat(1,EFACCIN)  FROM CFDENFAC where efarfou =  '" + Invoice + "')";
                    ErrorString = "Success Insert Detail";
                    */
                    cmd.CommandText =
                        "INSERT INTO KDSTRXINVOICE (IDD, IDH, SKUID, CREATEDBY, MODIFIEDBY, CREATEDDATE,MODIFIEDDATE, NOMODIFIED, BRUTO, NETTO, COMMENTDETAIL, DISCBRUTO, DISCNETTO,SITE, SUPPLIER, COMMERCIAL) " +
                        " (select (select nvl(max(IDD),0)+1 from KDSTRXINVOICE ), '" + IDH + "', '" + Invoice + "', '" + GlobalVar.GlobalVarUsername + "', '" + GlobalVar.GlobalVarUsername + "', SYSDATE, SYSDATE,0, " +
                        "EFAMFAC,EFAESCRECN, '" + Comment + "' ," +
                        "EFAESCOB,EFAESCON,kdspkinvoice.get_InvSite(efarfou), " +
                        "PKFOUDGENE.get_CNUF(1,EFACFIN), " +
                        "pkfouccom.get_NUMContrat(1,EFACCIN)  FROM CFDENFAC where efarfou =  '" + Invoice + "' and  EFACFIN in  (select FOUCFIN from foudgene  where FOULIBL = '" + DataSupply + "') )";
                    ErrorString = "Success Insert Detail";
                }
                else
                {
                    cmd.CommandText =
                      "UPDATE KDSTRXINVOICE set MODIFIEDBY = '" + GlobalVar.GlobalVarUsername + "', MODIFIEDDATE = SYSDATE, NOMODIFIED = NOMODIFIED + 1 , " +
                      " COMMENTDETAIL = '" + Comment + "'  where   IDH = '" + IDH + "' and  SKUID = '" + Invoice + "'";
                    ErrorString = "Success Update Detail";
                }
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
               
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Failed Please Call Head Office");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string UpdateArchiveData(String Invoice, String Status)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Update");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                
                cmd.CommandText =
                    "UPDATE CFDENFAC set EFAARCH = '" + Status + "' where EFARFOU = '" + Invoice + "'";
                ErrorString = "Success Update ";
                
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();

                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Failed Please Call Head Office");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string UpdateArchiveDataHeader(String IDH, String Status)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Update");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;

                cmd.CommandText =
                    "UPDATE CFDENFAC set EFAARCH = '" + Status + "' where EFARFOU in (select SKUID from KDSTRXINVOICE  where IDH = '" + IDH + "')";
                ErrorString = "Success Update ";

                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();

                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Failed Please Call Head Office");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
           
        }

        public DataTable SelectInvoiceDetailData(String NoDetailFaktur, String Data)
        //public DataTable SelectInvoiceDetailData(String NoDetailFaktur)
        {
            try
            {

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                            "SELECT " +
                            "   efarfou INVOICE, " +
                            "kdspkinvoice.get_InvSite(efarfou) SITE , " +
                            "pkfouccom.get_NUMContrat(1,EFACCIN) COMMERCIALCONTRACT, " +
                            "EFADATF INVOICEDATE , " +
                            "   TO_CHAR (EFAMFAC, '999,999,999,999.99')  GROSS " +
                            //"   TO_CHAR (EFAESCRECN, '999,999,999,999.99')  NET, " +
                            //"   TO_CHAR (EFAESCOB, '999,999,999,999.99')  DISCGROSS, " +
                            //"   TO_CHAR (EFAESCON, '999,999,999,999.99')  DISCNET  " +
                            "   FROM CFDENFAC " +
                            " WHere efarfou = '" + NoDetailFaktur + "' and EFACFIN in (select FOUCFIN from foudgene  where FOULIBL = '" + Data + "')  ";

                /*
                if (!string.IsNullOrWhiteSpace(SDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and SHORTDESC like '%' || :SDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":SDesc", OracleDbType.Varchar2)).Value = SDesc;
                }
                */

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Detail Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public string InsertInvParamDetail(String IDD, String Value, String IDParameter, String Status)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                if (Status == "Insert")
                {
                    cmd.CommandText =
                        "INSERT INTO KDSTRXEXPINVOICE " +
                        "(IDDP, IDD, PARAMID, VALUE, CREATEDBY, MODIFIEDBY, CREATEDDATE,MODIFIEDDATE, NOMODIFIED)  " +
                        "VALUES ((select nvl(max(IDDP),0)+1 from KDSTRXEXPINVOICE) , '" + IDD + "', '" + IDParameter + "', '" + Value + "', " +
                        "'" + GlobalVar.GlobalVarUsername + "','" + GlobalVar.GlobalVarUsername + "' , " +
                        "SYSDATE, SYSDATE, '0')";
                }
                else
                {
                    cmd.CommandText =
                        "Update KDSTRXEXPINVOICE " +
                        "SET VALUE = '" + Value + "', MODIFIEDBY = '" + GlobalVar.GlobalVarUsername + "', MODIFIEDDATE = SYSDATE , NOMODIFIED = NOMODIFIED + 1 " +
                        "WHERE IDD = '" + IDD + "'and PARAMID = '" + IDParameter + "' ";
                }
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Insert Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Detail Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string CekNoFaktur(String KODE)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT 'Already Exists' STATUS FROM KDSFAKTURPAJAK where KODE = '" + KODE + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Status = (String)dr["STATUS"];
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string CekParamHeader(String IDH, String Value, String IDParameter)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT 'Already Exists' STATUS FROM KDSEXPENSEFAKTURPAJAK where IDH = '" + IDH + "' AND PARAMID = '" + IDParameter + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Status = (String)dr["STATUS"];                   
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public string DeleteParamHeader(String IDH, String IDParameter, String Value)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Delete KDSEXPENSEFAKTURPAJAK where IDH = '" + IDH + "' AND PARAMID = (select ID from KDSPARAM where PRMVAR1 = 'Header' and LongDesc =  '" + IDParameter + "' )";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
               
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                Status = "Sukses";
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }


        public string DeleteInvoicePembayaranData(String ID)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = " insert into HKDSMSTBAYAR " +
                                  " select ID,DESCRIPTION,PEMBELI,PENGUSAHA,TOTAL,DATATUJUAN,ANTUJUAN,BANKTUJUAN, " +
                                  " DATAPENGIRIM,ADPENGIRIM,NPWP,STARTDATE,ENDDATE,FLAG,CREATEDDATE,SYSDATE, " +
                                  " CREATEDBY,'" + GlobalVar.GlobalVarUsername + "',ADPENERIMA,REKTUJUAN,'', BIAYA FROM KDSMSTBAYAR WHERE ID  = '" + ID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                cmd.CommandText = "Delete KDSMSTBAYAR where ID = '" + ID + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                Status = "Success";
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        
        public string DeleteInvExpDetail(String IDH, String Inv)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Delete from KDSTRXEXPINVOICE where IDD in (select IDD from KDSTRXINVOICE where IDH = '" + IDH + "' AND SKUID = '" + Inv + "')";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                Status = "Sukses";
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public string DeleteInvExpDetailMenu(String IDD, String Param)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Delete from KDSTRXEXPINVOICE where IDD = '"+IDD+"' and PARAMID = (select ID from KDSPARAM where PRMVAR1 = 'Detail' and LongDesc =  '" + Param + "' ) ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                Status = "Sukses";
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public string DeleteInvDetail(String IDH, String Inv)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "Delete KDSTRXINVOICE where IDH = '" + IDH + "' AND SKUID = '" + Inv + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                Status = "Sukses";
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public string CekParamDetail(String IDD, String Value, String IDParameter)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT 'Already Exists' STATUS FROM KDSTRXEXPINVOICE where IDD = '" + IDD + "' AND PARAMID = '" + IDParameter + "'";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Status = (String)dr["STATUS"];
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string NoInvoicePembayaran(FakturPajakSearch FPSearch)
        {
            string Status = "";
            logger.Debug("Start Connect");
            this.Connect();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT ID FROM KDSMSTBAYAR where DESCRIPTION = '" + FPSearch.InvoiceNo + "' and PEMBELI = '" + FPSearch.IDPEMBELI + "' and PENGUSAHA = '" + FPSearch.IDPENGUSAHA + "' and TOTAL = '" + FPSearch.Total + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");
                while (dr.Read())
                {
                    Status = (String)dr["ID"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return Status;
            }
            catch (Exception e)
            {
                logger.Error("Status Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }

        public string InsertInvoicePembayaran(FakturPajakSearch FPSearch , String Status)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                if (Status == "Insert")
                {
                    cmd.CommandText =
                        " INSERT INTO KDSMSTBAYAR (ID,DESCRIPTION, PEMBELI,PENGUSAHA,TOTAL,STARTDATE,ENDDATE,FLAG,CREATEDDATE,MODIFIEDDATE,CREATEDBY,MODIFIEDBY, MANUAL, BIAYA  ) " +
                        " VALUES ( (select nvl(max(ID),0) + 1 from KDSMSTBAYAR), '" + FPSearch.InvoiceNo  + "', '" + FPSearch.IDPEMBELI + "', '" + FPSearch.IDPENGUSAHA + "' " +
                        ", '" + FPSearch.Total + "', :FromDate , :ToDate , " +
                        " '0', SYSDATE, SYSDATE, '" + GlobalVar.GlobalVarUsername + "', '" + GlobalVar.GlobalVarUsername + "' , '" + FPSearch.STATUS + "', '" + FPSearch.Biaya + "' )";

                    cmd.Parameters.Add(new OracleParameter(":FromDate", OracleDbType.Date)).Value = FPSearch.StartDate.Date;
                    cmd.Parameters.Add(new OracleParameter(":ToDate", OracleDbType.Date)).Value = FPSearch.EndDate.Date;

                    cmd.CommandType = CommandType.Text;
                }
                else if (Status == "Update Data")
                {
                    cmd.CommandText =
                       " Update KDSMSTBAYAR set TOTAL = '" + FPSearch.Total + "', DESCRIPTION = '" + FPSearch.InvoiceNo + "', BIAYA =  '" + FPSearch.Biaya + "'  WHERE ID = '" + FPSearch.No + "' ";

                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd.CommandText =
                       " Update KDSMSTBAYAR SET DATATUJUAN = '" + FPSearch.Penerima  + "' " +
                       " ,ANTUJUAN = '" + FPSearch.ANPenerima +"' " +
                       " ,BANKTUJUAN = '" + FPSearch.BankPenerima  +"' " +
                       " ,DATAPENGIRIM = '" +  FPSearch.Pengirim  +"' " +
                       " ,ADPENGIRIM = '" + FPSearch.AdPengirim  +"' " +
                       " ,NPWP = '" + FPSearch.NPWP +"' " +
                       " ,MODIFIEDDATE = SYSDATE " +
                       " ,MODIFIEDBY = '" + GlobalVar.GlobalVarUsername + "' " +
                       " ,ADPENERIMA = '" + FPSearch.AdPenerima  +"' " +
                       " ,REKTUJUAN = '" + FPSearch.NoRekPenerima +"' " +
                       " ,MANUAL = '" + FPSearch.STATUS + "' " +
                       " , BIAYA =  '" + FPSearch.Biaya + "' "+
                       " WHERE ID = '" +  FPSearch.No  +"' ";

                    cmd.CommandType = CommandType.Text;

                }
                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();



                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string UpdateFakturInvoicePembayaran(String IDBayar, String Kode)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = " Update KDSFAKTURPAJAK SET FLAG = '0' , IDBAYAR = '' where IDBAYAR = '" + IDBayar + "' ";                    
                cmd.CommandType = CommandType.Text;                
                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

               // trans.Commit();

                cmd.CommandText = " Update KDSFAKTURPAJAK SET FLAG = '9' , IDBAYAR = '" + IDBayar + "' where KODE in (" + Kode + ") ";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();



                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string UpdatePembayaranDelete(String IDBayar)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = " Update KDSFAKTURPAJAK SET FLAG = '0', IDBAYAR = '' where IDBAYAR = '" + IDBayar + "' ";
                cmd.CommandType = CommandType.Text;
                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                // trans.Commit();
             
                trans.Commit();



                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string InsertInvParamHeader(String IDH, String Value, String IDParameter, String Status)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                if (Status == "Insert")
                {
                    cmd.CommandText =
                        "INSERT INTO KDSEXPENSEFAKTURPAJAK " +
                        "(IDHP, IDH, PARAMID, VALUE, CREATEDBY, MODIFIEDBY, CREATEDDATE, MODIFIEDDATE, NOMODIFIED)  " +
                        "VALUES ((select nvl(max(IDHP),0)+1 from KDSEXPENSEFAKTURPAJAK where IDH = '" + IDH + "') , '" + IDH + "', '" + IDParameter + "', '" + Value + "', " +
                        "'" + GlobalVar.GlobalVarUsername + "','" + GlobalVar.GlobalVarUsername + "'," +
                        "SYSDATE,SYSDATE, 0)";
                    cmd.CommandType = CommandType.Text;
                }
                else
                {
                    cmd.CommandText =
                        "Update KDSEXPENSEFAKTURPAJAK set VALUE = '" + Value + "', MODIFIEDBY = '" + GlobalVar.GlobalVarUsername + "', MODIFIEDDATE = SYSDATE, NOMODIFIED = NOMODIFIED + 1" +
                        "where IDH = '" + IDH + "' and PARAMID = '" + IDParameter + "'";
                    cmd.CommandType = CommandType.Text;
                }
                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string DeleteInvHeader(String IDH)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = "PKMOVEINVOICE.DELETE_DATA";
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.Parameters.Add("PIDH", OracleDbType.Varchar2, 2000).Value = IDH;                
                cmd.Parameters.Add("POUTRSNCODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("POUTRSNMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;


                logger.Debug(cmd.CommandText);


                cmd.ExecuteNonQuery();



                trans.Commit();
                this.Close();
                ErrorString = "Delete Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string MoveInvHeader(String UPD_HISTORY)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Move Invoice");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText = "PKMOVEINVOICE.UPD_HISTORY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("PKODE", OracleDbType.Varchar2, 2000).Value = UPD_HISTORY;
                cmd.Parameters.Add("POUTRSNCODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("POUTRSNMSG", OracleDbType.Varchar2, 2000).Direction = ParameterDirection.Output;


                logger.Debug(cmd.CommandText);


                cmd.ExecuteNonQuery();



                trans.Commit();
                this.Close();
                ErrorString = "Move History Succes";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Move History Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string UpdateInvHeaderEdit(String IDHeader)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Update Header");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Update KDSFAKTURPAJAK SET STATUS = 'CONFIRM EDIT' WHERE KODE = '" + IDHeader + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Update Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Update Header Error");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string UpdateInvHeader(String IDHeader)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Update Header");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Update KDSFAKTURPAJAK SET STATUS = 'CONFIRM' WHERE IDH = '" + IDHeader + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Update Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Update Header Error");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public String getidKodeDataIDH(String Kode, String Pengusaha, String Pembeli)
        {
            String IDHData = null;
            logger.Debug("Start Connect");
            this.ConnectLocal();
            logger.Debug("End Connect");
            try
            {

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select IDH " +
                                  "from KDSFAKTURPAJAK  " +
                                  "where KODE = :Kode " +
                                  "and IDPengusaha = :Pengusaha " +
                                  "and IDPembeli = :Pembeli ";
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(new OracleParameter(":Kode", OracleDbType.Varchar2)).Value = Kode;
                cmd.Parameters.Add(new OracleParameter(":Pengusaha", OracleDbType.Varchar2)).Value = Pengusaha;
                cmd.Parameters.Add(new OracleParameter(":Pembeli", OracleDbType.Varchar2)).Value = Pembeli;

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());
                OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                while (dr.Read())
                {
                    IDHData = (string)dr["IDH"].ToString();

                }
                logger.Debug("Start Close Connection");
                this.Close();
                logger.Debug("End Close Connection");
                return IDHData;
            }
            catch (Exception e)
            {
                logger.Error("getIDHData");
                logger.Error(e.Message);
                this.Close();
                return null;
            }
        }
        public string InsertInvHeader(String Kode, String Pengusaha, String Pembeli, String SDate, String EDate, String HComment, String FComment)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSFAKTURPAJAK " +
                    " (IDH, KODE, IDPENGUSAHA, IDPEMBELI, CREATEDBY, MODIFIEDBY ,CREATEDDATE, MODIFIEDDATE,  NOMODIFIED, STATUS, COMMENTHEADER,COMMENTFOOTER,STARTDATE,ENDDATE) " +
                    "VALUES ((select nvl(max(IDH),0) + 1 from kdsfakturpajak), '" + Kode + "', '" + Pengusaha + "', '" + Pembeli + "', '" + GlobalVar.GlobalVarUsername + "','" + GlobalVar.GlobalVarUsername + "', SYSDATE,  SYSDATE, '0', 'CREATED'," +
                    "  '" + HComment + "', '" + FComment + "' ,TO_DATE('" + SDate + "'), TO_DATE('" + EDate + "'))";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Insert Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert Header Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string UpdateInvHeaderStatus(String IDH, String SDate, String EDate, String HComment, String FComment, String Kode)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Update");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Update KDSFAKTURPAJAK set  COMMENTHEADER = '" + HComment + "', COMMENTFOOTER = '" + FComment + "' ,STARTDATE = TO_DATE('" + SDate + "'), ENDDATE = TO_DATE('" + EDate + "') , KODE = '" + Kode + "' where IDH = '" + IDH + "'  ";                    
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Update Header";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Update Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string insertParameterPembeli(String Short, String Long, String Type, String NPWP, String Alamat)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSPARAM (ID, SHORTDESC, LONGDESC, PRMVAR1,PRMVAR2,PRMVAR3, CREATEDBY, CREATEDDATE,NOMODIFIED) VALUES ((select Max(ID) +1 From KDSPARAM), '" + Short + "', '" + Long + "', '" + Type + "', '" + NPWP + "', '" + Alamat + "','" + GlobalVar.GlobalVarUserID + "' ,SYSDATE, 0)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Insert Param";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert param Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string UpdateParameterPembeli(String ID, String Short, String Long, String Type, String NPWP, String Alamat)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Update KDSPARAM set  SHORTDESC = '" + Short + "', LONGDESC = '" + Long + "', PRMVAR1 = '" + Type + "', PRMVAR2 = '" + NPWP + "', PRMVAR3 = '" + Alamat + "',MODIFIEDBY = '" + GlobalVar.GlobalVarUserID + "',MODIFIEDDATE = SYSDATE, NOMODIFIED = (NOMODIFIED + 1) where ID = '" + ID + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Update Param";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Update param Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
        public string insertParameter(String Short, String Long, String Type)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "INSERT INTO KDSPARAM (ID, SHORTDESC, LONGDESC, PRMVAR1, CREATEDBY, CREATEDDATE,NOMODIFIED) VALUES ((select Max(ID) +1 From KDSPARAM), '" + Short + "', '" + Long + "', '" + Type + "','" + GlobalVar.GlobalVarUserID + "' ,SYSDATE, 0)";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Insert Param";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("insert param Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }
      
        public string UpdateParameter(String ID, String Short, String Long, String Type)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Insert Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Update KDSPARAM set  SHORTDESC = '" + Short + "', LONGDESC = '" + Long + "', PRMVAR1 = '" + Type + "',MODIFIEDBY = '" + GlobalVar.GlobalVarUserID + "',MODIFIEDDATE = SYSDATE, NOMODIFIED = (NOMODIFIED + 1) where ID = '" + ID + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Update Param";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Update param Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }

        public string DeleteParameter(String ID)
        {
            String ErrorString;
            try
            {
                this.Connect();
                logger.Debug("Start Delete Parameter");
                OracleCommand cmd = new OracleCommand();
                OracleTransaction trans = con.BeginTransaction();
                cmd.Transaction = trans;
                cmd.Connection = con;
                cmd.CommandText =
                    "Delete KDSPARAM where ID = '" + ID + "' ";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                cmd.ExecuteNonQuery();

                trans.Commit();
                this.Close();
                ErrorString = "Success Delete Param";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("Delete param Function");
                logger.Error(e.Message);
                ErrorString = e.Message;
                this.Close();
                return ErrorString;
            }
        }


        public DataTable SelectDinvoiceEx(String NoDetailFaktur)
        {
            try
            {

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                            "SELECT " +
                            //"   IDDP, " +
                            //"   IDD, " +
                            "   (select LongDesc from KDSPARAM where PRMVAR1 = 'Detail' and ID =  PARAMID) BIAYA, " +
                            "   TO_CHAR (VALUE, '999,999,999,999.99') AMOUNT" +
                            //"   CREATEDBY, " +
                            //"   MODIFIEDBY, " +
                            //"   CREATEDDATE, " +
                            //"   MODIFIEDDATE, " +
                            //"   NOMODIFIED " +
                            " FROM " +
                            "   KDSTRXEXPINVOICE " +
                            " WHere IDD = '" + NoDetailFaktur + "' ";

                /*
                if (!string.IsNullOrWhiteSpace(SDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and SHORTDESC like '%' || :SDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":SDesc", OracleDbType.Varchar2)).Value = SDesc;
                }
                */

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Header Ex Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public DataTable SelectDinvoice(String NoFaktur)
        {
            try
            {

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                            "SELECT " +
                            //" IDD, " +
                            //" IDH, " +
                            "SKUID INVOICE, " +
                            "TO_CHAR (BRUTO, '999,999,999,999.99') as BRUTO , " +
                           // "TO_CHAR (NETTO, '999,999,999,999.99') as NETTO, " +
                            " COMMENTDETAIL \"COMMENT DETAIL\" " +
                          //  " TO_CHAR (DISCBRUTO, '999,999,999,999.99')   \"DISC BRUTO\"," +
                           // " TO_CHAR (DISCNETTO, '999,999,999,999.99')   \"DISC NETTO\"" +
                          //  " CREATEDBY, " +
                          //  " MODIFIEDBY, " +
                          //  " CREATEDDATE, " +
                          //  " MODIFIEDDATE " +
                            //" NOMODIFIED " +
                            "FROM " +
                            " KDSTRXINVOICE " +
                            "WHere IDH = '" + NoFaktur + "' ";

                /*
                if (!string.IsNullOrWhiteSpace(SDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and SHORTDESC like '%' || :SDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":SDesc", OracleDbType.Varchar2)).Value = SDesc;
                }
                */

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Header Ex Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public DataTable SelectHinvoiceEx(String NoFaktur)
        {
            try
            {

                this.Connect();    
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                            "SELECT " +
                           // "   IDHP ID, " +
                            //"   IDH, " +
                            "   (select LongDesc from KDSPARAM where PRMVAR1 = 'Header' and ID =  PARAMID) Biaya, " +
                            "    TO_CHAR (VALUE, '999,999,999,999.99') Amount , " +
                            "    TO_CHAR (VALUE, '999,999,999,999.99') AMOUNTVISIBLE " +
                           // "   CREATEDBY, " +
                           // "   MODIFIEDBY, " +
                           // "   CREATEDDATE, " +
                           // "   MODIFIEDDATE " +
                            "FROM " +
                            "   KDSEXPENSEFAKTURPAJAK " +
                            "WHere IDH = '" + NoFaktur + "'";                                
                
                /*
                if (!string.IsNullOrWhiteSpace(SDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and SHORTDESC like '%' || :SDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":SDesc", OracleDbType.Varchar2)).Value = SDesc;
                }
                */

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Header Ex Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public DataTable SelectHinvoice(String Kode, String Pengusaha, String Pembeli)
        {
            try
            {

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                            "SELECT " +
                            " IDH, " +
                            "KODE, " +
                            "TO_CHAR ((select nvl(SUM(BRUTO),0) from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL INVOICE\", " +
                            "TO_CHAR ((select nvl(sum(value),0) from KDSTRXEXPINVOICE where IDD in ( select IDD from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH)), '999,999,999,999.99') \"TOTAL BIAYA INVOICE\", " +
                            "(SELECT  foulibl from foudgene  where foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA) PENGUSAHA , " +
                            "(select LONGDESC from KDSPARAM where ID = KDSFAKTURPAJAK.IDPEMBELI) PEMBELI , " +
                            "STATUS, " +
                            "COMMENTHEADER as \"COMMENT HEADER\", " +
                            "COMMENTFOOTER as \"COMMENT FOOTER\"" +
                            //" CREATEDBY, " +
                            //" MODIFIEDBY, " +
                            //" CREATEDDATE, " +
                            //" MODIFIEDDATE " +
                            //" NOMODIFIED " + 
                            " FROM " +
                            "   KDSFAKTURPAJAK " +
                            " WHere IDH = IDH and STATUS in ('CREATED','CONFIRM EDIT') and CREATEDBY = '"+ GlobalVar.GlobalVarUsername + "'";


                if (!string.IsNullOrWhiteSpace(Kode))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and KODE like '%' || :Kode || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":Kode", OracleDbType.Varchar2)).Value = Kode;
                }
                if (!string.IsNullOrWhiteSpace(Pengusaha))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and IDPENGUSAHA like '%' || :Pengusaha || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":Pengusaha", OracleDbType.Varchar2)).Value = Pembeli;
                }
                if (!string.IsNullOrWhiteSpace(Pembeli))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and IDPEMBELI like '%' || :Pembeli || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":Pembeli", OracleDbType.Varchar2)).Value = Pengusaha;
                }

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Header Ex Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }
        public DataTable SelectInvoiceSlip(FakturPajakSearch FPSearch, String Status)
        {
            try
            {

                this.Connect();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                if (Status == "New")
                {
                    cmd.CommandText =
                                "SELECT " +
                                " IDH, " +
                                "KODE, " +
                                "TO_CHAR ((select nvl(SUM(BRUTO),0) from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL INVOICE\", " +
                                "TO_CHAR ((select nvl(sum(value),0) from KDSTRXEXPINVOICE where IDD in ( select IDD from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH)), '999,999,999,999.99') \"TOTAL BIAYA INVOICE\", " +
                                "TO_CHAR ((select nvl(sum(value),0) from KDSEXPENSEFAKTURPAJAK where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL BIAYA FAKTUR\", " +
                                "(SELECT  foulibl from foudgene  where foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA) PENGUSAHA , " +
                                "(select LONGDESC from KDSPARAM where ID = KDSFAKTURPAJAK.IDPEMBELI) PEMBELI , " +
                                "STATUS, " +
                                "COMMENTHEADER as \"COMMENT HEADER\", " +
                                "COMMENTFOOTER as \"COMMENT FOOTER\" ," +
                                " FLAG " +
                                " FROM " +
                                "   KDSFAKTURPAJAK " +
                                " WHere STATUS in ('CONFIRM','CONFIRM EDIT') " +
                                " and STARTDATE >= :FromDate " +
                                " and ENDDATE <= :ToDate and (FLAG is null OR FLAG != 9)  and IDPENGUSAHA = '" + FPSearch.IDPENGUSAHA + "' and IDPEMBELI = '" + FPSearch.IDPEMBELI + "'";
                }
                else
                {
                    cmd.CommandText =
                               " SELECT " +
                               " IDH, " +
                               " KODE, " +
                               " TO_CHAR ((select nvl(SUM(BRUTO),0) from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL INVOICE\", " +
                               " TO_CHAR ((select nvl(sum(value),0) from KDSTRXEXPINVOICE where IDD in ( select IDD from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH)), '999,999,999,999.99') \"TOTAL BIAYA INVOICE\", " +
                               " TO_CHAR ((select nvl(sum(value),0) from KDSEXPENSEFAKTURPAJAK where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL BIAYA FAKTUR\", " +
                               " (SELECT  foulibl from foudgene  where foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA) PENGUSAHA , " +
                               " (select LONGDESC from KDSPARAM where ID = KDSFAKTURPAJAK.IDPEMBELI) PEMBELI , " +
                               " STATUS, " +
                               " COMMENTHEADER as \"COMMENT HEADER\" , " +
                               " COMMENTFOOTER as \"COMMENT FOOTER\" ," +
                               " FLAG " +
                               " FROM " +
                               " KDSFAKTURPAJAK " +
                               " WHere STATUS in ('CONFIRM','CONFIRM EDIT') " +
                               " and STARTDATE >= :FromDate and ENDDATE <= :ToDate " +
                               " and IDBAYAR = '" + GlobalVar.GlobalVarKodeInvoice + "' and IDPENGUSAHA = '" + FPSearch.IDPENGUSAHA + "' and IDPEMBELI = '" + FPSearch.IDPEMBELI + "' " +
                               " UNION ALL " +
                               " SELECT " +
                               " IDH, " +
                               " KODE, " +
                               " TO_CHAR ((select nvl(SUM(BRUTO),0) from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL INVOICE\", " +
                               " TO_CHAR ((select nvl(sum(value),0) from KDSTRXEXPINVOICE where IDD in ( select IDD from KDSTRXINVOICE where IDH = KDSFAKTURPAJAK.IDH)), '999,999,999,999.99') \"TOTAL BIAYA INVOICE\", " +
                               " TO_CHAR ((select nvl(sum(value),0) from KDSEXPENSEFAKTURPAJAK where IDH = KDSFAKTURPAJAK.IDH), '999,999,999,999.99') \"TOTAL BIAYA FAKTUR\", " +
                               " (SELECT  foulibl from foudgene  where foucnuf = KDSFAKTURPAJAK.IDPENGUSAHA) PENGUSAHA , " +
                               " (select LONGDESC from KDSPARAM where ID = KDSFAKTURPAJAK.IDPEMBELI) PEMBELI , " +
                               " STATUS, " +
                               " COMMENTHEADER as \"COMMENT HEADER\", " +
                               " COMMENTFOOTER as \"COMMENT FOOTER\" ," +
                               " FLAG " +
                               " FROM " +
                               " KDSFAKTURPAJAK " +
                               " WHere STATUS in ('CONFIRM','CONFIRM EDIT') " +
                               " and STARTDATE >= :FromDate " +
                               " and ENDDATE <= :ToDate  and (FLAG is null OR FLAG != 9)  and IDPENGUSAHA = '" + FPSearch.IDPENGUSAHA + "' and IDPEMBELI = '" + FPSearch.IDPEMBELI + "' ";

                }
                cmd.Parameters.Add(new OracleParameter(":FromDate", OracleDbType.Date)).Value = FPSearch.StartDate.Date;
                cmd.Parameters.Add(new OracleParameter(":ToDate", OracleDbType.Date)).Value = FPSearch.EndDate.Date;

               

                logger.Debug(cmd.CommandText);
                OracleDataReader dr = cmd.ExecuteReader();

                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Header Ex Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

        public DataTable SelectParamInvoice(String SDesc, String LDesc, String Type)
        {
            try
            {

                this.Connect();    
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;

                cmd.CommandText =
                    "SELECT ID, " +
                    "SHORTDESC as ShortDescription, " +
                    "LONGDESC AS LongDescription, " +
                    "PRMVAR1 AS Type, " +
                    "PRMVAR2 AS NPWP, " +
                    "PRMVAR3 AS Alamat " +
                    //"CREATEDBY, " +
                    //"MODIFIEDBY, " +
                    //"CREATEDDATE, " +
                    //"MODIFIEDDATE " +
                    "FROM KDSPARAM " +
                    "WHERE ID = ID ";                                
                
                if (!string.IsNullOrWhiteSpace(SDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and SHORTDESC like '%' || :SDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":SDesc", OracleDbType.Varchar2)).Value = SDesc;
                }

                if (!string.IsNullOrWhiteSpace(LDesc))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and LONGDESC like '%' || :LDesc || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":LDesc", OracleDbType.Varchar2)).Value = LDesc;
                }

                if (!string.IsNullOrWhiteSpace(Type))
                {
                    cmd.CommandText = cmd.CommandText +
                                      " and PRMVAR1 like '%' || :Type || '%' ";
                    cmd.Parameters.Add(new OracleParameter(":Type", OracleDbType.Varchar2)).Value = Type;
                }

                //cmd.Parameters.Add(new OracleParameter(":ProfileId", OracleDbType.Varchar2)).Value = ProfileID;





                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();


                DataTable DT = new DataTable();
                DT.Load(dr);
                this.Close();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("Select Parameter Function");
                logger.Error(e.Message);
                this.Close();
                return null;
            }

        }

    }
}
