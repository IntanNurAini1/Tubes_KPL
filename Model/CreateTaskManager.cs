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
        private readonly string _filePath = "tasks.json"; // Nama file untuk menyimpan data

        public CreateTaskManager()
        {
            LoadTasks(); // Load data saat inisialisasi manajer
        }

        public void AddTask(T task)
        {
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
            Debug.Assert(!string.IsNullOrEmpty(userId), "Pre-condition: userId tidak boleh null atau kosong dalam GetTasks");
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh null atau kosong.");
                return new List<T>();
            }
            var result = tasks.Where(task => task.UserId == userId).ToList();
            Debug.Assert(result != null, "Post-condition: result tidak boleh null dalam GetTasks");
            return result;
        }

        public List<T> GetAllTasks()
        {
            Debug.Assert(tasks != null, "Post-condition: tasks tidak boleh null dalam GetAllTasks");
            return tasks;
        }

        public bool UpdateTask(string taskId, Action<T> updateAction)
        {
            Debug.Assert(!string.IsNullOrEmpty(taskId), "Pre-condition: taskId tidak boleh null atau kosong dalam UpdateTask");
            Debug.Assert(updateAction != null, "Pre-condition: updateAction tidak boleh null dalam UpdateTask");

            if (string.IsNullOrEmpty(taskId) || updateAction == null)
            {
                Console.WriteLine("taskId dan updateAction tidak boleh null atau kosong.");
                return false;
            }

            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                updateAction(task);
                SaveTasks(); // Simpan data setelah mengubah tugas
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
                Debug.WriteLine($"Exception in SaveTasks: {ex}");
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
    }
}
