using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Root : MonoBehaviour
{
	public static Root Instance { get; private set; }

	public GameObject rootLocal;
	public new GameObject camera;
	public List<GameObject> UILayers;
	public InputType controlType;

	[HideInInspector]
	public UnityEvent reconfEvent;

	[HideInInspector]
	public RootLocal local;

	[HideInInspector]
	public CameraController cameraController;

	[HideInInspector]
	public bool gamePaused;

	[HideInInspector]
	public KeyboardMouseController kmControl;

	private void Awake()
	{
		Debug.Log( "Root Awake" );

		if ( Instance == null )
		{
			this.local = this.rootLocal.GetComponent<RootLocal>();
			this.local.Link( this );
			Instance = this;
			DontDestroyOnLoad( this.gameObject );
			Instance.Init();
		}

		else
		{
			Instance.local = this.rootLocal.GetComponent<RootLocal>();
			Instance.local.Link( Instance );
			Destroy( this.gameObject );
			Debug.Log( "Root Invoke Reconfiguration" );
			Instance.reconfEvent.Invoke();
		}
	}

	private void Init ()
	{
		this.reconfEvent = new UnityEvent();
		this.reconfEvent.AddListener( this.Reconf );
		this.cameraController = this.camera.GetComponent<CameraController>();
		this.kmControl = this.gameObject.GetComponent<KeyboardMouseController>();
		this.DisableControlKeyboardMouse();
		this.DisableControlTouch();
		this.SwitchControl( this.controlType );
		this.Reconf();
	}

	private void Reconf ()
	{
		var scene = SceneManager.GetActiveScene();
		this.HideUILayers();
		this.ShowUILayer( "Misc" );

		if ( scene.name == "Menu" )
		{
			this.ShowUILayer( "MainMenu" );
		}

		else
		{
			this.ShowUILayer( "GameUI" );
			if ( this.controlType == InputType.Touch ) this.ShowUILayer( "TouchControl" );
		}
	}

	public void PlayPauseGame()
	{
		var scene = SceneManager.GetActiveScene();

		if ( scene.name != "Menu" )
		{
			if ( this.gamePaused )
			{
				this.PlayGame();
				this.HideUILayers();
				this.ShowUILayer( "GameUI" );
				if ( this.controlType == InputType.Touch ) this.ShowUILayer( "TouchControl" );
			}

			else
			{
				this.PauseGame();
				this.HideUILayers();
				this.ShowUILayer( "PauseMenu" );
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

	public void HideUILayers ()
	{
		foreach ( var layer in this.UILayers )
			layer.SetActive( false );
	}

	public void ShowUILayer ( string layerName )
	{
		foreach ( var layer in this.UILayers )
			if ( layerName == layer.name )
				layer.SetActive( true );
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

		this.controlType = inputType;
	}

	private void EnableControlTouch ()
	{
		// this.ShowUILayer( "TouchControl" );
	}

	private void DisableControlTouch ()
	{
		// this.ShowUILayer( "TouchControl" );
	}

	private void EnableControlKeyboardMouse ()
	{
		if ( this.kmControl )
			this.kmControl.enabled = true;
	}

	private void DisableControlKeyboardMouse ()
	{
		if ( this.kmControl )
			this.kmControl.enabled = false;
	}
}
