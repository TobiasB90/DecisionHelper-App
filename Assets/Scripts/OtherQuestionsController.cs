using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class OtherQuestionsController : MonoBehaviour
{
    public string apiURL = "http://goldgedicht.de/";
    public string deviceId = "unity-test-id";
    public TMP_Text questionText;
    public GameObject questionButtonPrefab;
    public Transform gridObject;
    public OtherQuestionsManager otherQuestionManager;

    public void getQuestions()
    {
        StartCoroutine(GetQuestions());
    }

    public void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    IEnumerator GetQuestions()
    {
        //clear old ones

        for(int i = 0; i < gridObject.childCount; i++)
        {
            Destroy(gridObject.GetChild(i).gameObject);
        }

        string url = apiURL + "api/posts";

        var request = new UnityWebRequest(url, "GET");
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

        var questions = SimpleJSON.JSON.Parse(request.downloadHandler.text);

        int randomQuestion = UnityEngine.Random.Range(0, questions.Count - 1);

        questionText.text = questions[randomQuestion]["question"];

        for(int i = 0; i < questions[randomQuestion]["answers"].Count; i++)
        {
            GameObject questionButton_tmp = GameObject.Instantiate(questionButtonPrefab, gridObject);
            questionButton_tmp.transform.GetChild(0).GetComponent<TMP_Text>().text = questions[randomQuestion]["answers"][i]["text"];
            questionButton_tmp.GetComponent<AnswerButton>().id = i;
            questionButton_tmp.GetComponent<AnswerButton>().otherQuestionController = this;
        }

        otherQuestionManager.currentPostId = questions[randomQuestion]["_id"];
    }

    public IEnumerator AnswerQuestion(int answerid)
    {
        string postid = otherQuestionManager.currentPostId;
        string url = apiURL + "api/posts/" + postid + "/" + answerid;
        var deviceObject = new Device { deviceId = deviceId };
        var json_deviceId = JsonUtility.ToJson(deviceObject);
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
            Debug.Log("ANSWER Received: " + request.downloadHandler.text);
            Debug.Log("Code: " + request.responseCode);
        }

        yield return new WaitForSeconds(.5f);

        StartCoroutine(GetQuestions());
    }

    [Serializable]
    public class Device
    {
        public string deviceId;
    }
}
