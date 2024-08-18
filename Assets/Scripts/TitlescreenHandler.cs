using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RainbowText : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public Camera mainCamera;
    public Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SampleScene");
        });
    }

    // Update is called once per frame
    void Update()
    {
        var newTitleColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.5f, 1), 1, 1);
        titleText.color = newTitleColor;

        var newBgColor = Color.HSVToRGB(Mathf.PingPong(Time.time * 0.1f, 1), 0.2f, 1);
        mainCamera.backgroundColor = newBgColor;
    }
}
