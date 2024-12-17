using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Shared.Machineries;

public abstract class MachineryResult
{
	public class Create
	{
		public int Id { get; set; }
		public List<string> UploadUris { get; set; } = default!;
	}
}
