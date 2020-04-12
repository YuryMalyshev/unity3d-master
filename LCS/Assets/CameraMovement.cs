using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	// Start is called before the first frame update
	public Camera camera;
	public GameObject StartUp;

	void Start()
	{
	}

	private Vector3 lastPosition;
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
			camera.transform.position += camera.transform.right * change.x * camera.orthographicSize / -500;
			camera.transform.position += camera.transform.up * change.y * camera.orthographicSize / -500;
			lastPosition = Input.mousePosition;
		}
		else if(Input.GetMouseButton(1)) // RMB pressed => rotate
		{
			Vector3 change = Input.mousePosition - lastPosition;
			change = new Vector3(change.y, change.x);
			camera.transform.RotateAround(StartUp.GetComponent<Visualization>().GetSurfaceCenter(), change, change.magnitude/7.5f);
			lastPosition = Input.mousePosition;
		}
		//camera.transform.RotateAround(StartUp.GetComponent<Visualization>().GetSurfaceCenter(), Vector3.up, 5 * Time.deltaTime);
		camera.transform.position += camera.transform.forward * (Input.mouseScrollDelta.y/5);
		camera.orthographicSize -= (Input.mouseScrollDelta.y / 5);
		if (camera.orthographicSize > 100)
			camera.orthographicSize = 100f;
		else if (camera.orthographicSize < 0.5)
			camera.orthographicSize = 0.5f;



	}
}
