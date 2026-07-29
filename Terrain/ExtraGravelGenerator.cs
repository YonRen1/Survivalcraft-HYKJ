using System;
using Game;
using Engine;

namespace HYKJ
{
    /// <summary>
    /// 通过 API 的 TerrainContentsGenerator24Initialize hook 追加额外的碎石矿袋生成步骤，
    /// 不修改 API 源码，只影响本模组，API 升级不会冲突。
    ///
    /// 原版 GeneratePockets 每个区块列已生成 20+20=40 个碎石矿袋，
    /// 这里再追加生成 GravelExtraCount 个，让碎石整体数量提升到 (40 + GravelExtraCount)。
    /// 调整 GravelExtraCount 即可控制增量。
    /// </summary>
    public static class ExtraGravelGenerator
    {
        // 额外生成的碎石矿袋数量（每个区块列）。
        // 40 ≈ 原版总量的 1 倍；改为 80 即总量变 3 倍；改为 0 即关闭本模组的额外生成。
        public const int GravelExtraCount = 40;

        // 矿袋中心 Y 范围（深度）。原版两轮分别是 20~120 与 4~120，
        // 这里取 4~120，覆盖从地表到深层。
        public const int YMin = 4;
        public const int YMax = 120;

        public static void Register(TerrainContentsGenerator24 generator)
        {
            if (GravelExtraCount <= 0) return;
            if (TerrainContentsGenerator24.m_gravelPocketBrushes == null
                || TerrainContentsGenerator24.m_gravelPocketBrushes.Count == 0)
            {
                return;
            }

            // 顺序号 250：原版 GeneratePockets 是 200，比它晚执行，
            // 保证 m_gravelPocketBrushes 已被静态构造函数填充。
            generator.ChunkGenerationStep3.Add(new ChunkGenerationStep(250, GenerateExtraGravel));
        }

        private static void GenerateExtraGravel(TerrainChunk chunk)
        {
            // 与原版 GeneratePockets 相同的 3x3 邻居遍历方式，
            // 使用相同的种子公式，保证可重现性。
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int cx = i + chunk.Coords.X;
                    int cz = j + chunk.Coords.Y;
                    // 注意：必须用 Game.Random，它的 Int(int,int) 是原版地形生成使用的扩展方法
                    Game.Random random = new Game.Random(generatorSeedOffset + cx + 71 * cz);

                    var brushes = TerrainContentsGenerator24.m_gravelPocketBrushes;
                    for (int n = 0; n < GravelExtraCount; n++)
                    {
                        int x = cx * 16 + random.Int(0, 15);
                        int y = random.Int(YMin, YMax);
                        int z = cz * 16 + random.Int(0, 15);
                        brushes[random.Int(0, brushes.Count - 1)]
                            .PaintFastSelective(chunk, x, y, z, 3);
                    }
                }
            }
        }

        // 种子偏移：选一个不和原版冲突的素数，保证额外矿袋的位置序列与原版不同。
        private const int generatorSeedOffset = 99991;
    }
}