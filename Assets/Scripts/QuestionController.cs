using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class QuestionController : MonoBehaviour
{
    public string apiURL = "http://goldgedicht.de/";
    public string deviceId = "unity-test-id";
    public TMP_InputField questionText;
    public GameObject answerGrid;

    public void Start()
    {
        StartCoroutine(CreateUser());
        deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    public void postQuestion()
    {
        StartCoroutine(PostQuestion());
    }

    IEnumerator PostQuestion()
    {
        string url = apiURL + "api/posts";
        string input_question = questionText.text;

        var post = new Post { question = questionText.text, answers = new List<Answer>() };

        for (int i = 0; i < answerGrid.transform.childCount; i++)
        {
            if (answerGrid.transform.GetChild(i).GetChild(0).GetComponent<TMP_InputField>().text == "") continue;

            Answer answer = new Answer();
            answer.text = answerGrid.transform.GetChild(i).GetChild(0).GetComponent<TMP_InputField>().text;
            post.answers.Add(answer);
        }

        var output = new DataStruct { deviceId = deviceId, post = post };

        var test = JsonUtility.ToJson(output);

        Debug.Log(test);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(test);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            Debug.Log("Code: " + request.responseCode);
        }
    }

    IEnumerator CreateUser()
    {
        string url = apiURL + "api/users/";

        var deviceObject = new Device { deviceId = deviceId } ;

        var json_deviceId = JsonUtility.ToJson(deviceObject);

        Debug.Log(json_deviceId);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json_deviceId);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            Debug.Log("Code: " + request.responseCode);
        }
    }

    [Serializable]
    public class Answer
    {
        public string text;
    }

    [Serializable]
    public class Post
    {
        public string question;
        public List<Answer> answers;
    }

    [Serializable]
    public class DataStruct
    {
        public string deviceId;
        public Post post;
    }

    [Serializable]
    public class Device
    {
        public string deviceId;
    }
}
