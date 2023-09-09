using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateTime : MonoBehaviour
{
    public string ObjectName;
    public float startTime;
    public float totalTime;
   
  
    // Start is called before the first frame update
    void Start()
    {
        ObjectName = this.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Calculate()
    {
       
        totalTime = Time.time - startTime;
       
    }
}


