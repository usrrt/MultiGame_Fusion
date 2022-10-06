using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    // ###############################################
    //             NAME : HongSW                      
    //             MAIL : gkenfktm@gmail.com         
    // ###############################################

    public TMP_InputField InputField;
    
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            InputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", InputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("World1");
    }
}
