using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.EventSystems;

public class CameraFocusController : MonoBehaviour
{
	public Transform focusCursor;
	Animator focusCursorAnim;

	void Start()
	{
		var vuforia = VuforiaARController.Instance;
		vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
		vuforia.RegisterOnPauseCallback(OnPaused);

		focusCursorAnim = focusCursor.GetComponent<Animator>();
	}

	private void Update()
	{
		//появление спрайта фокусировки
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (Input.GetMouseButtonDown(0))
		{
			focusCursor.position = Input.GetTouch(0).position;
			focusCursorAnim.Play(0);
		}

	}

	private void OnVuforiaStarted()
	{
		CameraDevice.Instance.SetFocusMode(
			CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}

	private void OnPaused(bool paused)
	{
		if (!paused) // resumed
		{
			// Set again autofocus mode when app is resumed
			CameraDevice.Instance.SetFocusMode(
			   CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		}
	}
}