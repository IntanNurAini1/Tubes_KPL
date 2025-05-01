using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tubes_KPL.Controller;
using Tubes_KPL.Manager;
using Tubes_KPL.Model;

namespace Tubes_KPL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Menampilkan debug output (jika diperlukan di IDE)
            Debug.WriteLine("Program dimulai");

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
                            var createTaskManager = new CreateTaskManager<Model.Task>();
                            var createTaskController = new CreateTaskController<Model.Task>(createTaskManager);
                            string loggedInUsername = loginUsername;

                            var userTasks = createTaskController.GetTasks(loggedInUsername);

                            // ✅ Set konfigurasi dan reminder pertama kali saat login
                            Reminder.SetConfig(new ReminderConfig());
                            Reminder.CekDanUpdateTugasHampirDeadline(userTasks);

                            bool loggedIn = true;
                            while (loggedIn)
                            {
                                Console.WriteLine("\n=== Menu Setelah Login ===");
                                Console.WriteLine($"Selamat datang, {loggedInUsername}!");
                                Console.WriteLine("1. Buat Tugas Baru");
                                Console.WriteLine("2. Lihat Daftar Tugas Saya");
                                Console.WriteLine("3. Edit Tugas");
                                Console.WriteLine("4. Hapus Tugas");
                                Console.WriteLine("5. Tandai Tugas Selesai");
                                Console.WriteLine("6. Logout");
                                Console.Write("Pilih opsi: ");
                                string loggedInChoice = Console.ReadLine() ?? "";

                                switch (loggedInChoice)
                                {
                                    case "1":
                                        Console.WriteLine("\n=== Buat Tugas Baru ===");
                                        Console.Write("Masukkan nama tugas: ");
                                        string name = Console.ReadLine() ?? "";
                                        Console.Write("Masukkan deskripsi tugas: ");
                                        string desc = Console.ReadLine() ?? "";

                                        Console.WriteLine("Masukkan deadline tugas:");
                                        int day = GetIntInput("Tanggal (DD): ");
                                        int month = GetIntInput("Bulan (MM): ");
                                        int year = GetIntInput("Tahun (YYYY): ");
                                        int hour = GetIntInput("Jam (HH): ");
                                        int minute = GetIntInput("Menit (MM): ");

                                        var deadline = new Deadline { Day = day, Month = month, Year = year, Hour = hour, Minute = minute };

                                        Debug.Assert(!string.IsNullOrWhiteSpace(name), "Nama tugas tidak boleh kosong!");
                                        createTaskController.CreateTask(name, desc, deadline, loggedInUsername);
                                        break;

                                    case "2":
                                        Console.WriteLine("\n=== Daftar Tugas Anda ===");
                                        var tasks = createTaskController.GetTasks(loggedInUsername);

                                        if (tasks.Count == 0)
                                        {
                                            Console.WriteLine("Belum ada tugas yang ditambahkan.");
                                        }
                                        else
                                        {
                                            var sorted = tasks
                                                .OrderBy(t => t.Status == Status.Incompleted ? 0 :
                                                              t.Status == Status.Overdue ? 1 : 2)
                                                .ThenBy(t => new DateTime(t.Deadline.Year, t.Deadline.Month, t.Deadline.Day, t.Deadline.Hour, t.Deadline.Minute, 0));

                                            foreach (var task in sorted)
                                            {
                                                Console.WriteLine($"Nama: {task.Name}, Deskripsi: {task.Description}, Deadline: {task.Deadline}, Status: {task.Status}");
                                            }
                                        }
                                        break;

                                    case "3":
                                        Console.WriteLine("\n=== Edit Tugas ===");
                                        Console.Write("Masukkan nama tugas yang ingin diedit: ");
                                        string taskToEdit = Console.ReadLine() ?? "";

                                        Console.Write("Nama baru (kosongkan jika tidak ingin mengubah): ");
                                        string newName = Console.ReadLine();
                                        Console.Write("Deskripsi baru (kosongkan jika tidak ingin mengubah): ");
                                        string newDesc = Console.ReadLine();

                                        Console.Write("Ubah deadline? (y/n): ");
                                        string changeDeadline = Console.ReadLine() ?? "";
                                        Deadline newDeadline = null;
                                        if (changeDeadline.ToLower() == "y")
                                        {
                                            int d = GetIntInput("Tanggal (DD): ");
                                            int m = GetIntInput("Bulan (MM): ");
                                            int y = GetIntInput("Tahun (YYYY): ");
                                            int h = GetIntInput("Jam (HH): ");
                                            int min = GetIntInput("Menit (MM): ");
                                            newDeadline = new Deadline { Day = d, Month = m, Year = y, Hour = h, Minute = min };
                                        }

                                        createTaskController.EditTask(taskToEdit, loggedInUsername, newName, newDesc, newDeadline);
                                        break;

                                    case "4":
                                        Console.WriteLine("\n=== Hapus Tugas ===");
                                        Console.Write("Masukkan nama tugas yang ingin dihapus: ");
                                        string taskToDelete = Console.ReadLine() ?? "";
                                        createTaskController.DeleteTask(taskToDelete, loggedInUsername);
                                        break;

                                    case "5":
                                        Console.WriteLine("\n=== Tandai Tugas Selesai ===");
                                        Console.Write("Masukkan nama tugas yang ingin ditandai selesai: ");
                                        string taskToComplete = Console.ReadLine() ?? "";
                                        createTaskController.MarkTaskAsCompleted(taskToComplete, loggedInUsername);
                                        break;

                                    case "6":
                                        userController.Logout();
                                        loggedIn = false;
                                        break;

                                    default:
                                        Console.WriteLine("Opsi tidak valid. Silakan coba lagi.");
                                        break;
                                }

                                if (loggedIn)
                                {
                                    userTasks = createTaskController.GetTasks(loggedInUsername);
                                    Reminder.CekDanUpdateTugasHampirDeadline(userTasks);
                                }
                            }
                        }
                        break;

                    case "3":
                        Console.WriteLine("Terima kasih telah menggunakan aplikasi ini!");
                        return;

                    default:
                        Console.WriteLine("Opsi tidak valid. Silakan coba lagi.");
                        break;
                }
            }
        }

        static int GetIntInput(string prompt)
        {
            Console.Write(prompt);
            int value;
            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Input tidak valid. Coba lagi: ");
            }
            return value;
        }
    }
}
