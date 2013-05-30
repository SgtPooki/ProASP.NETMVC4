using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    public class PayPalPaymentObject
    {
        public string intent = "";
        public RedirectUrls redirect_urls = new RedirectUrls();
        public Payer payer = new Payer();
        public List<Transaction> transactions = new List<Transaction>();


        public Transaction AddTransaction(string amount, string currency, string description)
        {
            Transaction transaction = new Transaction(new Amount(amount, currency), description);
            
            this.transactions.Add( transaction );

            return transaction;
        }

    }

    public class RedirectUrls
    {
        public string return_url;
        public string cancel_url;
    }

    public class Payer
    {
        public string payment_method;
    }

    public class Transaction
    {
        public Amount amount;
        public string description;
        public ItemList item_list;
        public Transaction(Amount amount, string desc)
        {
            this.amount = amount;
            this.description = desc;
        }
    }

    public class Amount
    {
        public string total;
        public string currency;
        public Amount(string total, string currency)
        {
            this.total = total;
            this.currency = currency;
        }
    }

    public class ItemList
    {
        public List<Item> items = new List<Item>();
        public ShippingAddress shipping_address;
        public void AddItem(string quantity, string name, string price, string currency)
        {
            this.items.Add( new Item( quantity,  name,  price,  currency) );
        }
    }

    public class Item
    {
        public string quantity;
        public string name;
        public string price;
        public string currency;
        public string sku;
        public Item(string quantity, string name, string price, string currency)
        {
            this.quantity = quantity;
            this.name = name;
            this.price = price;
            this.currency = currency;
            this.sku = quantity + name + price;
        }
    }

    public class ShippingAddress
    {
        public string recipient_name;
        public string type;
        public string line1;
        public string line2;
        public string city;
        public string country_code;
        public string postal_code;
        public string state;
        public string phone;

        public ShippingAddress(string name, string line1, string line2, string city, string country, string zip, string state)
        {
            this.recipient_name = name;
            this.type = "residential";
            this.line1 = line1;
            this.line2 = line2;
            this.city = city;
            this.country_code = country;
            this.postal_code = zip;
            this.state = state;
            this.phone = "(402) 234 5678";
        }
    }
}
