using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Start is called before the first frame update
	public Camera camera;

	void Start()
	{
	}

	private Vector3 lastPosition;
	private const float maxSize = 100;
	// Update is called once per frame
	void Update()
	{
		if(Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
		{
			lastPosition = Input.mousePosition;
		}
		if(Input.GetMouseButton(2)) // Mouse Wheel pressed => pan
		{
			Vector3 change = Input.mousePosition - lastPosition;
			camera.transform.position -= change/(float)((maxSize * 1.1 - camera.orthographicSize)/5);
			lastPosition = Input.mousePosition;
		}
		else if(Input.GetMouseButton(1)) // RMB pressed => rotate
		{
			Vector3 change = Input.mousePosition - lastPosition;
			Vector3 lookAtPosition = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, camera.nearClipPlane));
			camera.transform.position -= change / (float)((maxSize * 1.1 - camera.orthographicSize) / 10);
			camera.transform.LookAt(lookAtPosition);
			//Debug.Log(change + " " + Input.mousePosition);
			//camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles + change / 90);
			lastPosition = Input.mousePosition;
		}
		camera.transform.position += camera.transform.forward * (Input.mouseScrollDelta.y/5);
		camera.orthographicSize -= (Input.mouseScrollDelta.y / 5);
		if (camera.orthographicSize > 100)
			camera.orthographicSize = 100f;
		else if (camera.orthographicSize < 0.5)
			camera.orthographicSize = 0.5f;



	}
}
