using UnityEngine;
using System.Collections;

/// <summary>
/// 摄像机跟随player的控制器
/// </summary>
public class MouseOrbit : MonoBehaviour {

    #region =====公共变量
    /// <summary>
    /// 目标
    /// </summary>
    [HideInInspector]
    public GameObject m_target;
    /// <summary>
    /// X轴方向的速度
    /// </summary>
    public float m_xSpeed=8;
    /// <summary>
    /// Y轴方向的速度
    /// </summary>
    public float m_ySpeed=4;
    /// <summary>
    /// 鼠标Y轴方向旋转的最小值
    /// </summary>
    public float m_yMinLimit=10;
    /// <summary>
    /// 鼠标Y轴方向旋转的最大值
    /// </summary>
    public float m_yMaxLimit=50;
    /// <summary>
    /// 滚轮的速度
    /// </summary>
    public float m_scrollSpeed=20;
    /// <summary>
    /// 放大的最小值
    /// </summary>
    public float m_zoomMin=4;
    /// <summary>
    /// 放大的最大值
    /// </summary>
    public float m_zoomMax=10;
    #endregion

    #region=====私有变量
    /// <summary>
    /// 相机与目标的距离
    /// </summary>
    private float _distance;
    private float _distanceLerp;
    private Vector3 _position;
    /// <summary>
    /// 是否对摄像机进行旋转
    /// </summary>
    private bool _isActivated;
    /// <summary>
    /// (鼠标的左右滑动(X轴方向))=相机在y轴的旋转角度
    /// </summary>
    private float _x;
    /// <summary>
    /// (鼠标的上下滑动(Y轴方向))=相机x轴的旋转角度
    /// </summary>
    private float _y;
    private bool _setupCamera;
    private Transform myTransform;
    #endregion

    void Start()
    {
        myTransform = transform;
        //当不能发现目标时，发出警告
        if (m_target == null)
        {
            m_target = GameObject.FindGameObjectWithTag("Player");
            if (m_target == null)
            {
                Debug.LogWarning("不能找到目标，请给Player添加一个Player的标签");
            }
        }
        //设置坐标
        Vector3 angles = myTransform.eulerAngles;
        _x = angles.y;
        _y = angles.x;

        CalculateDistance();
    }

    void LateUpdate()
    {
        ScrollMouse();
        RotateCamera();
    }

    #region 根据距离，放大目标
    /// <summary>
    /// 根据距离，放大目标
    /// </summary>
    private void CalculateDistance()
    {
        if (m_target != null)
        {
            _distance = m_zoomMax;
            _distanceLerp = _distance;
            //构建相机旋转的四元数
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            //构建相机的原点坐标(因为四元数的旋转通常是以原点进行旋转的)
            Vector3 calPosition = new Vector3(0, 0, -_distanceLerp);
            //对于相机进行旋转，跟踪目标的坐标
            _position = rotation * calPosition + m_target.transform.position;
            //重新定位相机的旋转角
            myTransform.rotation = rotation;
            //重新定位相机的坐标
            myTransform.position = _position;
        }
        else
            Debug.LogWarning("不能找到目标，请给Player添加一个Player的标签");
    } 
    #endregion

    /// <summary>
    /// 鼠标滚轮的方法
    /// </summary>
    private void ScrollMouse()
    {
        _distanceLerp = Mathf.Lerp(_distanceLerp, _distance, Time.deltaTime * 5);
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _distance = Vector3.Distance(myTransform.position, m_target.transform.position);
            _distance = ScrollLimit(_distance - Input.GetAxis("Mouse ScrollWheel") * m_scrollSpeed, m_zoomMin, m_zoomMax);
        }
    }
    /// <summary>
    /// 放大缩小取值
    /// </summary>
    /// <param name="dist">目标值</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    private float ScrollLimit(float dist, float min, float max)
    {
        if (dist < min)
            dist = min;
        if (dist > max)
            dist = max;
        return dist;
    }

    /// <summary>
    /// 旋转摄像机
    /// </summary>
    private void RotateCamera()
    {
        //鼠标右键按下
        if (Input.GetMouseButtonDown(1))
        {
            _isActivated = true;
        }
        //鼠标右键弹起
        if (Input.GetMouseButtonUp(1))
        {
            _isActivated = false;
        }
        //如果鼠标右键按下并且目标不为空
        if (m_target && _isActivated)
        {
            _y -= Input.GetAxis("Mouse Y") * m_ySpeed;
            _x += Input.GetAxis("Mouse X") * m_xSpeed;

            _y = ClampAngle(_y, m_yMinLimit, m_yMaxLimit);
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            Vector3 calPos = new Vector3(0, 0, -_distanceLerp);
            _position = rotation * calPos + m_target.transform.position;
            transform.rotation = rotation;
            transform.position = _position;
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            Vector3 calPos = new Vector3(0, 0, -_distanceLerp);
            _position = rotation * calPos + m_target.transform.position;
            transform.rotation = rotation;
            transform.position = _position;
        }

    }
    /// <summary>
    /// 夹角的方法
    /// </summary>
    /// <param name="angle">角度</param>
    /// <param name="min">最小的角度</param>
    /// <param name="max">最大的角度</param>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}