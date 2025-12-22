using Unity.VisualScripting;
using UnityEngine;

public class SpinLock : MonoBehaviour
{
    [SerializeField] private float _angle = 0;

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _angle++;
            Debug.Log("왼쪽으로 회전중");
            // 왼쪽으로 회전
        }

        if (Input.GetKey(KeyCode.D))
        {
            _angle--;
            Debug.Log("오른쪽으로 회전중");
            // 오른쪽으로 회전
        }

        Spin();
    }

    private void Spin()
    {
        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }


}
