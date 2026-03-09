using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject[] subMenus;

    public void OpenSubMenu(int subMenuIndex)
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            if (i == subMenuIndex)
            {
                subMenus[i].SetActive(true);
            }
            else
            {
                subMenus[i].SetActive(false);
            }
        }

        GameObject[] scrolls = GameObject.FindGameObjectsWithTag("Scrollbar");
        foreach (GameObject scroll in scrolls)
        {
            scroll.SetActive(false);
        }
    }

    public void CloseSubMenu()
    {
        for (int i = 0; i < subMenus.Length; i++)
        {
            subMenus[(int)i].SetActive(false);
        }
    }
}
