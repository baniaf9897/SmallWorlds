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
    private ShapePropertyDomains m_propertyDomains;


    private List<Material> m_materials;
    private List<Shape> m_shapes;
    private List<ComputeShader> m_shaders;
    private List<ComputeBuffer> m_particleBuffers;
    private List<ComputeBuffer> m_particlePropsBuffers;
    private List<ComputeBuffer> m_argsBuffers;
    private Mesh m_mesh;

    ComputeBuffer m_shapePropBuffer;
    ShapeProps[] m_shapeProps;
    int maxShapeCount = 50;
    public ShapeManager(ComputeShader _computeShaderTmp, Material _matTmp, float _quadSize, ShapePropertyDomains _limits)
    {
        m_computeTemplate = _computeShaderTmp;
        m_matTemplate = _matTmp;
        m_quadSize = _quadSize;
        m_propertyDomains = _limits;

        Setup();
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

    void InitNewShape(float _value,  Vector3 _center, float _size, ShapeGeometry _shape, float _coherence, float _seperation, float _speed, int _number)
    {

        Shape shapeProps = new Shape();
        shapeProps.value = _value;
        shapeProps.repitions = 1;
        shapeProps.seed = Map(_value, 0.0f, 10000.0f, 0.0f, 1.0f);
        shapeProps.center = _center;
        shapeProps.size = _size;
        shapeProps.shape = _shape;
        shapeProps.coherence = _coherence;
        shapeProps.seperation = _seperation;
        shapeProps.speed = _speed;
        shapeProps.bounds = new Bounds(_center, Vector3.one * 10.0f);
        shapeProps.particles = new List<Particle>();
        shapeProps.particleProps = new List<ParticleProps>();
        shapeProps.args = new uint[5] { 0, 0, 0, 0, 0 };
        shapeProps.mass = Random.Range(0,10);


        shapeProps.rotation = Quaternion.identity;

        Color color = Color.Lerp(Color.red, Color.blue, Map(_value,0.0f, 5000.0f, 0,1.0f));

        for (int i = 0; i < _number; i++)
        {
            Particle p = new Particle();

            Vector3 randomOffset = new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));

            p.pos = _center + randomOffset;// + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            p.scale = Vector3.one;
            p.velocity = Vector3.zero;//new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            p.acceleration = Vector3.zero;//new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
            p.color = color;
            p.mass = Random.Range(0, 5);

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
        compute.SetFloat("speed", _speed);
        compute.SetFloat("coherenceFactor", _coherence);
        compute.SetFloat("seperationFactor", _seperation);
        compute.SetFloat("size", _size);

        material.SetBuffer("_Properties", boidPropertiesBuffer);

        m_materials.Add(material);
        m_shaders.Add(compute);
        m_argsBuffers.Add(argsBuffer);
        m_particleBuffers.Add(boidBuffer);
        m_particlePropsBuffers.Add(boidPropertiesBuffer);
        m_shapes.Add(shapeProps);

    }

    public void Update(AudioEvent _currentAudioEvent)
    {
        UpdateShapes(_currentAudioEvent);
        RunComputeShaders();
 
        Draw();
    }

    public void Draw()
    { 
        for (int i = 0; i < m_shapes.Count; i++)
        {
            Graphics.DrawMeshInstancedIndirect(m_mesh, 0, m_materials[i], m_shapes[i].bounds, m_argsBuffers[i]);
        };
    }


    void UpdateShapes(AudioEvent _currentAudioEvent)
    {
        if(_currentAudioEvent.value < 0.0f)
        {
            return;
        }

        for (int i = 0; i < m_shapes.Count; i++)
        {
            if (m_shapes[i].value == _currentAudioEvent.value)
            {
                UpdateShape(i);
                return;
            }
        }

        CreateNewShape(_currentAudioEvent);

    }

    void CreateNewShape(AudioEvent _audioEvent)
    {
        if(m_shapes.Count < maxShapeCount) { 
            //calc params
            //TODO: MAPPING!
            float size = Map(_audioEvent.value,0.0f,10000.0f, m_propertyDomains.minSize, m_propertyDomains.maxSize);
            float seperation = Map(_audioEvent.value, 0.0f, 10000.0f, m_propertyDomains.minSeperation, m_propertyDomains.maxSeperation);
            float coherence = 0;// Map(_audioEvent.value, 0.0f, 10000.0f, m_propertyDomains.minCoherence, m_propertyDomains.maxCoherence);
            float speed = Map(_audioEvent.value, 0.0f, 10000.0f, m_propertyDomains.minSpeed, m_propertyDomains.maxSpeed);
            int number = Map((int)_audioEvent.value, 0, 10000, m_propertyDomains.minNumber, m_propertyDomains.maxNumber);

            int r = Random.Range(0, 3);
            ShapeGeometry geometry = GetShapeByIndex(r);

            InitNewShape(_audioEvent.value, new Vector3(0,0,0),size,geometry,coherence,seperation,speed,number);
            Debug.Log("[ShapeManager] Create new Shape");
        }
    }

    void UpdateShape(int index)
    {
        m_shapes[index].repitions++; 
        Shape max = GetMostRepititiveShape();

         for (int i = 0; i < m_shapes.Count; i++)
        {
            float x = 0.0f;
            Shape s = m_shapes[i];

            if (max.repitions > 0.0f)
            {
                x = (float)s.repitions / (float)max.repitions;
            }

            float value = Map(s.value, 0.0f, 5.0f, 1.5f, 2.5f);

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

    public float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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
            Shape s = m_shapes[i];
            int kernel = GetIndexByShape(s.shape);
            m_shaders[i].SetVector("center", s.center);

            s.bounds = new Bounds(s.center, Vector3.one * 10.0f);

            Quaternion q = Quaternion.identity;
            q.SetLookRotation(s.center - Camera.main.transform.position);

            Matrix4x4 rot = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            m_shaders[i].SetMatrix("rotMat", rot);


            m_shaders[i].SetFloat("speed", s.speed);
            m_shaders[i].SetFloat("coherenceFactor", s.coherence);
            m_shaders[i].SetFloat("seperationFactor", s.seperation);
            m_shaders[i].SetFloat("size", s.size);
            m_shaders[i].SetBuffer(kernel, "_ShapeProps", m_shapePropBuffer);
            m_shaders[i].SetInt("maxShapeCount", maxShapeCount);

            m_shaders[i].Dispatch(kernel, Mathf.CeilToInt(s.particles.Count / 128), 1, 1);
            m_shapes[i] = s;
        }
    }

    Mesh CreateQuadMesh(float width, float height)
    {

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, height),
            new Vector3(width, height, height)
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

    public void SetGlobalCoherence(float coherence)
    {
        foreach(Shape shape in m_shapes)
        {
            shape.coherence = coherence;
        }

        RunComputeShaders();
    }
}
