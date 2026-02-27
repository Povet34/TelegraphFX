using System;
using UnityEngine;

namespace TelegraphFX
{
    public enum TelegraphShape
    {
        Circle = 0,
        Sector = 1,
        Rectangle = 2,
        Ring = 3,
    }

    public enum TelegraphFillMode
    {
        Radial = 0,  // 시계방향으로 채워짐
        Scale = 1,  // 중심에서 원이 커지는 방식
        Linear = 2,  // forward 방향으로 채워짐
        EdgePulse = 3,  // 테두리가 빨개지다 터지는 방식
    }

    [Serializable]
    public struct TelegraphData
    {
        // --- 트랜스폼 ---
        public Vector3 position;      // 월드 중심점
        public Vector3 forward;       // 방향 기준 벡터 (Sector, Linear에 사용)

        // --- 형태 ---
        public TelegraphShape shape;
        public TelegraphFillMode fillMode;
        public float radius;          // Circle, Sector, Ring 반지름
        public float sectorAngle;     // Sector 전체 각도 (degree)
        public float width;           // Rectangle 가로
        public float height;          // Rectangle 세로
        public float ringThickness;   // Ring 두께 (0~1, radius 기준 비율)

        // --- 시간 ---
        public float duration;        // 전체 지속 시간 (초)
        public float elapsed;         // 경과 시간 (Manager가 관리)

        // --- 비주얼 ---
        public Color baseColor;       // 기본 색상 (채워지기 전)
        public Color fillColor;       // 채워지는 색상
        public Color edgeColor;       // 테두리 강조 색상 (EdgePulse에 사용)
        public float edgeWidth;       // 테두리 두께 (UV 기준)

        // --- 내부 식별 ---
        public int id;                // Manager가 발급하는 고유 ID

        // fillAmount 계산 (읽기 전용)
        public float FillAmount => duration > 0f ? Mathf.Clamp01(elapsed / duration) : 0f;

        // 기본값 프리셋
        public static TelegraphData Default => new TelegraphData
        {
            forward = Vector3.forward,
            shape = TelegraphShape.Circle,
            fillMode = TelegraphFillMode.Radial,
            radius = 1f,
            sectorAngle = 90f,
            width = 1f,
            height = 2f,
            ringThickness = 0.05f,
            duration = 2f,
            elapsed = 0f,
            baseColor = new Color(1f, 0f, 0f, 0.2f),
            fillColor = new Color(1f, 0f, 0f, 0.7f),
            edgeColor = new Color(1f, 0.3f, 0f, 1f),
            edgeWidth = 0.03f,
        };
    }
}