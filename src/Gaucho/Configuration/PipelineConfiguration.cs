using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Configuration
{
	/// <summary>
	/// Configuration of a Pipeline
	/// </summary>
    public class PipelineConfiguration
    {
		/// <summary>
		/// Gets or sets the PipelineId
		/// </summary>
        public string Id { get; set; }

		/// <summary>
		/// Gets or set the configuration of the InputHandler
		/// </summary>
        public HandlerNode InputHandler { get; set; }

		/// <summary>
		/// Gets or set the configurations of the OutputHandlers
		/// </summary>
		public List<HandlerNode> OutputHandlers { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="PipelineOptions"/> that contain configurations for each pipeline
		/// </summary>
        public PipelineOptions Options { get; set; } = new PipelineOptions();

        /// <inheritdoc/>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine($"  id: {Id}");
			sb.AppendLine("  inpoutHandler:");
			if (string.IsNullOrEmpty(InputHandler.Name))
			{
				sb.AppendLine($"     type: {InputHandler.Type.FullName}");
			}
			else
			{
				sb.AppendLine($"     name: {InputHandler.Name}");
				if (InputHandler.Type != null)
				{
					sb.AppendLine($"     type: {InputHandler.Type.FullName}");
				}
			}

			if (InputHandler.Arguments != null)
			{
				sb.AppendLine($"     arguments: [{string.Join(", ", InputHandler.Arguments.Select(a => $"{a.Key}: {a.Value}"))}]");
			}

			if (InputHandler.Filters != null)
			{
				sb.AppendLine($"     filters: [{string.Join(", ", InputHandler.Filters)}]");
			}

			sb.AppendLine("  outputHandlers:");
			foreach (var handler in OutputHandlers)
			{
				if (string.IsNullOrEmpty(handler.Name))
				{
					sb.AppendLine($"     - type: {handler.Type.FullName}");
				}
				else
				{
					sb.AppendLine($"     - name: {handler.Name}");
					if (handler.Type != null)
					{
						sb.AppendLine($"       type: {handler.Type.FullName}");
					}
				}

				if (handler.Arguments != null)
				{
					sb.AppendLine($"       arguments: [{string.Join(", ", handler.Arguments.Select(a => $"{a.Key}: {a.Value}"))}]");
				}

				if (handler.Filters != null)
				{
					sb.AppendLine($"       filters: [{string.Join(", ", handler.Filters)}]");
				}
			}

			return sb.ToString();
		}
	}
}
