using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SendForm : MonoBehaviour
{
    public GameObject handler;
    private string baseUrl;
    private byte[] textureBytes;
    private string interiorJson;
    private string exteriorJson;
    private string clientJson;
    private string managerJson;
    private string preparationJson;

    public void Awake()
    {
        handler = GameObject.Find("GameHandler");
    }

    public void SendFormPreparation(int extGrade, int intGrade, int clientGrade, int managerGrade)
         {

        interiorJson = FillQuestionJson(LogIn.Server.intDictionary);
        exteriorJson = FillQuestionJson(LogIn.Server.extDictionary);
        clientJson = FillQuestionJson(LogIn.Server.clientDictionary);
        managerJson = FillQuestionJson(LogIn.Server.managerDictionary);
        preparationJson = FillQuestionJson(LogIn.Server.prepDictionary);
        
        Debug.Log(interiorJson);
        Debug.Log(exteriorJson);
        Debug.Log(clientJson);
        Debug.Log(managerJson);
        Debug.Log(preparationJson);
        

        StartCoroutine(SendCompletedForm(extGrade, intGrade, clientGrade, managerGrade));
    }

    private IEnumerator SendCompletedForm(int extGrade, int intGrade, int clientGrade, int managerGrade)
    {
        var form = new WWWForm();
        
        form.AddField("idCategory", 1);
        form.AddField("mail", LogIn.Server.currentMail);
        form.AddField("exteriorGrade", extGrade);
        form.AddField("interiorGrade", intGrade);
        form.AddField("clientGrade", clientGrade);
        form.AddField("managerGrade", managerGrade);
        form.AddField("spId", LogIn.Server.idSP);
        form.AddField("fileName", "realizacion");
        form.AddField("duration", EndOfForm.Info.totalTimeMinutes);
        form.AddField("comment", EndOfForm.Info.comments);
        form.AddField("userName", LogIn.Server.currentName + " " + LogIn.Server.currentLastName);
        form.AddField("managerName", EndOfForm.Info.managerName);
        form.AddField("exterior", exteriorJson);
        form.AddField("interior",interiorJson);
        form.AddField("client", clientJson);
        form.AddField("manager", managerJson);
        form.AddField("preparation", preparationJson);
        
        // AddImages(ref form, LogIn.Server.intDictionary);
        // LogIn.Server.intDictionary = null;
        // AddImages(ref form, LogIn.Server.extDictionary);
        // LogIn.Server.extDictionary = null;
        // AddImages(ref form, LogIn.Server.clientDictionary);
        // LogIn.Server.clientDictionary = null;
        // AddImages(ref form, LogIn.Server.managerDictionary);
        // LogIn.Server.managerDictionary = null;
        // AddImages(ref form, LogIn.Server.prepDictionary);
        // LogIn.Server.prepDictionary = null;
        
        var www = UnityWebRequest.Post("https://back2basics.software/api/form/postForm", form);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.Log(www.error);
        else
            Debug.Log("Form upload complete! " + www.downloadHandler.text);
    }

    // private Texture2D GetTextureCopy(Texture2D source)
    // {
    //     //Create a RenderTexture
    //     source.Compress(false);
    //     var rt = RenderTexture.GetTemporary(
    //         source.width,
    //         source.height,
    //         0,
    //         RenderTextureFormat.Default,
    //         RenderTextureReadWrite.Linear
    //     );
    //     
    //     //Copy source texture to the new render (RenderTexture) 
    //     Graphics.Blit(source, rt);
    //
    //     //Store the active RenderTexture & activate new created one (rt)
    //     var previous = RenderTexture.active;
    //     RenderTexture.active = rt;
    //
    //     //Create new Texture2D and fill its pixels from rt and apply changes.
    //     var readableTexture = new Texture2D(source.width, source.height);
    //     readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
    //     readableTexture.Apply();
    //
    //     //activate the (previous) RenderTexture and release texture created with (GetTemporary( ) ..)
    //     RenderTexture.active = previous;
    //     RenderTexture.ReleaseTemporary(rt);
    //
    //     return readableTexture;
    // }

    private string FillQuestionJson(Dictionary<int, LogIn.QuestionBundle> dictionary)
    {
        var section = "{\"questions\": [";
        foreach (var questionBundle in dictionary.Values)
        {
            for (int i = 0; i < questionBundle.questionList.Count; i++)
            {
                section += "{\"text\":\" " + questionBundle.questionList[i].p_text + "\", ";
                section += "\"answer\": " + questionBundle.answerList[i] + ", ";
                section += "\"file\":\"" + questionBundle.questionList[i].uploaded_image + "\"";
                section += "},";
            }

        }

        section = section.TrimEnd(',');
        // section.Remove(section.Length - 1, 1);
        section += "]}";

        return section;
    }

    // private void AddImages(ref WWWForm form, Dictionary<int, LogIn.QuestionBundle> dictionary)
    // {
    //     foreach (var questionBundle in dictionary.Values)
    //     {
    //         for (int i = 0; i < questionBundle.questionList.Count; i++)
    //         {
    //             if (questionBundle.questionList[i].uploaded_image != null)
    //             {
    //                 form.AddBinaryData("images", questionBundle.questionList[i].uploaded_image, questionBundle.questionList[i].file_name , "image/png");
    //             }
    //         }
        //}
    //}
}