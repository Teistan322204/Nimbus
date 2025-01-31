using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
	//Variable donde se le colocan los layers con los que se colisiona
	[Header("Layers")]
	public LayerMask groundLayer;
	
	//Booleano para saber si está en el suelo(se va a actualizar cada frame)
	[Space]
	public bool onGround;
	
	//Burbuja de colisión alrededor del personaje
	[Space]
	[Header("Collision")]
	public float collisionRadius = 0.25f;
	//Esto va a cambiar depende del sprite
	public Vector2 bottomOffset;

	void Start()
	{
        
	}


	// Revisa si está tocando los layer elegidos cada frame, según la burbuja
    void Update()
    {
	    onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
    }
}
