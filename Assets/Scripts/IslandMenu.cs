﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IslandMenu : MonoBehaviour
{
    public Image displayimage; //the image that will be used to show the images in the IslandImages group/array
    public Sprite[] IslandImages; //stores all the images that will give a preview of the island before it is selected
    private int index; //used to determine which item in an group/array to iterate on

    public TextMeshProUGUI text;

    //I understand that the sprite variable of the displayimage to equal the first item in the IslandImages group/array.
    public void Start() 
    {
        displayimage.sprite = IslandImages[0]; //sets the sprite variable of the displayimage to equal the first item in the IslandImages group/array
    }

    //I understand that the index is used if index is greater than 0, subtracts the index value by 1 and sets the sprite variable of the displayimage to be one of the IslandImages depending on what value index is.
    public void PreviousIsland() 
    {
        if (index > 0) //used if index is greater than 0
        {
            index -= 1; //subtracts the index value by 1
        }
        displayimage.sprite = IslandImages[index]; //sets the sprite variable of the displayimage to be one of the IslandImages depending on what value index is
        text.text = IslandImages[index].name;
    }

    //I understand that the index < Island Images.Length - 1 is used if index is less than the value of IslandImages.Length - 1, the index += 1 adds 1 to the index value and the displayimage.sprite = IslandImages[index] sets the sprite variable of the displayimage to be one of the IslandImages depending on what value index is.
    public void NextIsland()
    {
        if (index < IslandImages.Length - 1) //used if index is less than the value of IslandImages.Length - 1
        {
            index += 1; //adds 1 to the index value
        }
        displayimage.sprite = IslandImages[index]; //sets the sprite variable of the displayimage to be one of the IslandImages depending on what value index is
        text.text = IslandImages[index].name;
    }

    //I understand that the PlayerPrefs.SetInt("IslandCount", index); saves a value that can be used to determind what island to spawn in. IslandCount is the value set by index. Index is the value setting IslandCount.
    public void SelectIsland()
    {
        PlayerPrefs.SetInt("IslandCount", index); //saves a value that can be used to determine what island to spawn in
        SceneManager.LoadScene(1);
    }
}