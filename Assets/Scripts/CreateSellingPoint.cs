using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateSellingPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sellingPoint;
    [SerializeField] private string textURL;
    public GameObject sellingPointInfo;
    public GameObject titleText;
    public Dictionary<string, SellingPoint> Dictionary;
    public int idSelected;
    public string nameSelected;

    private void Awake()
    {
        titleText = GameObject.Find("Title");
        if (LogIn.Server != null) NewUser(LogIn.Server.currentName);
        // GetNewSellingPoint();
        StartCoroutine(GetNewSellingPoint());
    }


    // private void GetNewSellingPoint()
    // {
    //     var spoint = APIHelper.GetNewSellingPoint();
    //     Dictionary = new Dictionary<string, SellingPoint>();
    //     for (var i = 0; i < spoint.sellingPointList.Length; i++)
    //     {   
    //         var posI = i * 450;
    //         var rt = this.GetComponent<RectTransform>();
    //         var current = new SellingPoint();
    //         current.name = spoint.sellingPointList[i].name;
    //         current.phone = spoint.sellingPointList[i].phone;
    //         current.address = spoint.sellingPointList[i].address;
    //         current.id_category = spoint.sellingPointList[i].id_category;
    //         var totalHeight = spoint.sellingPointList.Length * 450;
    //         rt.sizeDelta = new Vector2(100,totalHeight);
    //         var heightPost = (totalHeight / 2)-200;
    //         var newSellingPoint = Instantiate(sellingPoint,new Vector2(0,heightPost-posI), quaternion.identity);
    //         newSellingPoint.name = ("SellingPoint") + i;
    //         newSellingPoint.transform.SetParent(gameObject.transform, false); 
    //         newSellingPoint.GetComponentInChildren<Text>().text = current.name;
    //         Dictionary.Add(newSellingPoint.name, current);
    //     }
    // }

    private IEnumerator GetNewSellingPoint()
    {
        using var request = UnityWebRequest.Get(textURL);
        yield return request.SendWebRequest();
        var text = request.downloadHandler.text;
        var spoint = JsonUtility.FromJson<SellingPointList>("{\"sellingPointList\":" + text + "}");
        Dictionary = new Dictionary<string, SellingPoint>();
        for (var i = 0; i < spoint.sellingPointList.Length; i++)
        {
            var posI = i * 450;
            var rt = GetComponent<RectTransform>();
            var current = new SellingPoint();
            current.id_sp = spoint.sellingPointList[i].id_sp;
            current.name = spoint.sellingPointList[i].name;
            current.phone = spoint.sellingPointList[i].phone;
            current.address = spoint.sellingPointList[i].address;
            current.id_category = spoint.sellingPointList[i].id_category;
            var totalHeight = spoint.sellingPointList.Length * 450;
            rt.sizeDelta = new Vector2(100, totalHeight);
            var heightPost = totalHeight / 2 - 200;
            var newSellingPoint = Instantiate(sellingPoint, new Vector2(0, heightPost - posI), quaternion.identity);
            newSellingPoint.name = "SellingPoint" + i;
            newSellingPoint.transform.SetParent(gameObject.transform, false);
            newSellingPoint.GetComponentInChildren<Text>().text = current.name;
            Dictionary.Add(newSellingPoint.name, current);
        }
    }

    public void NewUser(string username)
    {
        // add code here to handle when a color is selected
        LogIn.Server.currentName = username;
        titleText.GetComponent<Text>().text = "Bienvenido : " + username;
    }

    public void CloseInfo()
    {
        sellingPointInfo.SetActive(false);
        foreach (var sp in SellingPointChoice.sizeScroll.GetComponentsInChildren<Collider2D>()) sp.enabled = true;
    }

    public void NextScene()
    {
        LogIn.Server.startTime = DateTime.Now;
        SceneManager.LoadScene("Preparation");
    }
}