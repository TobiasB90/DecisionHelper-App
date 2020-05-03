using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    public int id;
    public OtherQuestionsController otherQuestionController;
    // Start is called before the first frame update
    public void answerQuestion()
    {
        StartCoroutine(otherQuestionController.AnswerQuestion(id));
    }
}
