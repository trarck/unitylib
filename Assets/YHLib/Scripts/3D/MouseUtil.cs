using UnityEngine;

namespace YH
{
    class MouseUtil
    {
        Vector3 ScreenPositionToWorldPosition(Vector2 pointer,Camera camera)
        {
            Vector3 cameraPos = new Vector3(pointer.x, pointer.y, GetDistanceFromScreenToWorld(pointer,camera));
            Vector3 worldPos = camera.ScreenToWorldPoint(cameraPos);
            return worldPos;
        }  xds433

        /// <summary>
        /// 取得屏幕点所在游戏内的距离
        /// 通常是距地面的距离
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        public float GetDistanceFromScreenToWorld(Vector2 pointer, Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(pointer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.transform.name + "," + hit.distance);
                return hit.distance;
            }
            return 0;
        }

        /// <summary>
        /// 取得屏幕坐标下的世界物体
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public GameObject GetGameObjectOfPointer(Vector2 pointer,Camera camera)
        {
            Ray ray = camera.ScreenPointToRay(pointer);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform.gameObject;
            }
            return null;
        }

        public GameObject GetGameObjectOfPointer(Vector2 pointer, Camera camera,out RaycastHit hit)
        {
            Ray ray = camera.ScreenPointToRay(pointer);
            
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform.gameObject;
            }
            return null;
        }
    }
}
