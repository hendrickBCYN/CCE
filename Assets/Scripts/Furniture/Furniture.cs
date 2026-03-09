using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Furniture : MonoBehaviour
{
    [SerializeField] protected Vector3 _furnitureScale = Vector3.one;

    [SerializeField] private FurnitureEvent _furnitureEvent;

    [SerializeField] bool _fixedCollider = false;

    private BoxCollider _boxCollider;

    private void Start()
    {
        if (_furnitureEvent != null)
            _furnitureEvent.Added(gameObject);
        Init();
    }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        if (!_fixedCollider)
        {
            Bounds bounds = Utility.CalculateLocalBounds(gameObject);
            _boxCollider.center = bounds.center;
            _boxCollider.size = bounds.size;
        }
    }

    protected virtual void Init() { }

    public void UpdateFurniture(Vector3 rotation, Vector3 up, Vector3 left)
    {
        transform.localPosition += up + left;
        transform.Rotate(rotation);
    }

    public bool IsMobile()
    {
        if (gameObject.GetComponentInParent<FurnitureRef>() != null)
            return gameObject.GetComponentInParent<FurnitureRef>().IsMobile;
        return false;
    }

    public Bounds GetLocalBounds()
    {
        Bounds result = new Bounds();
        if(_fixedCollider)
        {
            result.center = _boxCollider.center;
            result.size = _boxCollider.size;
            result.extents = result.size / 2f;
        }
        else
            result = Utility.CalculateLocalBounds(gameObject);

        return result;
    }
}
