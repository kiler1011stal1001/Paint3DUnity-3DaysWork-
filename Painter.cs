using UnityEngine;

public class Painter : MonoBehaviour
{
    public Camera mainCamera;
    public Shader drawShader;
    private RenderTexture renderTexture;
    private Material drawMaterial;
    private Material myMaterial;
    private RaycastHit hit;

    public Color drawColor = Color.red;
    public float brushSize = 0.01f; 

    void Start()
    {
        if (!drawShader)
        {
            Debug.LogError("Draw Shader is not assigned!");
            return;
        }

        renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGBFloat); 
        renderTexture.Create();

        drawMaterial = new Material(drawShader);
        drawMaterial.SetColor("_Color", drawColor);
        myMaterial = GetComponent<MeshRenderer>().material;
        myMaterial.mainTexture = renderTexture;

        if (GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>();
            Debug.Log("MeshCollider added to the object.");
        }

        Debug.Log("Initialization completed");

        LoadTexture();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Ray: " + ray);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                if (hit.collider.GetComponent<MeshCollider>() == null)
                {
                    Debug.LogWarning("Object does not have a MeshCollider.");
                    return;
                }

                Vector2 textureCoord = hit.textureCoord;
                Debug.Log("Texture Coordinates before scaling: " + textureCoord);

                textureCoord.x *= renderTexture.width;
                textureCoord.y *= renderTexture.height;
                Debug.Log("Texture Coordinates after scaling: " + textureCoord);

                drawMaterial.SetVector("_Coordinate", new Vector4(textureCoord.x, textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_BrushSize", brushSize);
                drawMaterial.SetColor("_Color", drawColor);

                RenderTexture temp = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(renderTexture, temp);
                Graphics.Blit(temp, renderTexture, drawMaterial);
                RenderTexture.ReleaseTemporary(temp);

                Debug.Log("Drawing completed");
            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }
    }

    public void SetBrushColor(Color color)
    {
        drawColor = color;
        drawMaterial.SetColor("_Color", drawColor);
    }

    public void SetBrushSize(float size)
    {
        brushSize = size;
    }

    public void SaveTexture()
    {
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "/savedTexture.png", bytes);

        Debug.Log("Texture saved at: " + Application.persistentDataPath + "/savedTexture.png");
    }

    public void LoadTexture()
    {
        string path = Application.persistentDataPath + "/savedTexture.png";
        if (System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            texture.LoadImage(bytes);

            Graphics.Blit(texture, renderTexture);
            Debug.Log("Texture loaded from: " + path);
        }
        else
        {
            Debug.LogWarning("No saved texture found at: " + path);
        }
    }

    public RenderTexture GetRenderTexture()
    {
        return renderTexture;
    }
}
