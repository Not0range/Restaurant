using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    static class WaitersMenu
    {
        public static List<string> waitersTable = new List<string>();
        public static void WriteMenu()
        {
            var changed = false;
            var error = false;
            ConsoleKeyInfo key;

            int prevBuffSize = Console.BufferWidth;
            int max = waitersTable.Max(t => t.Length);
            if (max > Console.BufferWidth)
                Console.BufferWidth = max;

            do
            {
                if (changed)
                {
                    CreateWaitersTable();
                    max = waitersTable.Max(t => t.Length);
                    if (max > Console.BufferWidth)
                        Console.BufferWidth = max;
                }

                waitersTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (error)
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }
                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Добавить официанта");
                Console.WriteLine("2 - Редактировать официанта");
                Console.WriteLine("3 - Удалить официанта");
                Console.WriteLine("4 - Вернуться в главное меню");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        changed = AddWaiter();
                        break;
                    case '2':
                        changed = EditWaiter();
                        break;
                    case '3':
                        changed = RemoveWaiter();
                        break;
                    default:
                        error = true;
                        break;
                }
                Console.Clear();
            } while (key.KeyChar != '4');
            Console.BufferWidth = prevBuffSize;
        }

        public static void CreateWaitersTable()
        {
            string[] columns = new string[] { "ID", "Фамилия", "Имя", "Дата рождения", 
                "Адрес", "Номер телефона", "Дата найма" };
            int[] widths = new int[columns.Length];
            waitersTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            int max;
            foreach (var t in Waiter.waiters)
            {
                if (t.id.ToString().Length > widths[0])
                    widths[0] = t.id.ToString().Length;
                if (t.lastName.Length > widths[1])
                    widths[1] = t.lastName.Length;
                if (t.firstName.Length > widths[2])
                    widths[2] = t.firstName.Length;
                max = "dd.MM.yy".Length;
                if (max > widths[3])
                    widths[3] = max;
                if (t.address.Length > widths[4])
                    widths[4] = t.address.Length;
                if (t.phone.Length > widths[5])
                    widths[5] = t.phone.Length;
                if (max > widths[6])
                    widths[6] = max;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            waitersTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var t in Waiter.waiters)
            {
                temp[0] = (t.id.ToString(), widths[0]);
                temp[1] = (t.lastName, widths[1]);
                temp[2] = (t.firstName, widths[2]);
                temp[3] = (t.birthDate.ToString("dd.MM.yy"), widths[3]);
                temp[4] = (t.address, widths[4]);
                temp[5] = (t.phone, widths[5]);
                temp[6] = (t.hireDate.ToString("dd.MM.yy"), widths[6]);
                waitersTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
            }
        }

        public static bool AddWaiter()
        {
            string lastName, firstName, birthStr, address, phone, hireStr;
            DateTime birthDate = new DateTime(); 
            DateTime hireDate = new DateTime();
            bool success;

            lastName = Program.ReadLine("Введите фамилию официанта: ");
            firstName = Program.ReadLine("Введите имя официанта: ");
            do
            {
                birthStr = Program.ReadLine("Введите дату рождения официанта: ");

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);
            address = Program.ReadLine("Введите адрес официанта: ");
            phone = Program.ReadLine("Введите номер телефона официанта: ");
            do
            {
                hireStr = Program.ReadLine("Введите дату найма официанта: ");

                success = DateTime.TryParse(hireStr, out hireDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Фамилия: {0}", lastName);
            Console.WriteLine("Имя: {0}", firstName);
            Console.WriteLine("Дата рождения: {0}", birthDate.ToString("dd.MM.yyyy"));
            Console.WriteLine("Адрес: {0}", address);
            Console.WriteLine("Номер телефона: {0}", phone);
            Console.WriteLine("Дата найма: {0}", hireDate.ToString("dd.MM.yyyy"));
            Console.WriteLine("Добавить данного официанта?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            Waiter.waiters.Add(new Waiter
            {
                id = Waiter.waiters.Count > 0 ? Waiter.waiters.Last().id + 1 : 0,
                lastName = lastName,
                firstName = firstName,
                birthDate = birthDate,
                address = address,
                phone = phone,
                hireDate = hireDate
            });
            return true;
        }

        public static bool EditWaiter()
        {
            waitersTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID официанта, которого необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Waiter w;
            if (!success || (w = Waiter.waiters.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();

            string lastName, firstName, birthStr, address, phone, hireStr;
            DateTime birthDate = new DateTime();
            DateTime hireDate = new DateTime();

            lastName = Program.ReadLine($"Введите фамилию официанта ({w.lastName}): ", true);
            firstName = Program.ReadLine($"Введите имя официанта ({w.firstName}): ", true);
            do
            {
                birthStr = Program.ReadLine("Введите дату рождения официанта " + 
                    $"({w.birthDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(birthStr))
                    break;

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);
            address = Program.ReadLine($"Введите адрес официанта ({w.address}): ", true);
            phone = Program.ReadLine($"Введите номер телефона официанта ({w.phone}): ", true);
            do
            {
                hireStr = Program.ReadLine("Введите дату найма официанта " +
                    $"({w.hireDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(hireStr))
                    break;

                success = DateTime.TryParse(hireStr, out hireDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Фамилия: {0}", string.IsNullOrWhiteSpace(lastName) ? w.lastName : lastName);
            Console.WriteLine("Имя: {0}", string.IsNullOrWhiteSpace(firstName) ? w.firstName : firstName);
            Console.WriteLine("Дата рождения: {0}", string.IsNullOrWhiteSpace(birthStr) ? w.birthDate.ToString("dd.MM.yyyy") : 
                birthDate.ToString("dd.MM.yyyy"));
            Console.WriteLine("Адрес: {0}", string.IsNullOrWhiteSpace(address) ? w.address : address);
            Console.WriteLine("Номер телефона: {0}", string.IsNullOrWhiteSpace(phone) ? w.phone : phone);
            Console.WriteLine("Дата найма: {0}", string.IsNullOrWhiteSpace(hireStr) ? w.hireDate.ToString("dd.MM.yyyy") :
                hireDate.ToString("dd.MM.yyyy"));
            Console.WriteLine("Сохранить данного официанта?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            if (!string.IsNullOrWhiteSpace(lastName)) w.lastName = lastName;
            if (!string.IsNullOrWhiteSpace(firstName)) w.firstName = firstName;
            if (!string.IsNullOrWhiteSpace(birthStr)) w.birthDate = birthDate;
            if (!string.IsNullOrWhiteSpace(address)) w.address = address;
            if (!string.IsNullOrWhiteSpace(phone)) w.phone = phone;
            if (!string.IsNullOrWhiteSpace(hireStr)) w.hireDate = hireDate;

            return true;
        }

        public static bool RemoveWaiter()
        {
            waitersTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID официанта, которого необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Waiter w;
            if (!success || (w = Waiter.waiters.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Check.checks.RemoveAll(c => c.waiter.id == w.id);
            Waiter.waiters.Remove(w);
            return true;
        }
    }
}
