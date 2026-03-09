using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_NotificationText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayNotification(string p_text)
    {
        if(m_NotificationText != null)
        {
            m_NotificationText.text = p_text;
            StartCoroutine(DisplayNotificationRoutine());
        }

    }

    IEnumerator DisplayNotificationRoutine()
    {
        m_NotificationText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(5f);
        m_NotificationText.gameObject.SetActive(false);
        yield return null;
    }
}
