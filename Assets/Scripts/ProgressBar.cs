using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ProgressBar : MonoBehaviour
{
    public GameObject btnPrep;
    public GameObject btnExt;
    public GameObject btnInt;
    public GameObject btnClient;
    public GameObject btnManager;
    public GameObject btnFinish;

    public void ChangeScenePrep()
    {
        SceneManager.LoadScene("Preparation");
        DontDestroyOnLoad(this);
    }

    public void ChangeSceneExt()
    {
        SceneManager.LoadScene("Exterior");
        DontDestroyOnLoad(this);
    }

    public void ChangeSceneInt()
    {
        SceneManager.LoadScene("Interior");
        DontDestroyOnLoad(this);
    }

    public void ChangeSceneClient()
    {
        SceneManager.LoadScene("Client");
        DontDestroyOnLoad(this);
    }

    public void ChangeSceneManager()
    {
        SceneManager.LoadScene("Manager");
        DontDestroyOnLoad(this);
    }

    /*
     * Link to details of User Story M8_H5 (TBM finaliza el recorrido)
     * https://docs.google.com/spreadsheets/d/1Eme0YIj9GZCc3QCBQehDUGZIgS7aTilZx4oUy35dcGc/edit#gid=682046239
     */
    public void FinishForm ()
    {
        if (LogIn.Server.FinishForm())
        {
            LogIn.Server.endTime = DateTime.Now;
            SceneManager.LoadScene("End of forms");
            Destroy(this);
        }
    }

}