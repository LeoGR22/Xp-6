using UnityEngine;
using UnityEngine.UI;

public class ColorRandomizer : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    private static readonly int Color1 = Shader.PropertyToID("_Color1");
    private static readonly int Color2 = Shader.PropertyToID("_Color2");
    private static readonly int Color3 = Shader.PropertyToID("_Color3");

    public void RandomizeColors()
    {
        if (targetImage != null && targetImage.material != null)
        {
            Material mat = targetImage.material;
            mat.SetColor(Color1, GetRandomColor());
            mat.SetColor(Color2, GetRandomColor());
            mat.SetColor(Color3, GetRandomColor());
        }
        else
        {
            Debug.LogWarning("Material ou Image não atribuídos!");
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
