using UnityEngine;
using UnityEngine.UI;

public class EndOfForm : MonoBehaviour
{
    public static EndOfForm Info;
    public Text sellingPointText;
    public Text counterText;
    public InputField managerInput;
    public InputField commentInput;
    public string comments;
    public string managerName;
    public int totalTimeMinutes;
    public Button sendButton;
    public GameObject handler;
    public SendForm sendForms;


    // Start is called before the first frame update
    void Awake()
    {
        if (Info != null)
        {
            Destroy(gameObject);
            return;
        }
        Info = this;
        handler = GameObject.Find("GameHandler");
        sendForms = handler.GetComponent<SendForm>();
        
        var aux = LogIn.Server.endTime - LogIn.Server.startTime;
        counterText.text = aux.Hours + ":" + aux.Minutes + ":" + aux.Seconds;
        totalTimeMinutes = aux.Hours * 60 + aux.Minutes;
        sellingPointText.text = LogIn.Server.spName;
    }

    public void sendForm()
    {
        managerName = managerInput.text;
        comments = commentInput.text;
        if (!string.IsNullOrEmpty(managerName))
        {
            sendForms.SendFormPreparation(LogIn.Server.exteriorGrade, LogIn.Server.interiorGrade, LogIn.Server.clientGrade, LogIn.Server.managerGrade);
        }
    }
}
