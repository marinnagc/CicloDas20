using UnityEngine;

public class FlashlightCone : MonoBehaviour
{
    [Header("Configurações da Lanterna")]
    [SerializeField] private float coneDistance = 5f; // Distância do cone (menor que antes)
    [SerializeField] private float coneAngle = 90f; // Ângulo do cone
    [SerializeField] private int coneSegments = 20; // Quantos segmentos para fazer o cone suave

    [Header("Aparência")]
    [SerializeField] private Color coneColor = new Color(1f, 1f, 0.8f, 0.3f); // Amarelo semi-transparente
    [SerializeField] private Color coneColorChasing = new Color(1f, 0.3f, 0.3f, 0.4f); // Vermelho quando perseguindo

    [Header("Horários")]
    [SerializeField] private int horaInicioVisao = 20; // Quando a lanterna aparece
    [SerializeField] private int horaFimVisao = 22;

    // Componentes
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private AiAgente aiAgente;

    void Awake()
    {
        // Adiciona componentes necessários
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Cria material para o cone
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = coneColor;

        // Configurações para renderizar corretamente
        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 5; // Renderiza por cima de outras coisas

        // Pega referência ao AiAgente
        aiAgente = GetComponentInParent<AiAgente>();

        if (aiAgente == null)
        {
            Debug.LogError("[FlashlightCone] Não encontrou AiAgente no pai! Certifique-se que este GameObject é filho do Security.");
        }
    }

    void Update()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Só mostra o cone das 20h às 22h
        if (horaAtual >= horaInicioVisao && horaAtual < horaFimVisao)
        {
            meshRenderer.enabled = true;
            UpdateConeMesh();
            UpdateConeColor();

            // Debug para ver a direção
            if (Time.frameCount % 60 == 0) // A cada segundo
            {
                Debug.Log($"[Flashlight] Parent Scale X: {transform.parent.localScale.x} | Olhando: {(transform.parent.localScale.x > 0 ? "DIREITA" : "ESQUERDA")}");
            }
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void UpdateConeMesh()
    {
        Mesh mesh = new Mesh();

        // Vértices do cone
        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3];

        // Ponto de origem (centro do cone)
        vertices[0] = Vector3.zero;

        // Determina a direção base do cone
        // Se scale.x > 0 = olhando DIREITA (0°)
        // Se scale.x < 0 = olhando ESQUERDA (180°)
        float baseAngle = transform.parent.localScale.x > 0 ? 0f : 180f;

        // Calcula os pontos ao redor do cone
        float startAngle = baseAngle - (coneAngle / 2f);
        float angleStep = coneAngle / coneSegments;

        for (int i = 0; i <= coneSegments; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            float rad = currentAngle * Mathf.Deg2Rad;

            // Calcula posição do vértice
            Vector3 direction = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0
            );

            vertices[i + 1] = direction * coneDistance;

            // Cria triângulos
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

    void UpdateConeColor()
    {
        // Muda cor baseado se está perseguindo ou não
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