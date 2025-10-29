using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingObject : MonoBehaviour
{
    [Header("Wave Reference")]
    public WaterWaveMesh water;

    [Header("Buoyancy Settings")]
    public float buoyancyStrength = 10f;   
    public float damping = 2f;            
    public float neutralOffset = 0.0f;     

    [Header("Debug Settings")]
    public bool showDebug = true;
    public float contactThreshold = 0.05f; 
    public float sinkThreshold = 0.2f;    

    private Rigidbody rb;
    private string lastState = "";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
    }

    void FixedUpdate()
    {
        if (water == null)
            return;

        Vector3 pos = transform.position;
        float waveY = water.GetWaveHeightAtPosition(pos);
        float difference = waveY + neutralOffset - pos.y;

        float force = difference * buoyancyStrength - rb.linearVelocity.y * damping;
        rb.AddForce(Vector3.up * force, ForceMode.Acceleration);

        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            if (hit.collider.GetComponent<WaterWaveMesh>() != null)
            {
                float dist = hit.distance;
                string currentState;

                if (dist <= contactThreshold)
                    currentState = "🟩 Menyentuh permukaan air";
                else if (dist > contactThreshold && dist <= sinkThreshold)
                    currentState = "🟥 Tenggelam di bawah permukaan air";
                else
                    currentState = "🟦 Di atas permukaan air";

                if (currentState != lastState && showDebug)
                {
                    Debug.Log($"[{name}] {currentState} | ΔY = {difference:F3}, Dist = {dist:F3}");
                    lastState = currentState;
                }
            }
        }
        else if (showDebug && lastState != "⚠️ Tidak ada air di bawah")
        {
            Debug.Log($"[{name}] ⚠️ Tidak ada air di bawah (ray miss)");
            lastState = "⚠️ Tidak ada air di bawah";
        }


         if (showDebug)
        {
            Vector3 vel = rb.linearVelocity;
            Debug.Log($"[{name}] PosY={pos.y:F3} | VelY={vel.y:F3} | ΔY={difference:F3}");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
             if (showDebug)
                Debug.Log($"[{name}] Di dalam volume air trigger");
        }
    }
}
