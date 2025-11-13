using UnityEngine;

public class FlashlightCone : MonoBehaviour
{
    [Header("Configurações da Lanterna")]
    [SerializeField] private float coneDistance = 5f; // Distância do cone
    [SerializeField] private float coneAngle = 90f;   // Ângulo do cone
    [SerializeField] private int coneSegments = 20;   // Segmentos para suavizar o cone

    [Header("Aparência")]
    [SerializeField] private Color coneColor = new Color(1f, 1f, 0.8f, 0.3f);      // Amarelo semi-transparente
    [SerializeField] private Color coneColorChasing = new Color(1f, 0.3f, 0.3f, 0.4f); // Vermelho quando perseguindo

    [Header("Horários")]
    [SerializeField] private int horaInicioVisao = 20; // Quando a lanterna aparece
    [SerializeField] private int horaFimVisao = 22;

    // Componentes
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private AiAgente aiAgente;

    // Controle de rotação
    private float currentBaseAngle = 0f;

    void Awake()
    {
        // Adiciona componentes necessários
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Cria material para o cone
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = coneColor;

        // Configurações de renderização
        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 5;

        // Pega referência ao AiAgente
        aiAgente = GetComponentInParent<AiAgente>();
        if (aiAgente == null)
            Debug.LogError("[FlashlightCone] Não encontrou AiAgente no pai! Certifique-se que este GameObject é filho do Security.");
    }

    void Update()
    {
        if (TimerController.Instance == null) return;

        int horaAtual = TimerController.Instance.GetHoraInteira();

        // Só mostra o cone das 20h às 22h
        if (horaAtual >= horaInicioVisao && horaAtual < horaFimVisao)
        {
            meshRenderer.enabled = true;

            // Atualiza direção antes de desenhar
            UpdateDirectionRotation();

            UpdateConeMesh();
            UpdateConeColor();
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Gira o cone da lanterna conforme a direção do personagem.
    /// </summary>
    void UpdateDirectionRotation()
    {
        if (transform.parent == null) return;

        float parentScaleX = transform.parent.localScale.x;

        // Se estiver olhando para a direita (escala positiva)
        //if (parentScaleX > 0)
        //{
        //    // Rotação zero (cone pra direita)
        //    currentBaseAngle = 0f;
        //    transform.localRotation = Quaternion.Euler(0, 0, 0);
        //}
        //else if (parentScaleX < 0)
        //{
        //    // Rotaciona 180° no eixo Z para virar pra esquerda
        //    currentBaseAngle = 180f;
        //    transform.localRotation = Quaternion.Euler(0, 0, 180f);
        //}
    }

    void UpdateConeMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3];

        // Origem do cone
        vertices[0] = Vector3.zero;

        float startAngle = -coneAngle / 2f;
        float angleStep = coneAngle / coneSegments;

        for (int i = 0; i <= coneSegments; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            float rad = currentAngle * Mathf.Deg2Rad;

            // Direção no eixo local (sempre pra frente)
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

            vertices[i + 1] = dir * coneDistance;

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
            meshRenderer.material.color = coneColorChasing;
        else
            meshRenderer.material.color = coneColor;
    }
}
