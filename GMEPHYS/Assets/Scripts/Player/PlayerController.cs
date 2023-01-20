using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public delegate void _CurrentController();

    [SerializeField] public GameObject player;

    public _CurrentController CurrentController;

    private int GameState;

    PC_Menu Menu;
    PC_Game Game;

    public enum GameStates
    {
        MAINMENU,
        PAUSED,
        GAME,
        DIALOG
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        Menu = gameObject.GetComponent<PC_Menu>();
        Game = gameObject.GetComponent<PC_Game>();
    }

    private void Update()
    {
        CurrentController?.Invoke();
    }

}
