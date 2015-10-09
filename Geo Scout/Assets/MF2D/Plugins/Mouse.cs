/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

public class Mouse{

	public enum ButtonState{ None,Down, Pressed, Up }

	public Vector2 position;
	public Vector2 delta;
	public Vector2 tiledPosition;

	public ButtonState leftButton = ButtonState.None;
	public ButtonState wheelButton = ButtonState.None;
	public ButtonState rightButton = ButtonState.None;

	public bool scrollWheel;
	public float wheelDelta;

	public void UpdateGlobal(Rect rect){

		Event e = Event.current;

		if (!rect.Contains(e.mousePosition)){

			if (e.button == 0 ){
				leftButton = ButtonState.None;
			}
			if (e.button == 1 ){
				rightButton = ButtonState.None;
			}
			if (e.button == 2 ){

				wheelButton = ButtonState.None;
			}
		}
	}

	public void UpdateLocal(float cellSize, Vector2 scroll,bool updateWheel = true){
		Event e = Event.current;

		position = e.mousePosition;
		if (position.x-scroll.x>0 && position.y-scroll.y>0){
			tiledPosition = new Vector2( Mathf.FloorToInt(e.mousePosition.x / cellSize), Mathf.FloorToInt(e.mousePosition.y / cellSize));
			delta = e.delta;
		}
		else{
			tiledPosition = new Vector2(-1,-1);
			delta=Vector2.zero;

		}

		if (e.isMouse){

			// left Button
			if (e.button == 0){
				if (e.type ==  EventType.MouseDown ){
					leftButton = ButtonState.Down;
				}
				else if (e.type == EventType.MouseUp){
					leftButton = ButtonState.Up;
				}
			}

			// Right Button
			if (e.button == 1){
				if (e.type ==  EventType.MouseDown ){
					rightButton = ButtonState.Down;
				}
				else if (e.type == EventType.MouseUp){
					rightButton = ButtonState.Up;
				}
			}

			// Wheel button
			if (e.button == 2){
				if (e.type ==  EventType.MouseDown ){
					wheelButton = ButtonState.Down;
				}
				else if (e.type == EventType.MouseUp){
					wheelButton = ButtonState.Up;
				}
			}

		}
		else{
			if (leftButton == ButtonState.Down){
				leftButton = ButtonState.Pressed;
			}
			else if (leftButton == ButtonState.Up){
				leftButton = ButtonState.None;
			}

			if (rightButton == ButtonState.Down){
				rightButton = ButtonState.Pressed;
			}
			else if (rightButton == ButtonState.Up){
				rightButton = ButtonState.None;
			}

			if (wheelButton == ButtonState.Down){
				wheelButton = ButtonState.Pressed;
			}
			else if (wheelButton == ButtonState.Up){
				wheelButton = ButtonState.None;
			}
		}

		if (updateWheel){
			if (e.type == EventType.scrollWheel){
				scrollWheel = true;
				wheelDelta = e.delta.y;
				e.Use();
			}
			else{
				scrollWheel = false;
			}
		}

	}

}
