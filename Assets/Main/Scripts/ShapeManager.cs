using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct ShapeProps
{
   public float mass;
   public Vector3 center;
   public int active;
}
 public class ShapeManager : Object
{
    private ComputeShader m_computeTemplate;
    private Material m_matTemplate;
    private float m_quadSize;

    private List<Material> m_materials;
    private List<Shape> m_shapes;
    private List<ComputeShader> m_shaders;
    private List<ComputeBuffer> m_particleBuffers;
    private List<ComputeBuffer> m_particlePropsBuffers;
    private List<ComputeBuffer> m_argsBuffers;
    private Mesh m_mesh;
    private Mapper m_mapper;

    ComputeBuffer m_shapePropBuffer;
    ShapeProps[] m_shapeProps;
    int maxShapeCount = 50;

    float creationCooldown = 2.0f;
    float timeSinceCreation = 0.0f;

    float globalGravityFactor = 1.0f;
    float globalAttractionFactor = 1.0f;

    public ShapeManager(ComputeShader _computeShaderTmp, Material _matTmp, float _quadSize, Mapper _mapper)
    {
        m_computeTemplate = _computeShaderTmp;
        m_matTemplate = _matTmp;
        m_quadSize = _quadSize;
        m_mapper = _mapper;

        Setup();
    }

    public void SetGlobalGravity(float gravity)
    {
        globalGravityFactor = gravity;
    }

    public void SetGlobalAttraction(float attraction)
    {
        globalAttractionFactor = attraction;
    }


    void Setup()
    {
        m_mesh = CreateQuadMesh(m_quadSize,m_quadSize);

        m_materials = new List<Material>();
        m_shapes = new List<Shape>();
        m_shaders = new List<ComputeShader>();
        m_particleBuffers = new List<ComputeBuffer>();
        m_particlePropsBuffers = new List<ComputeBuffer>();
        m_argsBuffers = new List<ComputeBuffer>();

        m_shapeProps =  new ShapeProps[maxShapeCount];

        for(int i = 0; i < maxShapeCount; i++)
        {
            ShapeProps shapeProps = new ShapeProps();
            shapeProps.center = new Vector3(0, 0, 0);
            shapeProps.mass = 0;
            shapeProps.active = 0;
            m_shapeProps[i] = shapeProps;
        }

        m_shapePropBuffer = new ComputeBuffer(maxShapeCount, sizeof(float) * 4 + sizeof(int));
        m_shapePropBuffer.SetData(m_shapeProps);

        //InitNewShape(new Vector3(0, 0, 0), 1.0f, ShapeGeometry.SPHERE, 0.05f, 0.05f, 0.05f, 1000);
        Debug.Log("[ShapeManager] Setup finished");

    }

