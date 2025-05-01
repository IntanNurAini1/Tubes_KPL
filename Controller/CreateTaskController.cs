using System;
using System.Collections.Generic;
using Tubes_KPL.Model;
using Tubes_KPL.Manager;
using System.Diagnostics;

namespace Tubes_KPL.Controller
{
    public class CreateTaskController<T> where T : Model.Task
    {
        private readonly CreateTaskManager<T> taskManager;

        public CreateTaskController(CreateTaskManager<T> taskManager)
        {
            if (taskManager == null)
            {
                Debug.WriteLine("CreateTaskController: taskManager is null. Throwing ArgumentNullException.");
                throw new ArgumentNullException(nameof(taskManager), "taskManager tidak boleh null.");
            }
            this.taskManager = taskManager;
        }

        public void CreateTask(string name, string description, Deadline deadline, string userId)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Nama tugas tidak boleh kosong.");
                return;
            }
            if (string.IsNullOrEmpty(description))
            {
                Console.WriteLine("Deskripsi tugas tidak boleh kosong.");
                return;
            }
            if (deadline == null)
            {
                Console.WriteLine("Deadline tugas tidak boleh null.");
                return;
            }
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh kosong.");
                return;
            }

            try
            {
                T newTask = (T)Activator.CreateInstance(typeof(T), name, description, deadline, userId);
                taskManager.AddTask(newTask);
                Console.WriteLine("Tugas berhasil dibuat!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi kesalahan saat membuat tugas: {ex.Message}");
                Debug.WriteLine($"Exception in CreateTask: {ex}");
            }
        }

        public List<T> GetTasks(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("userId tidak boleh null atau empty");
                return new List<T>();
            }
            var tasks = taskManager.GetTasks(userId);
            if (tasks == null)
            {
                Debug.WriteLine("GetTasks mengembalikan null");
                return new List<T>();
            }
            return tasks;
        }

        public void EditTask(string taskId, string userId, string newName = null, string newDescription = null, Deadline newDeadline = null)
        {
            // Validasi input
            if (string.IsNullOrEmpty(taskId))
            {
                Console.WriteLine("ID tugas tidak boleh kosong.");
                return;
            }
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh kosong.");
                return;
            }

            try
            {
                bool updated = taskManager.UpdateTask(taskId, task =>
                {
                    if (task.UserId != userId)
                    {
                        Console.WriteLine("Kamu tidak bisa mengedit task milik orang lain.");
                        return; // Penting: Keluar dari lambda expression
                    }

                    if (!string.IsNullOrWhiteSpace(newName)) task.Name = newName;
                    if (!string.IsNullOrWhiteSpace(newDescription)) task.Description = newDescription;
                    if (newDeadline != null) task.Deadline = newDeadline;
                });

                if (updated)
                    Console.WriteLine("Tugas berhasil diubah!");
                else
                    Console.WriteLine("Tugas tidak ditemukan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Terjadi kesalahan saat mengedit tugas: {ex.Message}");
                Debug.WriteLine($"Exception in EditTask: {ex}");
            }
        }
    }
}
