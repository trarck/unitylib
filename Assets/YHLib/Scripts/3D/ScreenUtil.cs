using UnityEngine;

namespace YH
{
    class ScreenUtil
    {
        Vector3 ScreenPositionToWorldPosition(Vector2 pointer,Camera camera)
        {
            Vector3 cameraPos = new Vector3(pointer.x, pointer.y, GetDistanceFromScreenToWorld(pointer,camera));
            Vector3 worldPos = camera.ScreenToWorldPoint(cameraPos);
            return worldPos;
        }  

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

        /// <summary>
        /// 通过屏幕点，相机到水平面的距离
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="camera"></param>
        /// <param name="planeY"></param>
        /// <returns></returns>
        float GetDistanceFromCameraToGameHorizontalPlaneAtScreenPointer(Vector2 pointer, Camera camera,float planeY=0)
        {
            //相机到屏幕的距离
            float h = camera.nearClipPlane;
            //相机对应世界坐标大小
            float cameraGameHalfHeight = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * h;
            float cameraGameHalfWidth = cameraGameHalfHeight * camera.aspect;

            //屏幕点转到相机坐标
            float x = (1 - 2 * pointer.x / camera.pixelWidth) * cameraGameHalfWidth;
            float y = (1 - 2 * pointer.y / camera.pixelHeight) * cameraGameHalfHeight;
            //相机到屏幕点的垂直方向距离平方
            float ll = h * h + x * x;
            //屏幕点垂直距离平方
            float yy = y * y;
            //相机到屏幕点的距离平方
            float cpSquare = ll + yy;

            float sina = Mathf.Sqrt(yy / cpSquare) * (y > 0 ? 1 : -1);
            float cosa = Mathf.Sqrt(ll / cpSquare);

            //相机x方向的旋转
            float radc = camera.transform.eulerAngles.x * Mathf.Deg2Rad;
            float sinc = Mathf.Sin(radc);
            float cosc = Mathf.Cos(radc);
            float cameraY = camera.transform.position.y+planeY;
            float distance = cameraY / (sina * cosc + cosa * sinc);
            
            return Mathf.Abs(distance);
        }

        /// <summary>
        ///屏幕点到水平面的距离
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="camera"></param>
        /// <param name="planeY"></param>
        /// <returns></returns>
        float GetDistanceFromScreenPointerToGameHorizontalPlane(Vector2 pointer, Camera camera, float planeY = 0)
        {
            //相机到屏幕的距离
            float h = camera.nearClipPlane;
            //相机对应世界坐标大小
            float cameraGameHalfHeight = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * h;
            float cameraGameHalfWidth = cameraGameHalfHeight * camera.aspect;

            //屏幕点转到相机坐标
            float x = (1 - 2 * pointer.x / camera.pixelWidth) * cameraGameHalfWidth;
            float y = (1 - 2 * pointer.y / camera.pixelHeight) * cameraGameHalfHeight;
            //相机到屏幕点的垂直方向距离平方
            float ll = h * h + x * x;
            //屏幕点垂直距离平方
            float yy = y * y;
            //相机到屏幕点的距离平方
            float cpSquare = ll + yy;

            float sina = Mathf.Sqrt(yy / cpSquare) * (y > 0 ? 1 : -1);
            float cosa = Mathf.Sqrt(ll / cpSquare);

            //相机x方向的旋转
            float radc = camera.transform.eulerAngles.x * Mathf.Deg2Rad;
            float sinc = Mathf.Sin(radc);
            float cosc = Mathf.Cos(radc);
            float cameraY = camera.transform.position.y + planeY;
            float distance = cameraY / (sina * cosc + cosa * sinc) - Mathf.Sqrt(ll) / cosa;
            return Mathf.Abs(distance);
        }

        /// <summary>
        /// 屏幕点到 水平面的相机中的z坐标
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="camera"></param>
        /// <param name="planeY"></param>
        /// <returns></returns>
        float GetZInCameraFromScreenPointerToGameHorizontalPlane(Vector2 pointer, Camera camera, float planeY = 0)
        {
            //相机到屏幕的距离
            float h = camera.nearClipPlane;
            //相机对应世界坐标大小
            float cameraGameHalfHeight = Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * h;
            float cameraGameHalfWidth = cameraGameHalfHeight * camera.aspect;

            //屏幕点转到相机坐标
            float x = (1 - 2 * pointer.x / camera.pixelWidth) * cameraGameHalfWidth;
            float y = (1 - 2 * pointer.y / camera.pixelHeight) * cameraGameHalfHeight;
            //相机到屏幕点的垂直方向距离平方
            float ll = h * h + x * x;
            //屏幕点垂直距离平方
            float yy = y * y;
            //相机到屏幕点的距离平方
            float cpSquare = ll + yy;

            float sina = Mathf.Sqrt(yy / cpSquare) * (y > 0 ? 1 : -1);
            float cosa = Mathf.Sqrt(ll / cpSquare);

            //相机x方向的旋转
            float radc = camera.transform.eulerAngles.x * Mathf.Deg2Rad;
            float sinc = Mathf.Sin(radc);
            float cosc = Mathf.Cos(radc);
            float cameraY = camera.transform.position.y + planeY;
            float distance = cameraY *cosa / (sina * cosc + cosa * sinc) - Mathf.Sqrt(ll) / cosa;
            return Mathf.Abs(distance);
        }
    }
}
