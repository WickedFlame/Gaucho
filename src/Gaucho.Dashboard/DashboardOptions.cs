
namespace Gaucho.Dashboard
{
    public class DashboardOptions
    {
        /// <summary>
        /// The title of the dashboard
        /// </summary>
	    public string Title { get; set; } = "Gaucho";

        /// <summary>
        /// The amount of logs that are displayed in the dashboard
        /// </summary>
        public int LogCount { get; set; } = 20;

        /// <summary>
        /// The amount of logs that are displayed in the WorkersLog in the dashboard
        /// </summary>
        public int WorkersLogCount { get; set; } = 50;
    }
}
