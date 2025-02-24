using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft.Json; // 確保 Newtonsoft.Json 被引用

public class DeepSeekAPI_Sample : MonoBehaviour
{
    private string apiKey = "sk-31320c884b7143f99bef5344634a2858";
    private string apiUrl = "https://api.deepseek.com/v1/chat/completions";
    // Start is called before the first frame update
    void Start()
    {
        sendMessageToDeepSeek("你好啊",null);
    }
    public void sendMessageToDeepSeek(string message, UnityAction<string> callback)
    {
        StartCoroutine(PostRequest(message, callback));
    }


    IEnumerator PostRequest(string message, UnityAction<string> callback)
    {

        //創建匿名類型請求體
        var requestBody = new
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new { role = "user", content = message }
            }
        };

        // 使用Newtonsoft.Json序列化
        string jsonBody = JsonConvert.SerializeObject(requestBody);
        Debug.Log(jsonBody);
        //yield return null;
        //創建unityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");//設置上傳處理器
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);//設置下載處理器
                                                                      //發送請求
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text); //列印詳細錯誤資訊
        }
        else
        {

            //處理響應
            string responseJson = request.downloadHandler.text;
            Debug.Log("Response: " + responseJson);
        }

    }
}
