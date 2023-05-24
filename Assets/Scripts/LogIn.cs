using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    public static LogIn Server;

    [SerializeField] private InputField emailField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Text errorMessages;
    [SerializeField] private Button loginButton;
    [SerializeField] private string baseUrl;

    public string currentToken;
    public int currentIdManager;
    public string currentName;
    public string currentLastName;
    public string currentMail;
    public int idSP;
    public string spName;
    public Dictionary<int, QuestionBundle> clientDictionary;
    public Dictionary<int, QuestionBundle> extDictionary;
    public Dictionary<int, QuestionBundle> intDictionary;
    public Dictionary<int, QuestionBundle> managerDictionary;
    public Dictionary<int, QuestionBundle> prepDictionary;

    public DateTime startTime;
    public bool prepVisit = false;
    public bool extVisit = false;
    public bool intVisit = false;
    public bool managerVisit = false;
    public DateTime endTime;
    public bool clientVisit = false;
    
    public int interiorGrade;
    public int exteriorGrade;
    public int clientGrade;
    public int managerGrade;
    

    public void Awake()
    {
        if (Server != null)
        {
            Destroy(gameObject);
            return;
        }

        Server = this;
        DontDestroyOnLoad(gameObject);
    }

    // Función utilizada para llamar el método Login, una vez el botón de ingresar sea presionado
    public void OnLoginButtonClicked()
    {
        var email = emailField.text;
        var password = passwordField.text;
        loginButton.interactable = false;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            errorMessages.text = "Favor de ingresar correo y/o contraseña.";
            loginButton.interactable = true;
            return;
        }

        StartCoroutine(LogInManager());
    }

    //  Método que realiza el form y post a la API con el email y contraseña, obtiene el token
    //  en caso de ser exitoso llama el método GetProtectedData
    private IEnumerator LogInManager()
    {
        var form = new WWWForm();

        form.AddField("email", emailField.text);
        form.AddField("password", passwordField.text);

        using var loginRequest = UnityWebRequest.Post(baseUrl, form);
        yield return loginRequest.SendWebRequest();

        if (loginRequest.result != UnityWebRequest.Result.Success)
        {
            errorMessages.text = "Correo u contraseña incorrecto";
        }
        else
        {
            if (loginRequest.isDone)
            {
                var text = loginRequest.downloadHandler.text;

                var responseJson = JsonUtility.FromJson<User>(text);
                currentToken = responseJson.token;
                currentIdManager = responseJson.id_manager;
                currentName = responseJson.name;
                currentLastName = responseJson.last_name;
                currentMail= responseJson.mail;

                extDictionary = new Dictionary<int, QuestionBundle>();
                intDictionary = new Dictionary<int, QuestionBundle>();
                prepDictionary = new Dictionary<int, QuestionBundle>();
                clientDictionary = new Dictionary<int, QuestionBundle>();
                managerDictionary = new Dictionary<int, QuestionBundle>();
                //GetProtectedData();
            }
            else
            {
                errorMessages.text = "Error " + loginRequest.responseCode;
            }

            SceneManager.LoadScene("SellingPointView");
        }

        loginButton.interactable = true;
        loginRequest.Dispose();
    }

    public class QuestionBundle
    {
        public List<int> answerList;
        public string areaTitle;
        public int currentIndex;
        public List<Question> questionList;
    }

    public bool FinishForm()
    {
        var prepCompleted = CheckIfCompleted(prepDictionary);
        var extCompleted = CheckIfCompleted(extDictionary);
        var intCompleted = CheckIfCompleted(intDictionary);
        var managerCompleted = CheckIfCompleted(managerDictionary);
        var clientCompleted = CheckIfCompleted(clientDictionary);

        if (prepCompleted && extCompleted && intCompleted && managerCompleted && clientCompleted && prepVisit && extVisit && intVisit && managerVisit && clientVisit)
        {
            interiorGrade = GetRating(intDictionary);
            exteriorGrade = GetRating(extDictionary);
            clientGrade = GetRating(clientDictionary);
            managerGrade = GetRating(managerDictionary);
            endTime = DateTime.Now;
            return true;           
        }
        else
        {
            Debug.Log("nel");
            return false;
        }
    }

    public int GetRating(Dictionary<int, QuestionBundle> questionDictionary)
    {
        var SumYes = 0;
        var SumTotal = 0;
        var SumNA = 0;

        foreach (var questionBundle in questionDictionary.Values)
        {
            for (int i = 0; i < questionBundle.answerList.Count; i++)
            {
                SumTotal++;

                if (questionBundle.answerList[i] == 1)
                {
                    SumYes++;
                }
                else if (questionBundle.answerList[i] == 3)
                {
                    SumNA++;
                }

            }
        }

        return (SumYes / (SumTotal - SumNA)) * 100;
    }

    private bool CheckIfCompleted(Dictionary<int, QuestionBundle> questionDictionary)
    {
        var Completed = true;

        foreach (var questionBundle in questionDictionary.Values)
        {
            for (int i = 0; i < questionBundle.answerList.Count; i++)
            {
                if (questionBundle.answerList[i] == 0)
                {
                    Completed = false;
                }
               
            }
        }
        return Completed;
    }
    
}