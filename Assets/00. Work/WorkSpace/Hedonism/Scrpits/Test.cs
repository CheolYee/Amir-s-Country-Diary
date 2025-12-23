using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            Die.Instance.OnDie();
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            Clear.Instance.OnClear();
        }
    }
}
