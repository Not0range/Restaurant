using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    class Dish
    {
        public static List<Dish> dishes = new List<Dish>();

        public int id;
        public string title;
        public string category;
        public decimal price;
        public List<string> compound;

        public override string ToString()
        {
            return $"{title} ({category})";
        }
    }

    class Waiter
    {
        public static List<Waiter> waiters = new List<Waiter>();

        public int id;
        public string lastName;
        public string firstName;
        public DateTime birthDate;
        public string address;
        public string phone;
        public DateTime hireDate;

        public override string ToString()
        {
            return $"{lastName} {firstName}";
        }
    }

    class Check
    {
        public static List<Check> checks = new List<Check>();    

        public int id;
        public List<(Dish menu, int count)> order;
        public Waiter waiter;
        public DateTime date;
        public bool cashPayment;
        public decimal tips;

        public decimal Price
        {
            get
            {
                return order.Sum(t => t.menu.price * t.count) + tips;
            }
        }
    }
}
