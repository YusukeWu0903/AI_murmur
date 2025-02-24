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
    private string aiPersona = "你是一位英俊優雅的紳士店主，經營著一家迷人的歐式酒館。你的語氣風趣且充滿智慧，像是小說裡的貴族，但又不失幽默感。你擅長與客人交談，會用細膩的方式講故事。請務必保持這個角色設定，不要模仿玩家，也不要偏離你的身份。";

    public void sendMessageToOllama(string message, System.Action<string> callback)
    {
        Debug.Log("📨 發送訊息到 Ollama: " + message);
        StartCoroutine(PostRequest(message, callback));
    }

    IEnumerator PostRequest(string message, System.Action<string> callback)
    {
        // 🔥 明確區分角色，強調店主的語氣與玩家互動方式
        string finalPrompt = $"{aiPersona}\n\n"
            + "玩家說的話會在『玩家：』後面，請根據玩家的話回應。\n"
            + "請用『店主：』開頭回答，不要扮演玩家。\n\n"
            + $"玩家：{message}\n\n"
            + "請回答：";


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

            // 🔥 確保變數只被定義一次
            string rawReply = responseData.response; // ← 保留這個，不要重複定義

            // 只刪除 `<think>`，保留其他內容
            string cleanedReply = Regex.Replace(rawReply, @"<think>|<\/think>", "").Trim();

            // ✅ 避免格式錯亂
            cleanedReply = cleanedReply.Replace("\n", " ").Replace("\t", " ").Trim();

            // 🔥 確保「店主：」只出現一次
            if (!cleanedReply.StartsWith("店主："))
            {
                cleanedReply = "店主：" + cleanedReply;
            }
            else
            {
                cleanedReply = cleanedReply.Replace("店主：店主：", "店主："); // 避免重複
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







