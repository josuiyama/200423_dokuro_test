using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
	Vector3 prev;
	void Update()
	{
		Vector3 p = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
		p.z = 0;

		if (Input.GetMouseButton(0))
		{
			GetComponent<Rigidbody2D>().velocity = (p - this.prev) * 80;
		}
		this.prev = p;
	}
}