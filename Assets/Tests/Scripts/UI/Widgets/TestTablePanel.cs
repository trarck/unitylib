using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH.UI;

public class TestTablePanel : MonoBehaviour,BigTable.IDataProvider
{
    [SerializeField]
    GameObject m_CellPrefab;

    [SerializeField]
    BigTable m_BigTableV;

    [SerializeField]
    BigTable m_BigTableH;

    public int count
    {
        get
        {
            return 59;
        }
    }

    public int startIndex
    {
        get
        {
            return 0;
        }
    }

    public BigTable.Cell CreateCell(int tag, BigTable list)
    {
        GameObject cellObj = GameObject.Instantiate<GameObject>(m_CellPrefab, list.content);
        BigTable.Cell cell = new BigTable.Cell();
        cell.content = cellObj.GetComponent<RectTransform>();
        return cell;
    }

    public void UpdateCell(BigTable.Cell cell)
    {
        Text text = cell.content.GetComponentInChildren<Text>();
        text.text = cell.index.ToString()+"("+cell.row+","+cell.col+")";
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_BigTableH)
        {
            m_BigTableH.dataProvider = this;
        }

        if(m_BigTableV)
        {
            m_BigTableV.dataProvider = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
