using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMRZone : MonoBehaviour, IPMRZoneEventListener
{
    [SerializeField] Color _collisionColor;
    [SerializeField] Color _validColor = new Color(0, 1, 0, 0.25f);
    [SerializeField] PMRZoneEvent _zoneEvent;
    [SerializeField] bool _isColliding = false;
    [SerializeField] bool _active = false;
    [SerializeField] bool _forced = false;
    bool _show = false;

    [SerializeField] List<Collider> _collidingObjects = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        _isColliding = false;
        GetComponent<Renderer>().material.color = _collisionColor;
        RenderCollision(_forced);
    }

    public void InitZone(Color p_color)
    {
        _collisionColor = p_color;
        _isColliding = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if(_zoneEvent != null)
            _zoneEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (_zoneEvent != null)
            _zoneEvent.UnregisterListener(this);
    }

    private void OnTriggerEnter(Collider p_colliding)
    {
        if (p_colliding.gameObject.GetComponentInChildren<Furniture>() != null)
        {
            if (p_colliding.gameObject.GetComponentInChildren<Furniture>().IsMobile())
                return;
        }
        else if (p_colliding.gameObject.GetComponentInParent<AnimatedPart>() != null)
        {
            return;
        }
        else if (p_colliding.gameObject.GetComponent<Anchor>() != null)
        {
            return;
        }
        else if (p_colliding.gameObject.layer == LayerMask.NameToLayer("ManipulationHelper"))
        {
            return;
        }

        if (!_collidingObjects.Contains(p_colliding))
            _collidingObjects.Add(p_colliding);

        UpdateCollisions();

        _zoneEvent.Raise(this);
        //Debug.Log("OnTriggerEnter " + this.name + " - " + p_colliding.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_collidingObjects.Contains(other))
            _collidingObjects.Remove(other);

        UpdateCollisions();

        _zoneEvent.Raise(this);
        //Debug.Log("OnTriggerExit " + this.name);
    }

    void UpdateCollisions()
    {
        _isColliding = _collidingObjects.Count > 0;
        GetComponent<Renderer>().material.color = _isColliding ? _collisionColor : _validColor;
    }

    public void RenderCollision(bool p_show)
    {
        _show = p_show;
        UpdateCollisions();
        GetComponent<Renderer>().enabled = _active ? (_show && _isColliding) : _forced;
    }

    void IPMRZoneEventListener.OnPMRZoneCollisionEventRaised(PMRZone p_zone)
    {
        //throw new System.NotImplementedException();
    }

    void IPMRZoneEventListener.OnPMRZoneRulesActivated(bool p_activation)
    {
        _active = p_activation;
        RenderCollision(_show);
    }

    public bool IsColliding => _isColliding;
}
