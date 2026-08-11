using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace CalamityVanilla.Common.Verlet;

internal struct VerletRope
{
    internal record struct RopeParticleProperties(float RestLength, bool IsFixed);
    //current
    [InlineArray(MaxParticleCount)]
    struct PositionData { Vector2 element0; }
    //prev
    [InlineArray(MaxParticleCount)]
    struct PrevPositionData { Vector2 element0; }

    const int MaxParticleCount = 32;

    readonly RopeParticleProperties[] _particleProps;

    PositionData _positions;
    PrevPositionData _prevPositions;

    public readonly int ParticleCount => _particleProps.Length;
    public readonly int Iterations;

    public readonly Vector2 this[int idx]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _positions[idx];
    }

    public VerletRope(Vector2[] initialPoints, float defaultRestLength, int iterations = 5)
    {
        if (initialPoints.Length > MaxParticleCount)
            throw new Exception($"points length ({initialPoints.Length}) exceeds max count of {MaxParticleCount}");

        Iterations = iterations;

        _particleProps = new RopeParticleProperties[initialPoints.Length];

        for (int i = 0; i < initialPoints.Length; i++)
        {
            _positions[i] = initialPoints[i];
            _prevPositions[i] = initialPoints[i];

            _particleProps[i] = new RopeParticleProperties(defaultRestLength, false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetSegmentRestLength(int segmentIndex, float restLength)
    {
        if (segmentIndex < 0 || segmentIndex >= ParticleCount - 1)
            throw new ArgumentOutOfRangeException(nameof(segmentIndex), "segment index must be valid and not the last particle");

        _particleProps[segmentIndex] = _particleProps[segmentIndex] with { RestLength = restLength };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFixedPoint(int index, Vector2 newPosition)
    {
        if (index < 0 || index >= ParticleCount) return;

        _positions[index] = newPosition;
        _prevPositions[index] = newPosition;
        _particleProps[index] = _particleProps[index] with { IsFixed = true };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetMovablePoint(int index, Vector2 newPosition)
    {
        if (index < 0 || index >= ParticleCount) return;

        _positions[index] = newPosition;
        _prevPositions[index] = newPosition;
        _particleProps[index] = _particleProps[index] with { IsFixed = false };
    }

    public void Update(Vector2 gravity, float damping, float deltaTime)
    {
        for (int i = 0; i < ParticleCount; i++)
        {
            if (_particleProps[i].IsFixed) continue;

            Vector2 velocity = _positions[i] - _prevPositions[i];
            velocity *= damping;

            Vector2 newPosition = _positions[i] + velocity + gravity * deltaTime;
            _prevPositions[i] = _positions[i];
            _positions[i] = newPosition;
        }

        for (int k = 0; k < Iterations; k++)
        {
            SolveConstraints();
        }
    }

    private void SolveConstraints()
    {
        for (int i = 0; i < ParticleCount - 1; i++)
        {
            var p1Pos = _positions[i];
            var p2Pos = _positions[i + 1];

            float segmentRestLength = _particleProps[i].RestLength;

            var delta = p2Pos - p1Pos;
            float currentDistance = delta.Length();
            if (currentDistance == 0) continue;

            float difference = (currentDistance - segmentRestLength) / currentDistance;

            if (!_particleProps[i].IsFixed)
            {
                _positions[i] += delta * 0.5f * difference;
            }
            if (!_particleProps[i + 1].IsFixed)
            {
                _positions[i + 1] -= delta * 0.5f * difference;
            }
        }
    }
}