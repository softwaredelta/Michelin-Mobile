using UnityEngine;
using UnityEngine.Networking;

public static class APIHelper
{
    public static Sprite LoadImage(string link, int index, string imgName)
    {
        var currentIndex = index;
        var url = "";
        try
        {
            url = link + imgName;
        }
        catch
        {
            url = "ERR";
        }

        if (url != "ERR")
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                var newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height),
                    new Vector2(0.5f, 0.5f));

                return newSprite;
            }
        }

        return null;
    }
}