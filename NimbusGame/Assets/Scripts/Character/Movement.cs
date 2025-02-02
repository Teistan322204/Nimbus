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
	//Para cambiar la animación
	private Animator animator;
	
	//Lo que dice... Stats
	[Space]
	[Header("Stats")]
	public float speed = 6;
	public float jumpForce = 15;
	public float slideSpeed = 5;
	public float wallJumpLerp = 10;
	public float dashSpeed = 20;
	
	//Variables para verificar la situación del jugador.
	[Space]
	[Header("Booleans")]
	//Para los saltos en pared
	public bool canMove = true;
	public bool wallGrab;
	public bool wallJumped;
	public bool isDashing;
	
	private bool groundTouch = true;
	private bool hasDashed = false;

	public int side = 1;
	
	void Start()
	{
		//Obtiene el rigidBody del jugador
		rb = GetComponent<Rigidbody2D>();
		//Obtiene quien actualiza las colisiones
		coll = GetComponent<Collision>();
		//Obtiene el elemento que anima
		animator = GetComponent<Animator>();
	}
	//En lugar de llamarse cada frame, se llama de forma fija.
	private void FixedUpdate()
	{
		//Cambia las variables de la animación, para activar la animación
		animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
	}
	
	void Update()
	{
		//Obtiene la flecha que está presionando para moverse
		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		//Valores directos sin decimales, solo la dirección
		float xRaw = Input.GetAxisRaw("Horizontal");
		float yRaw = Input.GetAxisRaw("Vertical");
		//Valores de movimiento dentro del Vector2
		Vector2 dir = new Vector2(x, y);
		
		//Llamada a la función que mueve al personaje lo indicado
		Walk(dir);
		
		//Está en pared? presionando agarrarse? se puede mover?
		if (coll.onWall && Input.GetButton("Fire2") && canMove)
		{
			//Está agarrando un muro
			wallGrab = true;
		}
		//solo está presionando agarrar? o NO está pegado pared? o No se puede mover?
		if (Input.GetButtonUp("Fire2") || !coll.onWall || !canMove)
		{
			//Entonces no se está agarrando
			wallGrab = false;
		}
		
		//Está en el suelo? y No está dasheando
		if (coll.onGround && !isDashing)
		{
			//Entonces 
			wallJumped = false;
			GetComponent<BetterJumping>().enabled = true;
		}
		//Se agarra a la pared y además no dashea?
		if (wallGrab && !isDashing)
		{
			//Quedate pegado a la pared
			rb.gravityScale = 0;
			
			//No me hagas cosas raras
			if(x > .2f || x < -.2f)
				rb.velocity = new Vector2(rb.velocity.x, 0);
			
			//Escala lentamente, baja rapido
			float speedModifier = y > 0 ? .5f : 1;

			rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
		}
		else
		{	
			//Si no, no te quedes pegado en la pared
			rb.gravityScale = 3;
		}

		//Salta presionando Espacio
		if (Input.GetButtonDown("Jump"))
		{
			//Si está en el suelo, salta hacia arriba
			if (coll.onGround)
				Jump(Vector2.up, false);
			//Si estás junto a una pared, y no en el suelo haz un walljump
			if (coll.onWall && !coll.onGround)
				WallJump();
		}
		//Dashear con C. Si no tiene el cooldown activo
		if (Input.GetButtonDown("Fire3") && !hasDashed)
		{
			//Dashea con estos valores enteros
			if(xRaw != 0 || yRaw != 0)
				Dash(xRaw, yRaw);
		}
		//SI está en el suelo (antes estaba en el aire), hay que cambiar las variables
		if (coll.onGround && !groundTouch)
		{
			GroundTouch();
			groundTouch = true;
		}
		//Si no está tocando el suelo, hay que cambiar la variable groundTouch
		if(!coll.onGround && groundTouch)
		{
			groundTouch = false;
		}

		//Si está agarrandose a la pared, o está wallJumpeando
		if (wallGrab || !canMove)
			return;
		
		//Cambiar la dirección del sprite y de la variable side
		if(x > 0)
		{
			side = -1;
			Vector3 scale = transform.localScale;
			scale.x = side;
			transform.localScale = scale;
		}
		if (x < 0)
		{
			//Se gira el sprite
			side = 1;
			Vector3 scale = transform.localScale;
			scale.x = side;
			transform.localScale = scale;
		}


	}
	//Despues de caer debes resetear la variables
	void GroundTouch()
	{
		//No has dasheado, caíste
		hasDashed = false;
		isDashing = false;

	}
	//Dash moment. Necesitamos la dirección
	private void Dash(float x, float y)
	{
		//En efecto está dasheando
		hasDashed = true;
		
		//La velocidad del personaje debe ser cero.
		rb.velocity = Vector2.zero;
		//Saquemos la dirección elegida
		Vector2 dir = new Vector2(x, y);
		//Muevete en la dirección elegida la cantidad de dash que tengas
		rb.velocity += dir.normalized * dashSpeed;
		//Cooldown
		StartCoroutine(DashWait());
	}
	//Función para el cooldown
	IEnumerator DashWait()
	{
		//Se quita la gravedad para que te puedas mover en direcciones
		rb.gravityScale = 0;
		//Esto puede interferir con el dash, por lo que se desactiva momentaneamente
		GetComponent<BetterJumping>().enabled = false;
		//También puede que haya saltado de una pared, por si acaso...
		wallJumped = true;
		//Está en el estado de dasheo
		isDashing = true;
		
		//Pausa este proceso temporalmente
		yield return new WaitForSeconds(.3f);

		//Regresemos a la normalidad. Nada pasó aqui...
		rb.gravityScale = 3;
		GetComponent<BetterJumping>().enabled = true;
		wallJumped = false;
		isDashing = false;
		hasDashed = false;
	}

	//Dasheaste en el suelo? Toma cooldown
	IEnumerator GroundDash()
	{
		yield return new WaitForSeconds(.15f);
		if (coll.onGround)
			hasDashed = false;
	}
	//Salta en paredes
	private void WallJump()
	{
		//Invierte la dirección en la que te vas a mover
		if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
		{
			side *= -1;
		}
		//No podrás escalar usando saltos
		StopCoroutine(DisableMovement(0));
		StartCoroutine(DisableMovement(.1f));
		//Obten la dirección a moverte dependiendo de la pared en la que estés
		Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
		//Salta, pero en esta direccíon diagonal
		Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);
		//En efecto, entraste a ese estado
		wallJumped = true;
	}
	
	
	//Movimiento
	private void Walk(Vector2 dir)
	{
		//No te muevas al dashear
		if (!canMove)
			return;
		//Si estás pegado en la pared, no te mueves libremente
		if (wallGrab)
			return;
		//Si No estás haciendo walljump (Movimiento normal)
		if (!wallJumped)
		{
			//cambia x, conserva el salto o la caida en y
			rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
		}
		else
		{
			//Para cuando estes en paredes muevete suavemente interpolando la velocidad
			rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
		}
	}
	//Salta en la dirección dada.
	private void Jump(Vector2 dir, bool wall)
	{
		//Literalmente en la dirección indicada
		rb.velocity = new Vector2(rb.velocity.x, 0);
		rb.velocity += dir * jumpForce;

	}
	//Cooldown para cuando se haga un walljump
	IEnumerator DisableMovement(float time)
	{
		canMove = false;
		yield return new WaitForSeconds(time);
		canMove = true;
	}
	
}
