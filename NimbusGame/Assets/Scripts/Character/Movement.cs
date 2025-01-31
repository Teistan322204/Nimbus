using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
	//Hay que obtener cada frame si está tocando el suelo
	private Collision coll;
	//Modificar las fisicas y el movimiento
	private Rigidbody2D rb;
	
	//Lo que dice... Stats
	[Space]
	[Header("Stats")]
	public float speed = 10;
	public float jumpForce = 70;
	
	//Variables para verificar la situación del jugador.
	[Space]
	[Header("Booleans")]
	public bool canMove = true;
	
	
	void Start()
	{
		//Obtiene el rigidBody del jugador
		rb = GetComponent<Rigidbody2D>();
		//Obtiene quien actualiza las colisiones
		coll = GetComponent<Collision>();
	}
	
	void Update()
	{
		//Obtiene la flecha que está presionando para moverse
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		//Valores de movimiento dentro del Vector2
		Vector2 dir = new Vector2(x, y);
		
		//Llamada a la función que mueve al personaje lo indicado
		Walk(dir);
		
		if (Input.GetButtonDown("Jump"))
		{

			if (coll.onGround)
				Jump(Vector2.up);
		}
	}
	
	//Función para producir el movimiento
	private void Walk(Vector2 dir)
	{
		//Si algo lo detiene, amonos de aquí
		if (!canMove)
			return;
		//Solo es el movimiento horizonta, el salto va aparte
		rb.velocity = (new Vector2(dir.x * speed, rb.velocity.y));
	}
	
	//Pide la dirección para cuando haya salto en las paredes
	private void Jump(Vector2 dir)
	{
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.velocity += dir * jumpForce;
		
	}
}
