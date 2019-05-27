using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotation : MonoBehaviour {
	Vector3 pos=new Vector3();
	float x;
	float y;
	public float xa;
	public float za;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnMouseDrag(){
		pos = Input.mousePosition;

		transform.rotation = Quaternion.Euler (xa,(x - pos.x)*0.5f + y,za);
		//float rot = Input.GetAxis ("Mouse X");
		//transform.rotation *= Quaternion.Euler (Vector3.up*-rot * 10);
	}
	void OnMouseDown(){
		x= Input.mousePosition.x;
		y = transform.localEulerAngles.y;
	}
}
