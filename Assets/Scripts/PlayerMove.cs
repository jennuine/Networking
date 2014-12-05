using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	#region Fields
	private Transform move = null;
	private float speed = 10f;
	//private Camera camera;
	//private Camera mainCamera;
	#endregion

	#region Unity Methods

	void Start () 
	{
		move = this.transform;
		//camera.enabled = false;
		//mainCamera.enabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!networkView.isMine)
			return;

		if (Input.GetKey (KeyCode.W))
			move.Translate(Vector3.forward * speed * Time.deltaTime);

		if (Input.GetKey(KeyCode.S))
			move.Translate(-Vector3.forward * speed * Time.deltaTime);

		if (Input.GetKey (KeyCode.A))
			move.Translate (-Vector3.right * speed * Time.deltaTime);

		if (Input.GetKey (KeyCode.D))
			move.Translate(Vector3.right * speed * Time.deltaTime);

		/*if (Input.GetKey(KeyCode.C))
		{
			camera.enabled = !camera.enabled;
			mainCamera.enabled = !mainCamera.enabled;
		}
*/

	}

	#endregion
}
