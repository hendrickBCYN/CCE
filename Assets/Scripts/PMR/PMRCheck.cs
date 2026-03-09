using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PMRCheck : MonoBehaviour, IPMRZoneEventListener
{
    [SerializeField] PMRZoneEvent _zoneEvent;
    private GameObject _pmrZones;
    private BoxCollider _boxCollider;
    private GameObject _pmrZonePrefab = null;

    //PMR Rules
    //-Bâtiment neuf:
    //S1 = 0.9m sur grands côtés lit et 1.2m pied de lit
    //S2 = 1.2m sur grands côtés et 0.9m pied de lit
    //-Bâtiment existant:
    //S3 = 0.9m sur un des grands côtés

    private List<PMRZone> _pmrZonesS1;
    private List<PMRZone> _pmrZonesS2;
    private List<PMRZone> _pmrZonesS3;

    [SerializeField] Color _pmrS1Color = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] Color _pmrS2Color = new Color(1f, 0.5f, 0, 0.25f);

    private bool _pmrS1 = true;
    private bool _pmrS2 = true;

    private bool _pmrS3Active = false;
    private bool _pmrS3 = true;

    [SerializeField] bool _pmrValid = false;
    [SerializeField] bool _active = false;

    private void OnEnable()
    {
        _zoneEvent.RegisterListener(this);
    }

    private void OnDisable()
    {
        _zoneEvent.UnregisterListener(this);
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {

    }

    public void Init()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void InitPMRZones(GameObject p_pmrZonePrefab)
    {
        _pmrZonePrefab = p_pmrZonePrefab;
        CreatePMRZones(_pmrZonePrefab);
        UpdatePMRValidity();
    }

    public void CreatePMRZones(GameObject p_pmrZonePrefab)
    {
        _pmrZones = new GameObject("PMRZones");
        _pmrZones.transform.parent = transform;
        _pmrZones.transform.localScale = Vector3.one;
        _pmrZones.transform.localRotation = Quaternion.identity;
        _pmrZones.transform.localPosition = new Vector3(0, 0.1f, 0);

        _pmrZonesS1 = new List<PMRZone>();
        _pmrZonesS2 = new List<PMRZone>();
        _pmrZonesS3 = new List<PMRZone>();

        //PiedDeLit
        GameObject l_piedDeLit = new GameObject("PiedDeLit");
        l_piedDeLit.transform.parent = _pmrZones.transform;
        l_piedDeLit.transform.localScale = Vector3.one;
        l_piedDeLit.transform.localRotation = Quaternion.identity;
        l_piedDeLit.transform.localPosition = Vector3.zero;
        GameObject l_piedDeLitS1 = Instantiate(p_pmrZonePrefab, l_piedDeLit.transform);
        l_piedDeLitS1.name = "PiedDeLitS1";
        l_piedDeLitS1.transform.localScale = new Vector3(_boxCollider.size.x * 0.95f, _boxCollider.size.y * 0.95f, 1.2f);
        l_piedDeLitS1.transform.localRotation = Quaternion.identity;
        l_piedDeLitS1.transform.localPosition = _boxCollider.center + new Vector3(0, 0, (_boxCollider.size.z / 2f + 0.6f + 0.001f));
        PMRZone l_zonePS1 = l_piedDeLitS1.GetComponent<PMRZone>();
        _pmrZonesS1.Add(l_zonePS1);
        GameObject l_piedDeLitS2 = Instantiate(p_pmrZonePrefab, l_piedDeLit.transform);
        l_piedDeLitS2.name = "PiedDeLitS2";
        l_piedDeLitS2.transform.localScale = new Vector3(_boxCollider.size.x * 0.95f, _boxCollider.size.y * 0.95f, 0.9f);
        l_piedDeLitS2.transform.localRotation = Quaternion.identity;
        l_piedDeLitS2.transform.localPosition = _boxCollider.center + new Vector3(0, 0, (_boxCollider.size.z / 2f + 0.45f + 0.001f));
        PMRZone l_zonePS2 = l_piedDeLitS2.GetComponent<PMRZone>();
        _pmrZonesS2.Add(l_zonePS2);

        //CoteDroit
        GameObject l_coteDroit = new GameObject("CoteDroit");
        l_coteDroit.transform.parent = _pmrZones.transform;
        l_coteDroit.transform.localScale = Vector3.one;
        l_coteDroit.transform.localRotation = Quaternion.identity;
        l_coteDroit.transform.localPosition = Vector3.zero;
        GameObject l_coteDroitS1 = Instantiate(p_pmrZonePrefab, l_coteDroit.transform);
        l_coteDroitS1.name = "CoteDroitS1";
        l_coteDroitS1.transform.localScale = new Vector3(0.9f, _boxCollider.size.y * 0.95f, _boxCollider.size.z * 0.95f);
        l_coteDroitS1.transform.localRotation = Quaternion.identity;
        l_coteDroitS1.transform.localPosition = _boxCollider.center + new Vector3(-(_boxCollider.size.x / 2f + 0.45f + 0.001f), 0, 0);
        PMRZone l_zoneCDS1 = l_coteDroitS1.GetComponent<PMRZone>();
        _pmrZonesS1.Add(l_zoneCDS1);
        GameObject l_coteDroitS2 = Instantiate(p_pmrZonePrefab, l_coteDroit.transform);
        l_coteDroitS2.name = "CoteDroitS2";
        l_coteDroitS2.transform.localScale = new Vector3(1.2f, _boxCollider.size.y * 0.95f, _boxCollider.size.z * 0.95f);
        l_coteDroitS2.transform.localRotation = Quaternion.identity;
        l_coteDroitS2.transform.localPosition = _boxCollider.center + new Vector3(-(_boxCollider.size.x / 2f + 0.6f + 0.001f), 0, 0);
        PMRZone l_zoneCDS2 = l_coteDroitS2.GetComponent<PMRZone>();
        _pmrZonesS2.Add(l_zoneCDS2);
        _pmrZonesS3.Add(l_zoneCDS2);

        //CoteGauche
        GameObject l_coteGauche = new GameObject("CoteGauche");
        l_coteGauche.transform.parent = _pmrZones.transform;
        l_coteGauche.transform.localScale = Vector3.one;
        l_coteGauche.transform.localRotation = Quaternion.identity;
        l_coteGauche.transform.localPosition = Vector3.zero;
        GameObject l_coteGaucheS1 = Instantiate(p_pmrZonePrefab, l_coteGauche.transform);
        l_coteGaucheS1.name = "CoteGaucheS1";
        l_coteGaucheS1.transform.localScale = new Vector3(0.9f, _boxCollider.size.y * 0.95f, _boxCollider.size.z * 0.95f);
        l_coteGaucheS1.transform.localRotation = Quaternion.identity;
        l_coteGaucheS1.transform.localPosition = _boxCollider.center + new Vector3((_boxCollider.size.x / 2f + 0.45f + 0.001f), 0, 0);
        PMRZone l_zoneCGS1 = l_coteGaucheS1.GetComponent<PMRZone>();
        _pmrZonesS1.Add(l_zoneCGS1);
        GameObject l_coteGaucheS2 = Instantiate(p_pmrZonePrefab, l_coteGauche.transform);
        l_coteGaucheS2.name = "CoteGaucheS2";
        l_coteGaucheS2.transform.localScale = new Vector3(1.2f , _boxCollider.size.y * 0.95f, _boxCollider.size.z * 0.95f);
        l_coteGaucheS2.transform.localRotation = Quaternion.identity;
        l_coteGaucheS2.transform.localPosition = _boxCollider.center + new Vector3((_boxCollider.size.x / 2f + 0.6f + 0.001f), 0, 0);
        PMRZone l_zoneCGS2 = l_coteGaucheS2.GetComponent<PMRZone>();
        _pmrZonesS2.Add(l_zoneCGS2);
        _pmrZonesS3.Add(l_zoneCGS2);

        foreach(PMRZone l_zone in _pmrZonesS1)
        {
            l_zone.InitZone(_pmrS1Color);
        }
        foreach (PMRZone l_zone in _pmrZonesS2)
        {
            l_zone.InitZone(_pmrS2Color);
        }
    }

    void UpdatePMRValidity()
    {
        _pmrValid = _pmrS3Active ? _pmrS3 : (_pmrS1 || _pmrS2);
    }

    void IPMRZoneEventListener.OnPMRZoneRulesActivated(bool p_activation)
    {
        _active = p_activation;
    }

    void IPMRZoneEventListener.OnPMRZoneCollisionEventRaised(PMRZone p_zone)
    {
        if (_pmrZonesS1.Contains(p_zone))
        {
            _pmrS1 = true;
            foreach (PMRZone l_zone in _pmrZonesS1)
            {
                if (l_zone.IsColliding)
                {
                    _pmrS1 = false;
                    break;
                }
            }
        }
        else if (_pmrZonesS2.Contains(p_zone))
        {
            _pmrS2 = true;
            foreach (PMRZone l_zone in _pmrZonesS2)
            {
                if (l_zone.IsColliding)
                {
                    _pmrS2 = false;
                    break;
                }
            }
        }

        if (_pmrZonesS3.Contains(p_zone))
        {
            _pmrS3 = true;
            foreach (PMRZone l_zone in _pmrZonesS1)
            {
                if (l_zone.IsColliding)
                {
                    _pmrS3 = false;
                    break;
                }
            }
        }

        if (_pmrS3Active)
        {
            _pmrValid = _pmrS3;
            foreach (PMRZone l_zone in _pmrZonesS3)
                l_zone.RenderCollision(!_pmrS3);
        }
        else
        {
            _pmrValid = _pmrS1 || _pmrS2;
            foreach (PMRZone l_zone in _pmrZonesS1)
                l_zone.RenderCollision(!_pmrS1 && !_pmrS2);
            foreach (PMRZone l_zone in _pmrZonesS2)
                l_zone.RenderCollision(!_pmrS1 && !_pmrS2);
        }

        UpdatePMRValidity();

        //Debug.Log("Bed " + name + " PMR validity=" + _pmrValid + " -> S1=" + _pmrS1 + ", S2=" + _pmrS2 + ", S3=" + _pmrS3);
    }
}