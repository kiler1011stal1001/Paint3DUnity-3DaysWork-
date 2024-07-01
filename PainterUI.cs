using UnityEngine;
using UnityEngine.UI;

public class PainterUI : MonoBehaviour
{
    public Painter painter;
    public Slider brushSizeSlider;
    public Button colorPickerButton;
    public Image selectedColorImage;
    public Button saveButton;
    public Button loadButton;
    public Button clearButton;

    void Start()
    {      
        brushSizeSlider.value = painter.brushSize;
        brushSizeSlider.onValueChanged.AddListener(delegate { OnBrushSizeChanged(); });

        selectedColorImage.color = painter.drawColor;
        colorPickerButton.onClick.AddListener(delegate { OnColorPickerClicked(); });

        saveButton.onClick.AddListener(delegate { painter.SaveTexture(); });
        loadButton.onClick.AddListener(delegate { painter.LoadTexture(); });
        clearButton.onClick.AddListener(delegate { ClearSurface(); });
    }

    void OnBrushSizeChanged()
    {
        painter.SetBrushSize(brushSizeSlider.value);
    }

    void OnColorPickerClicked()
    {
        Color selectedColor = new Color(Random.value, Random.value, Random.value); 
        painter.SetBrushColor(selectedColor);
        selectedColorImage.color = selectedColor;
    }

    void ClearSurface()
    {
        RenderTexture renderTexture = painter.GetRenderTexture();
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }
}
