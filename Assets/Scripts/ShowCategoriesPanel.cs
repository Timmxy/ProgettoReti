using UnityEngine;

public class ShowCategoriesPanel : MonoBehaviour
{
    [SerializeField] private Transform _parentPanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivatePanel(string name)
    {
        foreach (Transform child in _parentPanel)
        {
            if (child.name.Equals(name))
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
