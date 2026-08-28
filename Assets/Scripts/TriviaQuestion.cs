using System;
using UnityEngine;

[Serializable]
public class TriviaQuestion
{
    [TextArea(2, 4)]
    public string question;

    public string[] answers = new string[4];

    [Range(0, 3)]
    public int correctAnswerIndex;
}