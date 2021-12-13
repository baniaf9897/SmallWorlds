using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Particle
{
    public Vector3 pos;
    public Vector3 scale;

    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector4 color;

    public static int Size()
    {
        return
            sizeof(float) * 3 +     // pos;
            sizeof(float) * 3 +     // scale;
            sizeof(float) * 3 +     // vel;
            sizeof(float) * 3 +     // acc;
            sizeof(float) * 4;      // color;
    }

};

public struct ParticleProps
{

    public Matrix4x4 mat;
    public Vector4 color;

    public static int Size()
    {
        return
            sizeof(float) * 4 * 4 + // matrix;
            sizeof(float) * 4;      // color;
    }
}


public enum ShapeGeometry
{
    SPHERE,
    CUBE,
    PYRAMID,
    CAPSULE
}

public class Shape 
{
    public Vector3 center;
    public float size;
    public ShapeGeometry shape;
    public float seperation;
    public float coherence;
    public float speed;
    public Bounds bounds;
    public List<Particle> particles;
    public List<ParticleProps> particleProps;
    public uint[] args;
    public Quaternion rotation;

    public float value;
    public int repitions;
    public float seed;
}
