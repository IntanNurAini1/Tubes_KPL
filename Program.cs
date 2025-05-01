using System;
using Tubes_KPL.Controller;
using Tubes_KPL.Model;

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
                Console.WriteLine("3. Logout");
                Console.WriteLine("4. Tambah Pengguna Manual");
                Console.WriteLine("5. Lihat Daftar Pengguna");
                Console.WriteLine("6. Keluar");
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
                        break;
                    case "3":
                        userController.Logout();
                        break;
                    case "4":
                        Console.Write("Masukkan username yang ingin ditambahkan: ");
                        string addUserUsername = Console.ReadLine() ?? "";
                        Console.Write("Masukkan password untuk pengguna tersebut: ");
                        string addUserPassword = Console.ReadLine() ?? "";
                        userController.AddUser(addUserUsername, addUserPassword);
                        break;
                    case "5":
                        Console.WriteLine("\n=== Daftar Pengguna ===");
                        foreach (var username in userController.GetUsernames())
                        {
                            Console.WriteLine($"Username: {username}");
                        }
                        Console.WriteLine("(Password tidak ditampilkan untuk keamanan)");
                        break;
                    case "6":
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