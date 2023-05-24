using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NormalFormsController : MonoBehaviour
{
    public AudioSource audioSource;
    public Button greenBibendum;
    public Button redBibendum;
    public Button nextQuestion;
    public Button previousQuestion;
    public GameObject camaraButton;
    public GameObject saveMsg;
    public int idButton;
    public Button naButton;
    public CreateForms createForms;
    public NormalForm normalForm;
    public GameObject handler;
    public bool clickEnableNa;
    public Color clickColorNa;
    public bool clickEnableCamera;
    public Image imageUrl;
    public string routeApi = "https://back2basics.software/api/";
    public string QuestionQuery;
    public int scene;
    public int areaStart;
    public GameObject savedImage;
    public Dictionary<int, string> TitleDictionary;
    public GameObject btnFinished;


    public void Awake()
    {
        handler = GameObject.Find("GameHandler");
        createForms = handler.GetComponent<CreateForms>();
        normalForm = handler.GetComponent<NormalForm>();
        TitleDictionary = AreaTitles.LoadAreaTitles(scene);
    }

    public void OnMouseDownGB()
    {
        OnClickInstruction(1, false, true, clickEnableNa, Color.gray, Color.white, clickColorNa, false);
        saveMsg.SetActive(true);
        savedImage.SetActive(false);
        var currentIndex = createForms.QuestionDictionary[idButton].currentIndex;
        createForms.QuestionDictionary[idButton].questionList[currentIndex].uploaded_image = null;
    }

    public void OnMouseDownRB()
    {
        OnClickInstruction(2, true, false, clickEnableNa, Color.white, Color.gray, clickColorNa, clickEnableCamera);
        saveMsg.SetActive(true);
    }

    public void OnMouseDownCamera()
    {
        //audioSource.Play();
    }

    public void OnMouseDownNA()
    {
        OnClickInstruction(3, true, true, false, Color.white, Color.white, Color.gray, false);
        savedImage.SetActive(false);
    }

    public void OnMouseDownNQ()
    {
        ChangeQuestion(1, "Key");
    }

    public void OnMouseDownPQ()
    {
        ChangeQuestion(-1, "Key");
    }

    private void OnClickInstruction(int selected, bool green, bool red, bool na, Color greenColor, Color redColor,
        Color naColor, bool camara)
    {
        //audioSource.Play();
        var currentIndex = createForms.QuestionDictionary[idButton].currentIndex;
        createForms.QuestionDictionary[idButton].answerList[currentIndex] = selected;
        
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Exterior")
        {
            LogIn.Server.extDictionary = createForms.QuestionDictionary;
            LogIn.Server.extVisit = true;   
        }
        else if (sceneName == "Interior")
        {
            LogIn.Server.intDictionary = createForms.QuestionDictionary;   
            LogIn.Server.intVisit = true;  
        }
        else if (sceneName == "Preparation")
        {
            LogIn.Server.prepDictionary = createForms.QuestionDictionary;
            LogIn.Server.prepVisit = true;  
        }
        else if (sceneName == "Client")
        {
            LogIn.Server.clientDictionary = createForms.QuestionDictionary;
            LogIn.Server.clientVisit = true;  
        }
        else if (sceneName == "Manager")
        {
            LogIn.Server.managerDictionary = createForms.QuestionDictionary;
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
        var currentIndex = createForms.QuestionDictionary[idButton].currentIndex;
        if (createForms.QuestionDictionary[idButton].questionList.Count != 0)
        {
            var url = link + createForms.QuestionDictionary[idButton].questionList[currentIndex].picture;
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
        createForms.QuestionDictionary[idButton].currentIndex += change;

        var currentIndex = createForms.QuestionDictionary[idButton].currentIndex;
        createForms.descriptionText.text = createForms.QuestionDictionary[idButton].questionList[currentIndex].p_text;
        createForms.dictionary[key].question =
            createForms.QuestionDictionary[idButton].questionList[currentIndex].p_text;
        createForms.dictionary[key].buttonNa =
            createForms.QuestionDictionary[idButton].questionList[currentIndex].btn_na;
        createForms.dictionary[key].buttonCamera =
            createForms.QuestionDictionary[idButton].questionList[currentIndex].camera;

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
            if (currentIndex == createForms.QuestionDictionary[idButton].questionList.Count - 1)
            {
                nextQuestion.enabled = false;
                nextQuestion.GetComponent<Image>().color = Color.clear;
            }

            previousQuestion.enabled = true;
            previousQuestion.GetComponent<Image>().color = Color.white;
        }

        normalForm.loadAnswers(createForms.dictionary[key].buttonCamera, createForms.dictionary[key].buttonNa,
            createForms.dictionary[key].id); 
    }
}