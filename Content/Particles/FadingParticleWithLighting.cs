using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace CalamityVanilla.Content.Particles;

public class FadingParticleWithLighting : FadingParticle
{
    public static FadingParticleWithLighting RequestFadingParticleWithLighting()
    {
        return _poolFadingParticleWithLighting.RequestParticle();
    }
    private static FadingParticleWithLighting GetNewFadingParticleWithLighting() => new FadingParticleWithLighting();
    private static ParticlePool<FadingParticleWithLighting> _poolFadingParticleWithLighting = new ParticlePool<FadingParticleWithLighting>(100, new ParticlePool<FadingParticleWithLighting>.ParticleInstantiator(GetNewFadingParticleWithLighting));
    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
    {
        Color oc = ColorTint;
        ColorTint = new Color(ColorTint.ToVector3() * Lighting.GetSubLight(LocalPosition)) with { A = oc.A };
        base.Draw(ref settings, spritebatch);
        ColorTint = oc;
    }
}