using UnityEngine;
using System.Collections;

/// <summary>
/// ���������player�Ŀ�����
/// </summary>
public class MouseOrbit : MonoBehaviour {

    #region =====��������
    /// <summary>
    /// Ŀ��
    /// </summary>
    [HideInInspector]
    public GameObject m_target;
    /// <summary>
    /// X�᷽����ٶ�
    /// </summary>
    public float m_xSpeed=8;
    /// <summary>
    /// Y�᷽����ٶ�
    /// </summary>
    public float m_ySpeed=4;
    /// <summary>
    /// ���Y�᷽����ת����Сֵ
    /// </summary>
    public float m_yMinLimit=10;
    /// <summary>
    /// ���Y�᷽����ת�����ֵ
    /// </summary>
    public float m_yMaxLimit=50;
    /// <summary>
    /// ���ֵ��ٶ�
    /// </summary>
    public float m_scrollSpeed=20;
    /// <summary>
    /// �Ŵ����Сֵ
    /// </summary>
    public float m_zoomMin=4;
    /// <summary>
    /// �Ŵ�����ֵ
    /// </summary>
    public float m_zoomMax=10;
    #endregion

    #region=====˽�б���
    /// <summary>
    /// �����Ŀ��ľ���
    /// </summary>
    private float _distance;
    private float _distanceLerp;
    private Vector3 _position;
    /// <summary>
    /// �Ƿ�������������ת
    /// </summary>
    private bool _isActivated;
    /// <summary>
    /// (�������һ���(X�᷽��))=�����y�����ת�Ƕ�
    /// </summary>
    private float _x;
    /// <summary>
    /// (�������»���(Y�᷽��))=���x�����ת�Ƕ�
    /// </summary>
    private float _y;
    private bool _setupCamera;
    private Transform myTransform;
    #endregion

    void Start()
    {
        myTransform = transform;
        //�����ܷ���Ŀ��ʱ����������
        if (m_target == null)
        {
            m_target = GameObject.FindGameObjectWithTag("Player");
            if (m_target == null)
            {
                Debug.LogWarning("�����ҵ�Ŀ�꣬���Player���һ��Player�ı�ǩ");
            }
        }
        //��������
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

    #region ���ݾ��룬�Ŵ�Ŀ��
    /// <summary>
    /// ���ݾ��룬�Ŵ�Ŀ��
    /// </summary>
    private void CalculateDistance()
    {
        if (m_target != null)
        {
            _distance = m_zoomMax;
            _distanceLerp = _distance;
            //���������ת����Ԫ��
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            //���������ԭ������(��Ϊ��Ԫ������תͨ������ԭ�������ת��)
            Vector3 calPosition = new Vector3(0, 0, -_distanceLerp);
            //�������������ת������Ŀ�������
            _position = rotation * calPosition + m_target.transform.position;
            //���¶�λ�������ת��
            myTransform.rotation = rotation;
            //���¶�λ���������
            myTransform.position = _position;
        }
        else
            Debug.LogWarning("�����ҵ�Ŀ�꣬���Player���һ��Player�ı�ǩ");
    } 
    #endregion

    /// <summary>
    /// �����ֵķ���
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
    /// �Ŵ���Сȡֵ
    /// </summary>
    /// <param name="dist">Ŀ��ֵ</param>
    /// <param name="min">��Сֵ</param>
    /// <param name="max">���ֵ</param>
    private float ScrollLimit(float dist, float min, float max)
    {
        if (dist < min)
            dist = min;
        if (dist > max)
            dist = max;
        return dist;
    }

    /// <summary>
    /// ��ת�����
    /// </summary>
    private void RotateCamera()
    {
        //����Ҽ�����
        if (Input.GetMouseButtonDown(1))
        {
            _isActivated = true;
        }
        //����Ҽ�����
        if (Input.GetMouseButtonUp(1))
        {
            _isActivated = false;
        }
        //�������Ҽ����²���Ŀ�겻Ϊ��
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
    /// �нǵķ���
    /// </summary>
    /// <param name="angle">�Ƕ�</param>
    /// <param name="min">��С�ĽǶ�</param>
    /// <param name="max">���ĽǶ�</param>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}