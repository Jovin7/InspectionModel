using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    public bool canRotate;
    public bool canScale;
    public List<OriginalPosition> gameObjectDataList;
    public Text rotateinstructText;
    public Text scaleinstructText;
    //  public List<OriginalPosition> splitListData;
    public Transform interactables;
    public GameObject[] allInteractables;
    [SerializeField] private Transform selectedInteractable;
    private GameObject interactwithInitialPos;

    public bool isViewing;

    public OriginalPosition PreviousRef;

    public GameObject selectionScreen;

    private float InitialTime;

    //public List<CalculateTime> storeData = new List<CalculateTime>();
   // public List<StoreData> data = new List<StoreData>();

    public SessionData sessionData;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        sessionData.InitialTime = Time.time;
    }

    private void FindObj()
    {
        InspectParts[] gameObjects = FindObjectsOfType<InspectParts>();

        // Initialize the list to store the GameObjectData
        gameObjectDataList = new List<OriginalPosition>();

        // Store the position and rotation of each GameObject
        foreach (InspectParts go in gameObjects)
        {
            gameObjectDataList.Add(new OriginalPosition(go.name, go.transform.position, go.transform.rotation, go.transform.localScale));
        }
    }



    // Update is called once per frame
    public void AssembleFunction()
    {
        
        for (int j = 0; j < interactwithInitialPos.transform.childCount; j++)
        {
            for (int i = 0; i < gameObjectDataList.Count; i++)
            {
                if (interactwithInitialPos.transform.GetChild(j).name == gameObjectDataList[i].name)
                {
                    interactwithInitialPos.transform.GetChild(j).position = gameObjectDataList[i].position;
                    interactwithInitialPos.transform.GetChild(j).rotation = gameObjectDataList[i].rotation;
                    interactwithInitialPos.transform.GetChild(j).localScale = gameObjectDataList[i].scale;
                    interactwithInitialPos.transform.GetChild(j).GetComponent<InspectParts>()._isSelected = false;
                    interactwithInitialPos.transform.GetChild(j).GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
        
        interactwithInitialPos.GetComponent<InspectParts>()._isSelected = false;
      
        interactwithInitialPos.GetComponent<BoxCollider>().enabled = true;
        Raycast.isClickable = true;
        PreviousRef = null;
        isViewing = false;


    }

    public void OnRotateClick()
    {
        canRotate = !canRotate;

        rotateinstructText.text = canRotate ? "MoveBack" : "Rotate";

    }

    public void onScaleClick()
    {
        canScale = !canScale;
        scaleinstructText.text = canScale ? "MoveBack" : "Scale";
    }

    public void ButtonClick(GameObject button)
    {
        for (int i = 0; i < allInteractables.Length; i++)
        {
            if (button.name == allInteractables[i].name)
            {

                Instantiate(allInteractables[i]);

                interactwithInitialPos = GameObject.FindGameObjectWithTag("Parent");

                FindObj();

                interactwithInitialPos.GetComponent<CalculateTime>().startTime = Time.time;
            }


        }
    }

    public void Exit1()
    {

        interactwithInitialPos.GetComponent<CalculateTime>().Calculate();
        SaveData();

        StartCoroutine(Wait());

    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        selectionScreen.SetActive(true);

        DestroyImmediate(GameObject.FindGameObjectWithTag("Parent"));
        // AssembleFunction();
        gameObjectDataList = null;
        canRotate = false;
        canScale = false;
        PreviousRef = null;
        isViewing = false;
        scaleinstructText.text = "scale";
        rotateinstructText.text = "Rotate";
    }
    public void ExitApplication()
    {
        sessionData.TotalDuration = Time.time - sessionData.InitialTime;
        string json = JsonConvert.SerializeObject(sessionData);
        string uniqueFilename = "Time_Date" + DateTime.Now.ToString("HH_mm_ss") + ".json";

        string filePath = Path.Combine(Application.persistentDataPath, uniqueFilename);
        System.IO.File.WriteAllText(filePath, json);
        Application.Quit(); 
    }

   
    public void SaveData()
    {

        if (sessionData.data.Count == 0)
        {
            Debug.Log("1111");
            CalculateTime b = GameObject.FindObjectOfType<CalculateTime>();
            if (b != null)
            {
                Debug.Log("222");
                StoreData a = new StoreData(b.name, b.startTime, b.totalTime);

                sessionData.data.Add(a);
            }

        }
        else
        {
            Debug.Log("333");
            CalculateTime b = GameObject.FindObjectOfType<CalculateTime>();

            for (int i = 0; i < sessionData.data.Count; i++)
            {
                
                if (sessionData.data[i].name == b.name)
                {
                    Debug.Log("444");
                    sessionData.data[i].totalTime += b.totalTime;
                    break;
                }
                else
                {
                    Debug.Log("5555");
                    StoreData a = new StoreData(b.name, b.startTime, b.totalTime);
                    sessionData.data.Add(a);
                    break;
                }
            }
        }

    }

}
[System.Serializable]
public class OriginalPosition
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public OriginalPosition(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        this.name = _name;
        this.position = _position;
        this.rotation = _rotation;
        this.scale = _scale;

    }



}
[Serializable]
public class StoreData
{
    public string name;
    public float startTime;
    public float totalTime;
    public StoreData(string _ObjectName, float _startTime, float _totalTime)
    {

        name = _ObjectName;
        startTime = _startTime;
        totalTime = _totalTime;
    }
}
[Serializable]
public class SessionData
{
   public List<StoreData> data;
    public float InitialTime;
    public float TotalDuration;
    
}






