using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Renderers;

namespace CalamityVanilla.Content.Particles;

public class AnimatedParticle : ABasicParticle
{
    private Color _color = Color.White;

    private int _frames;

    private float _timeToLive;

    private float _timeSinceSpawn;
    public static AnimatedParticle RequestAnimatedParticle()
    {
        return _poolAnimatedParticle.RequestParticle();
    }
    private static AnimatedParticle GetNewAnimatedParticle() => new AnimatedParticle();
    private static ParticlePool<AnimatedParticle> _poolAnimatedParticle = new ParticlePool<AnimatedParticle>(100, new ParticlePool<AnimatedParticle>.ParticleInstantiator(GetNewAnimatedParticle));
    public void SetTypeInfo(int animationFramesAmount, float timeToLive, Asset<Texture2D> texture, Color color)
    {
        _frames = animationFramesAmount;
        _timeToLive = timeToLive;
        _color = color;
        _texture = texture;
    }
    public override void FetchFromPool()
    {
        base.FetchFromPool();
        _color = Color.White;
        _frames = 0;
        _timeToLive = 0f;
        _timeSinceSpawn = 0f;
    }
    public override void Update(ref ParticleRendererSettings settings)
    {
        base.Update(ref settings);
        _timeSinceSpawn++;
        _frame = _texture.Frame(1, _frames, 0, (int)MathF.Floor(_timeSinceSpawn / _timeToLive * _frames));
        _origin = _frame.Size() / 2;
        if (_timeSinceSpawn >= _timeToLive)
        {
            ShouldBeRemovedFromRenderer = true;
        }
    }

    public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
    {
        spritebatch.Draw(_texture.Value, settings.AnchorPosition + LocalPosition, _frame, _color, Rotation, _origin, Scale, SpriteEffects.None, 0f);
    }
}