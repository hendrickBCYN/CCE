using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICamera : MonoBehaviour
{
    public InputManager _controls; //Input System
    protected RoomManager _roomManager;
    protected GameObject _target;
    protected int _id;

    /// <summary>
    /// Reset the camera to a default position 
    /// </summary>
    public abstract void Reset();

    public void InitData(RoomManager p_roomManager, GameObject target)
    {
        _roomManager = p_roomManager;
        _target = target;
    }

    /// <summary>
    /// Update the different components with the inputs system 
    /// </summary>
    public abstract void UpdateCam();
    protected abstract void Move();

    /// <summary>
    /// Function call at the first frame
    /// </summary>
    public virtual void Start()
    {
        _controls = GameObject.Find("InputManager").GetComponent<InputManager>();

        //Reset();
    }

    public abstract float GetFieldOfView();
    public abstract void SetFieldOfView(float zoom);

    public abstract float GetDistToTarget();
    public abstract void SetDistToTarget(float dist);

    protected virtual void UpdateTransparent()
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRadius);

        //List<string> colliderNames = new List<string>();

        bool l_insideRoom = false;
        if (_roomManager != null)
        {
            BoxCollider roomCollider = _roomManager.GetRoomBounds();
            l_insideRoom = roomCollider.bounds.Contains(transform.position);
        }

        Wall[] walls = GameObject.FindObjectsOfType<Wall>();
        foreach (Wall wall in walls)
        {
            if (l_insideRoom)
                wall.UnsetTransparent();
            else
            {
                float prodScal = Vector3.Dot(wall.transform.forward, transform.forward);
                if (prodScal >= 0)
                    wall.UnsetTransparent();
                else
                    wall.SetTransparent();
            }

        }

        Ceiling [] l_ceilings = FindObjectsByType<Ceiling>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Ceiling l_ceiling in l_ceilings)
        {
            if (l_ceiling != null)
            {
                if (l_insideRoom)
                    l_ceiling.UnsetTransparent();
                else l_ceiling.SetTransparent();
            }
        }

        /*foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                collider.GetComponent<Wall>().SetTransparent();
            }
            else if (collider.CompareTag("WallLayer"))
            {
                collider.transform.parent.GetComponent<Wall>().SetTransparent();
            }
        }*/
    }

    public abstract Vector3 GetPlaneForwardVector();
    public abstract Vector3 GetPlaneRightVector();

    protected void ActivateCeilingLayer(bool p_activate)
    {
        Camera camera = GetComponent<Camera>();

        int indexLayerToActivate = LayerMask.NameToLayer("Ceiling");
        if (p_activate)
            camera.cullingMask |= 1 << indexLayerToActivate;
        else camera.cullingMask &= ~(1 << indexLayerToActivate);
    }
}
