using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    public DialogueData data;
    public TypewriterController typewriter;

    private int _currentIndex = 0;

    public void PlayNextLine()
    {
        if (_currentIndex < data.lines.Count)
        {
            typewriter.PlayNext(data.lines[_currentIndex]);
            _currentIndex++;
        }
    }
}
