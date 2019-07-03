using System;
using Newtonsoft.Json;

namespace ConsoleApp {
	public class Program {
		static void Main() {
			Console.WriteLine(JsonConvert.SerializeObject((Date: DateTime.Today, IsWorkDay: true)));
		}
	}
}
