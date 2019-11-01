using System.Collections.Generic;
using UnityEngine;

namespace YH.UI.Mvc
{
    public interface IView
    {
        RectTransform rectTransorm { get; }
        IView superView { get;  }
        List<IView> children { get; }
        IController controller { get; set; }

        void AddSubView(IView view);
        void RemoveFromSuperView();

        void InsertSubView(IView view, int index);
        void InsertSubView(IView view, IView siblingSubView);

        void DidAddSubView(IView subView);
        void WillRemoveSubView(IView subView);
        void CleanSubView();
        void Dispose();
    }
}
