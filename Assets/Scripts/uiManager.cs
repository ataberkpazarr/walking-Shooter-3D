using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class uiManager : Singleton<uiManager>
{
 

    public void onClickForGreenColor()
    {
        PlayerPrefs.SetInt("Color",0);
        SceneManager.LoadScene(1);
      
    }

    public void onClickForRedColor()
    {
        PlayerPrefs.SetInt("Color", 1);
        SceneManager.LoadScene(1);

    }

    public void onClickForYellowColor()
    {
        PlayerPrefs.SetInt("Color", 2);
        SceneManager.LoadScene(1);

    }

}
