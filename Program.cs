using System;
using Tubes_KPL.Controller;
using Tubes_KPL.Model;
using Tubes_KPL.Manager;
using System.Diagnostics;

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
                                Console.WriteLine("3. Edit Tugas");
                                Console.WriteLine("4. Hapus Tugas");
                                Console.WriteLine("5. Logout");
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
                                        List<Model.Task> userTasks = createTaskControllerForUser.GetTasks(loggedInUsername);
                                        if (userTasks.Count == 0)
                                        {
                                            Console.WriteLine("Belum ada tugas yang ditambahkan.");
                                        }
                                        else
                                        {
                                            foreach (var task in userTasks)
                                            {
                                                // Menampilkan detail tugas, termasuk ID-nya.
                                                Console.WriteLine($"Nama: {task.Name}, Deskripsi: {task.Description}, Deadline: {task.Deadline}");
                                            }
                                        }
                                        break;
                                    case "3":
                                        Console.WriteLine("\n=== Edit Tugas ===");
                                        Console.Write("Masukkan Nama tugas yang ingin diedit: ");
                                        string taskNameToEdit = Console.ReadLine() ?? "";
                                        // Pastikan user tahu ID yang mana yang harus dimasukkan
                                        Console.WriteLine("Masukkan Nama tugas yang ingin diedit (lihat daftar tugas untuk Nama Tugas).");
                                        Console.Write("Nama baru (kosongkan jika tidak ingin mengubah): ");
                                        string newName = Console.ReadLine();

                                        Console.Write("Deskripsi baru (kosongkan jika tidak ingin mengubah): ");
                                        string newDesc = Console.ReadLine();

                                        Console.Write("Ubah deadline? (y/n): ");
                                        string changeDeadline = Console.ReadLine() ?? "";

                                        Deadline newDeadline = null;
                                        if (changeDeadline.ToLower() == "y")
                                        {
                                            Console.Write("Tanggal (DD): ");
                                            int day = int.Parse(Console.ReadLine() ?? "0");
                                            Console.Write("Bulan (MM): ");
                                            int month = int.Parse(Console.ReadLine() ?? "0");
                                            Console.Write("Tahun (YYYY): ");
                                            int year = int.Parse(Console.ReadLine() ?? "0");
                                            Console.Write("Jam (HH): ");
                                            int hour = int.Parse(Console.ReadLine() ?? "0");
                                            Console.Write("Menit (MM): ");
                                            int minute = int.Parse(Console.ReadLine() ?? "0");
                                            newDeadline = new Deadline { Day = day, Month = month, Year = year, Hour = hour, Minute = minute };
                                        }
                                        createTaskControllerForUser.EditTask(taskNameToEdit, loggedInUsername, newName, newDesc, newDeadline);
                                        break;
                                    case "4":
                                        Console.WriteLine("\n=== Hapus Tugas ===");
                                        Console.Write("Masukkan nama tugas yang ingin dihapus: ");
                                        string taskNameToDelete = Console.ReadLine() ?? "";

                                        // Pass the taskNameToDelete and loggedInUsername to the controller
                                        createTaskControllerForUser.DeleteTask(taskNameToDelete, loggedInUsername);
                                        break;

                                    case "5":
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