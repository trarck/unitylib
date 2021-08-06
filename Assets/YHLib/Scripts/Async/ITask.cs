using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.Async
{
	public interface ITask
	{
		int id
		{
			get;
			set;
		}

		bool breakOnError
		{
			get;set;
		}

		void Run();
		void Done();
		void Error(int errorCode);
		void Clean();
	}
}
