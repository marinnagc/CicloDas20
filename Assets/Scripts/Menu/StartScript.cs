using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public Button startButton;
    private GameObject dialogueBox;
    // Start is called before the first frame update
    void Start()
    {
        string[] sentences = new string[5];
        sentences[0] = "Uau, eu não acredito que isso aconteceu! Como uma simples falha em um experimento pode causar tanto caos? Eu preciso fazer algo para ajudar!";
        sentences[1] = "As portas do laboratório foram trancadas automaticamente para evitar a disseminação da contaminação. Vou ter que chegar ao reator principal de qualquer jeito.";
        sentences[2] = "Terei de descer até o reator principal, mas para isso, vou ter que passar por todos os andares do laboratório. E para piorar, o sistema de segurança está ativado. Isso fez com que todas as portas fossem trancadas. Eu vou ter que encontrar uma forma de desativar cada passagem.";
        sentences[3] = "Da última vez que estava do laboratório, me lembro de ver diversas pessoas com crachás de identificação. Talvez eu possa encontrar um deles e usar para desativar o sistema de segurança de cada andar. Vale a pena tentar.";
        sentences[4] = "Espero que eu consiga chegar ao reator principal a tempo de evitar uma catástrofe... Vamos lá.";

        string name = "EU";

        dialogueBox = GameObject.FindGameObjectWithTag("MessageBox");
        if (dialogueBox != null)
        {
            Debug.Log("MessageBox found!");
            dialogueBox.GetComponent<MessagesScript>().SetNewDialogue(sentences, name);
        }
        else
        {
            Debug.Log("MessageBox not found!");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
