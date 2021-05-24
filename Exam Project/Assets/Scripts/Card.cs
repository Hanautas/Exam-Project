using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject cardFront;
    public GameObject cardBack;

    public Animator animator;
    public AudioSource audioSource;
    public AudioClip soundEffect;

    public int iD;
    public bool canFlip;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        canFlip = true;
    }

    void Update()
    {

    }

    public void FlipCard()
    {
        if (GameSystem.instance.object1 == null)
        {
            if (canFlip == true)
            {
                GameSystem.instance.object1 = this.gameObject;

                animator.SetBool("Flip Card", true);

                canFlip = false;
            }
        }
        else if (GameSystem.instance.object1 != null && GameSystem.instance.object2 == null)
        {
            if (canFlip == true)
            {
                GameSystem.instance.object2 = this.gameObject;
                GameSystem.instance.displayText = true;

                animator.SetBool("Flip Card", true);

                canFlip = false;
            }
        }

        audioSource.PlayOneShot(soundEffect, 1f);
    }

    public void GameSystemHideCard()
    {
        StartCoroutine(HideCard());
    }

    public IEnumerator HideCard()
    {
        animator.SetBool("Flip Card", false);

        yield return new WaitForSeconds(1f);

        canFlip = true;
    }
}