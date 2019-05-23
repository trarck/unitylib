using System;

namespace YH.Messages
{
    public class Message
    {
        //全局消息
        public const int GlobalMessageType = 0;

        //消息类型。逻辑消息最好从1000开始。
        int m_Type;
        //发送者
        int m_Sender;
        //接收者
        int m_Receiver;
        //数据
        object m_Data;

        public int type { get { return m_Type; } set { m_Type = value; } }

        public int sender { get { return m_Sender; } set { m_Sender = value; } }
        public int receiver { get { return m_Receiver; } set { m_Receiver = value; } }
        public object data { get { return m_Data; } set { m_Data = value; } }

        public void Clear()
        {
            m_Type = 0;
            m_Sender = 0;
            m_Receiver = 0;
            m_Data = null;
        }
    }
}
