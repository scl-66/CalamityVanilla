using Terraria.Graphics.Renderers;

namespace CalamityVanilla.Content.Particles;

// this is just a class that takes some stuff from vanillas particle orchestrator to use its particles
public class VanillaParticles
{
    public static PrettySparkleParticle RequestPrettySparkleParticle()
    {
        return _poolPrettySparkle.RequestParticle();
    }
    private static PrettySparkleParticle GetNewPrettySparkleParticle() => new PrettySparkleParticle();
    private static ParticlePool<PrettySparkleParticle> _poolPrettySparkle = new ParticlePool<PrettySparkleParticle>(200, new ParticlePool<PrettySparkleParticle>.ParticleInstantiator(GetNewPrettySparkleParticle));
    public static FadingParticle RequestFadingParticle()
    {
        return _poolFadingParticle.RequestParticle();
    }
    private static FadingParticle GetNewFadingParticle() => new FadingParticle();
    private static ParticlePool<FadingParticle> _poolFadingParticle = new ParticlePool<FadingParticle>(100, new ParticlePool<FadingParticle>.ParticleInstantiator(GetNewFadingParticle));

    public static RandomizedFrameParticle RequestRandomizedFrameParticle()
    {
        return _poolRandomizedFrameParticle.RequestParticle();
    }
    private static RandomizedFrameParticle GetNewRandomizedFrameParticle() => new RandomizedFrameParticle();
    private static ParticlePool<RandomizedFrameParticle> _poolRandomizedFrameParticle = new ParticlePool<RandomizedFrameParticle>(100, new ParticlePool<RandomizedFrameParticle>.ParticleInstantiator(GetNewRandomizedFrameParticle));
}