using System;
using System.Collections.Generic;

namespace CO2Monitor.Infrastructure.Helpers {
	/// <summary>
	/// Parser for command lines like "command [-flags] [--option optArg] arg0 arg1"
	/// </summary>
	public class CommandLineParser {
		private static readonly char[] Spaces = new[] { ' ', '\t', '\n', '\r' };

		public CommandLineParser(string args) {
			string[] words = args.Split(Spaces, StringSplitOptions.RemoveEmptyEntries);
			if (args.Length == 0) {
				return;
			}

			Commnad = words[0];
			var options = new Dictionary<string, string>();
			var arguments = new List<string>();
			var flags = new HashSet<string>();

			for (var i = 1; i < words.Length; i++) {
				if (words[i].StartsWith("--") && i + 1 < words.Length) {
					options.Add(words[i].Substring(2), words[++i]);
				} else if (words[i].StartsWith("-")) {
					flags.Add(words[i].Substring(1));
				} else {
					arguments.Add(words[i]);
				}
			}

			Flags = flags;
			Options = options;
			Arguments = arguments;
		}

		public string Commnad { get; }

		public IReadOnlyList<string> Arguments { get; }

		public IReadOnlyDictionary<string, string> Options { get; set; } 

		public IReadOnlyCollection<string> Flags { get; }
	}
}
