using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestOpenFile : MonoBehaviour
{
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(()=> {
            string path = OpenFile.OpenFileWin(filter: "*.jpg;*.png");
            print(path);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
