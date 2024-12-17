using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Machineries;

public static class ImageDto
{	
	public class Index
	{
		public int Id { get; set; }
		public required string Url { get; set; }
	}
}
