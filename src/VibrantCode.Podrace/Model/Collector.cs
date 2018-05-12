namespace VibrantCode.Podrace.Model
{
    /// <summary>
    /// Represents a collector, which collects data either during or after the run.
    /// </summary>
    public abstract class Collector
    {
        /// <summary>
        /// Gets or sets the type of the collector.
        /// </summary>
        public string Type { get; set; }
    }
}
