using System;
using System.Net.Http; 
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Xml;
using HtmlAgilityPack;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text;



namespace ExchangeConsole
{
  public class Exchange
    {
        public string name { get; set; }
    }
   public class ExchangePrice
    {
        public string name { get; set; }
        public string coin { get; set; }
        public Decimal volume { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            GetData();
            Console.ReadLine();
        }
       static async void GetData()
       {
         
           HttpClient client = new HttpClient();
           var serializer = new DataContractJsonSerializer(typeof(List<Exchange>));
          
           var streamTask = client.GetStreamAsync("http://localhost:3000/api/exchange");
           var exchanges = serializer.ReadObject(await streamTask) as List<Exchange>;
           foreach (var exchange in exchanges)
           {
                    var html = @"https://coinmarketcap.com/exchanges/"+exchange.name+"/";
                    HtmlWeb web = new HtmlWeb();
                    var htmlDoc = web.Load(html);
                    var node = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class, 'text-gray')]/span");
                    Decimal volume= Convert.ToDecimal(node.InnerHtml, CultureInfo.InvariantCulture);
                    ExchangePrice exchangePrice=new ExchangePrice();
                    exchangePrice.name=exchange.name;
                    exchangePrice.coin="ALL";
                    exchangePrice.volume=volume;
                    
                    //Console.WriteLine(exchangePrice.exchangeid+" "+exchangePrice.coin+" "+exchangePrice.volume);
             //https://stackoverflow.com/questions/37750451/send-http-post-message-in-asp-net-core-using-httpclient-postasjsonasync
             //https://docs.microsoft.com/tr-tr/dotnet/framework/wcf/feature-details/how-to-serialize-and-deserialize-json-data
             //Create a stream to serialize the object to.  
                  MemoryStream ms = new MemoryStream();  

             
                  // Serializer the User object to the stream.  
                  DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ExchangePrice));  
                  ser.WriteObject(ms, exchangePrice);  
                  byte[] json = ms.ToArray();  
                  ms.Close();  
                  string jsonInString =Encoding.UTF8.GetString(json, 0, json.Length);  
                 client.PostAsync("http://localhost:3000/api/exchangeprice", new StringContent(jsonInString, Encoding.UTF8, "application/json"));
             
             
             
           }
         
         
       }
    }
}
