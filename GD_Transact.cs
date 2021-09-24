using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;



namespace GD_SOAP
{
    [DataContract]
    public class response
    {
        [DataMember]
        public string statuscode { get; set; }
        [DataMember]
        public object data { get; set; }
    }
    public class buy_rate
    {
        public string Responsecode { get; set; }
        public buying_rates[] Message { get; set; }
    }
 
    public class Purchase
    {
        public string Responsecode { get; set; }
        public string Message { get; set; }
    }
    public class verify
    {
        public string Responsecode { get; set; }
        public customers[] Data { get; set; }
        public string Message { get; set; }
    }
    public class Sale
    {
        public string Responsecode { get; set; }
        public string Message { get; set; }
    }
    
    [DataContract]
    public class GD_Transact : IGDConnect
    {
        public string orpath = ConfigurationSettings.AppSettings["Orcl"];
        
        public products[] GetProducts(string devkey)
        {
            DataTable dto = new DataTable();
            OracleConnection con = new OracleConnection(orpath);
            products[] tt = new products[1];
            OracleCommand com = new OracleCommand("SELECT productid,description,product_map from products", con);
          
            com.CommandType = CommandType.Text;
            try
            {
                con.Open();
                DataTable actsds = new DataTable();
                OracleDataAdapter actsda = new OracleDataAdapter(com);
                // OracleDataReader ordr = com.ExecuteReader();

                actsda.Fill(actsds);
                int r = 0;
                products[] prods = new products[actsds.Rows.Count];
                while (r<actsds.Rows.Count)
                {
                //Console.WriteLine(Convert.ToInt16(ordr[0]));
                //Console.WriteLine(Convert.ToInt16(ordr[0]));
                prods[r] = new products
                {
                    productid = Convert.ToInt16(actsds.Rows[r][0]),
                    description = actsds.Rows[r][1].ToString(),
                    productmap = Convert.ToInt16(actsds.Rows[r][2])

                };
                r += 1;
                
                }
           
               // Console.WriteLine(r);
                if (r == 0)
                {
                    tt[0] = new products
                    {
                        description = "Something Happened:: Nothing found" 
                    };

                   // Console.WriteLine(tt[0].description);

                    return tt;
                }
            con.Close();
            return prods;
           
        }
            catch (Exception ed)
            {
               // return new products;
                tt[0] = new products
                {
                    description = "Something Happened::" + ed.Message
    };

               // Console.WriteLine(tt[0].description);

                return tt;
            }
            // return tt;
        }
           
