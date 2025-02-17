using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridParent;
    public float rowSpacing = 10f;
    public float columnSpacing = 10f;
    public int rows, cols;
    public List<string> categories;
    public List<Sprite> card_images;
    public List<GameObject> entire_cards;
    public List<string> assignedCategories = new List<string>();
    public Sprite default_sprite;
    public TextMeshProUGUI score, attempts, highscore;
    public int score_int, attempts_int, click;
    public GameObject first, second;
    public int total;
    public GameObject mainmenu, gameplay;
    public LoadScene load;
    public TMP_Dropdown layout;

    private void OnEnable()
    {
        score_int = 0;
        attempts_int = 0;
        score.text = "Score: " + score_int;
        attempts.text = "Score: " + attempts_int;
        highscore.text = "High Score: " + PlayerPrefs.GetInt("Highest") + "";
    }

    public void SetupGrid()
    {
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
        Invoke("ShowDefaultImage", 0.5f);
    }

    public void LoadLevel()
    {
        entire_cards = new List<GameObject>();
        gameplay.SetActive(true);
        mainmenu.SetActive(false);
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
        total = entire_cards.Count / 2;
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
        btn.image.sprite = btn.GetComponent<Card>().myImage;
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
            Invoke("check", 0.5f);
        }
    }


    void check()
    {
        if (first.name == second.name)
        {
            score_int++;
            score.text = "Score: " + score_int;
            first.SetActive(false);
            second.SetActive(false);
            first = second = null;
            click = 0;
            if(score_int == total)
            {
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

                mainmenu.SetActive(true);
                gameplay.SetActive(false);
            }
        }
        else
        {
            first.GetComponent<Image>().sprite = default_sprite;
            second.GetComponent<Image>().sprite = default_sprite;
            first = second = null;
            click = 0;
        }
    }

    public void SaveScene()
    {
        load.assignedcards.Clear();
        load.assignedcardspos.Clear();
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
