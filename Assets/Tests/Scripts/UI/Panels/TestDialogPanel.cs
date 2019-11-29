using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;

public class TestDialogPanel : UIPanel
{
    public void Cancel()
    {
        //通过UIManager的ShowPanel显示的Panel要用UIManager的Close来关闭。不能直接调用UIPanel的Close.
        UIManager.Instance.ClosePanel(this);
    }

    public void Confirm()
    {
        UIManager.Instance.ClosePanel(this);
    }
}
