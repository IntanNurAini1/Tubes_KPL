using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tubes_KPL.Model;

namespace Tubes_KPL.Model
{
    public enum State { Start, LoggedIn, LoggedOut }
    public enum Input { LoginSuccess, LoginFail, Logout, RegisterSuccess, RegisterFail }

    public class UserManager
    {
        private readonly Dictionary<string, string> userTable = new Dictionary<string, string>();
        public State CurrentState { get; private set; }

        // Tabel transisi automata
        private readonly Dictionary<(State, Input), State> transitions = new Dictionary<(State, Input), State>
        {
            {(State.Start, Input.LoginSuccess), State.LoggedIn},
            {(State.Start, Input.LoginFail), State.Start},
            {(State.LoggedIn, Input.Logout), State.LoggedOut},
            {(State.LoggedOut, Input.LoginSuccess), State.LoggedIn},
            {(State.Start, Input.RegisterSuccess), State.Start},
            {(State.Start, Input.RegisterFail), State.Start}
        };

        public UserManager()
        {
            CurrentState = State.Start;

            // Data awal dummy
            userTable["admin"] = "admin123";
            userTable["user"] = "user123";
        }

        public bool Register(string username, string password)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(username), "Username tidak boleh kosong");
            Debug.Assert(!string.IsNullOrWhiteSpace(password), "Password tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username wajib diisi.");
                Transition(Input.RegisterFail);
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password wajib diisi.");
                Transition(Input.RegisterFail);
                return false;
            }

            if (userTable.ContainsKey(username))
            {
                Console.WriteLine("Username sudah terdaftar.");
                Transition(Input.RegisterFail);
                return false;
            }

            userTable[username] = password;
            Debug.Assert(userTable.ContainsKey(username), "User harus sudah tersimpan setelah register");
            Console.WriteLine("Registrasi berhasil!");
            Transition(Input.RegisterSuccess);
            return true;
        }

        public bool Login(string username, string password)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(username), "Username tidak boleh kosong");
            Debug.Assert(!string.IsNullOrWhiteSpace(password), "Password tidak boleh kosong");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username dan password wajib diisi.");
                Transition(Input.LoginFail);
                return false;
            }

            if (userTable.ContainsKey(username) && userTable[username] == password)
            {
                Transition(Input.LoginSuccess);
                Console.WriteLine("Login berhasil!");
                return true;
            }

            Transition(Input.LoginFail);
            Console.WriteLine("Login gagal.");
            return false;
        }

        public void Logout()
        {
            if (CurrentState == State.LoggedIn)
            {
                Transition(Input.Logout);
                Console.WriteLine("Logout berhasil!");
            }
            else
            {
                Console.WriteLine("Kamu belum login.");
            }
        }

        private void Transition(Input input)
        {
            if (transitions.TryGetValue((CurrentState, input), out State newState))
            {
                Console.WriteLine($"[AUTOMATA] Transisi dari {CurrentState} ke {newState} karena input {input}");
                CurrentState = newState;
            }
            else
            {
                Console.WriteLine($"[AUTOMATA] Transisi tidak valid dari {CurrentState} dengan input {input}");
            }
        }

        public IEnumerable<string> GetUsernames() => userTable.Keys;

        public void AddUser(string username, string password)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(username), "Username tidak boleh kosong saat menambahkan user secara manual");
            Debug.Assert(!string.IsNullOrWhiteSpace(password), "Password tidak boleh kosong saat menambahkan user secara manual");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Username dan password wajib diisi saat menambahkan pengguna manual.");
                return;
            }

            if (!userTable.ContainsKey(username))
            {
                userTable[username] = password;
                Console.WriteLine($"Pengguna '{username}' berhasil ditambahkan ke tabel.");
            }
            else
            {
                Console.WriteLine($"Pengguna '{username}' sudah ada di tabel.");
            }
        }

        internal string GetPassword(string username)
        {
            return userTable.TryGetValue(username, out var password) ? password : null;
        }
    }
}