        bool authenticatekey(string akey)
        {
            using (OracleConnection con = new OracleConnection(orpath))
            {
                using (OracleCommand commnd = new OracleCommand("Select dkey from devkeys where dkey='" +
                    akey + "' and kstatus=1", con))
                {
                    //commnd.CommandType = CommandType.StoredProcedure;
                    //commnd.Parameters.Add("@accountnum", OracleDbType.VarChar, 50);
                    //commnd.Parameters.AddWithValue("@sessionid", sessionid);
                    try
                    {
                        con.Open();
                        OracleDataReader dr = commnd.ExecuteReader();
                        while (dr.Read())
                        {
                            return true;
                        }
                        return false;
                    }
                    catch (Exception km)
                    {
                        var er = new
                        {
                            Responsecode = "01",
                            Message = "Internal Server Error. Call Support." + km.Message
                        };
                        // Console.WriteLine("{0} {1}", DateTime.Now, km.Message);
                        return false;
                    }
                }
            }
            //return false;
        }
        public BankSecurities GetSecuritiesforsale(int start_day, int end_day, string debt_type) // Available Securities for Sale
        {
            DataTable dto = new DataTable();
            OracleConnection con = new OracleConnection(orpath);

            OracleCommand com = new OracleCommand("Select due_date,days_to_maturity,round(max_settlement_amount,2),round(face_value,2),round(market_rate,2)"+
                ",discount_bearing_deal_id,upper_bucket,lower_bucket,sectenor,debt_type from bank_securities_forsale " +
                "where days_to_maturity between to_number("+start_day+") And to_number("+end_day+") And debt_type='"+debt_type+"'", con);

            com.CommandType = CommandType.Text;
            try
            {
                con.Open();
                DataTable sectab= new DataTable();
                OracleDataAdapter actsda = new OracleDataAdapter(com);
                //List<accounts>  acts = new List<accounts>();
                actsda.Fill(sectab);
                // DataRow[] results = dto.Select("customerid=" + cusid);
                con.Close();

                int th = 0;
                int recount = sectab.Rows.Count;
                securities[] dtsec4sale = new securities[recount];
                while (th < recount)
                {
                    dtsec4sale[th] = new securities
                    {
                        due_date = sectab.Rows[th][0].ToString(),
                        days_to_maturity = Convert.ToInt16(sectab.Rows[th][1].ToString()),
                        market_value = Convert.ToDecimal(sectab.Rows[th][2].ToString()),
                        face_value = Convert.ToDecimal(sectab.Rows[th][3].ToString()),
                        Market_rate = Convert.ToDecimal(sectab.Rows[th][4]),
                        Discount_bearing_deal_id = sectab.Rows[th][5].ToString(),
                        upper_bucket = Convert.ToInt16(sectab.Rows[th][6].ToString()),
                        lower_bucket = Convert.ToInt16(sectab.Rows[th][7].ToString()),
                        sectenor = sectab.Rows[th][8].ToString(),
                        debt_type= sectab.Rows[th][9].ToString()

                    };
                    th += 1;
                }
                BankSecurities BSec;
                if (th > 0)
                {
                    BSec = new BankSecurities
                    {
                        Responsecode="00",
                        Data= dtsec4sale,
                        Message="SUCCESS"
                    };
                }
                else
                {
                    BSec = new BankSecurities
                    {
                        Responsecode = "201",
                        Data = null,
                        Message = "NO RECORD FOUND"
                    };
                }
                return  BSec;


            }
            catch (Exception km)
            {
                BankSecurities secerr = new BankSecurities
                {
                    Responsecode = "200",
                    Data = null,
                    Message = km.Message
                };
                return secerr;
            }


        }
        Object getcustomerproducts(string cusid, string pin)
        {
            DataTable dto = new DataTable();
            OracleConnection con = new OracleConnection(orpath);
//            SELECT a.fullname, null branchid , a.product_description description, a.productid, sum(a.settlememt_amount) as value from accounts a
//group by a.productid, a.fullname, a.product_description


            OracleCommand com = new OracleCommand("SELECT a.fullname, null branchid , a.description product_description, a.productid, sum(a.settlememt_amount),a.debt_type as value from accounts a" +
                //"inner join client_product_accounts ac on a.customerid= ac.new_customer_number " +
                //"Inner join products p on a.productid= p.productid " +
                //"inner join customers c on a.customerid= c.customerid " +
                //"where c.CUSTOMERID=TO_NUMBER(" + cusid + ") " +
                "group by a.productid, a.fullname, a.description ", con);

            com.CommandType = CommandType.Text;
            try
            {
                con.Open();
                DataTable actsds = new DataTable();
                OracleDataAdapter actsda = new OracleDataAdapter(com);
                //List<accounts>  acts = new List<accounts>();
                actsda.Fill(actsds);
                // DataRow[] results = dto.Select("customerid=" + cusid);
                con.Close();
                actsds.Dispose();
                if (actsds.Rows.Count == 0)
                {
                    var norec = new
                    {
                        Statuscode = "01",
                        Data = "no product(s) exist for customer."
                    };
                    return norec;
                }
                var validrecs = new
                {
                    Statuscode = "00",
                    Data = actsds
                };
                return validrecs;


            }
            catch (Exception km)
            {
                var er = new
                {
                    Statuscode = "03",
                    Data = "Getcustomerproducts()::Internal Server Error. Call Support." + km.Message
                };
                return er;
            }

        }
        public buy_rate getbuying_rate(int rec)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            
            DataTable dtbuyrate = new DataTable();
            OracleConnection con = new OracleConnection(orpath);

