using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Ресторан";

            bool error = false;
            ReadMenu(ref error);
            ReadWaiters(ref error);
            ReadChecks(ref error);

            if (error)
            {
                Console.WriteLine("При чтении файла были обнаружены некорректные записи.");
                Console.WriteLine("Они не были добавлены в таблицы и будут удалены");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                Console.ReadKey(true);
            }
            DishesMenu.CreateDishesTable();
            WaitersMenu.CreateWaitersTable();
            ChecksMenu.CreateChecksTable();

            ConsoleKeyInfo key;
            error = false;

            do
            {
                if (!error)
                {
                    Console.Write(new string('#', Console.BufferWidth));
                    Console.WriteLine("Добро пожаловать в систему управления рестораном");
                    Console.Write(new string('#', Console.BufferWidth));
                }
                else
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }

                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Работа с меню");
                Console.WriteLine("2 - Работа с таблицей официантов");
                Console.WriteLine("3 - Работа с чеками");
                Console.WriteLine("4 - Завершение работы");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        DishesMenu.WriteMenu();
                        break;
                    case '2':
                        WaitersMenu.WriteMenu();
                        break;
                    case '3':
                        ChecksMenu.WriteMenu();
                        break;
                    default:
                        error = true;
                        break;
                }
            } while (key.KeyChar != '4');
            SaveAll();
        }

        static void ReadMenu(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("menu.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));

            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Dish.dishes.Any(t => t.id == id))
                        throw new ArgumentException();

                    Dish.dishes.Add(new Dish
                    {
                        id = id,
                        title = data[1],
                        category = data[2],
                        price = decimal.Parse(data[3]),
                        compound = data.Skip(4).ToList(),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadWaiters(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("waiters.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Waiter.waiters.Any(t => t.id == id))
                        throw new ArgumentException();

                    Waiter.waiters.Add(new Waiter
                    {
                        id = id,
                        lastName = data[1],
                        firstName = data[2],
                        birthDate = DateTime.Parse(data[3]),
                        address = data[4],
                        phone = data[5],
                        hireDate = DateTime.Parse(data[6]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadChecks(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("checks.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Check.checks.Any(t => t.id == id))
                        throw new ArgumentException();

                    var orderStrs = data[1].Split(' ');
                    var order = new List<(Dish, int)>();
                    for (int i = 0; i < orderStrs.Length; i += 2)
                        order.Add((Dish.dishes.First(t => t.id == int.Parse(orderStrs[i])), 
                            int.Parse(orderStrs[i])));


                    Check.checks.Add(new Check
                    {
                        id = id,
                        order = order,
                        waiter = Waiter.waiters.First(t => t.id == int.Parse(data[2])),
                        date = DateTime.Parse(data[3]),
                        cashPayment = bool.Parse(data[4]),
                        tips = decimal.Parse(data[5]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void SaveAll()
        {
            StreamWriter writer;
            writer = new StreamWriter("menu.txt");
            foreach (var d in Dish.dishes)
                writer.WriteLine(string.Join("\t", d.id, d.title, d.category, 
                    d.price, string.Join("\t", d.compound)));
            writer.Close();

            writer = new StreamWriter("waiters.txt");
            foreach (var w in Waiter.waiters)
                writer.WriteLine(string.Join("\t", w.id, w.lastName, w.firstName, 
                    w.birthDate.ToString("dd.MM.yy"), w.address, w.phone, 
                    w.hireDate.ToString("dd.MM.yy")));
            writer.Close();

            writer = new StreamWriter("checks.txt");
            foreach (var c in Check.checks)
                writer.WriteLine(string.Join("\t", c.id, 
                    string.Join(" ", c.order.Select(t => $"{t.menu.id} {t.count}")), c.waiter.id,
                    c.date.ToString("dd.MM.yy"), c.cashPayment, c.tips));
            writer.Close();
        }

        public static string ReadLine(string text, bool allowEmpty = false)
        {
            bool success;
            string str;
            do
            {
                Console.Write(text);
                str = Console.ReadLine().Trim();
                success = !string.IsNullOrWhiteSpace(str) || allowEmpty;
                if (!success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string('\0', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            } while (!success);
            return str;
        }
    }
}
