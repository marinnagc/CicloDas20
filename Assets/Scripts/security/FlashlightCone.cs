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

    // Controle de rotação / suavização
    private float rotationVelocity = 0f;
    [SerializeField] private float smoothTime = 0.08f; // tempo de suavização da rotação

    void Awake()
    {
        Debug.Log("[FlashlightCone] Awake iniciado");

        // Adiciona componentes necessários
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        Debug.Log("[FlashlightCone] MeshFilter e MeshRenderer adicionados");

        // Cria material para o cone
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = coneColor;
        Debug.Log($"[FlashlightCone] Material criado com cor: {coneColor}");

        // Configurações de renderização
        meshRenderer.sortingLayerName = "Default";
        meshRenderer.sortingOrder = 5;

        // Pega referência ao AiAgente
        aiAgente = GetComponentInParent<AiAgente>();
        if (aiAgente == null)
        {
            Debug.LogError("[FlashlightCone] ERRO: Não encontrou AiAgente no pai!");
        }
        else
        {
            Debug.Log("[FlashlightCone] AiAgente encontrado com sucesso!");
        }
    }

    void Update()
    {
        if (TimerController.Instance == null)
        {
            Debug.LogWarning("[FlashlightCone] TimerController.Instance é NULL!");
            return;
        }

        int horaAtual = TimerController.Instance.GetHoraInteira();
        Debug.Log($"[FlashlightCone] Hora atual: {horaAtual} | Deve mostrar entre {horaInicioVisao}h e {horaFimVisao}h");

        // Só mostra o cone das 20h às 22h
        if (horaAtual >= horaInicioVisao && horaAtual < horaFimVisao)
        {
            Debug.Log("[FlashlightCone] Dentro do horário - ATIVANDO mesh");
            meshRenderer.enabled = true;

            // Atualiza direção antes de desenhar
            UpdateDirectionRotation();

            UpdateConeMesh();
            UpdateConeColor();
        }
        else
        {
            Debug.Log("[FlashlightCone] Fora do horário - DESATIVANDO mesh");
            meshRenderer.enabled = false;
        }
    }
    void UpdateDirectionRotation()
    {
        if (transform.parent == null || aiAgente == null) return;

        Transform player = aiAgente.GetPlayer();
        bool isChasing = aiAgente.IsChasing();

        // Pegamos o ângulo atual em espaço LOCAL (z local)
        float currentLocalZ = transform.localEulerAngles.z;

        float desiredLocalAngleDeg = currentLocalZ; // fallback: manter o atual

        if (isChasing && player != null)
        {
            // Vetor posição do player em relação ao pai, transformado para o espaço LOCAL do pai.
            // Assim lidamos corretamente com escala negativa/flip no pai.
            Vector3 localPlayerPos = transform.parent.InverseTransformPoint(player.position);
            Vector3 localDir = (localPlayerPos - transform.localPosition); // direção no espaço local do pai

            // Se a direção for quase zero, mantemos o ângulo atual
            if (localDir.sqrMagnitude > 0.0001f)
            {
                desiredLocalAngleDeg = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;
            }
        }
        else
        {
            // Opcional: quando não perseguindo, você pode fazer com que o cone acompanhe a "facing" do inimigo.
            // Se o AiAgente tiver um método para dizer pra que lado está virando (ex: GetFacingAngleLocal()),
            // use-o aqui para manter a lanterna alinhada quando não perseguindo.
        }

        // Suaviza a rotação (em espaço local)
        float smoothLocalZ = Mathf.SmoothDampAngle(currentLocalZ, desiredLocalAngleDeg, ref rotationVelocity, smoothTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, smoothLocalZ);
    }
    void UpdateConeMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3];

        // Origem do cone (local)
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
