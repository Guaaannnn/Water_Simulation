using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class WaterWaveMesh : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveAmplitude = 0.3f;     
    public float waveLength = 2f;          
    public float waveSpeed = 1f;           
    public Vector2 waveDirection = new Vector2(1f, 1f); 

    [Header("Mesh Settings")]
    public bool recalculateNormals = true; 
    public bool updateCollider = true;     
    public bool showGizmos = false;       

    private Mesh mesh;
    private MeshCollider meshCollider;
    private Vector3[] baseVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        baseVertices = mesh.vertices;
        displacedVertices = new Vector3[baseVertices.Length];
    }

    void Update()
    {
        AnimateWaves();
    }

    void AnimateWaves()
    {
        if (mesh == null) return;

        float time = Time.time * waveSpeed;
        Vector2 dir = waveDirection.normalized;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float wave = Mathf.Sin((v.x * dir.x + v.z * dir.y) / waveLength + time) * waveAmplitude;
            v.y = wave;
            displacedVertices[i] = v;
        }

        mesh.vertices = displacedVertices;
        if (recalculateNormals)
            mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        if (updateCollider && meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }

    public float GetWaveHeightAtPosition(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);
        Vector2 dir = waveDirection.normalized;

        float time = Time.time * waveSpeed;
        float wave = Mathf.Sin((localPos.x * dir.x + localPos.z * dir.y) / waveLength + time) * waveAmplitude;

        return transform.TransformPoint(new Vector3(0f, wave, 0f)).y;
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(5f, 0.05f, 5f));
    }
}
