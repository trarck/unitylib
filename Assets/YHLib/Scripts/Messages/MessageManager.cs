using System;
using System.Collections.Generic;

namespace YH.Messages
{
    public class MessageManager
    {
        //全局接收id
        public const UInt32 GlobalReceiver = 0;
        //全局发送id
        public const UInt32 GlobalSender = 0;
        //对象池
        private static readonly YH.Pool.ObjectPool<MessageHandler> s_MessageHandlerPool = new YH.Pool.ObjectPool<MessageHandler>(null, l => l.Clear());
        private static readonly YH.Pool.ObjectPool<Message> s_MessagePool = new YH.Pool.ObjectPool<Message>(null, l => l.Clear());

        //这里使用List，是因为处理队列有优先级。
        protected Dictionary<UInt32, Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>>> m_Messages;

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="handle"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public MessageHandler Register(UInt32 receiver, UInt32 type, UInt32 sender, MessageHandler.Handle handle, int priority=0)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (!m_Messages.TryGetValue(type,out msgMap))
            {
                msgMap = YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Get();// new Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>>();
                m_Messages[type] = msgMap;
            }

            //get receiver map
            Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
            if(!msgMap.TryGetValue(receiver,out receiverMap))
            {
                receiverMap = YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Get();//new Dictionary<uint, List<MessageHandler>>();
                msgMap[receiver] = receiverMap;
            }

            //get handle list
            List<MessageHandler> handlerList = null;
            if(!receiverMap.TryGetValue(sender,out handlerList))
            {
                handlerList = YH.Pool.ListPool<MessageHandler>.Get();// new List<MessageHandler>();
                receiverMap[sender] = handlerList;
            }

            MessageHandler handler = null;
            //check repeat
            int index = IndexOfHandle(handlerList, handle);
            if (index>-1)
            {
                handler = handlerList[index];
                //already register
                if (handler.priority != priority)
                {
                    //reorder
                    handler.priority = priority;
                    ReorderHandler(handlerList, handler);
                }
                return handler;
            }

            //add new
            handler = s_MessageHandlerPool.Get();
            handler.handle = handle;
            handler.priority = priority;

            AddHandler(handlerList, handler);

            return handler;
        }

        //private bool Register(UInt32 receiver, UInt32 type, UInt32 sender, MessageHandler handler)
        //{
        //    //get msg data
        //    Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
        //    if (!m_Messages.TryGetValue(type, out msgMap))
        //    {
        //        msgMap = YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Get();// new Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>>();
        //        m_Messages[type] = msgMap;
        //    }

        //    //get receiver map
        //    Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
        //    if (!msgMap.TryGetValue(receiver, out receiverMap))
        //    {
        //        receiverMap = YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Get();//new Dictionary<uint, List<MessageHandler>>();
        //        msgMap[receiver] = receiverMap;
        //    }

        //    //get handle list
        //    List<MessageHandler> handlerList = null;
        //    if (!receiverMap.TryGetValue(sender, out handleList))
        //    {
        //        handleList = YH.Pool.ListPool<MessageHandler>.Get();// new List<MessageHandler>();
        //        receiverMap[sender] = handleList;
        //    }

        //    //check repeat
        //    int index = IndexOfHandle(handleList, handler.handle);
        //    if (index > -1)
        //    {
        //        //已经注册过
        //        if (handleList[index].priority == handler.priority)
        //        {
        //            //查检是否要更新
        //            if (handleList[index] != handler)
        //            {
        //                //由于使用了对像池，要把旧的回收，使用新的。
        //                s_MessageHandlerPool.Release(handleList[index]);
        //                handleList[index] = handler;
        //            }
        //            return false;
        //        }
        //        else
        //        {
        //            //remove old
        //            s_MessageHandlerPool.Release(handleList[index]);
        //            handleList.RemoveAt(index);
        //        }
        //    }

        //    //add new
        //    AddHandler(handleList, handler);

        //    return true;
        //}
        /// <summary>
        /// 检查是否已经注册处理某个消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool IsRegister(UInt32 receiver, UInt32 type, UInt32 sender, MessageHandler.Handle handle)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (!m_Messages.TryGetValue(type, out msgMap))
            {
                return false;
            }

            //get receiver map
            Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
            if (!msgMap.TryGetValue(receiver, out receiverMap))
            {
                return false;
            }

