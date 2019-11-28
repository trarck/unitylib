using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.UI
{
    public class PanelInfo
    {
        public enum State
        {
            None,
            Loading,
            Loaded
        }

        //TODO:使用asset的hash(int)值代替字符串
        public string asset;
        public UIPanel panel;
        public object data;
        public bool visible = true;
        public bool closble = false;

        public State state= State.None;

        public PanelInfo(string asset, object data)
        {
            this.asset = asset;
            this.data = data;
        }

        public bool IsLoading
        {
            get
            {
                return state == State.Loading;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return state == State.Loaded;
            }
        }
    }
}
