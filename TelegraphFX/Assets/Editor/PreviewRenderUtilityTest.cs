// Editor 폴더 안에 넣어야 함
// Assets/Editor/PreviewRenderUtilityTest.cs

using UnityEngine;
using UnityEditor;

public class PreviewRenderUtilityTest : EditorWindow
{
    // -------------------------------------------------------
    // 핵심 멤버
    // -------------------------------------------------------
    private PreviewRenderUtility _preview;

    // 렌더할 대상
    private Mesh _mesh;
    private Material _material;

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
        // PreviewRenderUtility 생성
        _preview = new PreviewRenderUtility();

        // 카메라 초기 위치 (위에서 살짝 비스듬히)
        _preview.camera.transform.position = new Vector3(0f, 2f, -4f);
        _preview.camera.transform.LookAt(Vector3.zero);
        _preview.camera.nearClipPlane = 0.1f;
        _preview.camera.farClipPlane = 100f;

        // 기본 조명 세팅
        _preview.lights[0].intensity = 1f;
        _preview.lights[0].transform.rotation = Quaternion.Euler(30f, 30f, 0f);

        // 테스트용 Mesh : Unity 내장 Sphere
        _mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

        // 테스트용 Material : 기본 Standard
        _material = new Material(Shader.Find("Standard"));
        _material.color = Color.red;
    }

    // -------------------------------------------------------
    // 해제 - 반드시 Cleanup 호출
    // -------------------------------------------------------
    void OnDisable()
    {
        _preview?.Cleanup();
        _preview = null;

        if (_material != null)
            DestroyImmediate(_material);
    }

    // -------------------------------------------------------
    // GUI
    // -------------------------------------------------------
    void OnGUI()
    {
        // 1. 미리보기 영역 Rect 정의
        Rect previewRect = new Rect(0, 0, position.width, position.height - 60f);

        // 2. 마우스 드래그 입력 처리
        HandleMouseDrag(previewRect);

        // 3. 렌더링
        DrawPreview(previewRect);

        // 4. 하단 슬라이더 UI
        GUILayout.BeginArea(new Rect(0, position.height - 55f, position.width, 55f));
        GUILayout.Space(5f);
        GUILayout.Label($"Slider Value : {_sliderValue:F2}");
        float newVal = GUILayout.HorizontalSlider(_sliderValue, 0f, 1f);
        GUILayout.EndArea();

        // 슬라이더 값 바뀌면 색상 변경해서 Repaint
        if (!Mathf.Approximately(newVal, _sliderValue))
        {
            _sliderValue = newVal;
            _material.color = Color.Lerp(Color.blue, Color.red, _sliderValue);
            Repaint();
        }
    }

    // -------------------------------------------------------
    // 마우스 드래그로 오브젝트 회전
    // -------------------------------------------------------
    private void HandleMouseDrag(Rect rect)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDrag && rect.Contains(e.mousePosition))
        {
            _drag += e.delta;
            Repaint();
        }
    }

    // -------------------------------------------------------
    // 실제 렌더링
    // -------------------------------------------------------
    private void DrawPreview(Rect rect)
    {
        if (_preview == null || _mesh == null || _material == null) return;

        // BeginPreview : 렌더 시작
        _preview.BeginPreview(rect, GUIStyle.none);

        // 드래그 값으로 회전 행렬 생성
        Matrix4x4 matrix = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.Euler(-_drag.y, -_drag.x, 0f),
            Vector3.one
        );

        // Mesh를 해당 행렬로 그리기 등록
        _preview.DrawMesh(_mesh, matrix, _material, 0);

        // 카메라 렌더 실행
        _preview.camera.Render();

        // EndPreview : 렌더 완료 + Texture 반환
        Texture result = _preview.EndPreview();

        // 결과를 EditorWindow에 그리기
        GUI.DrawTexture(rect, result, ScaleMode.StretchToFill, false);
    }
}