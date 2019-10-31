using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.UI.Mvc
{
    public interface IController
    {
        IView view { get; set; }
        void Init();
        void Init(string viewAsset);
        void LoadView();
    }
}
