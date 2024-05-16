using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInThirdPart : MonoBehaviour
{
    public enum TriggerType
    {
        switch3zaTo3zb,
        switch3zbTo3zc
    };
    public TriggerType triggerType = TriggerType.switch3zaTo3zb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.IsPlayer(collision))
        {
            if (triggerType == TriggerType.switch3zaTo3zb)
            {
                CameraManager.AfterSwitchZ2BToZ3A();
                Destroy(gameObject);
            }
            else if (triggerType == TriggerType.switch3zbTo3zc)
            {
                CameraManager.AfterSwitchZ3AToZ3B();
                Destroy(gameObject);
            }
        }
    }
}
