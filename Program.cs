using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PracticTask1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            var VacationDictionary = new Dictionary<string, List<DateTime[]>>()
            {
                ["Принцесса Катания"] = new List<DateTime[]>(),
                ["Принцесса Женевьева"] = new List<DateTime[]>(),
                ["Принцесса Розелла"] = new List<DateTime[]>(),
                ["Принцесса Анна-Луиза"] = new List<DateTime[]>(),
                ["Принцесса Одет"] = new List<DateTime[]>(),
                ["Принцесса Кортни"] = new List<DateTime[]>(),
                ["Принцесса Рапунцель"] = new List<DateTime[]>(),
                ["Принц Луи"] = new List<DateTime[]>(),
                ["Принц Доминик"] = new List<DateTime[]>()
            };
            var AviableWorkingDaysOfWeekWithoutWeekends = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            List<int[]> vacationVariants = new List<int[]>{ new int[] { 7,14,7 },
                                                                  new int[] {7,7,7,7 },
                                                                  new int[] { 14,14 }
                                                                 };
            DateTime start = new DateTime(DateTime.Now.Year, 1, 1);

            while (true)
            {
                string temp = string.Empty;
                Console.WriteLine("Введите год за который вы хотели бы рассчитать отпуска сотрудникам");
                temp = Console.ReadLine();

                if (!DateTime.TryParse(string.Format("1/1/{0}", temp), out start))
                    Console.WriteLine("Не удалось распознать введёный год, попробуйте ещё раз");
                else
                    break;
            }

            // в одном месяце пусть будет возможен только один отрезок отпуска
            foreach (var VacationList in VacationDictionary)
            {
                var random = new Random();
                int index = random.Next(vacationVariants.Count);
                //идея: я буду содержать массив с номерами месяцев и рандомом из него выбирать месяц, после выбора
                //удалять его из массива, чтобы он не мог быть выбран второй раз. 
                // после выбора месяца я на основе календаря получая список целых в нем недель, так как отпуск у нас должен быть не кусками 
                // а сплошными днями, т.е. отпуск должен начинаться всегда с понедельника.
                // Получив список недель рандомом выбираю неделю и заношу её дни в переменную vacation
                // Таким образом я построю один отрезок отпуска, затем в цикле повторю эти действия для отсавшихся отрезков для текущего сотрудника.
                List<int> months = new List<int> { 1,2,3,4,5,6,7,8,9,10,11,12 };
                int len = vacationVariants[index].Length;
                // цикл для построения отрекзов отпуска
                foreach (int days in vacationVariants[index])
                {
                    // массив отпусков по датам
                    DateTime[] vacation = new DateTime[days];

                    int indexM = random.Next(months.Count); // выберем месяц из доступных
                    int month = months[indexM]; // получим номер выбранного рандомом месяца
                    months.RemoveAt(indexM);  // удалим выбранный месяц из списка, чтобы его нельзя было выбрать повторно

                    // получим недели месяца
                    // вначале получим все даты внутри месяца
                    var dates = Enumerable.Range(1, DateTime.DaysInMonth(start.Year, month)).Select(n => new DateTime(start.Year, month, n));
                    // теперь отфильтруем только те даты, которые приходятся на понедельник, т.о. мы получим даты начала недель.
                    var weekends = (from d in dates
                                   where d.DayOfWeek == DayOfWeek.Monday
                                   select d).ToList(); // мы получим список дат понедельников недели
                    // Выберем рандомом любой понедельник.
                    int indexWeek = random.Next(weekends.Count);
                    //рассчитаем от этого понедельника конец отпуска соласно заданному отрезку days
                    DateTime startDate = weekends[indexWeek];
                    DateTime endDate = startDate.AddDays(days);

                    // непосредственно дни отрезка отпуска положим в наш массив
                    int i = 0;
                    for (DateTime dt = startDate; dt < endDate; dt = dt.AddDays(1))
                    {
                        vacation[i] = dt;
                        i++;
                    }
                    VacationDictionary[VacationList.Key].Add(vacation);

                    // Далее, если неделя начинается с понедельника но заканчивается в следующем месяце, то необходимо
                    // этот следующий месяц удалить из массива доступных месяцев, чтобы он не был выбран для следующего отрезка отпуска
                    if (endDate.Month != startDate.Month)
                        months.Remove(endDate.Month);

                }
            }

            //выведем отпуска сотрудников
            foreach (var VacationList in VacationDictionary)
            {
                Console.WriteLine("Сотрудник: {0} получил следующие отпуска в {1}  году:", VacationList.Key, start.Year);
                int i = 1;
                foreach(var item in VacationList.Value)
                {
                    Console.WriteLine("{0}) будет приходиться на даты: {1:dd.MM.yyyy} - {2:dd.MM.yyyy}",i, item[0], item[item.Length-1]);
                    i++;
                }
            }
                Console.ReadKey();
        }
    }
}
