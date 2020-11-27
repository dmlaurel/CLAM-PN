using System.Collections;
using UnityEngine;

public class OrbitControls : MonoBehaviour
{
    public float Dist = 30f;
    public float MinDist = 10f;
    public float MaxDist = 40f;

    public float ZoomSpeed = 5f;
    public float OrbitSpeed = 5f;
    public float TranslateSpeed = 5f;

    public GameObject CameraAnchor;
    public GameObject dummy;

    public Camera MainCam;
    public GameObject MainCamRotHolder;
    public GameObject scene_objects;

    private float _x;
    private float _y;

    private Vector3 scene_objects_start;
    private Vector3 camera_anchor_start;

    private bool screen_is_locked;
    private bool first_click;

    void Start()
    {
        Debug.Assert(CameraAnchor != null, "Must be set in Editor!");
        Debug.Assert(MainCam != null, "Must be set in Editor!");

        //MainCam.transform.localEulerAngles = new Vector3(-90,0,0);

        // yes we do want to swap x & y from eulerX and eulerY
        _x = MainCamRotHolder.transform.rotation.eulerAngles.y;
        _y = MainCamRotHolder.transform.rotation.eulerAngles.x;

        MainCamRotHolder.transform.LookAt(CameraAnchor.transform);
        //Debug.Log("m " + MainCamRotHolder.transform.position);

        screen_is_locked = false;

        first_click = true;
    }

    void Update()
    {
        if (!screen_is_locked) {
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0)
            {
                StopAllCoroutines();
                Dist = Mathf.Clamp(Dist - (Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime), MinDist, MaxDist);
                MainCamRotHolder.transform.position = -MainCamRotHolder.transform.forward*Dist + CameraAnchor.transform.position;
            }


            if (Input.GetMouseButton(1)) {
            	StopAllCoroutines();
                Vector3 translateX = Input.GetAxis("Mouse X") * TranslateSpeed * Time.deltaTime * MainCamRotHolder.transform.right;
                Vector3 translateY = Input.GetAxis("Mouse Y") * TranslateSpeed * Time.deltaTime * MainCamRotHolder.transform.up;
                scene_objects.transform.position = scene_objects.transform.position + translateX + translateY;
            } else if (Input.GetMouseButton(0))
            {
                StopAllCoroutines();
                _x += Input.GetAxis("Mouse X") * OrbitSpeed * Time.deltaTime;
                _y -= Input.GetAxis("Mouse Y") * OrbitSpeed * Time.deltaTime;
                _y = AngleHelper.ClampAngles(_y, -80f, 80f);
                Quaternion rotation = Quaternion.Euler(_y, _x, 0f);
                dummy.transform.rotation = rotation;
                dummy.transform.position = CameraAnchor.transform.position;
                MainCamRotHolder.transform.position = CameraAnchor.transform.position - dummy.transform.forward * Dist;

                if (first_click) {
                	first_click = false;
                	scene_objects_start = scene_objects.transform.position;
        			camera_anchor_start = MainCamRotHolder.transform.position;
                }
                //this.transform.rotation = rotation;
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                StopAllCoroutines();
                StartCoroutine(ResetView());
            }

            /*if (Input.GetKeyDown(KeyCode.F))
            {
                RaycastHit hit;
                Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    StopAllCoroutines();
                    StartCoroutine(TranslateToFocusPoint(hit.point, MinDist));
                }
            }*/

            //CameraAnchor.transform.localPosition = new Vector3(0f, 0f, -Dist);
            MainCamRotHolder.transform.LookAt(CameraAnchor.transform.position);
        }
    }

    public void instantResetStation() {
            Debug.Log("resetting");
            MainCamRotHolder.transform.position = camera_anchor_start;
            scene_objects.transform.position = scene_objects_start;

            _x = MainCamRotHolder.transform.rotation.eulerAngles.y;
            _y = MainCamRotHolder.transform.rotation.eulerAngles.x;

            Dist = 30f;
    }

    private IEnumerator TranslateToFocusPoint(Vector3 focusPoint, float focusDist)
    {
        while (Vector3.Distance(MainCamRotHolder.transform.position, camera_anchor_start) > 0.01 && Vector3.Distance(scene_objects.transform.position,scene_objects_start) > 0.01)
        {
            MainCamRotHolder.transform.position = Vector3.Lerp(MainCamRotHolder.transform.position, camera_anchor_start, 0.1f);
            Dist = Mathf.Lerp(Dist, focusDist, 0.1f);
            yield return null;
        }
    }

    private IEnumerator ResetView()
    {
    	Debug.Log("reset!");
        while (Vector3.Distance(MainCamRotHolder.transform.position, camera_anchor_start) > 0.01 || Vector3.Distance(scene_objects.transform.position,scene_objects_start) > 0.01)
        {
        	Debug.Log("resetting");
            MainCamRotHolder.transform.position = Vector3.Lerp(MainCamRotHolder.transform.position, camera_anchor_start, 0.1f);
            scene_objects.transform.position = Vector3.Lerp(scene_objects.transform.position, scene_objects_start, 0.1f);

            _x = MainCamRotHolder.transform.rotation.eulerAngles.y;
        	_y = MainCamRotHolder.transform.rotation.eulerAngles.x;

        	Dist = 30f;

            yield return null;
        }
    }

    public void lockScreen(bool is_locked) {
        //Debug.Log("locking " + is_locked);
        screen_is_locked = is_locked;
    }
}
