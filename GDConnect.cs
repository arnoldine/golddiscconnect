using System;
using System.ServiceModel;
using System.ServiceModel.Web;
namespace GD_SOAP
{
    [ServiceContract]
    public interface IGDConnect
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        products[] GetProducts(string devkey);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        BankSecurities GetSecuritiesforsale(int start_day, int end_day, string debt_type);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        buy_rate getbuying_rate(int rec);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Customerbalance GetCustomerBalanceEnq(string Accountnumber);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Customerbalance GetAccountdetails(string Accountnumber);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Purchase NewPurchase_Primary(rentries Request_details);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Purchase NewPurchase_Secondary(rentries Request_details);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Sale Requestdisinvestment(rentries Request_Details);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Purchase Customer_onboarding(onboarding Request_Details);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        verify verifycustomer(string customernum);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Purchase updtphoto(string photo, string customer_number);
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml)]
        Purchase updtSignature(string signature, string customer_number);


    }
}
