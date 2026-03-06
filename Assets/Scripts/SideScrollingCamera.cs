using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SideScrollingCamera : MonoBehaviour
{
    public Transform trackedObject;
    /// <summary>Camera Y position (vertical placement in world). Does not affect zoom/scale.</summary>
    public float height = 6.5f;
    public float undergroundHeight = -9.5f;
    public float undergroundThreshold = 0f;

    /// <summary>Vertical half-height of the view in world units. Total visible height = orthographicSize * 2. Set to 7.5 to see 15 units (1:1 for a 15-unit-tall view).</summary>
    public float orthographicSize = 7f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera != null && _camera.orthographic)
            _camera.orthographicSize = orthographicSize;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, trackedObject.position.x);
        transform.position = cameraPosition;
    }

    public void SetUnderground(bool underground)
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.y = underground ? undergroundHeight : height;
        transform.position = cameraPosition;
    }

    /// <summary>Smoothly scrolls the camera up to the surface (above-ground) Y position. Use when exiting underground.</summary>
    public IEnumerator ScrollToSurface(float duration)
    {
        float startY = transform.position.y;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t); // smoothstep

            Vector3 pos = transform.position;
            pos.x = Mathf.Max(pos.x, trackedObject != null ? trackedObject.position.x : pos.x);
            pos.y = Mathf.Lerp(startY, height, t);
            transform.position = pos;

            yield return null;
        }

        Vector3 finalPos = transform.position;
        finalPos.y = height;
        if (trackedObject != null)
            finalPos.x = Mathf.Max(finalPos.x, trackedObject.position.x);
        transform.position = finalPos;
    }
}
