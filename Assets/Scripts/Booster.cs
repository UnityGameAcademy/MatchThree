using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class Booster : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// the UI.Image component
    Image m_image;

    // the RectTransform component
    RectTransform m_rectXform;

    // reset position
    Vector3 m_startPosition;

    // Board component
    Board m_board;

    // the Tile to apply the booster effect
    Tile m_tileTarget;

    // the one active Booster GameObject
    public static GameObject ActiveBooster;

    // UI.Text component for instructions
    public Text instructionsText;

    // text instructions 
    public string instructions = "drag over game piece to remove";

    // is the Booster enabled? (has the button been clicked once?)
    public bool isEnabled = false;

    // is this Booster intended to draggable (currently the only implemented behavior)
    public bool isDraggable = true;

    // has the Booster been locked (for use with another manager script)
    public bool isLocked = false;

    // useful for UI elements that may be colliding with drag event / add a CanvasGroup and add to List
    public List<CanvasGroup> canvasGroups;

    // actions to invoke when the drag is complete
    public UnityEvent boostEvent;

    // time bonus
    public int boostTime = 15;

    // initialize components
    void Awake()
    {
        m_image = GetComponent<Image>();
        m_rectXform = GetComponent<RectTransform>();
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    void Start()
    {
        EnableBooster(false);
    }

    // toggle the Booster on/off
    public void EnableBooster(bool state)
    {
        isEnabled = state;

        if (state)
        {
            DisableOtherBoosters();
            Booster.ActiveBooster = gameObject;
        }
        else if (gameObject == Booster.ActiveBooster)
        {
            Booster.ActiveBooster = null;
        }

        m_image.color = (state) ? Color.white : Color.gray;

        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(Booster.ActiveBooster != null);

            if (gameObject == Booster.ActiveBooster)
            {
                instructionsText.text = instructions;
            }
        }
    }

    // disable all other boosters
    void DisableOtherBoosters()
    {
        Booster[] allBoosters = Object.FindObjectsOfType<Booster>();

        foreach (Booster b in allBoosters)
        {
            if (b != this)
            {
                b.EnableBooster(false);
            }
        }
    }

    // toggle Booster state
    public void ToggleBooster()
    {
        EnableBooster(!isEnabled);
    }

    // frame where we begin dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked)
        {
            m_startPosition = gameObject.transform.position;
            EnableCanvasGroups(false);
        }
    }

    // still dragging
    public void OnDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked && Camera.main != null)
        {
            Vector3 onscreenPosition;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rectXform, eventData.position, 
                                                                    Camera.main, out onscreenPosition);
            gameObject.transform.position = onscreenPosition;

            RaycastHit2D hit2D = Physics2D.Raycast(onscreenPosition, Vector3.forward, Mathf.Infinity);

            if (hit2D.collider != null )
            {
                m_tileTarget = hit2D.collider.GetComponent<Tile>();
            }
            else
            {
                m_tileTarget = null;
            }
        }
    }

    // frame where we end drag
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEnabled && isDraggable && !isLocked)
        {
            gameObject.transform.position = m_startPosition;
            EnableCanvasGroups(true);

            if (m_board != null && m_board.isRefilling)
            {
                return;
            }

            if (m_tileTarget != null)
            {
                if (boostEvent != null)
                {
                    boostEvent.Invoke();
                }

                EnableBooster(false);

                m_tileTarget = null;
                Booster.ActiveBooster = null;
            }
        }
    }

    // enable/disable blocksRaycasts for CanvasGroup components
    void EnableCanvasGroups(bool state)
    {
        if (canvasGroups != null && canvasGroups.Count > 0)
        {
            foreach (CanvasGroup cGroup in canvasGroups)
            {
                if (cGroup != null)
                {
                    cGroup.blocksRaycasts = state;
                }
            }
        }
    }

    // action to remove one GamePiece
    public void RemoveOneGamePiece()
    {
        if (m_board != null && m_tileTarget != null)
        {
            m_board.ClearAndRefillBoard(m_tileTarget.xIndex, m_tileTarget.yIndex);
        }
    }

    // action to add bonus time
    public void AddTime()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddTime(boostTime);
        }
    }

    // action to replace GamePiece with Color Bomb
    public void DropColorBomb()
    {
        if (m_board != null && m_tileTarget != null)
        {
            m_board.MakeColorBombBooster(m_tileTarget.xIndex, m_tileTarget.yIndex);
        }
    }
}
