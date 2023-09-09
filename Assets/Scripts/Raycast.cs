using UnityEngine;

public class Raycast : MonoBehaviour
{
    public float RaycastDistance;

    public static System.Action<Transform> Raycasthitinfo;

    public static bool isClickable;
    private void OnEnable()
    {
        isClickable = true;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (isClickable)
        {
            if (Physics.Raycast(ray, out RaycastHit hitinfo, RaycastDistance))
            {
                Debug.DrawRay(Camera.main.transform.position, Vector3.forward, Color.red);

                Raycasthitinfo?.Invoke(hitinfo.collider.transform);

            }
        }

    }
}
