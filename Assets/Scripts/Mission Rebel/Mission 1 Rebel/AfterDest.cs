using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AfterDest : MonoBehaviour
{
    public enum TriggerType
    {
        SwitchCamera,
        TriggerBoatEnemy,
        TriggerEnemyWithHeli,
        TriggerParallaxis
    };
    public List<GameObject> ListSimpleSoldier;
    public GameObject parallaxisObj;
    public TriggerType triggerType = TriggerType.SwitchCamera;
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
            if(triggerType== TriggerType.SwitchCamera)
            {
                CameraManager.DoneSecondPart();
                Destroy(gameObject);
            }else if (triggerType == TriggerType.TriggerBoatEnemy)
            {
                StartCoroutine(SpawnEnemiesByTrigger());
                
            }else if(triggerType== TriggerType.TriggerParallaxis)
            {
                parallaxisObj.GetComponent<Parallaxing>().isActive = true;
            }else if(triggerType == TriggerType.TriggerEnemyWithHeli)
            {
                StartCoroutine(SpawnEnemiesByTrigger());
            }
        }
    }

    private IEnumerator SpawnEnemiesByTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var simpleSoldier in ListSimpleSoldier)
        {
            simpleSoldier.SetActive(true);
        }
        Destroy(gameObject);
    }
}
