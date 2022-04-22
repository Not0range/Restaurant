using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    static class ChecksMenu
    {
        public static List<string> checksTable = new List<string>();
        public static void WriteMenu()
        {
            var changed = false;
            var error = false;
            ConsoleKeyInfo key;

            int prevBuffSize = Console.BufferWidth;
            int max = checksTable.Max(t => t.Length);
            if (max > Console.BufferWidth)
                Console.BufferWidth = max;

            do
            {
                if (changed)
                {
                    CreateChecksTable();
                    max = checksTable.Max(t => t.Length);
                    if (max > Console.BufferWidth)
                        Console.BufferWidth = max;
                }

                checksTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (error)
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }
                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Добавить чек");
                Console.WriteLine("2 - Редактировать чек");
                Console.WriteLine("3 - Удалить чек");
                Console.WriteLine("4 - Вернуться в главное меню");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        changed = AddCheck();
                        break;
                    case '2':
                        changed = EditCheck();
                        break;
                    case '3':
                        changed = RemoveCheck();
                        break;
                    default:
                        error = true;
                        break;
                }
                Console.Clear();
            } while (key.KeyChar != '4');
            Console.BufferWidth = prevBuffSize;
        }

        public static void CreateChecksTable()
        {
            string[] columns = new string[] { "ID", "Заказ", "Официант", "Дата",
                "Наличный/безналичный", "Чаевые", "Стоимость" };
            int[] widths = new int[columns.Length];
            checksTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            int max;
            foreach (var c in Check.checks)
            {
                if (c.id.ToString().Length > widths[0])
                    widths[0] = c.id.ToString().Length;
                max = c.order.Max(t => t.ToString().Length);
                if (max > widths[1])
                    widths[1] = max;
                if (c.waiter.ToString().Length > widths[2])
                    widths[2] = c.waiter.ToString().Length;
                max = "dd.MM.yy".Length;
                if (max > widths[3])
                    widths[3] = max;
                if (c.tips.ToString().Length > widths[5])
                    widths[5] = c.tips.ToString().Length;
                if (c.Price.ToString().Length > widths[6])
                    widths[6] = c.Price.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            checksTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var c in Check.checks)
            {
                temp[0] = (c.id.ToString(), widths[0]);
                temp[1] = (c.order[0].ToString(), widths[1]);
                temp[2] = (c.waiter.ToString(), widths[2]);
                temp[3] = (c.date.ToString("dd.MM.yy"), widths[3]);
                temp[4] = (c.cashPayment ? "наличный" : "безналичный", widths[4]);
                temp[5] = (c.tips.ToString(), widths[5]);
                temp[6] = (c.Price.ToString(), widths[6]);
                checksTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

                foreach (var item in c.order.Skip(1))
                {
                    temp[0] = ("", widths[0]);
                    temp[1] = (item.ToString(), widths[1]);
                    temp[2] = ("", widths[2]);
                    temp[3] = ("", widths[3]);
                    temp[4] = ("", widths[4]);
                    temp[5] = ("", widths[5]);
                    temp[6] = ("", widths[6]);
                    checksTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
                }
            }
        }

        public static bool AddCheck()
        {
            string waiterStr, dateStr, tipsStr;
            int waiter = 0;
            DateTime date = new DateTime();
            decimal tips = 0;
            bool cashPayment = false;
            var order = new List<(Dish menu, int count)>();
            bool success;

            MenuWork(order);
            Console.Clear();

            WaitersMenu.waitersTable.ForEach(t => Console.WriteLine(t));
            do
            {
                waiterStr = Program.ReadLine("Введите ID официанта: ");
                if (string.IsNullOrWhiteSpace(waiterStr))
                    break;

                success = int.TryParse(waiterStr, out waiter) &&
                    Waiter.waiters.Any(t => t.id == waiter);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine("Введите дату: ");
                if (string.IsNullOrWhiteSpace(dateStr))
                    break;

                success = DateTime.TryParse(dateStr, out date);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.WriteLine("Введите тип оплаты");
            Console.WriteLine("1 - наличный");
            Console.WriteLine("2 - безналичный");
            ConsoleKeyInfo k;
            do
            {
                k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    cashPayment = true;
                else if (k.KeyChar == '2')
                    cashPayment = false;

            } while (k.KeyChar != '1' && k.KeyChar != '2');
            Console.WriteLine(cashPayment ? "наличный" : "безналичный");

            do
            {
                tipsStr = Program.ReadLine("Введите чаевые: ");
                if (string.IsNullOrWhiteSpace(tipsStr))
                    break;

                success = decimal.TryParse(tipsStr, out tips);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Заказ:");
            order.ForEach(t => Console.WriteLine(" {0}", t));
            Console.WriteLine("Официант: {0}", Waiter.waiters.First(t => t.id == waiter));
            Console.WriteLine("Дата: {0}", date.ToString("dd.MM.yyyy"));
            Console.WriteLine("Тип оплаты: {0}", cashPayment ? "наличный" : "безналичный");
            Console.WriteLine("Чаевые: {0}", tips);
            Console.WriteLine("Добавить данный чек?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            Check.checks.Add(new Check
            {
                id = Check.checks.Count > 0 ? Check.checks.Last().id + 1 : 0,
                order = order,
                waiter = Waiter.waiters.First(t => t.id == waiter),
                date = date,
                cashPayment = cashPayment,
                tips = tips,
            });
            return true;
        }

        public static bool EditCheck()
        {
            checksTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID чека, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Check c;
            if (!success || (c = Check.checks.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();

            string waiterStr, dateStr, tipsStr;
            int waiter = 0;
            DateTime date = new DateTime();
            decimal tips = 0;
            bool cashPayment = c.cashPayment;
            var order = new List<(Dish menu, int count)>(c.order);

            MenuWork(order);
            Console.Clear();

            WaitersMenu.waitersTable.ForEach(t => Console.WriteLine(t));
            do
            {
                waiterStr = Program.ReadLine($"Введите ID официанта ({c.id}): ", true);
                if (string.IsNullOrWhiteSpace(waiterStr))
                    break;

                success = int.TryParse(waiterStr, out waiter) &&
                    Waiter.waiters.Any(t => t.id == waiter);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine($"Введите дату ({c.date.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(dateStr))
                    break;

                success = DateTime.TryParse(dateStr, out date);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.WriteLine($"Введите тип оплаты ({(c.cashPayment ? "наличный" : "безналичный")})", true);
            Console.WriteLine("1 - наличный");
            Console.WriteLine("2 - безналичный");
            ConsoleKeyInfo k;
            do
            {
                k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    cashPayment = true;
                else if (k.KeyChar == '2')
                    cashPayment = false;

            } while (k.KeyChar != '1' && k.KeyChar != '2' && k.Key != ConsoleKey.Enter);
            Console.WriteLine(cashPayment ? "наличный" : "безналичный");

            do
            {
                tipsStr = Program.ReadLine($"Введите чаевые ({c.tips}): ", true);
                if (string.IsNullOrWhiteSpace(tipsStr))
                    break;

                success = decimal.TryParse(tipsStr, out tips);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Заказ:");
            order.ForEach(t => Console.WriteLine(" {0}", t));
            Console.WriteLine("Официант: {0}", string.IsNullOrWhiteSpace(waiterStr) ? c.waiter :
                Waiter.waiters.First(t => t.id == waiter));
            Console.WriteLine("Дата: {0}", string.IsNullOrWhiteSpace(dateStr) ? c.date.ToString("dd.MM.yyyy") :
                date.ToString("dd.MM.yyyy"));
            Console.WriteLine("Тип оплаты: {0}", cashPayment ? "наличный" : "безналичный");
            Console.WriteLine("Чаевые: {0}", string.IsNullOrWhiteSpace(tipsStr) ? c.tips : tips);
            Console.WriteLine("Сохранить данный чек?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            c.order = order;
            if (!string.IsNullOrWhiteSpace(waiterStr)) c.waiter =
                Waiter.waiters.First(t => t.id == waiter);
            if (!string.IsNullOrWhiteSpace(dateStr)) c.date = date;
            c.cashPayment = cashPayment;
            if (!string.IsNullOrWhiteSpace(tipsStr)) c.tips = tips;

            return true;
        }

        private static void MenuWork(List<(Dish menu, int count)> menu)
        {
            bool success;
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("Текущий заказ:");
                for (int i = 0; i < menu.Count; i++)
                    Console.WriteLine("{0}. {1}", i + 1, menu[i]);
                Console.WriteLine();
                Console.WriteLine("1 - Добавить пункт в состав");
                Console.WriteLine("2 - Удалить пункт из состава");
                if (menu.Count > 0)
                    Console.WriteLine("3 - Продолжить");
                key = Console.ReadKey(true);
                if (key.KeyChar == '1')
                {
                    Console.Clear();
                    DishesMenu.dishesTable.ForEach(t => Console.WriteLine(t));
                    Console.WriteLine();

                    string tempStr;
                    int id, count = 0;
                    Dish d = null;
                    do
                    {
                        tempStr = Program.ReadLine("Введите ID блюда: ");
                        if (string.IsNullOrWhiteSpace(tempStr))
                            break;

                        success = int.TryParse(tempStr, out id) &&
                            ((d = Dish.dishes.FirstOrDefault(t => t.id == id)) != null);
                        if (!success)
                            Console.WriteLine("Ошибка ввода");
                    } while (!success);

                    do
                    {
                        tempStr = Program.ReadLine("Введите количество: ");
                        if (string.IsNullOrWhiteSpace(tempStr))
                            break;

                        success = int.TryParse(tempStr, out count) && count > 0;
                        if (!success)
                            Console.WriteLine("Ошибка ввода");
                    } while (!success);

                    menu.Add((d, count));
                }
                else if (key.KeyChar == '2')
                {
                    Console.WriteLine("Введите номер пункта, который необходимо удалить ");
                    int id;
                    success = int.TryParse(Console.ReadLine(), out id);
                    if (!success || id - 1 < 0 || id - 1 >= menu.Count)
                        continue;
                    menu.RemoveAt(id - 1);
                }
            } while (key.KeyChar != '3' || menu.Count == 0);
        }

        public static bool RemoveCheck()
        {
            checksTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID чека, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Check c;
            if (!success || (c = Check.checks.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Check.checks.RemoveAll(t => t.id == c.id);
            return true;
        }
    }
}
