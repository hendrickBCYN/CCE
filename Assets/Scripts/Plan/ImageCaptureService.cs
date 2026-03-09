using UnityEngine;

public class ImageCaptureService : MonoBehaviour
{
    [SerializeField] private MeasurementDrawing _measurementDrawing;
    [SerializeField] private CameraManager _cameraManager;

    [SerializeField] private RenderTexture _renderTexture;
    private Camera _camera;

    public Texture2D CaptureSelectedSide()
    {
        if(_camera != null)
        {
            _measurementDrawing.DisplayRoomMeasurements();
            _camera.Render();
            return PdfUtility.ConvertRenderTextureToTexture2D(_renderTexture);
        }
        else
        {
            Debug.Log($"ImageCaptureService - {_camera} is not assigned");
            return null;
        }
    }

    public Texture2D[] CaptureAllSides()
    {
        CameraManager.Side[] l_sides = (CameraManager.Side[])System.Enum.GetValues(typeof(CameraManager.Side));
        Texture2D[] l_capturedTextures = new Texture2D[l_sides.Length];

        _renderTexture = new RenderTexture(Screen.height, Screen.height, 24);

        SetupCameraForPlansCapture(out float currentOrthographicSize, out Rect currentRect);

        for (int i = 0; i < l_sides.Length; i++)
        {
            _cameraManager.SetCurrentSide(l_sides[i]);
            _cameraManager.SetCameraNearFarClipPLane(CameraManager.CameraType.C2D, l_sides[i]);

            l_capturedTextures[i] = CaptureSelectedSide();
        }

        RestoreCamera2Dsettings(currentOrthographicSize, currentRect);

        return l_capturedTextures;
    }

     public Texture2D CaptureCoverImage()
    {
        _cameraManager.ActivatePlans(false);

        Camera l_camera2d = _cameraManager.GetCamera2D();
        Camera l_camera3d = _cameraManager.GetCamera3D();

        Rect l_currentRect = l_camera2d.rect;
        Vector3 l_currentCam3dPosition = l_camera3d.transform.position;
        Vector3 l_currentCam3dRotation = l_camera3d.transform.eulerAngles;

        SetupCamerasForCoverImageCapture(l_camera2d, l_camera3d);

        RenderTexture l_cam2dRenderTexture = CaptureCameraView(l_camera2d, Screen.width / 2, Screen.height);
        l_camera2d.gameObject.SetActive(false);
        l_camera3d.gameObject.SetActive(true);
        RenderTexture l_cam3dRenderTexture = CaptureCameraView(l_camera3d, Screen.width / 2, Screen.height);

        Texture2D l_combinedTexture = CombineTextures(l_cam2dRenderTexture, l_cam3dRenderTexture);

        // Restore cameras settings 
        l_camera3d.transform.position = l_currentCam3dPosition;
        l_camera3d.transform.eulerAngles = l_currentCam3dRotation;
        l_camera3d.rect = l_currentRect;
        l_camera2d.rect = l_currentRect;

        l_camera2d.gameObject.SetActive(true);
        l_camera3d.gameObject.SetActive(false);

        _cameraManager.ActivatePlans(true);

        return l_combinedTexture;
    }

    private RenderTexture CaptureCameraView(Camera p_camera, int p_width, int p_height, int p_depth = 24)
    {
        RenderTexture l_renderTexture = new RenderTexture(p_width, p_height, p_depth);
        p_camera.targetTexture = l_renderTexture;
        p_camera.Render();

        p_camera.targetTexture = null;
        return l_renderTexture;
    }

    private Texture2D CombineTextures(RenderTexture p_cam2dRenderTexture, RenderTexture p_cam3dRenderTexture)
    {
        Texture2D l_combinedTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        RenderTexture.active = p_cam2dRenderTexture;
        l_combinedTexture.ReadPixels(new Rect(0, 0, p_cam2dRenderTexture.width, p_cam2dRenderTexture.height), Screen.width / 2, 0);
        l_combinedTexture.Apply();

        RenderTexture.active = p_cam3dRenderTexture;
        l_combinedTexture.ReadPixels(new Rect(0, 0, p_cam3dRenderTexture.width, p_cam3dRenderTexture.height), 0, 0);
        l_combinedTexture.Apply();

        RenderTexture.active = null;

        return l_combinedTexture;
    }

    private void SetupCameraForPlansCapture(out float p_currentOrthographicSize, out Rect p_currentRect)
    {
        _camera = _cameraManager.GetCamera2D();

        p_currentOrthographicSize = 5f; 
        p_currentRect = new Rect();

        if (_camera != null)
        {
            p_currentOrthographicSize = _camera.orthographicSize;
            p_currentRect = _camera.rect;

            float l_orthographicSizeInMeter = (19f / PdfUtility.ScaleEnumToFloat(PdfUtility.ScaleInCm._1_50) / 100f);
            _camera.orthographicSize = l_orthographicSizeInMeter / 2f;
            _camera.rect = new Rect(0, 0, 1, 1);
            _camera.targetTexture = _renderTexture;
            RenderTexture.active = _renderTexture;
        }
    }

    private void RestoreCamera2Dsettings(float p_currentOrthographicSize, Rect p_currentRect)
    {
        if (_camera != null)
        {
            RenderTexture.active = null;
            _camera.targetTexture = null;

            _camera.orthographicSize = p_currentOrthographicSize;
            _camera.rect = p_currentRect;
            _camera.nearClipPlane = _cameraManager.CurrentCameraNearClipPlaneOffset;
            _camera.farClipPlane = _cameraManager.CurrentCameraFarClipPlaneOffset;
        }
    }

    private void SetupCamerasForCoverImageCapture(Camera p_camera2d, Camera p_camera3d)
    {
        _cameraManager.SetCurrentSide(CameraManager.Side.Floor);
        _measurementDrawing.DisplayRoomMeasurementsForFloorSide();

        Vector3 l_cam3dPositionIntented = new Vector3(5.44316339f, 7.89498186f, -4.74662066f);
        Vector3 l_cam3dRotationIntented = new Vector3(40.3931465f, 311.089508f, -1.12100304e-06f);

        p_camera3d.transform.position = l_cam3dPositionIntented;
        p_camera3d.transform.eulerAngles = l_cam3dRotationIntented;

        p_camera2d.rect = new Rect(0f, 0f, 1f, 1f);
        p_camera3d.rect = new Rect(0f, 0f, 1f, 1f);
    }
}
