using UnityEngine;

public enum ColorOption
{
    None,
    Red,
    Green,
    Blue
}

public class ObstacleController : MonoBehaviour
{
    public ColorOption selectedColorOption;

    void Start()
    {
        ApplySelectedColor();
    }
    
    void ApplySelectedColor()
    {
        Color selectedColor;

        if (selectedColorOption == ColorOption.None)
        {
            selectedColor = GetRandomColor();
        }
        else
        {
            selectedColor = GetColorOption(selectedColorOption);
        }

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = selectedColor;
        }
    }

    Color GetColorOption(ColorOption option)
    {
        switch (option)
        {
            case ColorOption.Red:
                return Color.red;
            case ColorOption.Green:
                return Color.green;
            case ColorOption.Blue:
                return Color.blue;
            case ColorOption.None:
                return GetRandomColor();
            default:
                return Color.white;
        }
    }

    Color GetRandomColor()
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, 3);
        return GetColorOption((ColorOption)randomIndex);
    }
}