            //get handle list
            List<MessageHandler> handlerList = null;
            if (!receiverMap.TryGetValue(sender, out handlerList))
            {
                return false;
            }

            //check exist
            if (ContainsHandle(handlerList, handle))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否已经注册处理某个消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool IsRegister(UInt32 receiver, UInt32 type, UInt32 sender)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (!m_Messages.TryGetValue(type, out msgMap))
            {
                return false;
            }

            //get receiver map
            Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
            if (!msgMap.TryGetValue(receiver, out receiverMap))
            {
                return false;
            }

            //get handle list
            List<MessageHandler> handlerList = null;
            if (!receiverMap.TryGetValue(sender, out handlerList))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="handle"></param>
        public void Unregister(UInt32 receiver, UInt32 type, UInt32 sender, MessageHandler.Handle handle)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (m_Messages.TryGetValue(type, out msgMap))
            {
                //get receiver map
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgMap.TryGetValue(receiver, out receiverMap))
                {
                    if (sender > 0)
                    {
                        //get handle list
                        List<MessageHandler> handlerList = null;
                        if (receiverMap.TryGetValue(sender, out handlerList))
                        {
                            RemoveHandle(handlerList, handle);

                            //remove empty list
                            if (handlerList.Count == 0)
                            {
                                receiverMap.Remove(sender);
                                YH.Pool.ListPool<MessageHandler>.Release(handlerList);
                            }
                        }
                    }
                    else
                    {
                        RemoveReceiverMap(receiverMap, handle);
                    }

                    if (receiverMap.Count == 0)
                    {
                        msgMap.Remove(receiver);
                        YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);

                        if (msgMap.Count == 0)
                        {
                            m_Messages.Remove(type);
                            YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(msgMap);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        public void Unregister(UInt32 receiver, UInt32 type, MessageHandler.Handle handle)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (m_Messages.TryGetValue(type, out msgMap))
            {
                //get receiver map
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgMap.TryGetValue(receiver, out receiverMap))
                {
                    RemoveReceiverMap(receiverMap, handle);

                    if (receiverMap.Count == 0)
                    {
                        msgMap.Remove(receiver);
                        YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);

                        if (msgMap.Count == 0)
                        {
                            m_Messages.Remove(type);
                            YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(msgMap);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="handle"></param>
        public void Unregister(UInt32 receiver, MessageHandler.Handle handle)
        {
            foreach (var msgIter in m_Messages)
            {
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgIter.Value.TryGetValue(receiver, out receiverMap))
                {
                    RemoveReceiverMap(receiverMap, handle);
                    if (receiverMap.Count == 0)
                    {
                        msgIter.Value.Remove(receiver);
                    }
                }
            }
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        public void Unregister(UInt32 receiver, UInt32 type, UInt32 sender)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (m_Messages.TryGetValue(type, out msgMap))
            {
                //get receiver map
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgMap.TryGetValue(receiver, out receiverMap))
                {
                    if (sender > 0)
                    {
                        //get handle list
                        List<MessageHandler> handlerList = null;
                        if (receiverMap.TryGetValue(sender, out handlerList))
                        {
                            receiverMap.Remove(sender);
                            YH.Pool.ListPool<MessageHandler>.Release(handlerList);
                        }
                    }
                    else
                    {
                        RemoveReceiverMap(receiverMap);
                    }

                    if (receiverMap.Count == 0)
                    {
                        msgMap.Remove(receiver);
                        YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);

                        if (msgMap.Count == 0)
                        {
                            m_Messages.Remove(type);
                            YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(msgMap);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="type"></param>
        public void Unregister(UInt32 receiver, UInt32 type)
        {
            //get msg data
            Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
            if (m_Messages.TryGetValue(type, out msgMap))
            {
                //get receiver map
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgMap.TryGetValue(receiver, out receiverMap))
                {

                    RemoveReceiverMap(receiverMap);
                    msgMap.Remove(receiver);
                    YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);

                    if (msgMap.Count == 0)
                    {
                        m_Messages.Remove(type);
                        YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(msgMap);
                    }

                }
            }
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="sender"></param>
        /// <param name="handle"></param>
        public void UnregisterAll(UInt32 receiver,UInt32 sender, MessageHandler.Handle handle)
        {
            Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
            List<MessageHandler> handlerList = null;

            List<UInt32> emptyMsgTypes = YH.Pool.ListPool<UInt32>.Get();

            foreach (var msgIter in m_Messages)
            {
                if (msgIter.Value.TryGetValue(receiver, out receiverMap))
                {
                    if (receiverMap.TryGetValue(sender, out handlerList))
                    {
                        RemoveHandle(handlerList, handle);

                        //remove empty list
                        if (handlerList.Count == 0)
                        {
                            receiverMap.Remove(sender);
                            YH.Pool.ListPool<MessageHandler>.Release(handlerList);

                            if (receiverMap.Count == 0)
                            {
                                msgIter.Value.Remove(receiver);
                                YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);
                                
                                if (msgIter.Value.Count == 0)
                                {
                                    emptyMsgTypes.Add(msgIter.Key);
                                }
                            }
                        }
                    }
                }
            }

            for(int i = 0, l = emptyMsgTypes.Count; i < l; ++i)
            {
                YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(m_Messages[emptyMsgTypes[i]]);
                m_Messages.Remove(emptyMsgTypes[i]);
            }

            YH.Pool.ListPool<UInt32>.Release(emptyMsgTypes);
        }

        /// <summary>
        /// 注销处理消息
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="sender"></param>
        public void UnregisterAll(UInt32 receiver, UInt32 sender)
        {
            Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
            List<MessageHandler> handlerList = null;

            List<UInt32> emptyMsgTypes = YH.Pool.ListPool<UInt32>.Get();

            foreach (var msgIter in m_Messages)
            {
                if (msgIter.Value.TryGetValue(receiver, out receiverMap))
                {
                    if (receiverMap.TryGetValue(sender, out handlerList))
                    {
                        receiverMap.Remove(sender);
                        YH.Pool.ListPool<MessageHandler>.Release(handlerList);

                        if (receiverMap.Count == 0)
                        {
                            msgIter.Value.Remove(receiver);
                            YH.Pool.DictionaryPool<UInt32, List<MessageHandler>>.Release(receiverMap);

                            if (msgIter.Value.Count == 0)
                            {
                                emptyMsgTypes.Add(msgIter.Key);
                            }
                        }
                    }
                }
            }

            for (int i = 0, l = emptyMsgTypes.Count; i < l; ++i)
            {
                YH.Pool.DictionaryPool<UInt32, Dictionary<UInt32, List<MessageHandler>>>.Release(m_Messages[emptyMsgTypes[i]]);
                m_Messages.Remove(emptyMsgTypes[i]);
            }

            YH.Pool.ListPool<UInt32>.Release(emptyMsgTypes);
        }

        /// <summary>
        /// 移除接收表中的处理数据
        /// </summary>
        /// <param name="receiverMap"></param>
        /// <param name="handle"></param>
        protected void RemoveReceiverMap(Dictionary<UInt32, List<MessageHandler>> receiverMap, MessageHandler.Handle handle)
        {
            List<UInt32> emptyKeys = YH.Pool.ListPool<UInt32>.Get();
            foreach(var iter in receiverMap)
            {
                RemoveHandle(iter.Value,handle);
                if (iter.Value.Count == 0)
                {
                    emptyKeys.Add(iter.Key);
                    YH.Pool.ListPool<MessageHandler>.Release(iter.Value);
                }
            }

            for(int i = 0, l = emptyKeys.Count; i < l; ++i)
            {
                receiverMap.Remove(emptyKeys[i]);
            }

            YH.Pool.ListPool<UInt32>.Release(emptyKeys);
        }

        /// <summary>
        /// 移除接收表中的处理数据
        /// </summary>
        /// <param name="receiverMap"></param>
        protected void RemoveReceiverMap(Dictionary<UInt32, List<MessageHandler>> receiverMap)
        {
            List<UInt32> emptyKeys = YH.Pool.ListPool<UInt32>.Get();
            foreach (var iter in receiverMap)
            {
                emptyKeys.Add(iter.Key);
                YH.Pool.ListPool<MessageHandler>.Release(iter.Value);
            }

            for (int i = 0, l = emptyKeys.Count; i < l; ++i)
            {
                receiverMap.Remove(emptyKeys[i]);
            }

            YH.Pool.ListPool<UInt32>.Release(emptyKeys);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public void Dispatch(Message message)
        {
            if (message!=null)
            {
                Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap = null;
                if (message.type != Message.GlobalMessageType)
                {
                    //trigger global message
                    if(m_Messages.TryGetValue(Message.GlobalMessageType,out msgMap))
                    {
                        DispatchMap(msgMap, message);
                    }
                }

                //trigger normal message
                if (m_Messages.TryGetValue(message.type, out msgMap))
                {
                    DispatchMap(msgMap, message);
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="data"></param>
        public void Dispatch(UInt32 type,UInt32 sender,UInt32 receiver,object data)
        {
            Message message = s_MessagePool.Get();
            message.type = type;
            message.sender = sender;
            message.receiver = receiver;
            message.data = data;
            Dispatch(message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Dispatch(UInt32 type, UInt32 sender,object data)
        {
            Message message = s_MessagePool.Get();
            message.type = type;
            message.sender = sender;
            message.data = data;
            Dispatch(message);
        }

        protected void DispatchMap(Dictionary<UInt32, Dictionary<UInt32, List<MessageHandler>>> msgMap,Message message)
        {
            UInt32 receiver = message.receiver;
            UInt32 sender = message.sender;
            if (receiver !=GlobalReceiver)
            {
                Dictionary<UInt32, List<MessageHandler>> receiverMap = null;
                if (msgMap.TryGetValue(receiver, out receiverMap))
                {
                    List<MessageHandler> handlerList = null;

                    if (sender != GlobalSender)
                    {
                        if (receiverMap.TryGetValue(sender, out handlerList))
                        {
                            ExecHandlerList(handlerList, message);
                        }
                    }

                    //同时发送sender为global的消息
                    if (receiverMap.TryGetValue(GlobalSender, out handlerList))
                    {
                        ExecHandlerList(handlerList, message);
                    }
                }
            }
            else
            {
                //发送到注册时的接收者为sender的所有接收者
                foreach(var iter in msgMap)
                {
                    List<MessageHandler> handlerList = null;

                    if (sender != GlobalSender)
                    {
                        if (iter.Value.TryGetValue(sender, out handlerList))
                        {
                            ExecHandlerList(handlerList, message);
                        }
                    }

                    //同时发送sender为global的消息
                    if (iter.Value.TryGetValue(GlobalSender, out handlerList))
                    {
                        ExecHandlerList(handlerList, message);
                    }
                }
            }
        }

        protected void ExecHandlerList(List<MessageHandler> handlerList,Message message)
        {
            //为了安全执行handler，需要一份handleList的复制。
            //在执行handle的时间，有可能会调用反注册函数。
            //如果反注册函数和当前handleList相关，则下面的执行会出错。

            List<MessageHandler> copyedHandlers = YH.Pool.ListPool<MessageHandler>.Get();
            copyedHandlers.AddRange(handlerList);
            for(int i=0,l= copyedHandlers.Count; i < l; ++i)
            {
                if (!copyedHandlers[i].Execute(message))
                {
                    //停止消息传递
                    break;
                }
            }
        }

        protected bool ContainsHandle(List<MessageHandler> handlers, MessageHandler.Handle handle)
        {
            if (handlers != null)
            {
                for (int i = 0, l = handlers.Count; i < l; ++i)
                {
                    if (handlers[i].handle == handle)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected int IndexOfHandle(List<MessageHandler> handlers, MessageHandler.Handle handle)
        {
            if (handlers != null)
            {
                for (int i = 0, l = handlers.Count; i < l; ++i)
                {
                    if (handlers[i].handle == handle)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        protected void RemoveHandle(List<MessageHandler> handlers, MessageHandler.Handle handle)
        {
            if (handlers != null)
            {
                for (int i = 0, l = handlers.Count; i < l; ++i)
                {
                    if (handlers[i].handle == handle)
                    {
                        handlers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        protected void AddHandler(List<MessageHandler> handlers, MessageHandler handler)
        {
            if (handlers != null)
            {
                for (int i = 0, l = handlers.Count; i < l; ++i)
                {
                    if (handlers[i].priority > handler.priority)
                    {
                        handlers.Add(handler);
                    }
                }
            }
        }

        protected void ReorderHandler(List<MessageHandler> handlers, MessageHandler handler)
        {
            if (handlers != null)
            {
                //remove old
                handlers.Remove(handler);
                //add new
                for (int i = 0, l = handlers.Count; i < l; ++i)
                {
                    if (handlers[i].priority > handler.priority)
                    {
                        handlers.Insert(i, handler);
                        break;
                    }
                }
            }
        }


    }
}
