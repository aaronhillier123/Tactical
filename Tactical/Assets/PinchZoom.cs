using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
	public float perspectiveZoomSpeed = 0.1f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.1f;        // The rate of change of the orthographic size in orthographic mode.


	void Update()
	{
		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// If the camera is orthographic...
			if (Camera.main.orthographic)
			{
				// ... change the orthographic size based on the change in distance between the touches.
				CameraZoom._instance.zoom(-1 * deltaMagnitudeDiff * orthoZoomSpeed);

			}
			else
			{
				// Otherwise change the field of view based on the change in distance between the touches.
				CameraZoom._instance.zoom(-1 * deltaMagnitudeDiff * perspectiveZoomSpeed);

			}
		}
	}
}