using System.Collections;
using UnityEngine;
public enum ObjectType { Parent, child };

public class InspectParts : MonoBehaviour
{

    public ObjectType obj;
    public float radius = 5f; // Adjust this value to control the radius of the circle.
    private Transform ExplorePos;
    public bool _isSelected;
    [SerializeField] private float _lerpSpeed = 0.6f;
    private Vector3 _initialMousePosition;


    // Start is called before the first frame update
    private void Start()
    {
        ExplorePos = FindObjectOfType<ViewPoint>().transform;
    }
    private void OnEnable()
    {
        Raycast.Raycasthitinfo += InspectTool;
    }
    private void OnDisable()
    {
        Raycast.Raycasthitinfo -= InspectTool;
    }


    /// <summary>
    /// On Raycast hit this function will be called by the Raycasthitinfo event recursively
    /// </summary>
    /// <param name="partTransform"> this is the raycast hit object</param>
    public void InspectTool(Transform partTransform)
    {
        if (partTransform != this.transform) return;
       // Debug.Log("enter");

        if (Input.GetMouseButtonUp(0) && !SelectionManager.Instance.canRotate)
        {
            //isRotating = false;
            InspectParts temp = partTransform.GetComponent<InspectParts>();
            if (temp)
            {
               
                if (temp.obj == ObjectType.Parent && !_isSelected)
                {
                   // Debug.Log("1111");
                    this.GetComponent<BoxCollider>().enabled = false;
                    this._isSelected = true;
                    SplitCircle();
                }
                else if (temp.obj == ObjectType.child && !_isSelected && !SelectionManager.Instance.isViewing)
                {
                   // Debug.Log("2222");
                    SelectionManager.Instance.isViewing = true;
                    this._isSelected = true;
                    SelectionManager.Instance.PreviousRef = new OriginalPosition(partTransform.name, partTransform.position, partTransform.rotation, partTransform.localScale);

                    Raycast.isClickable = false;
                    Debug.Log("" + ExplorePos.name, ExplorePos);
                    StartCoroutine(ObjectToViewPoint(this.transform.position, ExplorePos.position));
                    SelectionManager.Instance.rotateinstructText.transform.parent.gameObject.SetActive(true);
                    SelectionManager.Instance.scaleinstructText.transform.parent.gameObject.SetActive(true);

                }
                else if (temp.obj == ObjectType.child && _isSelected)
                {
                   // Debug.Log("333");
                    this._isSelected = false;
                    SelectionManager.Instance.isViewing = false;
                    Raycast.isClickable = false;
                    StartCoroutine(ObjectToViewPoint(this.transform.position, SelectionManager.Instance.PreviousRef.position));
                    StartCoroutine(ObjectToViewPoint(this.transform.rotation, SelectionManager.Instance.PreviousRef.rotation));
                    StartCoroutine(ObjectScale(this.transform.localScale, SelectionManager.Instance.PreviousRef.scale));
                    SelectionManager.Instance.PreviousRef = null; 
                    SelectionManager.Instance.rotateinstructText.transform.parent.gameObject.SetActive(false);
                    SelectionManager.Instance.scaleinstructText.transform.parent.gameObject.SetActive(false);

                }
            }
        }
        else if (Input.GetMouseButtonDown(0) && !SelectionManager.Instance.canRotate) 
        {
           // Debug.Log("down");
            _initialMousePosition = Input.mousePosition; 
        }
        if (Input.GetMouseButton(0) && SelectionManager.Instance.canRotate)
        {
           // Debug.Log("hold");
            if (partTransform.GetComponent<InspectParts>()._isSelected)
            {

                Vector3 currentMousePos = Input.mousePosition;
                Vector3 mouseDelta = currentMousePos - _initialMousePosition;

                // Adjust this value to change rotation speed
                float rotationSpeed = 3f;

                // Rotate the GameObject based on mouse movement
                transform.Rotate(mouseDelta.y * rotationSpeed, -mouseDelta.x * rotationSpeed, 0, Space.World);

                _initialMousePosition = currentMousePos;
            }
        }
        if (partTransform.GetComponent<InspectParts>()._isSelected && SelectionManager.Instance.canScale)
        {
            float scaleValue = Input.GetAxis("Mouse ScrollWheel");

            if (scaleValue != 0)
            {
               // Debug.Log("Scaling");
                transform.localScale +=  Vector3.one * scaleValue;
            }
        }
        }

    /// <summary>
    /// Moves the object to the target Position when an starting point and end point is passed as a parameter
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    IEnumerator ObjectToViewPoint(Vector3 startPos, Vector3 endPos)
    {
       
        float t = 0;
        while (t < 1)
        {
            t += _lerpSpeed * Time.deltaTime;
            this.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Raycast.isClickable = true;
    }
    IEnumerator ObjectToViewPoint(Quaternion startRot, Quaternion endRot)
    {
        float t = 0;
        while (t < 1)
        {
            t += _lerpSpeed * Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
        Raycast.isClickable = true;
    }
    IEnumerator ObjectScale(Vector3 startPos, Vector3 endPos)
    {

        float t = 0;
        while (t < 1)
        {
            t += _lerpSpeed * Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        Raycast.isClickable = true;
    }
    void ObjectRotate()
    {
        if (transform.GetComponent<InspectParts>()._isSelected)
        {
            //Debug.Log("_isRotating");
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 mouseDelta = currentMousePos - _initialMousePosition;

            // Adjust this value to change rotation speed
            float rotationSpeed = 3f;

            // Rotate the GameObject based on mouse movement
            transform.Rotate(mouseDelta.y * rotationSpeed, -mouseDelta.x * rotationSpeed, 0, Space.World);

            _initialMousePosition = currentMousePos;
        }
    }

    void SplitCircle()
    {
        int numChildren = transform.childCount;
        float angleStep = 180f / numChildren;
        for (int i = 0; i < numChildren; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float y = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            Vector3 targetPosition = new Vector3(x, y, 0f);
            Transform childTransform = transform.GetChild(i);
            childTransform.GetComponent<BoxCollider>().enabled = true;
            childTransform.localPosition = targetPosition;

        }
      //  Reset.Instance.SplitData();
    }



}