    void InitNewShape(float _value,  Vector3 _center, float _size,float _mass, ShapeGeometry _shape, Color _color, float _speed, float _friction, int _number)
    {

        Shape shapeProps = new Shape();
        shapeProps.value = _value;
        shapeProps.repitions = 1;
        shapeProps.seed = Random.Range(0.0f, 1.0f);//Map(_value, 0.0f, 10000.0f, 0.0f, 1.0f);
        shapeProps.center = _center;
        shapeProps.size = _size;
        shapeProps.shape = _shape;
        shapeProps.speed = _speed;
        shapeProps.bounds = new Bounds(_center, Vector3.one * 10.0f);
        shapeProps.particles = new List<Particle>();
        shapeProps.particleProps = new List<ParticleProps>();
        shapeProps.args = new uint[5] { 0, 0, 0, 0, 0 };
        shapeProps.mass = _mass;
        shapeProps.gravityFactor = 0.0f;
        shapeProps.attractionFactor = globalAttractionFactor;
        shapeProps.rotation = Quaternion.identity;
        shapeProps.lastUpdated = 0.0f;

        Color color = _color;

        for (int i = 0; i < _number; i++)
        {
            Particle p = new Particle();

            Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

            p.pos = _center + randomOffset;// + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            p.scale = Vector3.one;
            p.velocity = Vector3.zero;//new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            p.acceleration = Vector3.zero;//new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            p.color = color;
            p.mass = _mass;
            p.friction = _friction;

            shapeProps.particles.Add(p);

            ParticleProps pProps = new ParticleProps();
            pProps.color = color;
            pProps.mat = Matrix4x4.TRS(p.pos, Quaternion.identity, p.scale);
            
            shapeProps.particleProps.Add(pProps);

        }

        ComputeShader compute = Instantiate(m_computeTemplate);
        Material material = Instantiate(m_matTemplate);

        ComputeBuffer argsBuffer = new ComputeBuffer(1, shapeProps.args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        ComputeBuffer boidPropertiesBuffer = new ComputeBuffer(_number, ParticleProps.Size());
        ComputeBuffer boidBuffer = new ComputeBuffer(_number, Particle.Size());
 
        int kernel = GetIndexByShape(_shape);//compute.FindKernel("CSCube");


        shapeProps.args[0] = m_mesh.GetIndexCount(0);
        shapeProps.args[1] = (uint)shapeProps.particleProps.Count;
        shapeProps.args[2] = m_mesh.GetIndexStart(0);
        shapeProps.args[3] = m_mesh.GetBaseVertex(0);

        argsBuffer.SetData(shapeProps.args);

        boidPropertiesBuffer.SetData(shapeProps.particleProps);
        boidBuffer.SetData(shapeProps.particles);


        compute.SetBuffer(kernel, "_Boids", boidBuffer);
        compute.SetBuffer(kernel, "_Properties", boidPropertiesBuffer);
        compute.SetBuffer(kernel, "_ShapeProps", m_shapePropBuffer);

        compute.SetVector("center", _center);
        compute.SetFloat("centerMass", _mass);
        compute.SetFloat("gravityFactor", shapeProps.gravityFactor);
        compute.SetFloat("attractionFactor", shapeProps.attractionFactor);
        compute.SetFloat("speed", _speed);

        compute.SetFloat("size", _size);

        material.SetBuffer("_Properties", boidPropertiesBuffer);

        m_materials.Add(material);
        m_shaders.Add(compute);
        m_argsBuffers.Add(argsBuffer);
        m_particleBuffers.Add(boidBuffer);
        m_particlePropsBuffers.Add(boidPropertiesBuffer);
        m_shapes.Add(shapeProps);

    }

    public void Update(AudioEvent _currentAudioEvent, int pitch)
    {
        timeSinceCreation += Time.deltaTime;
        UpdateShapes(_currentAudioEvent,pitch);
        RunComputeShaders();
 
        Draw();
    }

    public void Draw()
    { 
        for (int i = 0; i < m_shapes.Count; i++)
        {
            m_shapes[i].lastUpdated += Time.deltaTime;
            if(m_shapes[i].lastUpdated > 5.0f && m_shapes[i].gravityFactor - 0.001f > 0.0f)
            {
                m_shapes[i].gravityFactor -= 0.0001f;
            }

            if (m_shapes[i].repitions > 0)
                Graphics.DrawMeshInstancedIndirect(m_mesh, 0, m_materials[i], m_shapes[i].bounds, m_argsBuffers[i]);
        };
    }


    void UpdateShapes(AudioEvent _currentAudioEvent, int pitch)
    {

        if(pitch <= 0)
        {
            return;
        }

        for (int i = 0; i < m_shapes.Count; i++)
        {
             if (m_shapes[i].value == (float)pitch)
            {
                UpdateShape(i);
                return;
            }
        }

        CreateNewShape(_currentAudioEvent, pitch);

    }

    void CreateNewShape(AudioEvent _audioEvent, int pitch)
    {
        if(m_shapes.Count < maxShapeCount) {

            float size = m_mapper.ValidateSize(_audioEvent);
            float speed = m_mapper.ValidateSpeed(_audioEvent);
            float friction = m_mapper.ValidateFriction(_audioEvent);
            float mass = m_mapper.ValidateMass(_audioEvent);
            int number = m_mapper.ValidateNumberofParticles(_audioEvent);
            float value = pitch;//m_mapper.ValidateValue(_audioEvent);

            Color color = m_mapper.ValidateColor(_audioEvent);
            Color hsv = new Color();

            Color.RGBToHSV(color, out hsv.r,out hsv.g, out hsv.b);

     

            if (value > 0 && _audioEvent.peakEnergy > 0.3f && timeSinceCreation > creationCooldown) {
                timeSinceCreation = 0.0f;
                int r = Random.Range(0, 4);
                ShapeGeometry geometry = GetShapeByIndex(r);

                InitNewShape(value, new Vector3(0,0,0),size, mass,geometry, hsv, speed, friction,number);
                Debug.Log("[ShapeManager] Create new Shape");
                Debug.Log("Size " + size);
                Debug.Log("speed " + speed);
                Debug.Log("friction " + friction);
                Debug.Log("mass " + mass);
                Debug.Log("number " + number);
                Debug.Log("value " + value);
            }
        }
    }

    void UpdateShape(int index)
    {

        m_shapes[index].lastUpdated = 0.0f ;
        m_shapes[index].repitions++;
        m_shapes[index].gravityFactor += 0.01f;

        Shape max = GetMostRepititiveShape();



         for (int i = 0; i < m_shapes.Count; i++)
        {

            float x = 0.0f;
            Shape s = m_shapes[i];
                 
            if (max.repitions > 0.0f)
            {
                x = (float)s.repitions / (float)max.repitions;
            }


            float[] bounds = m_mapper.GetParamLimit(m_mapper.numberMapper);
            float value = m_mapper.Map(s.value, 20, 127, 1.5f, 2.5f);

            float alpha = value * Mathf.PI;
            float r =  (1.0f - x) * 8.0f;

            SphericalToCartesian(r, s.seed * Mathf.PI, alpha, out Vector3 pos);


            s.center = pos;
            m_shapes[i] = s;

            m_shapeProps[i].mass = s.mass;
            m_shapeProps[i].center = s.center;
            m_shapeProps[i].active = 1;
        }


        m_shapePropBuffer.SetData(m_shapeProps);

    }

 

    public int Map(int value, int from1, int to1, int from2, int to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }
    Shape GetMostRepititiveShape()
    {
        int y = 0;
        int mostRepititons = -1;

        for (int i = 0; i < m_shapes.Count; i++)
        {
            if (m_shapes[i].repitions > mostRepititons)
            {
                mostRepititons = m_shapes[i].repitions;
                y = i;
            }
        }

        return m_shapes[y];
    }
    void RunComputeShaders()
    {
        for (int i = 0; i < m_shapes.Count; i++)
        {
            if(m_shapes[i].repitions > 0) { 
          
                Shape s = m_shapes[i];
                int kernel = GetIndexByShape(s.shape);
                m_shaders[i].SetVector("center", s.center);

                s.bounds = new Bounds(s.center, Vector3.one * 10.0f);

                m_shaders[i].SetFloat("speed", s.speed);
                m_shaders[i].SetFloat("gravityFactor", s.gravityFactor);
                m_shaders[i].SetFloat("attractionFactor", s.attractionFactor);

                m_shaders[i].SetFloat("size", s.size);
                m_shaders[i].SetBuffer(kernel, "_ShapeProps", m_shapePropBuffer);
                m_shaders[i].SetInt("maxShapeCount", maxShapeCount);
                
                m_shaders[i].SetFloat("timeSinceUpdate", s.lastUpdated);

                m_shaders[i].SetVector("_Time", Shader.GetGlobalVector("_Time"));

                m_shaders[i].Dispatch(kernel, Mathf.CeilToInt(s.particles.Count / 128), 1, 1);
                m_shapes[i] = s;
            }
        }
    }

    Mesh CreateQuadMesh(float width, float height)
    {

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3 cross = -Vector3.Cross(vertices[1], vertices[2]);
        Vector3 cross2 = -Vector3.Cross(vertices[3] - vertices[1], -vertices[1]);
        Vector3 cross3 = -Vector3.Cross(vertices[3] - vertices[2], vertices[2]);
        Vector3 cross4 = -Vector3.Cross(vertices[2] - vertices[3], vertices[1] - vertices[3]);

        cross.y -= 1;
        cross2.y -= 1;

        Vector3[] normals = new Vector3[4]
        {
            cross,
            cross2,
            cross3,
            cross4,
        };


        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;


        return mesh;
    }

    int GetIndexByShape(ShapeGeometry shape)
    {
        switch (shape)
        {
            case ShapeGeometry.CUBE:
                {
                    return 0;
                };
            case ShapeGeometry.SPHERE:
                {
                    return 1;
                };
            case ShapeGeometry.PYRAMID:
                {
                    return 2;
                }
            default:
                {
                    return 3;
                };
        }
    }

    ShapeGeometry GetShapeByIndex(int index )
    {
        switch (index)
        {
            case 0:
                {
                    return ShapeGeometry.CUBE;
                };
            case 1:
                {
                    return ShapeGeometry.SPHERE;
                };
            case 2:
                {
                    return ShapeGeometry.PYRAMID;
                }
            default:
                {
                    return ShapeGeometry.CAPSULE;
                };
        }
    }


    public void OnDestroy()
    {
        for (int i = 0; i < m_shapes.Count; i++)
        {
            m_particleBuffers[i].Release();
            m_particlePropsBuffers[i].Release();
            m_argsBuffers[i].Release();
        }
    }

}
