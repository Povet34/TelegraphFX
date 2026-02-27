using System;
using UnityEngine;

namespace TelegraphFX
{
    public enum TelegraphShape
    {
        Circle = 0,
        Sector = 1,
        ArcRect = 2,
    }

    public enum TelegraphFillMode
    {
        Radial = 0,
        Scale = 1,
        Linear = 2,
        EdgePulse = 3,
    }

    [Serializable]
    public struct TelegraphData
    {
        // --- 공통 ---
        public TelegraphShape shape;
        public TelegraphFillMode fillMode;
        public Vector3 position;
        public Vector3 forward;

        // --- Shape별 수치 ---
        public float radius;
        public float innerRadius;
        public float outerRadius;
        public float angle;

        // --- 시간 ---
        public float duration;
        public float elapsed;

        // --- 비주얼 ---
        public Color baseColor;
        public Color fillColor;
        public Color edgeColor;
        public float edgeWidth;

        // --- 식별 ---
        public int id;

        public float FillAmount => duration > 0f ? Mathf.Clamp01(elapsed / duration) : 0f;

        // -------------------------------------------------------
        // 기본값 프리셋 - struct 안에 있어야 함
        // -------------------------------------------------------
        public static TelegraphData DefaultCircle => new TelegraphData
        {
            shape = TelegraphShape.Circle,
            fillMode = TelegraphFillMode.Scale,
            forward = Vector3.forward,
            radius = 3f,
            duration = 2f,
            baseColor = new Color(1f, 0f, 0f, 0.2f),
            fillColor = new Color(1f, 0f, 0f, 0.7f),
            edgeColor = new Color(1f, 0.3f, 0f, 1f),
            edgeWidth = 0.03f,
        };

        public static TelegraphData DefaultSector => new TelegraphData
        {
            shape = TelegraphShape.Sector,
            fillMode = TelegraphFillMode.Radial,
            forward = Vector3.forward,
            radius = 5f,
            angle = 90f,
            duration = 2f,
            baseColor = new Color(1f, 0f, 0f, 0.2f),
            fillColor = new Color(1f, 0f, 0f, 0.7f),
            edgeColor = new Color(1f, 0.3f, 0f, 1f),
            edgeWidth = 0.03f,
        };

        public static TelegraphData DefaultArcRect => new TelegraphData
        {
            shape = TelegraphShape.ArcRect,
            fillMode = TelegraphFillMode.Radial,
            forward = Vector3.forward,
            innerRadius = 2f,
            outerRadius = 5f,
            angle = 60f,
            duration = 2f,
            baseColor = new Color(1f, 0f, 0f, 0.2f),
            fillColor = new Color(1f, 0f, 0f, 0.7f),
            edgeColor = new Color(1f, 0.3f, 0f, 1f),
            edgeWidth = 0.03f,
        };
    }
}