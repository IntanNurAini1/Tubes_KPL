using System.Collections.Generic;

namespace Tubes_KPL.Model
{
    public class ReminderConfig
    {
        public Dictionary<int, string> DayCategory { get; private set; }

        public ReminderConfig()
        {
            // Konfigurasi runtime (bisa diubah sesuai kebutuhan)
            DayCategory = new Dictionary<int, string>
            {
                { 0, "Hari Ini" },
                { 1, "Besok" }
            };
        }
    }
}
