﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBuilding : MonoBehaviour
{
    public List<GameObject> buildingOptions;
    public List<Sprite> buildingIcons;

    public int buildLimit = 250;
    public float rayDistance = 100;
    public float rotateSpeed = 5f;
    public LayerMask environmentLayer;
    public LineRenderer renderer;

    public float smoothSpeed = 75f;
    public int maxClones;
    public bool useHoldMech;

    [Space, Header("Object Rotation")]
    public float rotSpeed = 250;
   
    [Space,Header ("Object Scaling")]
    public float scaleSpeed = 10;
    public float maxScale = 15;
    public float minScale = 0.1f;
    [Header("UI Menu's")]
    public GameObject constructionModeSelection;
    public GameObject buildModeSelection;
    public GameObject objectSelectionMode;
    public GameObject rotateModeSelection;
    public GameObject rotationSelection;
    public GameObject scaleSelection;
    public GameObject destroyModeSelection;
    public Image selectionImage;
    public TextMeshProUGUI buildLimitText;

    public Text instructions;

    public Dictionary<GameObject, int> worldState = new Dictionary<GameObject, int>();

    bool useNormal = true;

    int clones;
    Vector3 previousPos;
    int lastSelection;
    int currentSelection;

    Vector3 renderPos1 = Vector3.zero;
    Vector3 renderPos2 = Vector3.zero;

    Camera cam;
    GameObject currentSelectionPreview;
    GameObject lastSelectionPreview;
    
    private void Awake()
    {
        constructionModeSelection.SetActive(true); //Select build or destory
        objectSelectionMode.SetActive(false); //Change what object you are placing
        buildModeSelection.SetActive(false); //Choose how to manipulate the object
        rotateModeSelection.SetActive(false); // Rotating Axis Selection
        rotationSelection.SetActive(false); //Rotating the object along the selected axis
        scaleSelection.SetActive(false); //Scaling Axis Selection
        destroyModeSelection.SetActive(false); //Destory or go back
    }

    private void Start()
    {
        worldState = GameManager.instance.GameLoad(buildingOptions);
        lastSelection = -1;
        StartCoroutine(UpdateLineRender(1));
        cam = Camera.main;
        currentSelectionPreview = Instantiate(buildingOptions[0]);
        currentSelectionPreview.gameObject.layer = LayerMask.NameToLayer("Default");
        currentSelectionPreview.gameObject.SetActive(false);

        renderer.SetPosition(0, Vector3.zero);
        renderer.SetPosition(1, Vector3.zero);
        renderer.endColor = Color.blue;

        buildLimitText.text = "Build Limit: " + worldState.Count + " / " + buildLimit;
    }

    bool buildMode = false;
    bool selectionMode = false;
    bool rotateMode = false;
    bool scaleMode = false;
    bool destroyMode = false;
    bool xRotateMode = false;
    bool yRotateMode = false;
    bool zRotateMode = false;
    bool showLineRender = false;
    bool scrollable = false;

    private void Update()
    {
        GetInput();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            useNormal = !useNormal;
        }

        if (buildMode)
        {
            renderer.startColor = Color.green;
        }
        else
        {
            renderer.startColor = Color.red;
        }
        
        ResetMenu();

        if (worldState.Count < buildLimit)
            buildLimitText.text = "Build Limit: " + worldState.Count + " / " + buildLimit;
        else
            buildLimitText.text = "Build Limit Reached!";
    }

    void ResetMenu()
    {
        currentSelectionPreview.SetActive(buildMode);
        showLineRender = (buildMode || destroyMode);
        constructionModeSelection.SetActive(!buildMode && !destroyMode);
        buildModeSelection.SetActive(buildMode && !selectionMode && !rotateMode && !scaleMode);
        objectSelectionMode.SetActive(buildMode && selectionMode && !rotateMode && !scaleMode);
        rotateModeSelection.SetActive(buildMode && rotateMode && !(xRotateMode || yRotateMode || zRotateMode) && !selectionMode && !scaleMode);
        rotationSelection.SetActive(buildMode && rotateMode && (xRotateMode || yRotateMode || zRotateMode) && !selectionMode && !scaleMode);
        scaleSelection.SetActive(buildMode && scaleMode && !rotateMode && !selectionMode);
        destroyModeSelection.SetActive(destroyMode && !buildMode);
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !destroyMode && buildMode)
        {
            Stamp();
        }

        if (!buildMode && !destroyMode)
        {
            Debug.Log("No Modes Activated!");
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Build Mode Active");
                buildMode = true;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Destroy Mode Active");
                destroyMode = true;
            }
            return;
        }

        if (buildMode)
        {
            Preview();
            Debug.Log("In Build Mode");

            instructions.text = "press 1 to select an object, 2 to rotate your object, or 3 to scale your object. Press right click to go back";

            //buildModeSelection.SetActive(true); // select how to manipulate your object and/or place it
            //constructionModeSelection.SetActive(false); //Select build or destory


            if (Input.GetKeyDown(KeyCode.Mouse1) && !selectionMode && !rotateMode && !scaleMode)
            {
                constructionModeSelection.SetActive(true); //Select build or destory

                instructions.text = "press 1 for build mode, and 2 for destroy mode.";

                Debug.Log("Build Mode");
                buildMode = false;
                //showLineRender = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && !rotateMode && !scaleMode && !selectionMode)
            {

                Debug.Log("Selection Mode");
                selectionMode = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && !selectionMode && !scaleMode && !rotateMode)
            {

                instructions.text = "press 1 to select the x axis, 2 for the y axis, and 3 for the z axis";

                Debug.Log("Rotate Mode");
                rotateMode = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && !selectionMode && !rotateMode && !scaleMode)
            {

                instructions.text = "Use the scroll wheel to scale your object, and left click to select a scale";

                Debug.Log("Scale Mode");
                scaleMode = true;
                return;
            }

            if (selectionMode)
            {
                scrollable = true;
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Debug.Log("Exit Selection Mode");
                    scrollable = false;
                    selectionMode = false;
                    return;
                }

                SelectionMode();
            }

            if (rotateMode)
            {
                Debug.Log("In Rotate Mode");
                if (Input.GetKeyDown(KeyCode.Mouse1) && !xRotateMode && !yRotateMode && !zRotateMode)
                {
                    Debug.Log("Exit Rotate Mode");
                    rotateMode = false;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) && !yRotateMode && !zRotateMode)
                {
                    Debug.Log("X Rotation Mode");
                    xRotateMode = true;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Alpha2) && !xRotateMode && !zRotateMode)
                {
                    Debug.Log("Y Rotation Mode");
                    yRotateMode = true;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Alpha3) && !xRotateMode && !yRotateMode)
                {
                    Debug.Log("Z Rotation Mode");
                    zRotateMode = true;
                    return;
                }

                if (xRotateMode)
                {
                    if  (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        Debug.Log("Exit X Rotation Mode");
                        xRotateMode = false;
                        return;
                    }
                    RotateXMode();
                }

                if (yRotateMode)
                {
                    if  (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        Debug.Log("Exit Y Rotation Mode");
                        yRotateMode = false;
                        return;
                    }
                    RotateYMode();
                }

                if (zRotateMode)
                {
                    if  (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        Debug.Log("Exit Z Rotation Mode");
                        zRotateMode = false;
                        return;
                    }
                    RotateZMode();
                }
            }

            if (scaleMode)
            {
                if  (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Debug.Log("Exit Scale Mode");
                    scaleMode = false;
                    return;
                }
                ScaleMode();
            }
        }
        else if (destroyMode)
        {
            Debug.Log("IN Destroy Mode");
            renderer.startColor = Color.red;
            //showLineRender = true;
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log("Exit Destroy Mode");
                destroyMode = false;
                return;
            }
            
            DestroyMode();
        }
    }
    void SelectionMode()
    {
      

        Debug.Log("Calling Selection Mode Function");

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        //showLineRender = true;

        if (scrollable)
        {
            if (scroll >= .1)
            {
                if (currentSelection + 1 >= buildingOptions.Count)
                {
                    currentSelection = 0;
                }
                else
                {
                    currentSelection++;
                }
            }
            else if (scroll <= -0.1)
            {
                if (currentSelection - 1 < 0)
                {
                    currentSelection = buildingOptions.Count - 1;
                }
                else
                {
                    currentSelection--;
                }
            }

            Debug.Log(currentSelectionPreview); //Already Known
        }

        if(selectionImage.sprite != buildingIcons[currentSelection])
            selectionImage.sprite = buildingIcons[currentSelection];
        //Debug.Log("Current selection: " + currentSelection); 

        //Debug.LogError("The current selection is not within the bounds of the array!");

        if (currentSelection != lastSelection && currentSelectionPreview)
        {
            lastSelection = currentSelection;
            lastSelectionPreview = currentSelectionPreview;
        }
    }

    void ScaleMode()
    {
        float mouseWheelInput = Input.mouseScrollDelta.y;
        mouseWheelInput *= scaleSpeed*Time.deltaTime;
        currentSelectionPreview.transform.localScale += Vector3.one * mouseWheelInput;
        currentSelectionPreview.transform.localScale = new Vector3(Mathf.Clamp(currentSelectionPreview.transform.localScale.x, minScale, maxScale), Mathf.Clamp(currentSelectionPreview.transform.localScale.y, minScale, maxScale), Mathf.Clamp(currentSelectionPreview.transform.localScale.z, minScale, maxScale));
        
    }

    void RotateXMode()
    {
        float mouseWheelInput = Input.mouseScrollDelta.y;
        currentSelectionPreview.transform.Rotate(mouseWheelInput * rotSpeed * Time.deltaTime, 0, 0);
    }

    void RotateYMode()
    {
        float mouseWheelInput = Input.mouseScrollDelta.y;
        currentSelectionPreview.transform.Rotate(0, mouseWheelInput * rotSpeed * Time.deltaTime, 0);
    }

    void RotateZMode()
    {
        float mouseWheelInput = Input.mouseScrollDelta.y;
        currentSelectionPreview.transform.Rotate(0, 0, mouseWheelInput * rotSpeed * Time.deltaTime);
    }

    void DestroyMode()
    {
        Debug.Log("Calling Destroy Mode Function");

        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out hit, rayDistance, environmentLayer))
        {
            renderPos1 = transform.position;
            renderPos2 = hit.point;

            if (Input.GetKey(KeyCode.Mouse0))
            {

                if (hit.collider.gameObject.tag == "DestroyMe")
                {
                    worldState.Remove(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    void Preview()
    {
        if (lastSelectionPreview == currentSelectionPreview)
        {
            Destroy(lastSelectionPreview);
            currentSelectionPreview = Instantiate(buildingOptions[currentSelection], Vector3.zero, Quaternion.identity);
          

            List<Collider> colliders = new List<Collider>();

            colliders.Add(currentSelectionPreview.GetComponent<Collider>());

            foreach (Collider collider in currentSelectionPreview.GetComponentsInChildren<Collider>())
            {
                colliders.Add(collider);
            }

            foreach (Collider collider1 in colliders)
            {
                Destroy(collider1);
            }
        }

        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out hit, rayDistance, environmentLayer))
        {
            Debug.Log("Hit: " + hit.transform.name);
            renderPos1 = transform.position;
            renderPos2 = hit.point;

            currentSelectionPreview.transform.position = hit.point;
        }
    }

    void Stamp()
    {
        if(worldState.Count < buildLimit)
        {
            Vector3 pos = currentSelectionPreview.transform.position;
            Quaternion rot = currentSelectionPreview.transform.rotation;
            Vector3 scale = currentSelectionPreview.transform.localScale;

            GameObject buildObject = Instantiate(buildingOptions[currentSelection], pos, rot);

            Rigidbody rb = buildObject.GetComponent<Rigidbody>();

            if(rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Rigidbody[] rbs = buildObject.GetComponentsInChildren<Rigidbody>();

            if(rbs.Length > 0)
            {
                foreach(Rigidbody _rb in rbs)
                {
                    _rb.isKinematic = false;
                    _rb.useGravity = true;
                }
            }

            worldState.Add(buildObject, currentSelection);
            buildObject.transform.localScale = scale;
        }
    }

    IEnumerator UpdateLineRender(int updateDelay)
    {
        while (true)
        {
            if (showLineRender)
            {
                renderer.SetPosition(0, renderPos1 + new Vector3(0, 1f, 0));
                renderer.SetPosition(1, renderPos2);
            }
            else
            {
                renderer.SetPosition(0, Vector3.zero);
                renderer.SetPosition(1, Vector3.zero);
            }
            yield return new WaitForSeconds(updateDelay / 100);
        }
    }

    public void Save()
    {
        GameManager.instance.GameSave(worldState);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("on application quit");
        Save();
    }
}

