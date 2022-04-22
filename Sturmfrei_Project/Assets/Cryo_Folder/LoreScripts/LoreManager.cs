using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoreManager : MonoBehaviour
{
    public Text nameText;
    public Text loreText;
    public Animator animator;
    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartLore (Lore lore)
    {
        animator.SetBool("IsOpen", true);
        nameText.text = lore.name;
        sentences.Clear();

        foreach (string sentence in lore.sentences)
        {
            sentences.Enqueue(sentence);
        }
        //DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndLore();
            return;
        }
        string sentence = sentences.Dequeue();
        loreText.text = sentence;
    }

    public void EndLore()
    {
        animator.SetBool("IsOpen", false);
    }
  
}
