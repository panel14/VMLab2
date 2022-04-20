namespace VMLab2
{
    class Programm
    {
        private const String ERR_STRING = "Неверный формат ввода. Повторите попытку.";
        public static void Main(String[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Добро пожаловать.\nДля решения нелинейного уравнения нажмите 1.\nДля решения СНУ нажмите 2.\n" +
                "Чтобы посмотреть список доступных для решения уравнений и систем нажмите 3.");
            string? command = Console.ReadLine();
            switch (command)
            {
                case "1":
                    Console.WriteLine("Выберите уравнение:\n" +
                        "(1): x³ + 4x - 3 = 0\n" +
                        "(2): x² - 20sin(x) = 0\n" +
                        "(3): (x + 1)^(1/2) - 1/x = 0");

                    int iMeth;
                    while (!Int32.TryParse(Console.ReadLine(), out iMeth))
                        Console.WriteLine(ERR_STRING);
                    Console.WriteLine("Введите границы отрезка для поиска корней. Формат ввода: \"5 6\".");

                    string? bordersStr;
                    while(!checkInputStr(bordersStr = Console.ReadLine()))
                        Console.Write(ERR_STRING);

                    double[] borders = bordersStr.Split(" ").Select(Double.Parse).ToArray();

                    Console.WriteLine("Введите точность (разделителем в числе является запятая):");
                    double quality;
                    while (!Double.TryParse(Console.ReadLine(), out quality))
                        Console.WriteLine(ERR_STRING);

                    int DMIterations;
                    int NMIterations;

                    double? DMRoot = Solver.executeDivisionMethod(iMeth - 1, borders, quality, out DMIterations);
                    double? NMRoot = Solver.executeNewtonMethod(iMeth - 1, borders, quality, out NMIterations);

                    if (DMRoot != null)  Console.WriteLine("Корень, полученный методом половинного деления: x = {0}\nЧисло итераций метода: {1}", DMRoot, DMIterations);                   
                    else Console.WriteLine("Не удалось вычислить значение методом половинного деления.\n");

                    if (NMRoot != null) Console.WriteLine("Корень, полученный методом касательных: x = {0}\nЧисло итераций метода: {1}", NMRoot, NMIterations);
                    else Console.WriteLine("Не удалось вычислить значение методом касательных.");

                    if (DMRoot != null && NMRoot != null)
                        Console.WriteLine("Разница между значениями методов: \u0394 = {0}",Math.Abs((double)DMRoot - (double)NMRoot));
                    
                    break;
                case "2":
                    Console.WriteLine("Будет решена следующая система нелинейных уравнений:\n" +
                        "x + 3lg(x) - y² = 0\n" +
                        "2x² - xy - 5x + 1 = 0\n" +
                        "методом Ньютона. Введите точность(разделителем является запятая) :");

                    while (!Double.TryParse(Console.ReadLine(), out quality))
                        Console.WriteLine(ERR_STRING);

                    int NSMIterations;
                    double[] fallVals;

                    double[]? roots = Solver.executeNewtonSystemMethod(quality, out NSMIterations, out fallVals);
                    if (roots != null)
                        Console.WriteLine("Найденные решения:\nx1 = {0}\nx2 = {1}\nЧисло итераций метода: {2}", roots[0], roots[1], NSMIterations);
                    else Console.WriteLine("Не удалось найти решения с заданной точностью.");

                    Console.WriteLine("Сгенерированные начальные приближения: " + String.Join(" ", fallVals));

                    break;
                case "3":
                    Console.WriteLine("Уравнения:\n" +
                        "(1): x³ + 4x - 3 = 0\n" +
                        "(2): x² - 20sin(x) = 0\n" +
                        "(3): (x + 1)^(1/2) - 1/x = 0");
                    Console.WriteLine("Система:\n" +
                        "x + 3lg(x) - y² = 0\n" +
                        "2x² - xy - x + 1 = 0");
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        public static bool checkInputStr(string? str)
        {
            if (str == null)
                return false;
            string[] vs = str.Split(' ');
            double tmp;
            return Array.TrueForAll(vs, x => Double.TryParse(x, out tmp)) && vs.Length == 2;
        }
    }
}
