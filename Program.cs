using System;
using Tubes_KPL.Controller;
using Tubes_KPL.Model;
using Tubes_KPL.Manager;

namespace Tubes_KPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var userManager = new UserManager();
            var userController = new UserController(userManager);

            while (true)
            {
                Console.WriteLine("\n=== Aplikasi Login dan Register ===");
                Console.WriteLine($"[AUTOMATA] State saat ini: {userController.GetCurrentState()}");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Keluar");
                Console.Write("Pilih opsi: ");

                string choice = Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":
                        Console.Write("Masukkan username untuk register: ");
                        string regUsername = Console.ReadLine() ?? "";
                        Console.Write("Masukkan password untuk register: ");
                        string regPassword = Console.ReadLine() ?? "";
                        userController.Register(regUsername, regPassword);
                        break;
                    case "2":
                        Console.Write("Masukkan username untuk login: ");
                        string loginUsername = Console.ReadLine() ?? "";
                        Console.Write("Masukkan password untuk login: ");
                        string loginPassword = Console.ReadLine() ?? "";
                        userController.Login(loginUsername, loginPassword);

                        if (userController.GetCurrentState() == State.LoggedIn)
                        {
                            string loggedInUsername = loginUsername;
                            var createTaskManagerForUser = new CreateTaskManager<Model.Task>();
                            var createTaskControllerForUser = new CreateTaskController<Model.Task>(createTaskManagerForUser);

                            bool loggedIn = true;
                            while (loggedIn)
                            {
                                Console.WriteLine("\n=== Menu Setelah Login ===");
                                Console.WriteLine($"Selamat datang, {loggedInUsername}!");
                                Console.WriteLine("1. Buat Tugas Baru");
                                Console.WriteLine("2. Lihat Daftar Tugas Saya");
                                Console.WriteLine("3. Logout");
                                Console.Write("Pilih opsi: ");

                                string loggedInChoice = Console.ReadLine() ?? "";

                                switch (loggedInChoice)
                                {
                                    case "1":
                                        Console.WriteLine("\n=== Buat Tugas Baru ===");
                                        Console.Write("Masukkan nama tugas: ");
                                        string taskName = Console.ReadLine() ?? "";
                                        Console.Write("Masukkan deskripsi tugas: ");
                                        string taskDeskripsi = Console.ReadLine() ?? "";
                                        Console.WriteLine("Masukkan deadline tugas:");
                                        Console.Write("Tanggal (DD): ");
                                        int taskDay = int.Parse(Console.ReadLine() ?? "0");
                                        Console.Write("Bulan (MM): ");
                                        int taskMonth = int.Parse(Console.ReadLine() ?? "0");
                                        Console.Write("Tahun (YYYY): ");
                                        int taskYear = int.Parse(Console.ReadLine() ?? "0");
                                        Console.Write("Jam (HH): ");
                                        int taskHour = int.Parse(Console.ReadLine() ?? "0");
                                        Console.Write("Menit (MM): ");
                                        int taskMinute = int.Parse(Console.ReadLine() ?? "0");

                                        var deadline = new Deadline
                                        {
                                            Day = taskDay,
                                            Month = taskMonth,
                                            Year = taskYear,
                                            Hour = taskHour,
                                            Minute = taskMinute
                                        };

                                        createTaskControllerForUser.CreateTask(taskName, taskDeskripsi, deadline, loggedInUsername);
                                        break;
                                    case "2":
                                        Console.WriteLine("\n=== Daftar Tugas Anda ===");
                                        foreach (var task in createTaskControllerForUser.GetTasks(loggedInUsername))
                                        {
                                            Console.WriteLine(task);
                                        }
                                        if (createTaskControllerForUser.GetTasks(loggedInUsername).Count == 0)
                                        {
                                            Console.WriteLine("Belum ada tugas yang ditambahkan.");
                                        }
                                        break;
                                    case "3":
                                        userController.Logout();
                                        loggedIn = false;
                                        break;
                                    default:
                                        Console.WriteLine("Opsi tidak valid. Silakan coba lagi.");
                                        break;
                                }
                            }
                        }
                        break;
                    case "3":
                        Console.WriteLine("Terima kasih!");
                        return;
                    default:
                        Console.WriteLine("Opsi tidak valid. Silakan coba lagi.");
                        break;
                }
            }
        }
    }
}
