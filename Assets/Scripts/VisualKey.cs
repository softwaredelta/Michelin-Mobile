using UnityEngine;
using UnityEngine.UI;

public class VisualKey : MonoBehaviour
{
    public static GameObject vkSpawn;
    public AudioSource audioSource;
    public GameObject forms;
    public CreateVK createVk;
    public GameObject handler;
    public FormsController formsController;


    // Start is called before the first frame update
    private void Awake()
    {
        forms = GameObject.Find("Forms");
        handler = GameObject.Find("GameHandler");
        createVk = handler.GetComponent<CreateVK>();
        formsController = handler.GetComponent<FormsController>();
    }

    private void Start()
    {
        forms.SetActive(false);
    }

    public void OnMouseDown()
    {
        formsController.visualKey = this;
        var pair = createVk.dictionary[name];
        createVk.titleText.text = pair.title;
        createVk.descriptionText.text = pair.question;
        formsController.idButton = pair.id;
        var currentIndex = createVk.QuestionDictionary[pair.id].currentIndex;

        //
        formsController.previousQuestion.enabled = true;
        formsController.previousQuestion.GetComponent<Image>().color = Color.white;
        formsController.nextQuestion.enabled = true;
        formsController.nextQuestion.GetComponent<Image>().color = Color.white;

        if (currentIndex == 0)
        {
            formsController.previousQuestion.enabled = false;
            formsController.previousQuestion.GetComponent<Image>().color = Color.clear;
        }

        if (currentIndex == createVk.QuestionDictionary[pair.id].questionList.Count - 1)
        {
            formsController.nextQuestion.enabled = false;
            formsController.nextQuestion.GetComponent<Image>().color = Color.clear;
        }

        //
        loadAnswers(pair.buttonCamera, pair.buttonNa, pair.id);
        audioSource.Play();
        StartCoroutine(formsController.LoadImage(formsController.routeApi + "question/placeholder/"));
        forms.SetActive(true);

        foreach (var vk in createVk.dictionary)
        {
            vkSpawn = GameObject.Find(vk.Key);
            vkSpawn.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void AnswerChoice(bool green, bool red, bool na, Color greenColor, Color redColor, Color naColor,
        bool camara, bool saveMsg, bool savedImage)
    {
        formsController.greenBibendum.enabled = green;
        formsController.redBibendum.enabled = red;
        formsController.naButton.enabled = na;
        formsController.redBibendum.GetComponent<Image>().color = redColor;
        formsController.greenBibendum.GetComponent<Image>().color = greenColor;
        formsController.naButton.GetComponent<Image>().color = naColor;
        formsController.camaraButton.SetActive(camara);
        formsController.saveMsg.SetActive(saveMsg);
        formsController.savedImage.SetActive(savedImage);
    }

    private void clickChoice(bool na, Color naColor, bool camara)
    {
        formsController.clickEnableNa = na;
        formsController.clickColorNa = naColor;
        formsController.clickEnableCamera = camara;
    }

    public void loadAnswers(int buttonCamera, int buttonNa, int id)
    {
        if (createVk.descriptionText.text == "No hay pregunta")
        {
            AnswerChoice(false, false, false, Color.clear, Color.clear, Color.clear, false, false, false);
            formsController.nextQuestion.enabled = false;
            formsController.nextQuestion.GetComponent<Image>().color = Color.clear;
        }
        else
        {
            var currentIndex = createVk.QuestionDictionary[id].currentIndex;
            var greenSelected = createVk.QuestionDictionary[id].answerList[currentIndex] == 1;
            var redSelected = createVk.QuestionDictionary[id].answerList[currentIndex] == 2 ||
                              createVk.QuestionDictionary[id].answerList[currentIndex] == 4;
            var buttonNaSelected = createVk.QuestionDictionary[id].answerList[currentIndex] == 3 && buttonNa == 1;
            var cameraActive = redSelected && buttonCamera == 1;
            var msgActive = createVk.QuestionDictionary[id].answerList[currentIndex] != 0;
            var savedIconActive = createVk.QuestionDictionary[id].answerList[currentIndex] == 4;
            Color greenColor;
            Color redColor;
            Color naColor;
            Color naColorClick;

            if (greenSelected)
                greenColor = Color.gray;
            else
                greenColor = Color.white;

            if (redSelected)
                redColor = Color.gray;
            else
                redColor = Color.white;

            if (buttonNaSelected && buttonNa == 1)
            {
                naColor = Color.gray;
                naColorClick = Color.white;
            }
            else if (buttonNa == 1)
            {
                naColor = Color.white;
                naColorClick = Color.white;
            }
            else
            {
                naColor = Color.clear;
                naColorClick = Color.clear;
            }

            clickChoice(buttonNa == 1, naColorClick, buttonCamera == 1);


            AnswerChoice(
                !greenSelected,
                !redSelected,
                !buttonNaSelected && buttonNa == 1,
                greenColor,
                redColor,
                naColor,
                cameraActive,
                msgActive,
                savedIconActive);
        }
    }
}