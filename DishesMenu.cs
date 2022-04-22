using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant
{
    static class DishesMenu
    {
        public static List<string> dishesTable = new List<string>();
        public static void WriteMenu()
        {
            var changed = false;
            var error = false;
            ConsoleKeyInfo key;

            int prevBuffSize = Console.BufferWidth;
            int max = dishesTable.Max(t => t.Length);
            if (max > Console.BufferWidth)
                Console.BufferWidth = max;

            do
            {
                if (changed)
                {
                    CreateDishesTable();
                    max = dishesTable.Max(t => t.Length);
                    if (max > Console.BufferWidth)
                        Console.BufferWidth = max;
                }

                dishesTable.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (error)
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }
                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Добавить блюдо");
                Console.WriteLine("2 - Редактировать блюдо");
                Console.WriteLine("3 - Удалить блюдо");
                Console.WriteLine("4 - Вернуться в главное меню");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        changed = AddDish();
                        break;
                    case '2':
                        changed = EditDish();
                        break;
                    case '3':
                        changed = RemoveDish();
                        break;
                    default:
                        error = true;
                        break;
                }
                Console.Clear();
            } while (key.KeyChar != '4');
            Console.BufferWidth = prevBuffSize;
        }

        public static void CreateDishesTable()
        {
            string[] columns = new string[] { "ID", "Название", "Категория", "Цена", "Состав" };
            int[] widths = new int[columns.Length];
            dishesTable.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            int max;
            foreach (var d in Dish.dishes)
            {
                if (d.id.ToString().Length > widths[0])
                    widths[0] = d.id.ToString().Length;
                if (d.title.Length > widths[1])
                    widths[1] = d.title.Length;
                if (d.category.Length > widths[2])
                    widths[2] = d.category.Length;
                if (d.price.ToString().Length > widths[3])
                    widths[3] = d.price.ToString().Length;
                max = d.compound.Max(t => t.Length);
                if (max > widths[4])
                    widths[4] = max;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            int w = widths.Sum() + Environment.NewLine.Length;
            if (w > Console.BufferWidth)
                Console.BufferWidth = widths.Sum() + Environment.NewLine.Length;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            dishesTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var p in Dish.dishes)
            {
                temp[0] = (p.id.ToString(), widths[0]);
                temp[1] = (p.title, widths[1]);
                temp[2] = (p.category, widths[2]);
                temp[3] = (p.price.ToString(), widths[3]);
                temp[4] = (p.compound[0], widths[4]);
                dishesTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

                foreach (var item in p.compound.Skip(1))
                {
                    temp[0] = ("", widths[0]);
                    temp[1] = ("", widths[1]);
                    temp[2] = ("", widths[2]);
                    temp[3] = ("", widths[3]);
                    temp[4] = (item, widths[4]);
                    dishesTable.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
                }
            }
        }

        public static bool AddDish()
        {
            string title, category, priceStr;
            decimal price = 0;
            var compound = new List<string>();
            bool success;

            title = Program.ReadLine("Введите название блюда: ");
            category = Program.ReadLine("Введите категорию блюда: ");
            do
            {
                priceStr = Program.ReadLine("Введите стоимость блюда: ");
                if (string.IsNullOrWhiteSpace(priceStr))
                    break;

                success = decimal.TryParse(priceStr, out price);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            CompoundWork(compound);

            Console.Clear();
            Console.WriteLine("Название: {0}", title);
            Console.WriteLine("Категория: {0}", category);
            Console.WriteLine("Стоимость: {0}", price);
            Console.WriteLine("Состав:");
            compound.ForEach(t => Console.WriteLine(" {0}", t));
            Console.WriteLine("Добавить данное блюдо?");
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

            Dish.dishes.Add(new Dish
            {
                id = Dish.dishes.Count > 0 ? Dish.dishes.Last().id + 1 : 0,
                title = title,
                category = category,
                price = price,
                compound = compound,
            });
            return true;
        }

        public static bool EditDish()
        {
            dishesTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID блюда, которое необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Dish d;
            if (!success || (d = Dish.dishes.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();

            string title, category, priceStr;
            var compound = new List<string>(d.compound);
            decimal price = 0;

            title = Program.ReadLine($"Введите название блюда ({d.title}): ", true);
            category = Program.ReadLine($"Введите категорию блюда ({d.category}): ", true);
            do
            {
                priceStr = Program.ReadLine($"Введите стоимость блюда ({d.price}): ", true);
                if (string.IsNullOrWhiteSpace(priceStr))
                    break;

                success = decimal.TryParse(priceStr, out price);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            CompoundWork(compound);

            Console.Clear();
            Console.WriteLine("Название: {0}", string.IsNullOrWhiteSpace(title) ? d.title : title);
            Console.WriteLine("Категория: {0}", string.IsNullOrWhiteSpace(category) ? d.category : category);
            Console.WriteLine("Стоимость: {0}", string.IsNullOrWhiteSpace(priceStr) ? d.price : price);
            Console.WriteLine("Состав:");
            compound.ForEach(t => Console.WriteLine(" {0}", t));
            Console.WriteLine("Сохранить данное блюдо?");
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

            if (!string.IsNullOrWhiteSpace(title)) d.title = title;
            if (!string.IsNullOrWhiteSpace(category)) d.category = category;
            if (!string.IsNullOrWhiteSpace(priceStr)) d.price = price;
            d.compound = compound;

            return true;
        }

        private static void CompoundWork(List<string> compound)
        {
            bool success;
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("Текущий состав блюда:");
                for (int i = 0; i < compound.Count; i++)
                    Console.WriteLine("{0}. {1}", i + 1, compound[i]);
                Console.WriteLine();
                Console.WriteLine("1 - Добавить пункт в состав");
                Console.WriteLine("2 - Удалить пункт из состава");
                if (compound.Count > 0)
                    Console.WriteLine("3 - Продолжить");
                key = Console.ReadKey(true);
                if (key.KeyChar == '1')
                    compound.Add(Program.ReadLine("Введите пункт состава: "));
                else if (key.KeyChar == '2')
                {
                    Console.WriteLine("Введите номер пункта, который необходимо удалить ");
                    int id;
                    success = int.TryParse(Console.ReadLine(), out id);
                    if (!success || id - 1 < 0 || id - 1 >= compound.Count)
                        continue;
                    compound.RemoveAt(id - 1);
                }
            } while (key.KeyChar != '3' || compound.Count == 0);
        }

        public static bool RemoveDish()
        {
            dishesTable.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID блюда, которое необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Dish d;
            if (!success || (d = Dish.dishes.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Check.checks.ForEach(c => c.order.RemoveAll(t => t.menu.id == d.id));
            Check.checks.RemoveAll(c => !c.order.Any());
            Dish.dishes.Remove(d);
            return true;
        }
    }
}
