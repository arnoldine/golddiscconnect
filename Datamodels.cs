using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using Oracle.ManagedDataAccess.Types;
namespace GD_SOAP
{
    class Datamodels
    {
    }
    public class rentries
    {

        public string transaction_id { get; set; }
        public int transaction_code { get; set; }
        public int productid { get; set; }
        public string amount { get; set; }
        public string account_branch { get; set; }
        public string accountnumber { get; set; }
        public string flex_cust_num { get; set; }
        public string discount_bearing_deal_id { get; set; }
        public string maturity_instruction_code { get; set; }

    }
    enum transaction_codes
    {
        Rediscount,
        Primary,
        Secondary
    }

    public class securities
    {
        public string due_date { get; set; }
        public int days_to_maturity { get; set; }
        public Decimal market_value { get; set; }
        public Decimal face_value { get; set; }
        public Decimal Market_rate { get; set; }
        public string Discount_bearing_deal_id { get; set; }
        public int upper_bucket { get; set; }
        public int lower_bucket { get; set; }
        public string sectenor { get; set; }
        public string debt_type { get; set; }

    }
    //    CUSTOMERID NUMBER
    //FULLNAME VARCHAR2(100 BYTE)
    //CELLPHONE VARCHAR2(15 BYTE)
    //PHONE VARCHAR2(15 BYTE)
    //BRANCHID NUMBER
    //ACCOUNTNUM VARCHAR2(17 BYTE)
    public class customers
    {
        // public string customerid { get; set; }
        public string AccountNumber { get; set; }
        public string currency_code { get; set; }
        public string client_id { get; set; }
        public string branch_code { get; set; }
        public string Account_holder { get; set; }
        public string cbs_customernumber { get; set; }
    }
    public class products
    {
        [DataMember]
        public int productid { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int productmap { get; set; }
    }
    public class buying_rates
    {
     //   "Select upper_bucket,lower_bucket, currency_code,round(market_rate,4) as market_rate,approval_status,trading_date,yield_or_discount,"+
      //          "special_rate,debt_type,price_band from  buying_rate "
    public Decimal upper_bucket { get; set; }
    public Decimal lower_bucket { get; set; }
        public string currency_code { get; set; }
        public Decimal market_rate { get; set; }
        public string approval_status { get; set; }
        public string trading_date { get; set; }
        public string yield_or_discount { get; set; }
        public Decimal special_rate { get; set; }
        public string debt_type { get; set; }
        public string price_band { get; set; }

    }
  
    public class accounts
    {
        public string Trans_date { get; set; }
        public string Product { get; set; }
        public Decimal Principal { get; set; }
        public Decimal Rate { get; set; }
        public int Tenure { get; set; }
        public Decimal Interest { get; set; }
        public Decimal Current_Value { get; set; }
        public string Due_date { get; set; }
        public Int16 Days_to_Maturity { get; set; }
        //public Int16 Lower_Bucket { get; set; }
        //public Int16 Upper_Bucket { get; set; }
        //public string sectenor { get; set; }
        public Decimal Market_Value { get; set; }
        public Decimal Market_Rate { get; set; }
        public string Security_id { get; set; }
        public Decimal Maturity_Value { get; set; }
        public string Discount_bearing_deal_id { get; set; }
        //DEBT_TYPE,
        //ISSUER_CODE,
        //TRANSACTION_DATE,
        //PURCHASE_RATE
        public string Debt_type { get; set; }
        public string Issuer_code { get; set; }
        public string Transaction_date { get; set; }
        public Decimal Purchase_Rate { get; set; }
        public Decimal Lien_Amount { get; set; }
    }
   public class Customerbalance
    {
        public string Responsecode { get; set; }
        public accounts[] Data { get; set; }
        public string Message { get; set; }
    }
    public class BankSecurities
    {
        public string Responsecode { get; set; }
        public securities [] Data { get; set; }
        public string Message { get; set; }
    }

    public class onboarding
    {
        public string cbs_customer_number { get; set; }
        public string Account_Number { get; set; }
        public string account_branch { get; set; }
        public string security_type { get; set; }
         public int Lower_tenor { get; set; }
        public int Upper_tenor { get; set; }
              
        public Decimal amount { get; set; }

         public string title_code { get; set; }
        public string first_name { get; set; }
        public string surname { get; set; }
                public string other_name { get; set; }
            public string Address_Post_box { get; set; }
                public string Address_city { get; set; }
                public string Address_Country { get; set; }
                public string Id_type_code { get; set; }
                public string Id_Number { get; set; }
                public string expiry_date { get; set; }
                public string place_of_issue { get; set; }  //place_of_issue          varchar2(50)      null ,--flexcube or app
            public string dob { get; set; }//dob                     date              null ,--app
               public string country_code { get; set; }//country_code            varchar2(10)      null ,--flexcube or manual update
            public string nationality_code { get; set; }                                   // nationality_code varchar2(10)      null ,--flexcube or manual update
             public string phone_number { get; set; }                                   // phone_number varchar2(15)      null ,--flexcube or app
             public string email { get; set; }                                   //email                   varchar2(50)           ,--flexcube or app
              public string occupation { get; set; }                                  //occupation              varchar2(100)          ,--app
               public string residential_status_code { get; set; }                                 //residential_status_code  varchar2(10)    null  ,--app values are 'RG' - Resident Ghanaian, 'RF' - Resident Foreigner, 'NRG' - Non Resident Ghanaian , 'NRF' - Non Resident Foreigner
               public string customer_photo { get; set; }                                //customer_photo          blob                   ,--app
                 public string customer_signature { get; set; }                               //customer_signature      blob                   ,--app
                  public string nok_name { get; set; }                              //nok_name                varchar2(100)          ,--app
                  public string nok_phone { get; set; }                            //nok_phone               varchar2(15)           ,--app
                  public string nok_relationship { get; set; }                             //nok_relationship        varchar2(50)           ,--app
                  public string nok_address { get; set; }                              //nok_address             varchar2(100)          ,--app
                  public int maturity_instruction { get; set; }                              //maturity_instruction    number(3)              ,--app -- 1 full rollver, 6-- full payment, 4 - principal rollover, 5 , interest rollover
                  public string date_Received { get; set; }                             // date_Received date              null ,
                  public string gender { get; set; }                              // gender char (1)       null ,
                  public string client_code { get; set; }                              // client_code varchar2(10)  null, 
                  public string rm_code { get; set; }                              // rm_code varchar2(10)  null,  --flexcube
                  public string rm_name { get; set; }                              //rm_name                          varchar2(100)  null, --flexcube


    }
}
