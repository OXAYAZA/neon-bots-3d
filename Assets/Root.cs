using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Root : MonoBehaviour
{
	public static Root Instance { get; private set; }

	public GameObject map;
	public GameObject hero;
	public GameObject pauseMenu;
	public List<GameObject> touchControl;
	public InputType controlType;

	[HideInInspector]
	public bool gamePaused;

	[HideInInspector]
	public KeyboardMouseController kmControl;

	private void Awake()
	{
		if ( Instance == null )
			Instance = this;
	}

	private void Start ()
	{
		this.kmControl = this.gameObject.GetComponent<KeyboardMouseController>();
		this.controlType = InputType.KeyboardMouse;

		if ( this.pauseMenu )
			this.pauseMenu.SetActive( false );

		this.DisableControlTouch();
		this.EnableControlKeyboardMouse();
	}

	public void PlayPauseGame()
	{
		var scene = SceneManager.GetActiveScene();

		if ( scene.name != "Menu" )
		{
			if ( this.gamePaused )
			{
				this.ClosePauseMenu();
				this.PlayGame();
			}

			else
			{
				this.PauseGame();
				this.OpenPauseMenu();
			}
		}
	}

	public void PauseGame ()
	{
		Time.timeScale = 0;
		this.gamePaused = true;
	}

	public void PlayGame ()
	{
		Time.timeScale = 1;
		this.gamePaused = false;
	}

	public void ExitGame ()
	{
		Debug.Log( "Quit" );
		Application.Quit();
	}

	public void GoToScene( string sceneName )
	{
		var scene = SceneManager.GetActiveScene();

		if ( scene.name != sceneName )
		{
			this.PlayGame();
			SceneManager.LoadScene( sceneName );
		}
	}

	public void GoToMainMenu ()
	{
		this.GoToScene( "Menu" );
	}

	public void OpenPauseMenu ()
	{
		if ( this.pauseMenu )
			this.pauseMenu.SetActive( true );
	}

	public void ClosePauseMenu ()
	{
		if ( this.pauseMenu )
			this.pauseMenu.SetActive( false );
	}

	public void SwitchControl ( InputType inputType )
	{
		switch ( this.controlType )
		{
			case InputType.KeyboardMouse:
				this.DisableControlKeyboardMouse();
				break;
			case InputType.Touch:
				this.DisableControlTouch();
				break;
		}

		switch ( inputType )
		{
			case InputType.KeyboardMouse:
				this.EnableControlKeyboardMouse();
				break;
			case InputType.Touch:
				this.EnableControlTouch();
				break;
		}
	}

	private void EnableControlTouch ()
	{
		foreach ( GameObject gObject in this.touchControl )
			gObject.SetActive( true );

		this.controlType = InputType.Touch;
	}

	private void DisableControlTouch ()
	{
		foreach ( GameObject gObject in this.touchControl )
			gObject.SetActive( false );
	}

	private void EnableControlKeyboardMouse ()
	{
		if ( this.kmControl )
			this.kmControl.enabled = true;

		this.controlType = InputType.KeyboardMouse;
	}
	
	private void DisableControlKeyboardMouse ()
	{
		if ( this.kmControl )
			this.kmControl.enabled = false;
	}
}
