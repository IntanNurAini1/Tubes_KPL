using System;
using System.Collections.Generic;
using Tubes_KPL.Model;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Diagnostics;

namespace Tubes_KPL.Manager
{
    public class CreateTaskManager<T> where T : Model.Task
    {
        private List<T> tasks = new List<T>();
        private readonly string _filePath = "tasks.json";

        // Automata: Tabel transisi status tugas
        private readonly Dictionary<(Status, string), Status> statusTransitions = new()
    {
        {(Status.Incompleted, "TandaiSelesai"), Status.Completed},
        {(Status.Overdue, "TandaiSelesai"), Status.Completed}
    };

        public CreateTaskManager()
        {
            LoadTasks();
        }

        public void AddTask(T task)
        {
            // Pre-condition: Task tidak boleh null
            Debug.Assert(task != null, "Pre-condition: task tidak boleh null dalam AddTask");
            if (task == null)
            {
                Console.WriteLine("Tugas yang akan ditambahkan tidak boleh null.");
                return;
            }

            tasks.Add(task);
            SaveTasks(); // Simpan data setelah menambahkan tugas
            Debug.Assert(tasks.Count > 0, "Post-condition: tasks.Count harus bertambah 1 dalam AddTask");
        }

        public List<T> GetTasks(string userId)
        {
            // Pre-condition: userId tidak boleh null atau kosong
            Debug.Assert(!string.IsNullOrEmpty(userId), "Pre-condition: userId tidak boleh null atau kosong dalam GetTasks");
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh null atau kosong.");
                return new List<T>(); // Mengembalikan list kosong daripada null
            }
            var result = tasks.Where(task => task.UserId == userId).ToList();
            // Post-condition: result tidak boleh null
            Debug.Assert(result != null, "Post-condition: result tidak boleh null dalam GetTasks");
            return result;
        }

        public List<T> GetAllTasks()
        {
            // Post-condition: Tidak ada post-condition khusus, tapi memastikan tidak mengembalikan null
            Debug.Assert(tasks != null, "Post-condition: tasks tidak boleh null dalam GetAllTasks");
            return tasks;
        }

        public bool UpdateTask(string taskName, string userId, Action<T> updateAction)
        {
            // Pre-condition
            Debug.Assert(!string.IsNullOrEmpty(taskName), "Pre-condition: taskName tidak boleh null atau kosong dalam UpdateTask");
            Debug.Assert(updateAction != null, "Pre-condition: updateAction tidak boleh null dalam UpdateTask");
            Debug.Assert(!string.IsNullOrEmpty(userId), "Pre-condition: userId tidak boleh null atau kosong dalam UpdateTask");

            if (string.IsNullOrEmpty(taskName) || updateAction == null || string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("taskName, userId, dan updateAction tidak boleh null atau kosong.");
                return false;
            }

            var task = tasks.FirstOrDefault(t => t.Name == taskName && t.UserId == userId);
            if (task != null)
            {
                updateAction(task); // Memakai lambda agar reusable
                SaveTasks(); // Simpan perubahan
                return true;
            }
            return false;
        }

        private void SaveTasks()
        {
            try
            {
                // Serialize data ke JSON
                var options = new JsonSerializerOptions { WriteIndented = true }; //pretty print
                string jsonString = JsonSerializer.Serialize(tasks, options);
                File.WriteAllText(_filePath, jsonString); // Simpan ke file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Gagal menyimpan tugas ke file: {ex.Message}");
                Debug.WriteLine($"Exception in SaveTasks: {ex}"); // Log exception
            }
        }

        private void LoadTasks()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(_filePath);
                    tasks = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>(); //handle null
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Gagal memuat tugas dari file: {ex.Message}");
                    Debug.WriteLine($"Exception in LoadTasks: {ex}");
                    tasks = new List<T>(); // Inisialisasi tasks sebagai list kosong
                }
            }
            else
            {
                tasks = new List<T>();
            }
        }

        public bool DeleteTask(string taskName, string userId)
        {
            // Defensive Programming: Validate input parameters
            if (string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("taskName dan userId tidak boleh null atau kosong.");
                return false; // Early exit if input parameters are invalid
            }

            // Design by Contract: Pre-condition assertion to validate inputs
            Debug.Assert(!string.IsNullOrEmpty(taskName), "Pre-condition: taskName tidak boleh kosong dalam DeleteTask");
            Debug.Assert(!string.IsNullOrEmpty(userId), "Pre-condition: userId tidak boleh kosong dalam DeleteTask");

            // Debugging: Log the attempt to delete the task
            Debug.WriteLine($"Attempting to delete task: taskName = {taskName}, userId = {userId}");

            try
            {
                var task = tasks.FirstOrDefault(t => t.Name == taskName && t.UserId == userId);



                if (task != null)
                {
                    tasks.Remove(task); // Remove the task from the list
                    SaveTasks(); // Save changes to file

                    // Design by Contract: Ensure the task is successfully removed
                    Debug.Assert(!tasks.Contains(task), "Post-condition: task harus sudah dihapus setelah RemoveTask");

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Design by Contract: Handle unexpected errors and ensure the invariants are not broken
                Console.WriteLine($"Terjadi kesalahan saat menghapus tugas: {ex.Message}");
                Debug.WriteLine($"Exception in DeleteTask: {ex}"); // Log exception details

                return false; // Ensure method returns false in case of error
            }
        }
        public bool MarkAsCompleted(string taskName, string userId)
        {
            Debug.WriteLine($"[DEBUG] Menandai selesai: {taskName}, User: {userId}");

            if (string.IsNullOrEmpty(taskName) || string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Nama tugas dan userId tidak boleh kosong.");
                return false;
            }

            var task = tasks.FirstOrDefault(t => t.Name == taskName && t.UserId == userId);

            if (task == null)
            {
                Console.WriteLine("Tugas tidak ditemukan.");
                return false;
            }

            if (statusTransitions.TryGetValue((task.Status, "TandaiSelesai"), out Status newStatus))
            {
                Debug.Assert(task.Status != Status.Completed, "Task seharusnya belum selesai.");
                task.Status = newStatus;
                SaveTasks();
                Console.WriteLine("Tugas berhasil ditandai selesai!");
                return true;
            }

            Console.WriteLine("Transisi status tidak valid.");
            return false;
        }



    }
}