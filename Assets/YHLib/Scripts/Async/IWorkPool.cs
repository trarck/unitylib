using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.Async
{
	public interface IWorkPool
	{
		void Add(ITask task);
		void DoTaskDone(ITask task);
		void DoTaskError(ITask task, int errorCode);
		void Start();
		void Next();
		void Finish();
		void Join();
		void Complete();
		void Clean();
	}
}
