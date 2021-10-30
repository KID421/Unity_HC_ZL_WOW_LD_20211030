using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region 欄位
    [Header("追蹤速度"), Range(0, 1000)]
    public float speed = 10;
    [Header("上下限制")]
    public Vector2 limitUpDown;
    [Header("左右限制")]
    public Vector2 limitRightLeft;

    /// <summary>
    /// 目標物件
    /// </summary>
    private Transform target;
    #endregion

    #region 事件
    private void Start()
    {
        target = GameObject.Find("小恐龍").transform;
    }

    private void LateUpdate()
    {
        Track();
    }
    #endregion

    #region 方法
    private void Track()
    {
        Vector3 posTarget = target.position;
        Vector3 posCamera = transform.position;

        posTarget.z = -10;
        posTarget.x = Mathf.Clamp(posTarget.x, limitRightLeft.x, limitRightLeft.y);
        posTarget.y = Mathf.Clamp(posTarget.y, limitUpDown.x, limitUpDown.y);

        transform.position = Vector3.Lerp(posCamera, posTarget, speed * 0.5f * Time.deltaTime);
    }
    #endregion
}
