using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateForms : MonoBehaviour
{
    // Start is called before the first frame update
    public Text titleText;
    public Text descriptionText;
    public List<Question> questions;
    public GameObject handler;
    public NormalFormsController normalFormsController;
    public NormalForm normalForm;
    public Dictionary<string, Form> dictionary;
    public Dictionary<int, LogIn.QuestionBundle> QuestionDictionary;
    private int start;

    private void Awake()
    {
        handler = GameObject.Find("GameHandler");
        normalFormsController = handler.GetComponent<NormalFormsController>();
        start = normalFormsController.areaStart;
        normalForm = handler.GetComponent<NormalForm>();
        StartCoroutine(WithoutVisualKey());
    }

    private IEnumerator WithoutVisualKey()
    {
        using var request = UnityWebRequest.Get(normalFormsController.QuestionQuery);
        yield return request.SendWebRequest();
        var text = request.downloadHandler.text;
        yield return questions = JsonUtility.FromJson<QuestionList>("{ \"questionList\": " + text + "}").questionList;

        normalFormsController.idButton = start;
        var sceneName = SceneManager.GetActiveScene().name;

        QuestionDictionary = new Dictionary<int, LogIn.QuestionBundle>();

        if (sceneName == "Preparation" && LogIn.Server.prepDictionary.Count > 0)
        {
            QuestionDictionary = LogIn.Server.prepDictionary;
        }
        else if (sceneName == "Client" && LogIn.Server.clientDictionary.Count > 0)
        {
            QuestionDictionary = LogIn.Server.clientDictionary;
        }
        else if (sceneName == "Manager" && LogIn.Server.managerDictionary.Count > 0)
        {
            QuestionDictionary = LogIn.Server.managerDictionary;
        }
        else
        {
            QuestionDictionary[start] = new LogIn.QuestionBundle
            {
                questionList = new List<Question>(),
                currentIndex = 0,
                answerList = new List<int>(),
                areaTitle = ""
            };
            for (var i = 0; i < questions.Count; i++)
            {
                QuestionDictionary[start].questionList.Add(questions[i]);
                QuestionDictionary[start].answerList.Add(0);
                if (QuestionDictionary[start].areaTitle == "")
                    QuestionDictionary[start].areaTitle = questions[i].area_title;
            }
        }

        dictionary = new Dictionary<string, Form>();
        var name = "Key";
        var current = new Form();
        var currentQuestion = QuestionDictionary[start].currentIndex;
        current.id = start;
        current.title = normalFormsController.TitleDictionary[1];
        if (QuestionDictionary[start].questionList.Count != 0)
        {
            current.question = QuestionDictionary[start].questionList[currentQuestion].p_text;
            current.buttonNa = QuestionDictionary[start].questionList[currentQuestion].btn_na;
            current.buttonCamera = QuestionDictionary[start].questionList[currentQuestion].camera;
        }
        else
        {
            current.question = "No hay pregunta";
            current.buttonNa = 0;
            current.buttonCamera = 0;
        }

        dictionary.Add(name, current); // pass the key and the stuff you want to have in the dictionary
        normalForm.OnStart();
    }

    public class Form
    {
        public int buttonCamera;
        public int buttonNa;
        public int id;
        public string question; //field or data member  
        public string title; // field or data member
    }
}