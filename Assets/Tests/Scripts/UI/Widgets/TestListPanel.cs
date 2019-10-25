using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YH.UI;
public class TestListPanel : MonoBehaviour,BigList.IDataProvider
{
    [SerializeField]
    GameObject m_ItemPrefab;

    [SerializeField]
    BigList m_BigListV;

    [SerializeField]
    BigList m_BigListH;

    public int count
    {
        get
        {
            return 20;
        }
    }

    public float itemSize
    {
        get
        {
            return 100;
        }
    }

    public int startIndex
    {
        get
        {
            return 0;
        }
    }

    public BigList.Item CreateItem(int index, BigList list)
    {
        GameObject itemObj = GameObject.Instantiate<GameObject>(m_ItemPrefab, list.content);
        BigList.Item item = new BigList.Item();
        item.content = itemObj.GetComponent<RectTransform>();
        return item;
    }

    public void UpdateItem(BigList.Item item)
    {
        Text text = item.content.GetComponentInChildren<Text>();
        text.text = item.index.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BigListH.dataProvider = this;
        m_BigListV.dataProvider = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
