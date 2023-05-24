using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormsController : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject closePanel;
    public Button greenBibendum;
    public Button redBibendum;
    public Button nextQuestion;
    public Button previousQuestion;
    public GameObject camaraButton;
    public GameObject saveMsg;
    public int idButton;
    public Button naButton;
    public CreateVK createVk;
    public VisualKey visualKey;
    public GameObject handler;
    public bool clickEnableNa;
    public Color clickColorNa;
    public bool clickEnableCamera;
    public Image imageUrl;
    public string routeApi = "https://back2basics.software/api/";
    public string QuestionQuery;
    public int scene;
    public int visualKeyAmount;
    public int areaStart;
    public GameObject savedImage;
    public Dictionary<int, string> TitleDictionary;
    public GameObject btnFinished;

    public void Awake()
    {
        handler = GameObject.Find("GameHandler");
        createVk = handler.GetComponent<CreateVK>();
        TitleDictionary = AreaTitles.LoadAreaTitles(scene);
    }

    public void OnMouseDownGB()
    {
        OnClickInstruction(1, false, true, clickEnableNa, Color.gray, Color.white, clickColorNa, false);
        saveMsg.SetActive(true);
        savedImage.SetActive(false);
        var currentIndex = createVk.QuestionDictionary[idButton].currentIndex;
        createVk.QuestionDictionary[idButton].questionList[currentIndex].uploaded_image = null;
    }

    public void OnMouseDownRB()
    {
        OnClickInstruction(2, true, false, clickEnableNa, Color.white, Color.gray, clickColorNa, clickEnableCamera);
        saveMsg.SetActive(true);
    }

    public void OnMouseDownCamera()
    {
        audioSource.Play();
    }

    public void OnMouseDownNA()
    {
        OnClickInstruction(3, true, true, false, Color.white, Color.white, Color.gray, false);
        savedImage.SetActive(false);
    }

    public void OnMouseDownCS()
    {
        closePanel.SetActive(false);
        greenBibendum.enabled = true;
        redBibendum.enabled = true;

        foreach (var vk in createVk.dictionary)
        {
            VisualKey.vkSpawn = GameObject.Find(vk.Key);
            VisualKey.vkSpawn.GetComponent<Collider2D>().enabled = true;
        }
    }

    public void OnMouseDownNQ()
    {
        var key = "VisualKey" + idButton;
        ChangeQuestion(1, key);
    }

    public void OnMouseDownPQ()
    {
        var key = "VisualKey" + idButton;
        ChangeQuestion(-1, key);
    }

    private void OnClickInstruction(int selected, bool green, bool red, bool na, Color greenColor, Color redColor,
        Color naColor, bool camara)
    {
        audioSource.Play();
        var currentIndex = createVk.QuestionDictionary[idButton].currentIndex;
        createVk.QuestionDictionary[idButton].answerList[currentIndex] = selected;

        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Exterior")
        {
            LogIn.Server.extDictionary = createVk.QuestionDictionary;
            LogIn.Server.extVisit = true;   
        }
        else if (sceneName == "Interior")
        {
            LogIn.Server.intDictionary = createVk.QuestionDictionary;   
            LogIn.Server.intVisit = true;  
        }
        else if (sceneName == "Preparation")
        {
            LogIn.Server.prepDictionary = createVk.QuestionDictionary;
            LogIn.Server.prepVisit = true;  
        }
        else if (sceneName == "Client")
        {
            LogIn.Server.clientDictionary = createVk.QuestionDictionary;
            LogIn.Server.clientVisit = true;  
        }
        else if (sceneName == "Manager")
        {
            LogIn.Server.managerDictionary = createVk.QuestionDictionary;
            LogIn.Server.managerVisit = true;  
        }

        greenBibendum.enabled = green;
        redBibendum.enabled = red;
        naButton.enabled = na;
        greenBibendum.GetComponent<Image>().color = greenColor;
        redBibendum.GetComponent<Image>().color = redColor;
        naButton.GetComponent<Image>().color = naColor;
        camaraButton.SetActive(camara);
    }

    public IEnumerator LoadImage(string link)
    {
        var currentIndex = createVk.QuestionDictionary[idButton].currentIndex;
        if (createVk.QuestionDictionary[idButton].questionList.Count != 0)
        {
            var url = link + createVk.QuestionDictionary[idButton].questionList[currentIndex].picture;
            using var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                var newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height),
                    new Vector2(0.5f, 0.5f));

                imageUrl.sprite = newSprite;
            }
        }
        else
        {
            imageUrl.sprite = null;
        }
    }

    private void ChangeQuestion(int change, string key)
    {
        imageUrl.sprite = null;
        StartCoroutine(LoadImage(routeApi + "question/placeholder/"));
        createVk.QuestionDictionary[idButton].currentIndex += change;

        var currentIndex = createVk.QuestionDictionary[idButton].currentIndex;
        createVk.descriptionText.text = createVk.QuestionDictionary[idButton].questionList[currentIndex].p_text;
        createVk.dictionary[key].question = createVk.QuestionDictionary[idButton].questionList[currentIndex].p_text;
        createVk.dictionary[key].buttonNa = createVk.QuestionDictionary[idButton].questionList[currentIndex].btn_na;
        createVk.dictionary[key].buttonCamera = createVk.QuestionDictionary[idButton].questionList[currentIndex].camera;

        if (change < 0)
        {
            if (currentIndex == 0)
            {
                previousQuestion.enabled = false;
                previousQuestion.GetComponent<Image>().color = Color.clear;
            }

            nextQuestion.enabled = true;
            nextQuestion.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (currentIndex == createVk.QuestionDictionary[idButton].questionList.Count - 1)
            {
                nextQuestion.enabled = false;
                nextQuestion.GetComponent<Image>().color = Color.clear;
            }

            previousQuestion.enabled = true;
            previousQuestion.GetComponent<Image>().color = Color.white;
        }

        visualKey.loadAnswers(createVk.dictionary[key].buttonCamera, createVk.dictionary[key].buttonNa, idButton);
    }
}