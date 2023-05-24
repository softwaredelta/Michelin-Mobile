using System;
using UnityEngine;

public class PDF : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ScreenCapture.CaptureScreenshot("Recorrido" + DateTime.Now.ToString("MM-dd-yy(HH-mm-ss)") + ".png");
    }
}