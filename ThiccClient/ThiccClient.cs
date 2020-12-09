using System;
using System.Collections.Generic;

namespace ThiccClient
{
	public class ThiccClient
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int FreeSpace { get; set; }
		public int UsedSpace { get; set; }
		public string IP { get; set; }
		public int Port { get; set; }
	}
}
