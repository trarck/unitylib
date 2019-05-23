using System;

namespace YH.Messages
{
    public class Message
    {
        //全局消息
        public const UInt32 GlobalMessageType = 0;

        //消息类型。逻辑消息最好从1000开始。
        UInt32 m_Type;
        //发送者
        UInt32 m_Sender;
        //接收者
        UInt32 m_Receiver;
        //数据
        object m_Data;

        public UInt32 type { get { return m_Type; } set { m_Type = value; } }

        public UInt32 sender { get { return m_Sender; } set { m_Sender = value; } }
        public UInt32 receiver { get { return m_Receiver; } set { m_Receiver = value; } }
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
