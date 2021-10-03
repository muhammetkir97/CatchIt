using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    [SerializeField] private Button BtnStartGame;
    [SerializeField] private TextMeshProUGUI TxtLevelText;



    // Start is called before the first frame update
    void Start()
    {
        BtnStartGame.onClick.AddListener(ClickStartGame);
        TxtLevelText.text = $"Level {(Globals.Instance.GetCurrentLevel()+1)}";
    }

    void ClickStartGame()
    {
        SceneManager.LoadScene("GameScene",LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
