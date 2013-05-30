using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Collections;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SportsStore.Domain.Concrete
{
    public class tokenResponse
    {
        public string scope;
        public string access_token;
        public string token_type;
        public string app_id;
        public string expires_in;
        public List<object> authn_schemes { get; set; } 
    }

    public class authScheme
    {
        public string email_password;
        public string remember_me;
        public string phone_pin;
    }


    public class PayPalOrderProcessor : IOrderProcessor
    {
        //public OrderProcessorStatus status { get; set; }

        //status variables;
        public string StatusText { get; set; }
        public bool IsDone { get; set; }
        public bool HaveToken { get; set; }

        //classic PayPal Signature credentials
        private string API_username = "EC_Merchant_api1.SportsStore.com";
        private string API_password = "1369856939";
        private string API_signature = "AFf061sSPXjZTkrcG9vGLIRSvomMAkiQ6fCyct0ne5Ua9S4yemVEtqKd";

        //new REST API credentials (for sandbox)
        private string endpoint = "https://api.sandbox.paypal.com:443";
        private string clientID = "Ad_uORBnMVRPsc-9Yj7vJWGNVsLeTczS2cQV3z9LQy50nDD4jpB6cu4GV1vi";
        private string secret = "EKK6KxAaHjVhxR9G3MZui-yOwoRhb7AOKdl9HvM36SXk1mIZoc15mx61KD7H";

        private WebClient client = new WebClient();

        private string token;
        private string tokenType;
        private string appID;

        private PayPalPaymentObject PaymentData;
        private string PaymentDataString;
        private bool PaymentHeadersSet = false;
        private bool PaymentDataSet = false;

        private string responseData;
        private string sentData;

        private string debugData = "";

        private void getToken()
        {
            //need to send request similar to the below curl request:
            /*
             curl https://api.sandbox.paypal.com/v1/oauth2/token \
                 -H "Accept: application/json" \
                 -H "Accept-Language: en_US" \
                 -u "EOJ2S-Z6OoN_le_KS1d75wsZ6y0SFdVsY9183IvxFyZp:EClusMEUk8e9ihI7ZdVLF5cZ6y0SFdVsY9183IvxFyZp" \
                 -d "grant_type=client_credentials"
            */
            this.SetAccessTokenHeaders();


            NameValueCollection data = new NameValueCollection();
            data.Add("grant_type", "client_credentials");

            //this.client.Credentials = new NetworkCredential(this.clientID, this.secret);

            //byte[] responseData = this.client.UploadValues(this.endpoint + "/v1/oauth2/token", data);
            //byte[] responseData = this.Send("/v1/oauth2/token", data);
            string responseData = this.Send("/v1/oauth2/token", data);
            //this.setTokenInfo(responseData);

            this.setTokenInfo(responseData);


        }

        //private byte[] Send(string url, NameValueCollection data)
        private string Send(string url, NameValueCollection data)
        {
            string destinationURL = this.endpoint + url;


            //return this.client.UploadValues(destinationURL, data);
            return this.client.UploadString(destinationURL, "grant_type=client_credentials");

        }


        private string Send(string url, string dataToSend)
        {
            string destinationURL = this.endpoint + url;

            string data = dataToSend;

            try
            {
                return this.client.UploadString(destinationURL, data);
            }
            catch (WebException exception)
            {
                string responseText;

                using (var reader = new System.IO.StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                return responseText;
            }
        }

        private void SetPaymentHeaders()
        {
            if (!this.HaveToken)
            {
                throw new Exception("You don't have a token... try again");
            }

            this.client.Headers[HttpRequestHeader.ContentType] = "application/json";
            string credentials = this.tokenType + " " + this.token;
            //this.client.Headers[HttpRequestHeader.Authorization] = this.tokenType + " " + this.token;

            this.client.Headers[HttpRequestHeader.Authorization] = credentials;
            this.PaymentHeadersSet = true;

        }

        private void SetAccessTokenHeaders()
        {
            //this.client.Headers.Add("Accept", "application/json");
            this.client.Headers[HttpRequestHeader.Accept] = "application/json";
            //this.client.Headers.Add("Accept-Language", "en_US");
            this.client.Headers[HttpRequestHeader.AcceptLanguage] = "en_US";

            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(this.clientID + ":" + this.secret));
            this.client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentials;
        }

        //private void setTokenInfo(byte[] responseData)
        private void setTokenInfo(string responseData)
        {
            //string responseString = this.client.Encoding.GetString(responseData);
            string responseString = responseData;

            tokenResponse tokenInfo =  JsonConvert.DeserializeObject<tokenResponse>(responseString);

            this.token = tokenInfo.access_token;
            this.tokenType = tokenInfo.token_type;
            this.appID = tokenInfo.app_id;
            this.HaveToken = true;
            this.resetClient();
        }

        private void resetClient()
        {
            this.client = new WebClient();
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            this.getToken();

            if (!this.HaveToken) { return; }
            //now that we have a token, we can submit a payment..
            this.SetPaymentHeaders();

            this.SetPaymentData(cart, shippingDetails);

            this.SendPayment();
        }

        private void SetPaymentData(Cart cart, ShippingDetails shippingDetails)
        {
            if (!this.PaymentHeadersSet) { this.SetPaymentHeaders(); }

            //NameValueCollection data = new NameValueCollection();
            PayPalPaymentObject data = new PayPalPaymentObject();

            data.intent = "sale";
            Uri uri = System.Web.HttpContext.Current.Request.Url;
            
            data.redirect_urls.return_url = uri.Scheme + Uri.SchemeDelimiter + uri.Host + (uri.Port != 80 ? ":" + uri.Port : "") + "/Cart/SUCCESS";
            data.redirect_urls.cancel_url = uri.Scheme + Uri.SchemeDelimiter + uri.Host + (uri.Port != 80 ? ":" + uri.Port : "") + "/Cart/CANCEL";
            data.payer.payment_method = "paypal";

            
            Transaction transaction = data.AddTransaction(cart.Lines.Sum(l => l.Product.Price * l.Quantity).ToString(), "USD", "A SportsStore order");
            ItemList itemList = new ItemList();
            itemList.shipping_address = new ShippingAddress(shippingDetails.Name, shippingDetails.Address1, shippingDetails.Address2, shippingDetails.City, "US", shippingDetails.Zip, shippingDetails.State);
            foreach (var line in cart.Lines)
            {
                //string descriptionText = "(" + line.Quantity + ")" + line.Product.Name + ": " + line.Product.Description;
                itemList.AddItem(line.Quantity.ToString(), line.Product.Name, line.Product.Price.ToString(), "USD");
            }
            transaction.item_list = itemList;
            //data.AddTransaction((line.Product.Price * line.Quantity).ToString(), "USD", descriptionText);
            //data.AddTransaction("7.47", "USD", "Some random description");

            this.PaymentData = data;
            this.PaymentDataString = JsonConvert.SerializeObject(data); ;
            this.PaymentDataSet = true;

        }

        private void SendPayment()
        {
            if (!this.PaymentDataSet) { throw new Exception("Set headers and payment data first!"); }

            this.responseData = this.Send("/v1/payments/payment", this.PaymentDataString);
            this.StatusText = "Data Sent: " + this.PaymentDataString + "\n" + "Data received: " + this.responseData;
        }

    }
}
