using System.Collections.Generic;
using UnityEngine;

namespace YH.UI.Mvc
{
    public class NavigateController :  Controller
    {
        Stack<IController> m_Stack=new Stack<IController>();

        public void Push(IController controller)
        {
            IController current = m_Stack.Count>0?m_Stack.Peek():null;
            m_Stack.Push(controller);
            controller.viewDidLoadHandle = (subView) =>
            {
                view.AddSubView(subView);
                if (current != null)
                {
                    current.view.RemoveFromSuperView();
                }
            };

            controller.LoadViewIfNeed();
        }

        public void Pop()
        {
            if (m_Stack.Count == 1)
            {
                Debug.Log("At root can't pop");
                return;
            }

            IController current = m_Stack.Pop();
            IController next = m_Stack.Peek();

            next.viewDidLoadHandle = (subView) =>
            {
                view.AddSubView(subView);

                if (IsUsing(current))
                {
                    current.view.RemoveFromSuperView();
                }
                else
                {
                    current.Dispose();
                }
            };
            next.LoadViewIfNeed();
        }

        public void Replace(IController controller)
        {
            IController current = m_Stack.Pop();
            m_Stack.Push(controller);
            controller.viewDidLoadHandle = (subView) =>
            {
                view.AddSubView(subView);

                if (IsUsing(current))
                {
                    current.view.RemoveFromSuperView();
                }
                else
                {
                    current.Dispose();
                }
            };

            controller.LoadViewIfNeed();
        }

        List<IController> m_Temps = null;

        public void PopToStackLevel(int level)
        {
            if (level == 0)
            {
                //do nothing
                return;
            }
            else if (level < 0)
            {
                //remove tops
                level = m_Stack.Count + level;
            }

            int c = m_Stack.Count;

            if (m_Temps == null)
            {
                m_Temps = new List<IController>();
            }

            while (c-- > level)
            {
                m_Temps.Add(m_Stack.Pop());
            }

            IController next = m_Stack.Peek();
            //先show，后remove
            next.viewDidLoadHandle = (subView) =>
            {
                view.AddSubView(subView);
                //remove poped controllers
                for (int i = 0; i < m_Temps.Count; ++i)
                {
                    IController old = m_Temps[i];
                    if (IsUsing(old))
                    {
                        old.view.RemoveFromSuperView();
                    }
                    else
                    {
                        old.Dispose();
                    }
                }
                m_Temps.Clear();
            };
            next.LoadViewIfNeed();            
        }

        public void PopToRoot()
        {
            PopToStackLevel(1);
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (m_Stack != null)
            {
                foreach (var controller in m_Stack)
                {
                    controller.Dispose();
                }
            }
            m_Stack = null;
        }

        bool IsUsing(IController controller)
        {
            foreach (var iter in m_Stack)
            {
                if (iter == controller)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
