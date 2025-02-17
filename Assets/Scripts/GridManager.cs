using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform gridParent;
    public float rowSpacing = 10f;
    public float columnSpacing = 10f;
    public int rows, cols;
    public List<string> categories;   
    public List<Sprite> card_images;
    private List<string> assignedCategories = new List<string>();  

    private void Start()
    {
        SetupGrid();
    }

    public void SetupGrid()
    {

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
                newButton.name = categoryPairs[num_of_card - 1];y

                newButton.GetComponent<Card>().myValue = categoryPairs[num_of_card - 1];
                newButton.GetComponent<Card>().myImage = GetSpriteByCategory(categoryPairs[num_of_card - 1]);

                // Add click listener
                Button buttonComponent = newButton.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() => OnButtonClick(newButton.name)); 
            }
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

    // Button click handler
    void OnButtonClick(string name)
    {
        Debug.Log($"Button clicked! Category: {name}"); 
    }
}
