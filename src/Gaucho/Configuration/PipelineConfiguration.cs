using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Configuration
{
    public class PipelineConfiguration
    {
        public string Id { get; set; }

        public HandlerNode InputHandler { get; set; }

        public List<HandlerNode> OutputHandlers { get; set; }

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
