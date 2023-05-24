using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateVK : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public Text titleText;
    public Text descriptionText;
    public GameObject[] spawn;
    public List<Question> questions;
    public GameObject handler;
    public FormsController formsController;
    public int scene;
    public NormalForm normalForm;
    private int amount;
    public Dictionary<string, visualKey> dictionary;
    public Dictionary<int, LogIn.QuestionBundle> QuestionDictionary;
    private int start;

    private void Awake()
    {
        handler = GameObject.Find("GameHandler");
        formsController = handler.GetComponent<FormsController>();
        amount = formsController.visualKeyAmount + 1;
        start = formsController.areaStart;

        StartCoroutine(SetUpVisualKey());
    }

    private void Start()
    {
    }

    private IEnumerator SetUpVisualKey()
    {
        using var request = UnityWebRequest.Get(formsController.QuestionQuery);
        yield return request.SendWebRequest();
        var text = request.downloadHandler.text;
        yield return questions = JsonUtility.FromJson<QuestionList>("{ \"questionList\": " + text + "}").questionList;


        QuestionDictionary = new Dictionary<int, LogIn.QuestionBundle>();
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Exterior" && LogIn.Server.extDictionary.Count > 0)
        {
            QuestionDictionary = LogIn.Server.extDictionary;
        }
        else if (sceneName == "Interior" && LogIn.Server.intDictionary.Count > 0)
        {
            QuestionDictionary = LogIn.Server.intDictionary;
        }
        else
        {
            for (var i = start; i < amount + start; i++)
                QuestionDictionary[i] = new LogIn.QuestionBundle
                {
                    questionList = new List<Question>(),
                    currentIndex = 0,
                    answerList = new List<int>(),
                    areaTitle = ""
                };
            for (var i = 0; i < questions.Count; i++)
            {
                var currentArea = questions[i].id_area;
                QuestionDictionary[currentArea].questionList.Add(questions[i]);
                QuestionDictionary[currentArea].answerList.Add(0);
                if (QuestionDictionary[currentArea].areaTitle == "")
                    QuestionDictionary[currentArea].areaTitle = questions[i].area_title;
            }
        }

        dictionary = new Dictionary<string, visualKey>();
        for (var i = start; i < amount + start - 1; i++)
        {
            var newPrefab = Instantiate(prefab, spawn[i - start].transform.position, Quaternion.identity);
            newPrefab.name = "VisualKey" + i;
            var current = new visualKey();
            var currentQuestion = QuestionDictionary[i].currentIndex;
            current.id = i;
            current.title = formsController.TitleDictionary[i - start + 1];
            if (QuestionDictionary[i].questionList.Count != 0)
            {
                current.question = QuestionDictionary[i].questionList[currentQuestion].p_text;
                current.buttonNa = QuestionDictionary[i].questionList[currentQuestion].btn_na;
                current.buttonCamera = QuestionDictionary[i].questionList[currentQuestion].camera;
            }
            else
            {
                current.question = "No hay pregunta";
                current.buttonNa = 0;
                current.buttonCamera = 0;
            }

            dictionary.Add(newPrefab.name, current); // pass the key and the stuff you want to have in the dictionary
        }
    }

    public class visualKey
    {
        public int buttonCamera;
        public int buttonNa;
        public int id; //field or data member   
        public string question; //field or data member  
        public string title; // field or data member
    }
}