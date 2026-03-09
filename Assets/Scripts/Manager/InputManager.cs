using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class InputManager : MonoBehaviour
{
    //Singleton
    private static InputManager Instance;

    //Input Controls System
    private PlayerControls controls;

    //Camera control
    public float ZoomInput;
    public bool isPanning;
    public float PanInputX;
    public float PanInputY;
    public bool isRightClick;
    public bool isLeftClick;
    public bool isLeftClickHold;
    public bool isLeftDoubleClick;

    public bool isLeftArrow;
    public bool isRightArrow;
    public bool isUpArrow;
    public bool isDownArrow;

    public bool isOverUI;
    public Vector2 mousePos;
    public bool isInViewport;

    /// <summary>
    /// Awake function at init moment
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        controls = new PlayerControls(); //Create the controls 
        controls.Enable();
        isPanning = false;
        isOverUI = false;


        //------------------------------------------------------------------------- CAMERA EVENT

        controls.Player.Zoom.performed += OnZoomPerformed;
        controls.Player.Zoom.canceled += OnZoomCanceled;

        controls.Player.PanX.performed += OnPanXPerformed;
        controls.Player.PanX.canceled += OnPanXCanceled;

        controls.Player.PanY.performed += OnPanYPerformed;
        controls.Player.PanY.canceled += OnPanYCanceled;

        controls.Player.RightClick.performed += OnRightClickPerformed;
        controls.Player.RightClick.canceled += OnRightClickCanceled;

        controls.Player.LeftClick.performed += OnLeftClickPerformed;
        controls.Player.LeftClick.canceled += OnLeftClickCanceled;

        isDownArrow= false;
        isUpArrow= false;
        isLeftArrow= false;
        isRightArrow= false;

        isLeftClick = isLeftClickHold = isLeftDoubleClick = false;
        isRightClick = false;

    }

    private void OnDestroy()
    {
        //Unsubscribe to input
        controls.Player.Zoom.performed -= OnZoomPerformed;
        controls.Player.Zoom.canceled -= OnZoomCanceled;

        controls.Player.PanX.performed -= OnPanXPerformed;
        controls.Player.PanX.canceled -= OnPanXCanceled;

        controls.Player.PanY.performed -= OnPanYPerformed;
        controls.Player.PanY.canceled -= OnPanYCanceled;

        controls.Player.RightClick.performed -= OnRightClickPerformed;
        controls.Player.RightClick.canceled -= OnRightClickCanceled;

        controls.Player.LeftClick.performed -= OnLeftClickPerformed;
        controls.Player.LeftClick.canceled -= OnLeftClickCanceled;

        controls.Disable();
    }

    private void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        isInViewport = mousePos.x >=0 && mousePos.y >= 0 && mousePos.x < Screen.width && mousePos.y < Screen.height;
    }

    public bool Reset()
    {
        if (controls.Player.Reset.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool UpArrow()
    {
        if (controls.Player.UpArrow.WasPerformedThisFrame() || controls.Player.UpArrow.IsPressed())
        {
            isUpArrow= true;
        }
        else
        {
            isUpArrow = false;
        }

        return isUpArrow;
    }
    public bool DownArrow()
    {
        if (controls.Player.DownArrow.WasPerformedThisFrame() || controls.Player.DownArrow.IsPressed())
        {
            isDownArrow = true;
        }
        else
        {
            isDownArrow = false;
        }

        return isDownArrow;
    }
    public bool RightArrow()
    {
        if (controls.Player.RightArrow.WasPerformedThisFrame() || controls.Player.RightArrow.IsPressed())
        {
            isRightArrow = true;
        }
        else
        {
            isRightArrow = false;
        }

        return isRightArrow;
    }
    public bool LeftArrow()
    {
        if (controls.Player.LeftArrow.WasPerformedThisFrame() || controls.Player.LeftArrow.IsPressed())
        {
            isLeftArrow = true;
        }
        else
        {
            isLeftArrow = false;
        }

        return isLeftArrow;
    }
    public bool View3D()
    {
        if (controls.Player.View3D.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool ViewImmersive()
    {
        if (controls.Player.ViewImmersive.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool View2D()
    {
        if (controls.Player.View2D.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool SplitScreen()
    {
        if (controls.Player.SplitScreen.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool OneScreen()
    {
        if (controls.Player.OneScreen.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Undo()
    {
        if (controls.Player.Undo.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Redo()
    {
        if (controls.Player.Redo.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool LeftClick()
    {
        if (isLeftClick && controls.Player.LeftClick.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool LeftClickHold()
    {
        return isLeftClickHold;
    }

    public bool LeftDoubleClick()
    {
        if (isLeftDoubleClick && controls.Player.LeftClick.WasPerformedThisFrame())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Get the scroll mouse value when performed
    /// </summary>
    /// <param name="context"></param>
    public void OnZoomPerformed(InputAction.CallbackContext context)
    {
        ZoomInput = context.ReadValue<float>();
    }

    /// <summary>
    /// Get the scroll mouse value when idle
    /// </summary>
    /// <param name="context"></param>
    public void OnZoomCanceled(InputAction.CallbackContext context)
    {
        ZoomInput = 0;
    }

    /// <summary>
    /// Get the Pan X value when performed
    /// </summary>
    /// <param name="context"></param>
    private void OnPanXPerformed(InputAction.CallbackContext context)
    {
        isPanning = true;
        PanInputX = context.ReadValue<float>();
    }

    /// <summary>
    /// Get the Pan X value value when idle
    /// </summary>
    /// <param name="context"></param>
    private void OnPanXCanceled(InputAction.CallbackContext context)
    {
        isPanning = false;
        PanInputX = 0;
    }

    /// <summary>
    /// Get the Pan Y value when performed
    /// </summary>
    /// <param name="context"></param>
    private void OnPanYPerformed(InputAction.CallbackContext context)
    {
        isPanning = true;
        PanInputY = context.ReadValue<float>();
    }

    /// <summary>
    /// Get the Pan Y value value when idle
    /// </summary>
    /// <param name="context"></param>
    private void OnPanYCanceled(InputAction.CallbackContext context)
    {
        isPanning = false;
        PanInputY = 0;
    }

    /// <summary>
    /// Get the right click value when performed
    /// </summary>
    /// <param name="context"></param>
    private void OnRightClickPerformed(InputAction.CallbackContext context)
    {
        isRightClick = true;
        CheckClickOverUI();
    }

    /// <summary>
    /// Get the right click value value when idle
    /// </summary>
    /// <param name="context"></param>
    private void OnRightClickCanceled(InputAction.CallbackContext context)
    {
        isRightClick = false;
    }

    /// <summary>
    /// Get the left click value when performed
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftClickPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is MultiTapInteraction)
        {
            isLeftDoubleClick = true;
            isLeftClickHold = false;
            isLeftClick = false;
        }
        else if (context.interaction is TapInteraction)
        {
            isLeftClick = true;
            isLeftClickHold = false;
            isLeftDoubleClick = false;
        }
        else if (context.interaction is HoldInteraction)
        {
            isLeftClickHold = true;
            isLeftClick = false;
            isLeftDoubleClick = false;
        }

        CheckClickOverUI();
    }

    /// <summary>
    /// Get the left click value value when idle
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftClickCanceled(InputAction.CallbackContext context)
    {
        if (context.interaction is MultiTapInteraction)
        {
            isLeftDoubleClick = false;
        }
        else if (context.interaction is TapInteraction)
        {
            isLeftClick = false;
        }
        else if (context.interaction is HoldInteraction)
        {
            isLeftClickHold = false;
        }
    }

    void CheckClickOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        isOverUI = false;
        for (int i = 0; i < raycastResultsList.Count; i++)
        {
            if (raycastResultsList[i].gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                isOverUI = true;
                break;
            }
        }
    }
}
