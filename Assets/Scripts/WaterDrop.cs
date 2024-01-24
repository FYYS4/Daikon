using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    public AudioClip collectedClip;// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
      void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Object that entered the trigger : " + other);

        DaikonController controller = other.GetComponent<DaikonController>();

        if (controller != null)
        {
            controller.ChangeWaterCount(1);
            Destroy(gameObject);
            controller.PlaySound(collectedClip);
            
        }
    }
}

