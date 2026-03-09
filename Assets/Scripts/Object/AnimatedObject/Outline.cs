using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Outline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool outlineActive = false;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject outlineObjectPrefab;
    [SerializeField] GameObject outlineObject;
    //[SerializeField] bool useExistingBounds = false;
    Material usedMaterial;
    //[SerializeField] BoxCollider outlineBox;
    bool selected = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Outline " + gameObject.name + " PointerEnter " + eventData.pointerEnter.name);
        ActiveOutline(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Outline exit " + gameObject.name + " PointerEnter " + (eventData.pointerEnter != null ? eventData.pointerEnter.name : "null"));
        ActiveOutline(false);
    }

    private void Awake()
    {
        usedMaterial = new Material(outlineMaterial);
    }

    // Start is called before the first frame update
    void Start()
    {
        outlineActive = true;
        SetSelected(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActiveOutline(bool active)
    {
        if (!active && outlineActive && !selected)
        {
            Destroy(outlineObject);
            outlineActive = false;
            //if(outlineBox != null)
            //    outlineBox.enabled = false;
        }
        else if(active && !outlineActive)
        {
            InstantiateOutlineObject();
            outlineActive = true;
        }
        else if(active && outlineActive)
        {
            UpdateOutlineObject();
        }
    }

    public void SetSelected(bool p_selected)
    {
        selected = p_selected;
        Color c = selected ? UIManager.Orange : UIManager.Blue;
        c.a = 0.3f;
        usedMaterial.color = c;
        if(!selected)
            ActiveOutline(false);
    }

    void InstantiateOutlineObject()
    {
        outlineObject = Instantiate(outlineObjectPrefab);
        outlineObject.name = gameObject.name + "_outline";
        outlineObject.transform.parent = transform;

        /*outlineBox = GetComponent<BoxCollider>();
        if(outlineBox == null) outlineBox = gameObject.AddComponent<BoxCollider>();
        outlineBox.enabled = false;*/

        UpdateOutlineObject();

        Renderer[] renderers = outlineObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            renderer.material = usedMaterial;
        }

        outlineObject.SetActive(true);
    }

    void UpdateOutlineObject()
    {
        Bounds bounds = new Bounds();
        Vector3 localCenter = Vector3.zero;
        /*if (useExistingBounds)
        {
            BoxCollider boxCollider = GetComponentInChildren<BoxCollider>();
            localCenter = transform.InverseTransformPoint(boxCollider.transform.TransformPoint(boxCollider.center));
            outlineBox.center = boxCollider.center;
            outlineBox.size = boxCollider.size;
            outlineBox.enabled = true;
        }
        else*/
        {
            bounds = Utility.CalculateLocalBounds(gameObject);
            /*if (outlineBox != null)
            {
                outlineBox.center = bounds.center;
                outlineBox.size = bounds.size;
                localCenter = bounds.center;
                outlineBox.enabled = true;
            }*/
        }

        outlineObject.transform.localPosition = bounds.center;// transform.InverseTransformPoint(outlineBox.center);
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = /*outlineBox.*/bounds.size * 1.05f;
    }
}
