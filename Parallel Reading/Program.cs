using System.Diagnostics;

namespace ParallelFileReader
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            // Генерация тестовых файлов для демонстрации
            CreateTestFiles();

            // Задание 1 + Задание 3 (замер времени)
            string[] threeFiles = { "file1.txt", "file2.txt", "file3.txt" };

            var sw1 = Stopwatch.StartNew();
            long spacesInThreeFiles = await CountSpacesInFilesAsync(threeFiles);
            sw1.Stop();
            Console.WriteLine($"[Задание 1] Найдено пробелов в 3 файлах: {spacesInThreeFiles}");
            Console.WriteLine($"[Задание 3] Время выполнения: {sw1.ElapsedMilliseconds} мс\n");

            // Задание 2 + Задание 3 (замер времени)
            string folderPath = "test_folder";
            var sw2 = Stopwatch.StartNew();
            long spacesInFolder = await CountSpacesInFolderAsync(folderPath);
            sw2.Stop();
            Console.WriteLine($"[Задание 2] Найдено пробелов в папке '{folderPath}': {spacesInFolder}");
            Console.WriteLine($"[Задание 3] Время выполнения: {sw2.ElapsedMilliseconds} мс");
        }

        // Задание 1: Параллельное чтение заданного массива файлов
        static async Task<long> CountSpacesInFilesAsync(string[] filePaths)
        {
            // Создаём задачи для каждого файла
            var tasks = filePaths.Select(path => CountSpacesInSingleFileAsync(path));

            // Ждём завершения всех задач параллельно
            long[] results = await Task.WhenAll(tasks);

            // Суммируем результаты
            return results.Sum();
        }

        // Задание 2: Функция, принимающая путь к папке
        static async Task<long> CountSpacesInFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"Папка '{folderPath}' не найдена.");
                return 0;
            }

            // Получаем все файлы (фильтр *.txt, чтобы не читать бинарные)
            string[] files = Directory.GetFiles(folderPath, "*.txt");
            if (files.Length == 0) return 0;

            // Запускаем параллельное чтение
            var tasks = files.Select(path => CountSpacesInSingleFileAsync(path));
            long[] results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        // Базовый метод: чтение одного файла и подсчёт пробелов
        static async Task<long> CountSpacesInSingleFileAsync(string filePath)
        {
            try
            {
                // Асинхронное чтение
                string content = await File.ReadAllTextAsync(filePath);

                long count = 0;
                foreach (char c in content)
                {
                    if (c == ' ') count++;
                }
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении {Path.GetFileName(filePath)}: {ex.Message}");
                return 0;
            }
        }
        static void CreateTestFiles()
        {
            string[] names = { "file1.txt", "file2.txt", "file3.txt" };
            foreach (var name in names)
                File.WriteAllText(name, GenerateRandomText(200_000));

            string folder = "test_folder";
            Directory.CreateDirectory(folder);
            for (int i = 0; i < 5; i++)
                File.WriteAllText(Path.Combine(folder, $"data_{i}.txt"), GenerateRandomText(300_000));
        }

        static string GenerateRandomText(int length)
        {
            var rnd = new Random();
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
                buffer[i] = rnd.NextDouble() > 0.15 ? (char)rnd.Next(33, 126) : ' ';
            return new string(buffer);
        }
    }
}