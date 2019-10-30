using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YH.UI;

public class TestDialogPanel : UIPanel
{
    public void Cancel()
    {
        Close();
    }

    public void Confirm()
    {
        Close();
    }
}
