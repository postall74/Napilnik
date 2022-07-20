using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentSystems
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Выведите платёжные ссылки для трёх разных систем платежа: 
            //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
            //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
            //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}

            Order order = new Order(2423, 180);
            PaySystemOne paySystemOne = new PaySystemOne();
            PaySystemTwo paySystemTwo = new PaySystemTwo();
            PaySystemThree paySystemThree = new PaySystemThree();
            Console.WriteLine(paySystemOne.GetPayingLink(order));
            Console.WriteLine(paySystemTwo.GetPayingLink(order));
            Console.WriteLine(paySystemThree.GetPayingLink(order));
            Console.ReadKey();
        }
    }

    public class Order
    {
        public readonly int Id;
        public readonly int Amount;

        public Order(int id, int amount) => (Id, Amount) = (id, amount);
    }

    public interface IPaymentSystem
    {
        string GetPayingLink(Order order);
    }

    public class PaySystemOne : IPaymentSystem
    {
        private string _link = "pay.system1.ru/order?amount=12000RUB&hash=";

        public string GetPayingLink(Order order)
        {
           var hashCode = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(order.Id.ToString()));
            return _link + Convert.ToBase64String(hashCode);
        }
    }

    public class PaySystemTwo : IPaymentSystem
    {
        private string _link = "order.system2.ru/pay?hash=";

        public string GetPayingLink(Order order)
        {
            var hashCode = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(order.Id.ToString() + order.Amount.ToString()));
            return _link + Convert.ToBase64String(hashCode);
        }
    }

    public class PaySystemThree : IPaymentSystem
    {
        private string _link = "system3.com/pay?amount=12000&curency=RUB&hash=";
        private string _secretKey = DateTime.Now.GetHashCode().ToString();

        public string GetPayingLink(Order order)
        {
            var hasCode = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(order.Amount.ToString()));
            return _link + Convert.ToBase64String(hasCode) + order.Id.ToString() + _secretKey;
        }
    }

}
