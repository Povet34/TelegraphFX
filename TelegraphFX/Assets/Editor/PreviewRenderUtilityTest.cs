// Editor 폴더 안에 넣어야 함
// Assets/Editor/PreviewRenderUtilityTest.cs

using UnityEngine;
using UnityEditor;

public class PreviewRenderUtilityTest : EditorWindow
{
    // -------------------------------------------------------
    // 핵심 멤버
    // -------------------------------------------------------

    // 씬에 숨겨진 전용 오브젝트들
    private Camera _previewCamera;
    private Light _previewLight;
    private GameObject _previewObject;  // 렌더할 Mesh를 가진 오브젝트
    private RenderTexture _renderTexture;

    // 카메라 회전 제어
    private Vector2 _drag;

    // 테스트용 슬라이더 값
    private float _sliderValue = 0.5f;

    // -------------------------------------------------------
    // 열기
    // -------------------------------------------------------
    [MenuItem("TelegraphFX/Preview Test")]
    static void Open()
    {
        GetWindow<PreviewRenderUtilityTest>("Preview Test");
    }

    // -------------------------------------------------------
    // 초기화
    // -------------------------------------------------------
    void OnEnable()
    {
        SetupPreviewScene();
    }

    // 프리뷰 전용 레이어 번호 (31번 사용)
    private const int PreviewLayer = 31;

    private void SetupPreviewScene()
    {
        // --- RenderTexture ---
        _renderTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        _renderTexture.Create();

        // --- Camera ---
        var camGO = new GameObject("PreviewCamera") { hideFlags = HideFlags.HideAndDontSave };
        _previewCamera = camGO.AddComponent<Camera>();
        _previewCamera.transform.position = new Vector3(0f, 3f, -5f);
        _previewCamera.transform.LookAt(Vector3.zero);
        _previewCamera.nearClipPlane = 0.1f;
        _previewCamera.farClipPlane = 100f;
        _previewCamera.clearFlags = CameraClearFlags.SolidColor;
        _previewCamera.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        _previewCamera.targetTexture = _renderTexture;
        _previewCamera.enabled = true;
        // 프리뷰 레이어만 렌더
        _previewCamera.cullingMask = 1 << PreviewLayer;

        // --- Light ---
        var lightGO = new GameObject("PreviewLight") { hideFlags = HideFlags.HideAndDontSave };
        _previewLight = lightGO.AddComponent<Light>();
        _previewLight.type = LightType.Directional;
        _previewLight.intensity = 1f;
        _previewLight.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
        // 라이트도 프리뷰 레이어에만 영향
        _previewLight.cullingMask = 1 << PreviewLayer;

        // --- Preview Object ---
        _previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _previewObject.hideFlags = HideFlags.HideAndDontSave;
        _previewObject.transform.position = Vector3.zero;
        // 프리뷰 레이어로 이동
        SetLayerRecursive(_previewObject, PreviewLayer);

        // URP용 머티리얼
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = Color.red;
        _previewObject.GetComponent<MeshRenderer>().sharedMaterial = mat;

        // 씬/게임 카메라에서 프리뷰 레이어 제외
        ExcludeLayerFromSceneCameras();
    }

    // 씬에 있는 모든 카메라에서 PreviewLayer 제외
    private void ExcludeLayerFromSceneCameras()
    {
        foreach (var cam in Camera.allCameras)
        {
            if (cam == _previewCamera) continue;
            cam.cullingMask &= ~(1 << PreviewLayer);
        }
    }

    private void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    // -------------------------------------------------------
    // 해제
    // -------------------------------------------------------
    void OnDisable()
    {
        if (_previewCamera != null) DestroyImmediate(_previewCamera.gameObject);
        if (_previewLight != null) DestroyImmediate(_previewLight.gameObject);
        if (_previewObject != null) DestroyImmediate(_previewObject);
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            DestroyImmediate(_renderTexture);
        }
    }

    // -------------------------------------------------------
    // GUI
    // -------------------------------------------------------
    void OnGUI()
    {
        Rect previewRect = new Rect(0, 0, position.width, position.height - 60f);

        HandleMouseDrag(previewRect);
        DrawPreview(previewRect);

        // 하단 슬라이더
        GUILayout.BeginArea(new Rect(0, position.height - 55f, position.width, 55f));
        GUILayout.Space(5f);
        GUILayout.Label($"Slider Value : {_sliderValue:F2}");
        float newVal = GUILayout.HorizontalSlider(_sliderValue, 0f, 1f);
        GUILayout.EndArea();

        if (!Mathf.Approximately(newVal, _sliderValue))
        {
            _sliderValue = newVal;

            // 슬라이더 값으로 색상 변경
            var mat = _previewObject.GetComponent<MeshRenderer>().sharedMaterial;
            mat.color = Color.Lerp(Color.blue, Color.red, _sliderValue);

            Repaint();
        }
    }

    // -------------------------------------------------------
    // 마우스 드래그 → 오브젝트 회전
    // -------------------------------------------------------
    private void HandleMouseDrag(Rect rect)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDrag && rect.Contains(e.mousePosition))
        {
            _drag += e.delta;
            // 오브젝트를 회전시킴 (카메라는 고정)
            _previewObject.transform.rotation = Quaternion.Euler(-_drag.y, -_drag.x, 0f);
            Repaint();
        }
    }

    // -------------------------------------------------------
    // 렌더링
    // -------------------------------------------------------
    private void DrawPreview(Rect rect)
    {
        if (_previewCamera == null || _renderTexture == null) return;

        // RenderTexture 크기를 창 크기에 맞게 조정
        int w = Mathf.Max(1, (int)rect.width);
        int h = Mathf.Max(1, (int)rect.height);
        if (_renderTexture.width != w || _renderTexture.height != h)
        {
            _renderTexture.Release();
            _renderTexture.width = w;
            _renderTexture.height = h;
            _renderTexture.Create();
            _previewCamera.targetTexture = _renderTexture;
        }

        // URP가 자동으로 targetTexture에 렌더해줌
        // 그 결과를 EditorWindow에 그리기
        GUI.DrawTexture(rect, _renderTexture, ScaleMode.StretchToFill, false);
    }
}