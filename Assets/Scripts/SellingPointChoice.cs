using UnityEngine;
using UnityEngine.UI;

public class SellingPointChoice : MonoBehaviour
{
    public static GameObject sizeScroll;
    public GameObject sellingPointInfo;
    public GameObject handler;
    public CreateSellingPoint createSellingPoint;

    private void Awake()
    {
        sellingPointInfo = GameObject.Find("SellingPointInfo");
        sizeScroll = GameObject.Find("SizeScroll");
    }

    // Start is call,ed before the first frame update
    private void Start()
    {
        sellingPointInfo.SetActive(false);
        handler = GameObject.Find("SizeScroll");
        createSellingPoint = handler.GetComponent<CreateSellingPoint>();
        //Dictionary = new Dictionary<string, CreateSellingPoint.SellingPoints>();
    }

    private void OnMouseDown()
    {
        foreach (var pair in createSellingPoint.Dictionary)
            if (pair.Key == name)
            {
                LogIn.Server.idSP = pair.Value.id_sp;
                LogIn.Server.spName = pair.Value.name;
                sellingPointInfo.transform.Find("Direction").GetComponent<Text>().text = pair.Value.address;
                sellingPointInfo.transform.Find("Phone").GetComponent<Text>().text = pair.Value.phone;
                sellingPointInfo.transform.Find("Category").GetComponent<Text>().text =
                    pair.Value.id_category.ToString();
                sellingPointInfo.SetActive(true);
                foreach (var sp in sizeScroll.GetComponentsInChildren<Collider2D>()) sp.enabled = false;
            }
    }
}