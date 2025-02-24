using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class OllamaAPI_Sample : MonoBehaviour
{
    private string ollamaUrl = "http://localhost:11434/api/generate";
    private string modelName = "mistral"; // ✅ 改為 Mistral

    // 🔥 設定 AI 的人格
    private string aiPersona = "你是一位幽默風趣的老酒館店主，總是帶點江湖氣息的口吻說話，喜歡講故事和開玩笑。你只需要扮演店主，請不要模仿玩家。";

    public void sendMessageToOllama(string message, System.Action<string> callback)
    {
        Debug.Log("📨 發送訊息到 Ollama: " + message);
        StartCoroutine(PostRequest(message, callback));
    }

    IEnumerator PostRequest(string message, System.Action<string> callback)
    {
        // 🔥 明確區分角色
        string finalPrompt = $"{aiPersona}\n\n玩家：{message}\n\n請用「店主：」開頭回答，不要扮演玩家。";

        var requestBody = new
        {
            model = modelName,
            prompt = finalPrompt, // ✅ 明確告知 AI 角色分工
            stream = false
        };

        string jsonBody = JsonConvert.SerializeObject(requestBody);
        Debug.Log("📝 Request JSON: " + jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(ollamaUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ HTTP Error: {request.responseCode}, {request.error}");
                callback?.Invoke("（AI 伺服器錯誤，請稍後再試！）");
                yield break;
            }

            string responseJson = request.downloadHandler.text;
            Debug.Log("📝 Full Response JSON: " + responseJson); // ✅ 確保 API 有回應

            var responseData = JsonConvert.DeserializeObject<OllamaResponse>(responseJson);

            if (responseData == null || string.IsNullOrEmpty(responseData.response))
            {
                Debug.LogError("❌ AI 回應為空，請檢查 Ollama 是否正確回應！");
                callback?.Invoke("（AI 沒有回應，請再試一次！）");
                yield break;
            }

            string rawReply = responseData.response;

            // 只刪除 `<think>`，保留其他內容
            string cleanedReply = Regex.Replace(rawReply, @"<think>|<\/think>", "").Trim();

            // 🔥 確保 AI 只扮演店主
            if (!cleanedReply.StartsWith("店主："))
            {
                cleanedReply = "店主：" + cleanedReply;
            }

            Debug.Log("🎯 AI 回應: " + cleanedReply);

            callback?.Invoke(cleanedReply);
        }
    }
}

// 🔥 Ollama 回應格式
[System.Serializable]
public class OllamaResponse
{
    public string response;
}





