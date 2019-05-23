using System;

namespace YH.Messages
{
    public class MessageHandler
    {
        //反回值表示消息是否传递下去
        public delegate bool Handle(Message message);
        //public static int sId=0;

        //public int id;

        //处理函数
        public Handle handle;
        //优先级.越大越在前面;同样大小，先注册在前，后注册在后。
        public int priority;

        public MessageHandler()
        {
            //id = ++sId;
        }

        public bool Execute(Message message)
        {
            return handle(message);
        }

        public void Clear()
        {
            handle = null;
            priority = 0;
        }
    }
}
