using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Die.Instance.OnDie();
        }
    }
}
