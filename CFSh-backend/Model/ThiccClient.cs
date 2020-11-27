using System;
using System.Collections.Generic;

namespace CFSh_backend.Model
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
