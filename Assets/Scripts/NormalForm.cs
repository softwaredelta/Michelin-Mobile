using UnityEngine;
using UnityEngine.UI;

public class NormalForm : MonoBehaviour
{
    public GameObject forms;
    public GameObject handler;
    public CreateForms createForm;
    public NormalFormsController normalFormsController;

    // Start is called before the first frame update
    private void Awake()
    {
        forms = GameObject.Find("Forms");
        handler = GameObject.Find("GameHandler");
        normalFormsController = handler.GetComponent<NormalFormsController>();
        createForm = handler.GetComponent<CreateForms>();
    }

    public void OnStart()
    {
        var pair = createForm.dictionary["Key"];
        createForm.titleText.text = pair.title;
        createForm.descriptionText.text = pair.question;
        normalFormsController.idButton = pair.id;
        var currentIndex = createForm.QuestionDictionary[pair.id].currentIndex;

        //
        normalFormsController.previousQuestion.enabled = true;
        normalFormsController.previousQuestion.GetComponent<Image>().color = Color.white;
        normalFormsController.nextQuestion.enabled = true;
        normalFormsController.nextQuestion.GetComponent<Image>().color = Color.white;

        if (currentIndex == 0)
        {
            normalFormsController.previousQuestion.enabled = false;
            normalFormsController.previousQuestion.GetComponent<Image>().color = Color.clear;
        }

        if (currentIndex == createForm.QuestionDictionary[pair.id].questionList.Count - 1)
        {
            normalFormsController.nextQuestion.enabled = false;
            normalFormsController.nextQuestion.GetComponent<Image>().color = Color.clear;
        }

        //
        loadAnswers(pair.buttonCamera, pair.buttonNa, pair.id);
        StartCoroutine(normalFormsController.LoadImage(normalFormsController.routeApi + "question/placeholder/"));
    }

    private void AnswerChoice(bool green, bool red, bool na, Color greenColor, Color redColor, Color naColor,
        bool camara, bool saveMsg, bool savedImage)
    {
        normalFormsController.greenBibendum.enabled = green;
        normalFormsController.redBibendum.enabled = red;
        normalFormsController.naButton.enabled = na;
        normalFormsController.redBibendum.GetComponent<Image>().color = redColor;
        normalFormsController.greenBibendum.GetComponent<Image>().color = greenColor;
        normalFormsController.naButton.GetComponent<Image>().color = naColor;
        normalFormsController.camaraButton.SetActive(camara);
        normalFormsController.saveMsg.SetActive(saveMsg);
        normalFormsController.savedImage.SetActive(savedImage);
    }

    private void clickChoice(bool na, Color naColor, bool camara)
    {
        normalFormsController.clickEnableNa = na;
        normalFormsController.clickColorNa = naColor;
        normalFormsController.clickEnableCamera = camara;
    }

    public void loadAnswers(int buttonCamera, int buttonNa, int id)
    {
        if (createForm.descriptionText.text == "No hay pregunta")
        {
            AnswerChoice(false, false, false, Color.clear, Color.clear, Color.clear, false, false, false);
            normalFormsController.nextQuestion.enabled = false;
            normalFormsController.nextQuestion.GetComponent<Image>().color = Color.clear;
        }
        else
        {
            var currentIndex = createForm.QuestionDictionary[id].currentIndex;
            var greenSelected = createForm.QuestionDictionary[id].answerList[currentIndex] == 1;
            var redSelected = createForm.QuestionDictionary[id].answerList[currentIndex] == 2 ||
                              createForm.QuestionDictionary[id].answerList[currentIndex] == 4;
            var buttonNaSelected = createForm.QuestionDictionary[id].answerList[currentIndex] == 3 && buttonNa == 1;
            var cameraActive = redSelected && buttonCamera == 1;
            var msgActive = createForm.QuestionDictionary[id].answerList[currentIndex] != 0;
            var savedIconActive = createForm.QuestionDictionary[id].answerList[currentIndex] == 4;
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