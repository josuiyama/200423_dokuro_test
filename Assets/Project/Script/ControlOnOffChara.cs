using UnityEngine;
using System.Collections;

public class ControlOnOffChara : MonoBehaviour
{

	private CharacterController characterController;
	private Animator animator;
	private Vector3 velocity = Vector3.zero;
	[SerializeField]
	private float walkSpeed = 1.5f;
	//　現在キャラクターを操作出来るかどうか
	private bool control;

	// Use this for initialization
	void Awake()
	{
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		//if (characterController.isGrounded)
		//{
		//	velocity = Vector3.zero;

			if (control)
			{

		//		var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

		//		if (input.magnitude > 0f)
		//		{
		//			animator.SetFloat("Speed", input.magnitude);
		//			transform.LookAt(transform.position + input);
		//			velocity = transform.forward * walkSpeed;
		//		}
		//		else
		//		{
		//			animator.SetFloat("Speed", 0f);
		//		}

		//		if (Input.GetButtonDown("Jump"))
		//		{
		//			velocity.y += 5f;
		//		}
			}
		//}

		//velocity.y += Physics.gravity.y * Time.deltaTime;
		//characterController.Move(velocity * Time.deltaTime);
	}

	public void ChangeControl(bool controlFlag)
	{
		control = controlFlag;
		animator.SetFloat("Speed", 0f);
	}

}