            OracleCommand com = new OracleCommand("Select upper_bucket,lower_bucket, currency_code,round(market_rate,4) as market_rate,"+
                "approval_status,trading_date,yield_or_discount,"+
                "special_rate,debt_type,price_band from  buying_rate ", con);
            com.CommandType = CommandType.Text;
            try
            {
                con.Open();
                OracleDataAdapter adbrates = new OracleDataAdapter(com);
                adbrates.Fill(dtbuyrate);
                con.Close();

                int recs = 0;
                int rcound = dtbuyrate.Rows.Count;
                buying_rates[] buy = new buying_rates[rcound];
                Console.WriteLine(dtbuyrate.Rows.Count);
                while (recs < rcound)
                {
                    buy[recs] = new buying_rates
                    {
                        upper_bucket = Convert.ToDecimal(dtbuyrate.Rows[recs][0]),
                        lower_bucket = Convert.ToDecimal(dtbuyrate.Rows[recs][1]),
                        currency_code = dtbuyrate.Rows[recs][2].ToString(),
                        market_rate = Convert.ToDecimal(dtbuyrate.Rows[recs][3]),
                        approval_status = dtbuyrate.Rows[recs][4].ToString(),
                        trading_date = dtbuyrate.Rows[recs][5].ToString(),
                        yield_or_discount = dtbuyrate.Rows[recs][6].ToString(),
                        special_rate = getdec(dtbuyrate.Rows[recs][7]),
                        debt_type = dtbuyrate.Rows[recs][8].ToString(),
                        price_band = dtbuyrate.Rows[recs][9].ToString()

                    };
                    recs += 1;
                }
                if (recs > 0)
                {
                   buy_rate response = new buy_rate
                    {
                        Responsecode = "00",
                        Message = buy

                    };
                return response;
                }

                loginf.Info("No Records");
                buy_rate resp = new buy_rate
                {
                    Responsecode = "201",
                    Message =null
                };
                return resp;
                

            }
            catch(Exception byr)
            {
                loginf.Info($"Request:{byr.Message}");
                buy_rate erresponse = new buy_rate
                {
                    Responsecode = "200",
                    Message = null
                };
                return erresponse;
            }

        }
       public decimal getdec(object d)
        {
            if (d == System.DBNull.Value) return 0;
            return (decimal)d;
        }
        public Customerbalance GetCustomerBalanceEnq(string Accountnumber)// Get Customer Balances
        {
            //DataTable dto = new DataTable();
            OracleConnection con = new OracleConnection(orpath);
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Request:{Accountnumber}");
            //Select a.productid,c.fullname,c.branchid,p.description,c.customerid from customers c
            //inner join client_product_accounts a on c.customerid = a.new_customer_number
            //Inner join products p on a.productid = p.productid
            //where c.CUSTOMERID = TO_NUMBER(404971)


            OracleCommand com = new OracleCommand("Select ac.startdate,ac.description,round(ac.Amount_invested,4),round(ac.Transaction_rate,4),ac.days_to_maturity As Tenure,"+
                "round(ac.int_accrued,4),round(ac.current_value,4),round(ac.maturity_value,4),ac.due_date,ac.days_to_maturity,ac.max_available,round(ac.market_rate,4),"+
                "ac.DISCOUNT_BEARING_DEAL_ID,ac.lien_amount,ac.debt_type" +
                " from accounts ac where ac.accountnum='" + Accountnumber+ "'", con);
            com.CommandType = CommandType.Text;
            DataTable cusacts = new DataTable();

            try
            {
                con.Open();
                
                OracleDataAdapter actsda = new OracleDataAdapter(com);
                //List<accounts>  acts = new List<accounts>();
                actsda.Fill(cusacts);
                // DataRow[] results = dto.Select("customerid=" + cusid);
                con.Close();
                // actsds.Dispose();
                int y = 0;
                int acto = cusacts.Rows.Count;
                accounts[] Custo = new accounts[acto];
               // if 
                while (y< acto)
                {
                    Custo[y] = new accounts
                    {
                        Trans_date= cusacts.Rows[y][0].ToString(),
                        Product = cusacts.Rows[y][1].ToString(),
                        Principal = Convert.ToDecimal(cusacts.Rows[y][2].ToString()),
                        Rate = Convert.ToDecimal(cusacts.Rows[y][3].ToString()),
                        Tenure = Convert.ToInt16(cusacts.Rows[y][4]),
                        Interest = Convert.ToDecimal(cusacts.Rows[y][5]),
                        Current_Value = Convert.ToDecimal(cusacts.Rows[y][6].ToString()),
                        Maturity_Value = Convert.ToDecimal(cusacts.Rows[y][7].ToString()),
                        Due_date=cusacts.Rows[y][8].ToString(),
                        Days_to_Maturity = Convert.ToInt16(cusacts.Rows[y][9]),
                        Market_Value = Convert.ToDecimal(cusacts.Rows[y][10]),
                        Market_Rate = Convert.ToDecimal(cusacts.Rows[y][11]),
                        Security_id=cusacts.Rows[y][12].ToString(),
                        Discount_bearing_deal_id=cusacts.Rows[y][12].ToString(),
                        Lien_Amount= Convert.ToDecimal(cusacts.Rows[y][13]),
                        Debt_type=cusacts.Rows[y][14].ToString()
                        //Upper_Bucket=Convert.ToInt16(cusacts.Rows[y][12].ToString()),
                        //Lower_Bucket=Convert.ToInt16(cusacts.Rows[y][13].ToString()),
                        //sectenor= cusacts.Rows[y][14].ToString()


                    };
                y += 1;
                }
                Customerbalance resp;
                if (y > 0)
                {
                 resp = new Customerbalance
                {
                    Responsecode="00",
                    Data=Custo,
                    Message="SUCCESS"
                };
                }
                else
                {
                    resp = new Customerbalance
                    {
                        Responsecode = "104",
                        Data = null,
                        Message = "NO RECORD FOUND!"
                    };
                }

                loginf.Info($"Response::{resp}");
                return resp;


            }
            catch (Exception km)
            {
                Customerbalance Err = new Customerbalance
                {
                    Responsecode = "200",
                    Data = null,
                    Message = km.Message
                };

                return Err;
            }



        }
        public  Purchase Customer_onboarding(onboarding Request_Details)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"New Onboarding:{Request_Details.cbs_customer_number}");

            // nationality_code varchar2(10)      null ,--flexcube or manual update
            // phone_number varchar2(15)      null ,--flexcube or app
            //email                   varchar2(50)           ,--flexcube or app
            //occupation              varchar2(100)          ,--app
            //residential_status_code  varchar2(10)    null  ,--app values are 'RG' - Resident Ghanaian, 'RF' - Resident Foreigner, 'NRG' - Non Resident Ghanaian , 'NRF' - Non Resident Foreigner
            //customer_photo          blob                   ,--app
            //customer_signature      blob                   ,--app
            //nok_name                varchar2(100)          ,--app
            //nok_phone               varchar2(15)           ,--app
            //nok_relationship        varchar2(50)           ,--app
            //nok_address             varchar2(100)          ,--app
            //maturity_instruction    number(3)              ,--app -- 1 full rollver, 6-- full payment, 4 - principal rollover, 5 , interest rollover
            // date_Received date              null ,
            // gender char (1)       null ,
            // client_code varchar2(10)  null, 
            // rm_code varchar2(10)  null,  --flexcube
            //rm_name                          varchar2(100)  null, --flexcube
            onboarding ob = Request_Details;
            string commandText = "Insert into ap_customer_info(cbs_customer_number,account_number,account_branch,security_type,lower_tenor,upper_tenor,amount,"+
                "title_code,first_name,surname,other_name,Address_Post_box,Address_city,Address_country,Id_type_code,"+
                "Id_Number,expiry_date,place_of_issue,dob,country_code,"+
                "nationality_code,phone_number,email,occupation,residential_status_code,nok_name,nok_phone,nok_relationship,"+
                "nok_address,maturity_instruction,gender" +
                ",client_code,rm_code,rm_name) "+
                "Values(:cbs_customer_number,:account_number,:account_branch,:security_type,:lower_tenor,:upper_tenor,:amount,"+ //done

                ":title_code,:first_name,:surname,:other_name,:Address_Post_box,:Address_city,:Address_country,:Id_type_code,"+
                ":Id_Number,:expiry_date,:place_of_issue,:dob,:country_code"+
                ",:nationality_code,:phone_number,:email,:occupation,:residential_status_code,:nok_name,:nok_phone,:nok_relationship,"+
                ":nok_address,:maturity_instruction,:gender," +
                ":client_code,:rm_code,:rm_name)";

                                    OracleConnection con = new OracleConnection(orpath);
            OracleCommand commnd = new OracleCommand(commandText, con);
            //commnd.CommandType = CommandType.StoredProcedure;
            OracleParameter cbscusno = commnd.Parameters.Add(":cbs_customer_number", OracleDbType.NVarchar2,17);
            cbscusno.Value = ob.cbs_customer_number;
            OracleParameter acct_no = commnd.Parameters.Add(":account_number", OracleDbType.NVarchar2, 17);
            acct_no.Value = ob.Account_Number;
            OracleParameter act_branch = commnd.Parameters.Add(":account_branch", OracleDbType.NVarchar2, 10);
            act_branch.Value = ob.account_branch;
            OracleParameter sec_type = commnd.Parameters.Add(":security_type", OracleDbType.NVarchar2, 2);
            sec_type.Value = ob.security_type;

            OracleParameter lowert = commnd.Parameters.Add(":lower_tenor", OracleDbType.Int16, 10);
            lowert.Value = ob.Lower_tenor;
            OracleParameter uppert = commnd.Parameters.Add(":upper_tenor", OracleDbType.Int16, 10);
            uppert.Value = ob.Upper_tenor;
            OracleParameter amt = commnd.Parameters.Add(":amount", OracleDbType.Decimal);
            amt.Value = ob.amount;
          //line 1
            OracleParameter tcode = commnd.Parameters.Add(":title_code", OracleDbType.NVarchar2, 10);
            tcode.Value = ob.title_code;
            OracleParameter fname = commnd.Parameters.Add(":first_name", OracleDbType.NVarchar2, 100);
            fname.Value = ob.first_name;
            OracleParameter sname = commnd.Parameters.Add(":surname", OracleDbType.NVarchar2, 100);
            fname.Value = ob.surname;
            OracleParameter oname = commnd.Parameters.Add(":other_name", OracleDbType.NVarchar2, 100);
             oname.Value = ob.other_name;
            OracleParameter adpost = commnd.Parameters.Add(":Address_Post_box", OracleDbType.NVarchar2, 50);
            adpost.Value = ob.Address_Post_box;
            OracleParameter adcity = commnd.Parameters.Add(":Address_city", OracleDbType.NVarchar2, 50);
            adpost.Value = ob.Address_city;
            OracleParameter adcountry = commnd.Parameters.Add(":Address_country", OracleDbType.NVarchar2, 50);
            adpost.Value = ob.Address_Country;
            OracleParameter idtyp = commnd.Parameters.Add(":Id_type_code", OracleDbType.NVarchar2, 10);
            idtyp.Value = ob.Id_type_code;
            OracleParameter idno = commnd.Parameters.Add(":Id_Number", OracleDbType.NVarchar2, 25);
            idno.Value = ob.Id_Number;
            OracleParameter expdte = commnd.Parameters.Add(":expiry_date", OracleDbType.Date);
            if (ob.expiry_date == string.Empty)
            {
                ob.expiry_date = "1900-01-01";
            }
            expdte.Value = Convert.ToDateTime(ob.expiry_date);
            OracleParameter plofi = commnd.Parameters.Add(":place_of_issue", OracleDbType.NVarchar2,50);
            plofi.Value = ob.place_of_issue;
            OracleParameter dob = commnd.Parameters.Add(":dob", OracleDbType.Date);
            if (ob.dob == string.Empty)
            {
                ob.dob = "1900-01-01";
            }
            dob.Value = Convert.ToDateTime(ob.dob);
            OracleParameter ccode = commnd.Parameters.Add(":country_code", OracleDbType.NVarchar2,10);
            ccode.Value = ob.country_code;

             OracleParameter nat_code = commnd.Parameters.Add(":nationality_code", OracleDbType.NVarchar2, 10);
            nat_code.Value = ob.nationality_code;
            OracleParameter phone = commnd.Parameters.Add(":phone_number", OracleDbType.NVarchar2, 15);
            phone.Value = ob.phone_number;
            OracleParameter email = commnd.Parameters.Add(":email", OracleDbType.NVarchar2, 50);
            email.Value = ob.email;
            OracleParameter occup = commnd.Parameters.Add(":occupation", OracleDbType.NVarchar2, 100);
            occup.Value = ob.occupation;
            OracleParameter res_stats = commnd.Parameters.Add(":residential_status_code", OracleDbType.NVarchar2, 10);
            res_stats.Value = ob.residential_status_code;
            OracleParameter nok_n = commnd.Parameters.Add(":nok_name", OracleDbType.NVarchar2, 100);
            nok_n.Value = ob.nok_name;
            OracleParameter nok_ph = commnd.Parameters.Add(":nok_phone", OracleDbType.NVarchar2, 15);
            nok_ph.Value = ob.nok_phone;
            OracleParameter nok_rel = commnd.Parameters.Add(":nok_relationship", OracleDbType.NVarchar2, 50);
            nok_rel.Value = ob.nok_relationship;
            OracleParameter nok_ad = commnd.Parameters.Add(":nok_address", OracleDbType.NVarchar2, 100);
            nok_ad.Value = ob.nok_address;
            OracleParameter mat_inst = commnd.Parameters.Add(":maturity_instruction", OracleDbType.NVarchar2,3);
            mat_inst.Value = ob.maturity_instruction;
            OracleParameter gender = commnd.Parameters.Add(":gender", OracleDbType.NVarchar2, 1);
            gender.Value = ob.gender;
            
            OracleParameter client_c = commnd.Parameters.Add(":client_code", OracleDbType.NVarchar2, 10);
            client_c.Value = ob.client_code;
            OracleParameter rm_co = commnd.Parameters.Add(":rm_code", OracleDbType.NVarchar2, 10);
            rm_co.Value = ob.rm_code;
            OracleParameter rm_nm = commnd.Parameters.Add(":rm_name", OracleDbType.NVarchar2, 100);
            rm_nm.Value = ob.rm_name;
            try
            {
                con.Open();
                OracleDataAdapter da = new OracleDataAdapter(commnd);
            commnd.ExecuteNonQuery();
            // return dt; // sonConvert.SerializeObject(dt);
            con.Close();
                Purchase purs = new Purchase
                {
                    Responsecode = "00",
                    Message = "SUCCESS"
                };
                return purs;
        }
            catch (Exception km)
            {
                // throw;
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"{DateTime.Now}:An Error Occured. {km.Message}");
                //Console.ResetColor();
                loginf.Info($"Unhandled Exception::{km.Message}");
                Purchase purserr = new Purchase
                {
                    Responsecode = "200",
                    Message = km.Message
                };
                return purserr;
            }
        }
        public Customerbalance GetAccountdetails(string Accountnumber) 
        {
            OracleConnection con = new OracleConnection(orpath);
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Request:{Accountnumber}");
            //Select a.productid,c.fullname,c.branchid,p.description,c.customerid from customers c
            //inner join client_product_accounts a on c.customerid = a.new_customer_number
            //Inner join products p on a.productid = p.productid
            //where c.CUSTOMERID = TO_NUMBER(404971)

//            DEBT_TYPE,
//ISSUER_CODE,
//TRANSACTION_DATE,
//PURCHASE_RATE
            OracleCommand com = new OracleCommand("Select ac.startdate,ac.description,round(ac.Amount_invested,4),"+
                "round(ac.Transaction_rate,4),ac.days_to_maturity As Tenure," +
                "round(ac.int_accrued,4),round(ac.current_value,4),round(ac.maturity_value,4),ac.due_date,"+
                "ac.days_to_maturity,ac.face_value,round(ac.market_rate,4)," +
                "ac.DISCOUNT_BEARING_DEAL_ID,Debt_type,Issuer_code,Transaction_date,"+
                "round(Purchase_rate,4),lien_amount from accounts_details ac where ac.accountnum='" + Accountnumber + "'", con);
            com.CommandType = CommandType.Text;
            DataTable cusacts = new DataTable();

            try
            {
                con.Open();

                OracleDataAdapter actsda = new OracleDataAdapter(com);
                //List<accounts>  acts = new List<accounts>();
                actsda.Fill(cusacts);
                // DataRow[] results = dto.Select("customerid=" + cusid);
                con.Close();
                // actsds.Dispose();
                int y = 0;
                int acto = cusacts.Rows.Count;
                accounts[] Custo = new accounts[acto];
                // if 
                while (y < acto)
                {
                    Custo[y] = new accounts
                    {
                        Trans_date = cusacts.Rows[y][0].ToString(),
                        Product = cusacts.Rows[y][1].ToString(),
                        Principal = Convert.ToDecimal(cusacts.Rows[y][2].ToString()),
                        Rate = Convert.ToDecimal(cusacts.Rows[y][3].ToString()),
                        Tenure = Convert.ToInt16(cusacts.Rows[y][4]),
                        Interest = Convert.ToDecimal(cusacts.Rows[y][5]),
                        Current_Value = Convert.ToDecimal(cusacts.Rows[y][6].ToString()),
                        Maturity_Value = Convert.ToDecimal(cusacts.Rows[y][7].ToString()),
                        Due_date = cusacts.Rows[y][8].ToString(),
                        Days_to_Maturity = Convert.ToInt16(cusacts.Rows[y][9]),
                        Market_Value = Convert.ToDecimal(cusacts.Rows[y][10]),
                        Market_Rate = Convert.ToDecimal(cusacts.Rows[y][11]),
                        Security_id = cusacts.Rows[y][12].ToString(),
                        Discount_bearing_deal_id = cusacts.Rows[y][12].ToString(),
                        Debt_type=cusacts.Rows[y][13].ToString(),
                        Issuer_code=cusacts.Rows[y][14].ToString(),
                        Transaction_date= cusacts.Rows[y][15].ToString(),
                        Purchase_Rate= Convert.ToDecimal(cusacts.Rows[y][11]),
                        Lien_Amount=Convert.ToDecimal(cusacts.Rows[y][10])

                    };
                    y += 1;
                }
                Customerbalance resp;
                if (y > 0)
                {
                    resp = new Customerbalance
                    {
                        Responsecode = "00",
                        Data = Custo,
                        Message = "SUCCESS"
                    };
                }
                else
                {
                    resp = new Customerbalance
                    {
                        Responsecode = "104",
                        Data = null,
                        Message = "NO RECORD FOUND!"
                    };
                }

                loginf.Info($"Response::{resp}");
                return resp;


            }
            catch (Exception km)
            {
                Customerbalance Err = new Customerbalance
                {
                    Responsecode = "200",
                    Data = null,
                    Message = km.Message
                };

                return Err;
            }

        }
        public verify verifycustomer(string customernum)
        {
            DataTable cust = new DataTable();
            //verify verifyaccount=null;
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Request:{customernum}");
            OracleConnection vcon = new OracleConnection(orpath);
            
            string sqltxt="Select account_number,client_id,account_branch_code,primary_account_holder," +
                "cbs_customer_number from customer_account_numbers where cbs_customer_number='" + customernum + "'";
            OracleCommand oCom = new OracleCommand(sqltxt, vcon);
            oCom.CommandType = CommandType.Text;
            try
            {
                vcon.Open();
                
                OracleDataAdapter vact = new OracleDataAdapter(oCom);
                 vact.Fill(cust);
                  vcon.Close();
                int c = 0;
                // string rescode = "104";
                int recs = cust.Rows.Count;
                customers[] vercus = new customers[recs];
                while (c<recs)
                    {

                        //Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine($"{customernum}: Results:");
                        //Console.ResetColor();
                        vercus[c] = new customers
                        {
                            AccountNumber = cust.Rows[c][0].ToString(),
                            client_id = cust.Rows[c][1].ToString(),
                            branch_code = cust.Rows[c][2].ToString(),
                            Account_holder = cust.Rows[c][3].ToString(),
                            cbs_customernumber = cust.Rows[c][4].ToString()
                        };
                        c += 1;

                    };
                verify verifyaccount;
                if (c > 0)
                {
                verifyaccount= new verify
                    {
                        Responsecode = "00",
                        Data = vercus,
                        Message = "Success"

                    };

                }
                else
                {
                    verifyaccount = new verify
                     {
                         Responsecode = "104",
                         Message = "Customer not found!"
                     };
                }
                loginf.Info($"Response::{verifyaccount}");
                 return verifyaccount;
                
                              

            }
            catch (Exception vexp)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"{DateTime.Now}:Something happened.::{vexp.Message}");
                loginf.Info($"Unhandled Exception::{vexp.Message}");
               verify verifyaccount = new verify
                {
                    Responsecode = "200",
                    Message = vexp.Message
                };
                return verifyaccount;

            }
                      
        }
        Purchase putrequets(rentries treqs)
        {
            string commandText = "Insert into rq_entries(EXT_TRANSID,RQ_TIME,TRNS_CODE,PRODUCTID,ACCOUNT_NUMBER,AMOUNT,MATURITY_INSTRUCTION_CODE,TRANSACTION_ID,ACCOUNT_BRANCH,FLEX_CUST_NUM)" +
               " VALUES(:EXT_TRANSID,:RQ_TIME,:TRNS_CODE,:PRODUCTID,:ACCOUNT_NUMBER,:AMOUNT,:MATURITY_INSTRUCTION_CODE,:TRANSACTION_ID,:ACCOUNT_BRANCH,:FLEX_CUST_NUM)";
            OracleConnection con = new OracleConnection(orpath);

            OracleCommand commnd = new OracleCommand(commandText, con);
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Received:{treqs} ");
            //commnd.CommandType = CommandType.StoredProcedure;
            OracleParameter transid = commnd.Parameters.Add(":EXT_TRANSID", OracleDbType.NVarchar2, 50);
            transid.Value = treqs.transaction_id;
            OracleParameter Rqtime = commnd.Parameters.Add(":RQ_TIME", OracleDbType.TimeStamp);
            Rqtime.Value = DateTime.Now;
            OracleParameter Transcode = commnd.Parameters.Add(":TRNS_CODE", OracleDbType.Int16);
            Transcode.Value = treqs.transaction_code;
            OracleParameter accoutnum = commnd.Parameters.Add(":PRODUCTID", OracleDbType.Int16, 15);
            accoutnum.Value = treqs.productid;
            OracleParameter custnum = commnd.Parameters.Add(":ACCOUNT_NUMBER", OracleDbType.Int16);
            custnum.Value = treqs.accountnumber;
             OracleParameter amount = commnd.Parameters.Add(":AMOUNT", OracleDbType.Decimal, 15);
            amount.Value = treqs.amount;
            OracleParameter matinstruct = commnd.Parameters.Add(":MATURITY_INSTRUCTION_CODE", OracleDbType.NVarchar2, 3);
            matinstruct.Value = treqs.maturity_instruction_code;
            OracleParameter deal_id = commnd.Parameters.Add(":TRANSACTION_ID", OracleDbType.Varchar2, 35);
            deal_id.Value = treqs.discount_bearing_deal_id;
            OracleParameter acct_branch = commnd.Parameters.Add(":ACCOUNT_BRANCH", OracleDbType.Varchar2, 10);
            acct_branch.Value =treqs.account_branch;
            OracleParameter flex_cust_num = commnd.Parameters.Add(":FLEX_CUST_NUM", OracleDbType.Varchar2, 10);
            flex_cust_num.Value = treqs.flex_cust_num;

            try
            {
                con.Open();
                //  OracleDataAdapter da = new OracleDataAdapter(commnd);
                commnd.ExecuteNonQuery();
                // return dt; // sonConvert.SerializeObject(dt);
                con.Close();
                Purchase pur= new Purchase {
                    Responsecode="00",
                    Message="SUCCESS"
                 };
                return pur;
            }
            catch (Exception km)
            {
                //if (km.Message.Split(':')[0]== "ORA-20999")
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine($"{DateTime.Now}: An error has occured! {km.Message}");
                //    Console.ResetColor();
                //    Purchase noact = new Purchase
                //    {
                //        Responsecode = "104",
                //        Message = km.Message //"This account is new to Tbill and has to be registered with the central securities depository."
                //    };
                //    return noact;
                //}

                Purchase purerr = new Purchase
                {
                    Responsecode = "104",
                    Message = km.Message.Split(':')[1]
                };
                loginf.Info($"Response::{km.Message}");
                return purerr;
            }



        }
        public Purchase updtphoto(string photo,string customer_number)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Update photo::{customer_number}");
            loginf.Info($"Data::{photo.Length} Received");
            string cmdtxt = "Update ap_customer_info set customer_photo=:customer_photo Where cbs_customer_number=" + customer_number;

            OracleConnection con = new OracleConnection(orpath);
            OracleCommand commnd = new OracleCommand(cmdtxt, con);
            //commnd.CommandType = CommandType.StoredProcedure;
            try
            {
                OracleParameter cbscusno = commnd.Parameters.Add(":customer_photo", OracleDbType.Blob);
            byte[] inphoto = Convert.FromBase64String(photo);
            cbscusno.Value = inphoto;

            
                con.Open();
                OracleDataAdapter da = new OracleDataAdapter(commnd);
                commnd.ExecuteNonQuery();
                // return dt; // sonConvert.SerializeObject(dt);
                con.Close();
                Purchase purs = new Purchase
                {
                    Responsecode = "00",
                    Message = "SUCCESS"
                };
                return purs;
            }
            catch (Exception km)
            {
                // throw;
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"{DateTime.Now}:An Error Occured. {km.Message}");
                //Console.ResetColor();
                loginf.Info($"Unhandled Exception::{km.Message}");
                Purchase purserr = new Purchase
                {
                    Responsecode = "200",
                    Message = km.Message
                };
                return purserr;
            }
        }
        public Purchase updtSignature(string signature, string customer_number)
        {
            var loginf = NLog.LogManager.GetCurrentClassLogger();
            loginf.Info($"Update photo::{customer_number}");
            loginf.Info($"Data::{signature.Length} Received");
            string cmdtxt = "Update ap_customer_info set customer_signature=:customer_signature Where cbs_customer_number=" + customer_number;
            try
            {
            OracleConnection con = new OracleConnection(orpath);
            OracleCommand commnd = new OracleCommand(cmdtxt, con);
            //commnd.CommandType = CommandType.StoredProcedure;
            OracleParameter cbscusno = commnd.Parameters.Add(":customer_signature", OracleDbType.Blob);
            byte[] inphoto = Convert.FromBase64String(signature);
            cbscusno.Value = inphoto;

            
                con.Open();
                OracleDataAdapter da = new OracleDataAdapter(commnd);
                commnd.ExecuteNonQuery();
                // return dt; // sonConvert.SerializeObject(dt);
                con.Close();
                Purchase purs = new Purchase
                {
                    Responsecode = "00",
                    Message = "SUCCESS"
                };
                return purs;
            }
            catch (Exception km)
            {
                // throw;
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"{DateTime.Now}:An Error Occured. {km.Message}");
                //Console.ResetColor();
                loginf.Info($"Unhandled Exception::{km.Message}");
                Purchase purserr = new Purchase
                {
                    Responsecode = "200",
                    Message = km.Message
                };
                return purserr;
            }
        }
        
       
        //public accounts[] getInvInfo (string Accountnumber)
        //{
     
        //    OracleConnection con = new OracleConnection(orpath);

        

        //    //            ACCOUNTNUM VARCHAR2(17 BYTE)
        //    //CUSTOMERID NUMBER
        //    //PRODUCTID NUMBER
        //    //VALUE FLOAT
        //    //INT_ACCRUED FLOAT
        //    //STARTDATE DATE
        //    //MATDATE DATE
        //    //SECURITY_ID VARCHAR2(35 BYTE)
        //    //TRANSACTION_RATE NUMBER
        //    //BUYING_RATE NUMBER
        //    //MATURITY_VALUE NUMBER
        //    //MAX_AVAILABLE NUMBER
        //    //TRANSACTION_ID VARCHAR2(14 BYTE)
        //    //public class accounts
        //    //{
        //    //    public string accountnum { get; set; }
        //    //    public string Product { get; set; }
        //    //    public string Amount { get; set; }
        //    //    public string Int_Accrued { get; set; }
        //    //    public string start_date { get; set; }
        //    //    public DateTime maturity_date { get; set; }
        //    //    public string buying_rate { get; set; }
        //    //    public decimal  maturity_value { get; set; }
        //    //    public decimal max_available { get; set; }
        //    //    public string securityid { get; set; }
        //    //}

        //    OracleCommand commnd = new OracleCommand("SELECT ACCOUNTNUM,PRODUCTID,ROUND(VALUE,4),STARTDATE,MATDATE,SECURITY_ID,TRANSACTION_RATE,"+
        //        "BUYING_RATE,ROUND(MATURITY_VALUE,4),ROUND(MAX_AVAILABLE,4),ROUND(INT_ACCRUED,4) FROM Accounts" +
        //        " WHERE ACCOUNTNUM='" + Accountnumber+"'" , con);
        //    commnd.CommandType = CommandType.Text;
        //    DataTable accts = new DataTable();

        //    try
        //    {
        //        con.Open();

        //        OracleDataAdapter actsda = new OracleDataAdapter(commnd);
        //        //List<accounts>  acts = new List<accounts>();
        //        actsda.Fill(accts);
        //        // DataRow[] results = dto.Select("customerid=" + cusid);
        //        con.Close();
        //        // actsds.Dispose();
        //        int y = 0;
        //        accounts[] Custo = new accounts[accts.Rows.Count];

        //        while (y < accts.Rows.Count)
        //        {
        //            Custo[y] = new accounts
        //            {
        //                accountnum=accts.Rows[y][0].ToString(),
        //                Product=accts.Rows[y][1].ToString(),
        //                Amount= accts.Rows[y][2].ToString(),
        //                start_date= accts.Rows[y][3].ToString(),
        //                maturity_date= accts.Rows[y][4].ToString(),
        //                securityid= accts.Rows[y][5].ToString(),
        //                Int_Accrued= accts.Rows[y][10].ToString(),
        //                max_available= accts.Rows[y][9].ToString()
        //            };
        //            y += 1;
        //        }
        //        return Custo;


        //    }
        //    catch (Exception km)
        //    {
        //        accounts[] cust = new accounts[1];
        //        cust[0] = new accounts
        //        {
        //            accountnum = "NOT_FOUND::" + km.Message
        //        };
        //        return cust;
        //    }

        //}
        public Sale Requestdisinvestment(rentries Request_Details)
        {
            Request_Details.transaction_code = 100;
            Purchase et= putrequets(Request_Details);
            Sale sale;
            if (et.Responsecode == "00")
            {
                sale = new Sale
                {
                    Responsecode="00",
                    Message="SUCCESS"
                };

            }
            else
            {
                sale = new Sale
                {
                    Responsecode = et.Responsecode,
                    Message = et.Message
                };
            }
            return sale;
        }
       public Purchase NewPurchase_Primary(rentries Request_Details)
        {
           
                Request_Details.transaction_code = 103;
                      
            return putrequets(Request_Details);
        }
        public Purchase NewPurchase_Secondary(rentries Request_Details)
        {
            Request_Details.transaction_code = 102;
            return putrequets(Request_Details);
        }

        }

}
