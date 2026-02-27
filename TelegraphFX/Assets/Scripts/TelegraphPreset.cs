using UnityEngine;

namespace TelegraphFX
{
    [CreateAssetMenu(
        fileName = "TelegraphPreset",
        menuName = "TelegraphFX/Telegraph Preset"
    )]
    public class TelegraphPreset : ScriptableObject
    {
        public TelegraphData data;

        // -------------------------------------------------------
        // Shape별 기본값으로 초기화하는 유틸
        // 에디터에서 Shape 바꿀 때 호출하면 편함
        // -------------------------------------------------------
        public void ResetToDefault()
        {
            data = data.shape switch
            {
                TelegraphShape.Circle => TelegraphData.DefaultCircle,
                TelegraphShape.Sector => TelegraphData.DefaultSector,
                TelegraphShape.ArcRect => TelegraphData.DefaultArcRect,
                _ => TelegraphData.DefaultCircle,
            };
        }

        // 런타임에서 TelegraphData 복사본을 가져갈 때 사용
        // (Preset 원본이 오염되지 않도록 값 복사)
        public TelegraphData CreateInstance() => data;
    }
}