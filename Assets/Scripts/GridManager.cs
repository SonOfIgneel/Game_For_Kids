using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridParent;
    public float rowSpacing = 10f;
    public float columnSpacing = 10f;
    public int rows, cols;
    public List<string> categories;
    public List<Sprite> card_images;
    public List<string> categories1;
    public List<Sprite> card_images1;
    public List<string> categories2;
    public List<Sprite> card_images2;
    public List<GameObject> entire_cards;
    public List<string> assignedCategories = new List<string>();
    public Sprite default_sprite;
    public TextMeshProUGUI score, attempts, highscore, congrats;
    public int score_int, attempts_int, click;
    public GameObject first, second;
    public int total, value;
    public GameObject mainmenu, gameplay;
    public LoadScene load;
    public TMP_Dropdown layout, category;
    public AudioClip flip, match, missmatch, gameover;
    public AudioSource audio;


    public void SetupGrid()
    {
        score_int = 0;
        attempts_int = 0;
        score.text = "Score: " + score_int;
        attempts.text = "Score: " + attempts_int;
        highscore.text = "High Score: " + PlayerPrefs.GetInt("Highest") + "";
        total = 0;
        entire_cards = new List<GameObject>();
        gameplay.SetActive(true);
        mainmenu.SetActive(false);
        if (layout.value == 0)
        {
            rows = 2; cols = 2;
        }
        else if (layout.value == 1)
        {
            rows = 2; cols = 3;
        }
        else if (layout.value == 2)
        {
            rows = 2; cols = 5;
        }

        if (category.value == 0)
        {
            categories = categories1;
            card_images = card_images1;
            value = 0;
        }
        else if (category.value == 1)
        {
            categories = categories2;
            card_images = card_images2;
            value = 1;
        }

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }


        int gridSize = rows * cols;
        if (gridSize > categories.Count * 2)
        {
            Debug.LogWarning("Not enough unique categories for the grid size.");
            return;
        }

        List<string> categoryPairs = new List<string>();


        int numPairs = gridSize / 2;
        for (int i = 0; i < numPairs; i++)
        {
            categoryPairs.Add(categories[i % categories.Count]);
            categoryPairs.Add(categories[i % categories.Count]);
        }

        ShuffleList(categoryPairs);

        int num_of_card = 0;
        Vector3 startPosition = gridParent.position;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                num_of_card++;
                float xPos = startPosition.x + (col * (columnSpacing));
                float yPos = startPosition.y - (row * (rowSpacing));

                Vector3 buttonPosition = new Vector3(xPos, yPos, startPosition.z);

                GameObject newButton = Instantiate(buttonPrefab, buttonPosition, Quaternion.identity, gridParent);
                newButton.name = categoryPairs[num_of_card - 1];
                entire_cards.Add(newButton);

                newButton.GetComponent<Card>().myValue = categoryPairs[num_of_card - 1];
                newButton.GetComponent<Card>().myImage = GetSpriteByCategory(categoryPairs[num_of_card - 1]);
                newButton.GetComponent<Button>().image.sprite = newButton.GetComponent<Card>().myImage;
                Button buttonComponent = newButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() => OnButtonClick(newButton.name, newButton.GetComponent<Button>()));
            }
        }
        total = entire_cards.Count / 2;
        Invoke("ShowDefaultImage", 0.5f);
    }

    public void LoadLevel()
    {
        score_int = 0;
        attempts_int = 0;
        score_int = load.score;
        attempts_int = load.attempts;
        score.text = "Score: " + score_int;
        attempts.text = "Score: " + attempts_int;
        highscore.text = "High Score: " + PlayerPrefs.GetInt("Highest") + "";
        total = load.total;
        entire_cards = new List<GameObject>();
        gameplay.SetActive(true);
        mainmenu.SetActive(false);
        if (load.value == 0)
        {
            categories = categories1;
            card_images = card_images1;
        }
        else if (load.value == 1)
        {
            categories = categories2;
            card_images = card_images2;
        }
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < load.assignedcards.Count; i++)
        {
            GameObject newCard = Instantiate(buttonPrefab, load.assignedcardspos[i], Quaternion.identity, gridParent);
            newCard.name = load.assignedcards[i];
            entire_cards.Add(newCard);

            newCard.GetComponent<Card>().myValue = load.assignedcards[i];
            newCard.GetComponent<Card>().myImage = GetSpriteByCategory(load.assignedcards[i]);
            newCard.GetComponent<Button>().image.sprite = newCard.GetComponent<Card>().myImage;

            newCard.GetComponent<Image>().sprite = newCard.GetComponent<Card>().myImage;

            Button buttonComponent = newCard.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => OnButtonClick(newCard.name, newCard.GetComponent<Button>()));
        }

        Invoke("ShowDefaultImage", 0.5f);
    }

    public void ShowDefaultImage()
    {
        for (int i = 0; i < entire_cards.Count; i++)
        {
            entire_cards[i].GetComponent<Image>().sprite = default_sprite;
        }
    }

    private void ShuffleList(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private Sprite GetSpriteByCategory(string category)
    {
        int index = categories.IndexOf(category);
        if (index != -1 && index < card_images.Count)
        {
            return card_images[index];
        }
        else
        {
            Debug.LogWarning("Sprite for category " + category + " not found!");
            return null;
        }
    }

    void OnButtonClick(string name, Button btn)
    {
        Debug.Log($"Button clicked! Category: {name}");
        audio.clip = flip;
        btn.gameObject.transform.DORotate(new Vector3(0, 180, 0), 0.5f, RotateMode.Fast).OnUpdate(() =>
        {
            float yRotation = btn.gameObject.transform.eulerAngles.y;

            if (yRotation >= 85 && yRotation <= 95) 
            {
                Debug.Log("Reached 90 degrees");
                btn.image.sprite = btn.GetComponent<Card>().myImage;
            }
        });

        click++;
        if (click == 1)
        {
            first = btn.gameObject;
        }
        else if (click == 2)
        {
            attempts_int++;
            attempts.text = "Attempts: " + attempts_int;
            second = btn.gameObject;
            Invoke("check", 1f);
        }
    }


    void check()
    {
        if (first.name == second.name)
        {
            audio.clip = match;
            score_int++;
            score.text = "Score: " + score_int;
            first.SetActive(false);
            second.SetActive(false);
            first = second = null;
            click = 0;
            if(score_int == total)
            {
                audio.clip = gameover;
                Debug.Log("Game Finish");
                if (PlayerPrefs.HasKey("Highest"))
                {
                    if (attempts_int < PlayerPrefs.GetInt("Highest"))
                    {
                        PlayerPrefs.SetInt("Highest", attempts_int);
                        highscore.text = "High Score: " + PlayerPrefs.GetInt("Highest");
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("Highest", attempts_int);
                    highscore.text = "High Score: " + PlayerPrefs.GetInt("Highest");
                }
                congrats.gameObject.SetActive(true);
                congrats.text = "You made it!!";
                Invoke("GameOver", 1f);
            }
        }
        else
        {
            audio.clip = missmatch;
            first.gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.Fast);
            second.gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.5f, RotateMode.Fast).OnUpdate(() =>
            {
                float yRotation = second.gameObject.transform.eulerAngles.y;

                // Normalize rotation to ensure values are between -180 and 180
                if (yRotation > 180)
                    yRotation -= 360;

                Debug.Log("Rotation: " + yRotation);

                if (yRotation >= -95 && yRotation <= -85) // When rotation is around -90 degrees
                {
                    Debug.Log("Reached -90 degrees");

                    // Change sprites for both buttons
                }
            });
            Invoke("ChangeSprite", 0.15f);
            click = 0;
        }
    }

     void GameOver()
     {
        congrats.gameObject.SetActive(false);
        mainmenu.SetActive(true);
        gameplay.SetActive(false);
     }

    void ChangeSprite()
    {
        first.GetComponent<Image>().sprite = default_sprite;
        second.GetComponent<Image>().sprite = default_sprite;
        first = second = null;
    }

    public void SaveScene()
    {
        load.assignedcards.Clear();
        load.assignedcardspos.Clear();
        load.value = value;
        load.attempts = attempts_int;
        load.total = total;
        load.score = score_int;
        for(int i = 0; i < entire_cards.Count; i++)
        {
            if (entire_cards[i].activeSelf)
            {
                load.assignedcards.Add(entire_cards[i].name);
                load.assignedcardspos.Add(entire_cards[i].transform.position);
            }
        }

        mainmenu.SetActive(true);
        gameplay.SetActive(false);
    }
}
