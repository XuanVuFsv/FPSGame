using UnityEngine;

public class Test : MonoBehaviour
{
    Collider m_Collider;
    Vector3 m_Center;
    Vector3 m_Size, m_Min, m_Max;
    public Vector3 rotation;
    public float mag;

    void Start()
    {
        //Fetch the Collider from the GameObject
        //m_Collider = GetComponent<Collider>();
        //Fetch the center of the Collider volume
        //m_Center = m_Collider.bounds.center;
        //Fetch the size of the Collider volume
        //m_Size = m_Collider.bounds.size;
        //Fetch the minimum and maximum bounds of the Collider volume
        //m_Min = m_Collider.bounds.min;
        //m_Max = m_Collider.bounds.max;
        //Output this data into the console
        //OutputData();
    }

    void OutputData()
    {
        //Output to the console the center and size of the Collider volume
        Debug.Log("Collider Center : " + m_Center);
        Debug.Log("Collider Size : " + m_Size);
        Debug.Log("Collider bound Minimum : " + m_Min);
        Debug.Log("Collider bound Maximum : " + m_Max);
        Debug.Log("Collider bound Extend : " + m_Collider.bounds.extents);
    }

    private void Update()
    {
        rotation = transform.localEulerAngles;
        mag = rotation.magnitude;
        //transform.localEulerAngles = new Vector3(rotation.x, Mathf.Clamp(rotation.y, 175, 185), rotation.z);
    }
}