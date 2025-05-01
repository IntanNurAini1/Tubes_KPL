using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tubes_KPL.Model;

namespace Tubes_KPL
{
    public static class Reminder
    {
        private static ReminderConfig config;

        public static void SetConfig(ReminderConfig newConfig)
        {
            Debug.WriteLine("Setting ReminderConfig...");
            config = newConfig ?? throw new ArgumentNullException(nameof(newConfig));
        }

        public static void CekDanUpdateTugasHampirDeadline(List<Model.Task> tasks)
        {
            Debug.Assert(config != null, "ReminderConfig harus sudah di-set sebelum memanggil fungsi ini.");
            Debug.WriteLine($"[Reminder] Mulai pengecekan {tasks.Count} tugas...");

            DateTime now = DateTime.Now;

            foreach (var task in tasks)
            {
                Debug.WriteLine($"Memeriksa tugas: {task.Name} (Status: {task.Status})");

                DateTime deadline = new DateTime(
                    task.Deadline.Year, task.Deadline.Month, task.Deadline.Day,
                    task.Deadline.Hour, task.Deadline.Minute, 0);

                TimeSpan sisaWaktu = deadline - now;

                if (task.Status == Status.Incompleted)
                {
                    if (sisaWaktu.TotalMinutes < 0)
                    {
                        task.Status = Status.Overdue;
                        Debug.WriteLine($"Tugas '{task.Name}' sudah lewat deadline. Status diubah ke OVERDUE.");
                    }
                    else
                    {
                        int sisaHari = (int)sisaWaktu.TotalDays;

                        if (config.DayCategory.ContainsKey(sisaHari))
                        {
                            string label = config.DayCategory[sisaHari];
                            string deadlineStr = deadline.ToString("dd/MM/yyyy HH:mm");
                            string message = $"[PENGINGAT] Tugas '{task.Name}' akan jatuh tempo {label} pada {deadlineStr}.";

                            Debug.WriteLine(message);
                            Console.WriteLine(message);
                        }
                    }
                }
            }

            Debug.WriteLine("[Reminder] Selesai memeriksa tugas.\n");
        }
    }
}
