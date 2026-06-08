using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI txt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txt=GetComponent<TextMeshProUGUI>();    
        InvokeRepeating("Animatetext", 0f, 0.5f);
    }
    void Animatetext()
    {
        if (txt.text == "Loading...")
        {
            txt.text = "Loading";
        }
        else
        {
            txt.text += ".";
        }
    }   

  
}
