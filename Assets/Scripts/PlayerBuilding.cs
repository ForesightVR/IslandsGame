﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuilding : MonoBehaviour
{
    public List<GameObject> buildingOptions;
    public float rayDistance = 100;
    public float rotateSpeed = 5f;
    public LayerMask environmentLayer;
    public LineRenderer renderer;

    public float smoothSpeed = 75f;
    public int maxClones;
    public bool useHoldMech;

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

    private void Start()
    {
        StartCoroutine(UpdateLineRender(1));
        cam = Camera.main;
        currentSelectionPreview = Instantiate(buildingOptions[0]);
        currentSelectionPreview.gameObject.layer = LayerMask.NameToLayer("Default");
        currentSelectionPreview.gameObject.SetActive(false);

        renderer.SetPosition(0, Vector3.zero);
        renderer.SetPosition(1, Vector3.zero);
        renderer.endColor = Color.blue;
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
        RaycastHit hit;

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
    }

    void GetInput()
    {
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
            SelectionMode();
            Debug.Log("In Build Mode");
            if (Input.GetKeyDown(KeyCode.Mouse1) && !selectionMode && !rotateMode && !scaleMode)
            {
                Debug.Log("Build Mode");
                buildMode = false;
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
                Debug.Log("Rotate Mode");
                rotateMode = true;
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && !selectionMode && !rotateMode && !scaleMode)
            {
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
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        Debug.Log("Exit X Rotation Mode");
                        xRotateMode = false;
                        return;
                    }
                    RotateXMode();
                }

                if (yRotateMode)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        Debug.Log("Exit Y Rotation Mode");
                        yRotateMode = false;
                        return;
                    }
                    RotateYMode();
                }

                if (zRotateMode)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1))
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
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Debug.Log("Exit Scale Mode");
                    scaleMode = false;
                    ScaleMode();
                    return;
                }
            }
        }
        else if (destroyMode)
        {
            Debug.Log("IN Destroy Mode");
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
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out hit, rayDistance, environmentLayer))
        {
            currentSelectionPreview.transform.position = hit.point;
        }

        Debug.Log("Calling Selection Mode Function");

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        renderPos1 = transform.position;
        renderPos2 = hit.point;
        showLineRender = true;

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
        }

        //Debug.Log("Current selection: " + currentSelection);

        //Debug.LogError("The current selection is not within the bounds of the array!");

        if (currentSelectionPreview == null)
        {
            
        }

        if (currentSelection != lastSelection && currentSelectionPreview)
        {
            Preview(currentSelection, lastSelection, hit.point);
            lastSelection = currentSelection;
            lastSelectionPreview = currentSelectionPreview;
        }
    }

    void ScaleMode()
    {
        Debug.Log("Calling Scale Mode Function");
    }

    void RotateXMode()  
    {
        Debug.Log("Calling Rotate X Mode Function");
    }

    void RotateYMode()
    {
        Debug.Log("Calling Rotate Y Mode Function");
    }

    void RotateZMode()
    {
        Debug.Log("Calling Rotate Z Mode Function");
    }

    void DestroyMode()
    {
        Debug.Log("Calling Destroy Mode Function");
    }

    void Preview(int index, int lastIndex, Vector3 hit)
    {
        if (lastSelectionPreview)
        {
            Destroy(lastSelectionPreview);
        }

        currentSelectionPreview = Instantiate(buildingOptions[index], hit, Quaternion.identity);

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

    /*void Build()
    {

        if (destroyModeActive)
        {
            DestroyObject();
        }
    }

    void DestroyObject()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out hit, rayDistance, environmentLayer))
        {
            renderer.SetPosition(0, transform.position + new Vector3(0, 1f, 0));
            renderer.SetPosition(1, hit.point);

            if (Input.GetKey(KeyCode.Mouse0))
            {

                if (hit.collider.gameObject.tag == "DestroyMe")
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }*/

    /*void CreateBuildPiece()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out hit, rayDistance, environmentLayer))
        {
            renderer.SetPosition(0, transform.position + new Vector3(0, 1f, 0));
            renderer.SetPosition(1, hit.point);

            if (currentSelectionPreview)
            {

                if (useNormal)
                    currentSelectionPreview.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                else
                    currentSelectionPreview.transform.rotation = Quaternion.Euler(Vector3.up);

                RotatePiece();
            }

            if (useHoldMech)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {

                    if (hit.collider.gameObject.tag != "DestroyMe")
                    {
                        if (clones < maxClones)
                        {
                            if (previousPos != hit.point)
                            {
                                Instantiate(buildingOptions[currentSelection], hit.point, currentSelectionPreview.transform.rotation);
                            }
                            previousPos = hit.point;
                            clones++;
                        }

                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Instantiate(buildingOptions[currentSelection], hit.point, currentSelectionPreview.transform.rotation);
                }
            }
            
        }
    }

    void RotatePiece()
    {
        scrollWheel += Input.mouseScrollDelta.y;

        if (currentSelectionPreview)
        {
            currentSelectionPreview.transform.Rotate(Vector3.up, scrollWheel * rotateSpeed);
        }

    }*/

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
        
}
