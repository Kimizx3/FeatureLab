using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterController : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textUI;
    
    [Header("Settings")]
    public float charInterval = 0.05f;
    public float punctuationDelay = 0.2f;
    public bool autoPlayNext = false;

    private Coroutine _typingRoutine;
    private bool _isSkipping = false;

    [Header("Audio")]
    public AudioClip typeSfx;
    public AudioClip punctuationSfx;

    private AudioSource _audio;
    
    // ========================= Events ===================================
    public event Action         OnTypeStart;
    public event Action<char>   OnCharacterPrinted;
    public event Action<string> OnWordPrinted;
    public event Action<char>   OnPunctuation;
    public event Action         OnLineCompleted;
    public event Action         OnAllTextCompleted;
    public event Action         OnSkipRequested;
    
    // ========================= Public API ================================
    public void PlayNext(string text)
    {
        StopTyping();

        textUI.text = string.Empty;
        _typingRoutine = StartCoroutine(TypeRoutine(text));
    }

    public void Skip()
    {
        _isSkipping = true;
        OnSkipRequested?.Invoke();
    }

    public void StopTyping()
    {
        if (_typingRoutine != null)
        {
            StopCoroutine(_typingRoutine);
        }

        _typingRoutine = null;
        _isSkipping = false;
    }
    
    // ========================= CORE ROUTINE ==============================
    private IEnumerator TypeRoutine(string fullText)
    {
        OnTypeStart?.Invoke();

        int length = fullText.Length;
        int lastSpaceIndex = -1;

        for (int i = 0; i < length; i++)
        {
            char c = fullText[i];
            textUI.text += c;
            
            // -------- Events ---------
            OnCharacterPrinted?.Invoke(c);

            if (c == ' ' && i > 0)
            {
                string word = fullText.Substring(lastSpaceIndex + 1, i - lastSpaceIndex - 1);
                OnWordPrinted?.Invoke(word);
                lastSpaceIndex = i;
            }

            if (IsPunctuation(c))
            {
                OnPunctuation?.Invoke(c);
                yield return new WaitForSeconds(punctuationDelay);
            }

            if (_isSkipping)
            {
                textUI.text = fullText;
                break;
            }

            yield return new WaitForSeconds(charInterval);
        }
        
        OnLineCompleted?.Invoke();

        if (autoPlayNext)
        {
            // Let dialogue loader call next
        }
        
        OnAllTextCompleted?.Invoke();
    }

    private bool IsPunctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' || c == '?' || c == ';' || c == ':' ||
               c == '，' || c == '。' || c == '！' || c == '？' || c == '；' || c == '：';
    }
}
