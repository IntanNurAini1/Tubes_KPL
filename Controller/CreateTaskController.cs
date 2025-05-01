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
            // Defensive Programming:  Cek null di konstruktor
            if (taskManager == null)
            {
                Debug.WriteLine("CreateTaskController: taskManager is null. Throwing ArgumentNullException.");
                throw new ArgumentNullException(nameof(taskManager), "taskManager tidak boleh null.");
            }
            this.taskManager = taskManager;
        }

        public void CreateTask(string name, string description, Deadline deadline, string userId)
        {
            // Defensive Programming: Validasi parameter
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
                T newTask = (T)Activator.CreateInstance(typeof(T), name, description, deadline, userId); // Tambah userId ke instance
                taskManager.AddTask(newTask);
                Console.WriteLine("Tugas berhasil dibuat!");
            }
            catch (Exception ex)
            {
                // Defensive Programming: Tangkap exception dan log
                Console.WriteLine($"Terjadi kesalahan saat membuat tugas: {ex.Message}");
                Debug.WriteLine($"Exception in CreateTask: {ex}"); // Log exception
            }
        }

        public List<T> GetTasks(string userId)
        {
            // Defensive Programming: userId tidak boleh null atau empty
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

        public void EditTask(string taskName, string userId, string newName = null, string newDescription = null, Deadline newDeadline = null)
        {
            // Defensive Programming: Validasi parameter
            if (string.IsNullOrEmpty(taskName))
            {
                Console.WriteLine("Nama Tugas tidak boleh kosong.");
                return;
            }
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh kosong.");
                return;
            }

            try
            {
                bool updated = taskManager.UpdateTask(taskName, userId, task =>
                {
                    // Design by Contract: Pre-condition di dalam lambda
                    Debug.Assert(task != null, "Pre-condition: task tidak boleh null di dalam lambda EditTask");
                    if (task == null)
                    {
                        Console.WriteLine("Task tidak ditemukan.");
                        return;
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
                // Defensive Programming: Tangkap dan log exception
                Console.WriteLine($"Terjadi kesalahan saat mengedit tugas: {ex.Message}");
                Debug.WriteLine($"Exception in EditTask: {ex}");
            }
        }

        public void DeleteTask(string taskName, string userId)
        {
            // Defensive Programming: Validasi parameter
            if (string.IsNullOrEmpty(taskName))
            {
                Console.WriteLine("Nama tugas tidak boleh kosong.");
                return;
            }
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User ID tidak boleh kosong.");
                return;
            }

            try
            {
                bool deleted = taskManager.DeleteTask(taskName, userId);
                if (deleted)
                {
                    Console.WriteLine("Tugas berhasil dihapus!");
                }
                else
                {
                    Console.WriteLine("Tugas tidak ditemukan atau gagal dihapus.");
                }
            }
            catch (Exception ex)
            {
                // Defensive Programming: Tangkap dan log exception
                Console.WriteLine($"Terjadi kesalahan saat menghapus tugas: {ex.Message}");
                Debug.WriteLine($"Exception in DeleteTask: {ex}");
            }
        }

    }
}