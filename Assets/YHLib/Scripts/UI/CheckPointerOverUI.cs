using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YH.UI
{
    class CheckPointerOverUI
    {
        //方法一， 使用该方法的另一个重载方法，使用时给该方法传递一个整形参数
        // 该参数即使触摸手势的 id
        // int id = Input.GetTouch(0).fingerId;
        public static bool IsPointerOverUIObject(int fingerID)
        {
            return EventSystem.current.IsPointerOverGameObject(fingerID);
        }

        //方法二 通过UI事件发射射线
        //是 2D UI 的位置，非 3D 位置
        public static bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            //实例化点击事件
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            //将点击位置的屏幕坐标赋值给点击事件
            eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

            List<RaycastResult> results = new List<RaycastResult>();
            //向点击处发射射线
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        //方法三 通过画布上的 GraphicRaycaster 组件发射射线
        public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
        {
            //实例化点击事件
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            //将点击位置的屏幕坐标赋值给点击事件
            eventDataCurrentPosition.position = screenPosition;
            //获取画布上的 GraphicRaycaster 组件
            GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();

            List<RaycastResult> results = new List<RaycastResult>();
            // GraphicRaycaster 发射射线
            uiRaycaster.Raycast(eventDataCurrentPosition, results);

            return results.Count > 0;
        }
    }
}
