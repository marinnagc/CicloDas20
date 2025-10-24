using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCatraca : Interactable
{
    public GameObject CatracaUpFechada, CatracaUpAberta, CatracaDownFechada, CatracaDownAberta;
    public Collider2D CatracaUpCollider;
    public Item OfficeAccessCard;

    public override void Interact()
    {
        Debug.Log("Interagiu com a catraca");
        if (Inventory.instance.Contains(OfficeAccessCard.name)){
            Debug.Log("Possui o cartão");
            CatracaUpFechada.SetActive(false);
            CatracaUpAberta.SetActive(true);
            CatracaDownFechada.SetActive(false);
            CatracaDownAberta.SetActive(true);
            CatracaUpCollider.enabled = false;
        }
    }
}
