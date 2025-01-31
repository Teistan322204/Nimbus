using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
	//Variable donde se le colocan los layers con los que se colisiona
	[Header("Layers")]
	public LayerMask groundLayer;
	
	//Booleano para saber si está en el suelo o en una pared(se va a actualizar cada frame)
	[Space]
	public bool onGround;
	public bool onWall;
	public bool onRightWall;
	public bool onLeftWall;
	public int wallSide;
	
	//Burbuja de colisión alrededor del personaje
	[Space]
	[Header("Collision")]
	public float collisionRadius = 0.25f;
	//Esto va a cambiar depende del sprite
	public Vector2 bottomOffset,rightOffset, leftOffset;

	void Start()
	{
        
	}


	// Revisa si está tocando los layer elegidos cada frame, según la burbuja
    void Update()
	{
		//Revisa el suelo
		onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
		//Revisa si está pegado en la pared derecha
		onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
		//Revisa si está pegado a la pared izquierda
		onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
	    
		//Revisa si está en alguna pared
		onWall = onRightWall || onLeftWall;
		//Revisa CUAL pared
	    wallSide = onRightWall ? -1 : 1;   
    }
}
