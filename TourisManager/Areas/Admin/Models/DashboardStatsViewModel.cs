using System.Collections.Generic;

namespace TourisManager.Areas.Admin.Models
{
    // Chứa dũ liệu thống kê
    public class DashboardStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalLocations { get; set; }

        public Dictionary<string, int> UsersByRole { get; set; }
            = new();

        public List<AreaLocationStat> LocationsByArea { get; set; }
            = new();
    }

    public class AreaLocationStat
    {
        public string AreaName { get; set; } = string.Empty;
        public int TotalLocations { get; set; }
    }
}