using UnityEngine;

public class FlashlightConeRotation : MonoBehaviour
{
    [Header("Configurações da Lanterna")]
    [SerializeField] private float coneDistance = 5f;
    [SerializeField] private float coneAngle = 90f;
    [SerializeField] private int coneSegments = 20;

    [Header("Aparência")]
    [SerializeField] private Color coneColor = new Color(1f, 1f, 0.8f, 0.3f);
    [SerializeField] private Color coneColorChasing = new Color(1f, 0.3f, 0.3f, 0.4f);

    [Header("Horários")]
    [SerializeField] private int horaInicioVisao = 20;
    [SerializeField] private int horaFimVisao = 22;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private AiAgente aiAgente;
    private float lastScaleX = 1f;

    void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = coneColor;

        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 5;

        aiAgente = GetComponentInParent<AiAgente>();

        if (aiAgente == null)
        {
            Debug.LogError("[FlashlightCone] Não encontrou AiAgente no pai!");
        }

        // Cria o mesh uma vez (sempre apontando para DIREITA)
        CreateConeMesh();
    }

    void Update()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        if (horaAtual >= horaInicioVisao && horaAtual < horaFimVisao)
        {
            meshRenderer.enabled = true;
            UpdateConeDirection();
            UpdateConeColor();
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void CreateConeMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3];

        vertices[0] = Vector3.zero;

        // Cria cone apontando para DIREITA (0°)
        float startAngle = -(coneAngle / 2f);
        float angleStep = coneAngle / coneSegments;

        for (int i = 0; i <= coneSegments; i++)
        {
            float angle = startAngle + (angleStep * i);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            );

            vertices[i + 1] = direction * coneDistance;

            if (i < coneSegments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void UpdateConeDirection()
    {
        if (transform.parent == null) return;

        float parentScaleX = transform.parent.localScale.x;

        // Se a direção mudou, atualiza a rotação
        if (Mathf.Sign(parentScaleX) != Mathf.Sign(lastScaleX))
        {
            lastScaleX = parentScaleX;
            Debug.Log($"[Flashlight] Direção mudou! Scale X: {parentScaleX}");
        }

        // Rotaciona o flashlight baseado na direção
        if (parentScaleX > 0)
        {
            // Olhando DIREITA - sem rotação
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Olhando ESQUERDA - rotaciona 180°
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }

    void UpdateConeColor()
    {
        if (aiAgente != null && aiAgente.IsChasing())
        {
            meshRenderer.material.color = coneColorChasing;
        }
        else
        {
            meshRenderer.material.color = coneColor;
        }
    }
}